using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FrameWork.Service
{
    public static class KcpProtocalType
    {
        public const byte SYN = 1;
        public const byte ACK = 2;
        public const byte FIN = 3;
        public const byte MSG = 4;
    }

    public sealed class KService : AService
    {
        // KService������ʱ��
        private readonly long startTime;

        // ��ǰʱ�� - KService������ʱ��, �̰߳�ȫ
        public uint TimeNow
        {
            get
            {
                return (uint)(TimeHelper.ClientNow() - this.startTime);
            }
        }

        private Socket socket;


        #region �ص�����

        static KService()
        {
            //Kcp.KcpSetLog(KcpLog);
            Kcp.KcpSetoutput(KcpOutput);
        }

        private static readonly byte[] logBuffer = new byte[1024];

#if ENABLE_IL2CPP
		[AOT.MonoPInvokeCallback(typeof(KcpOutput))]
#endif
        private static void KcpLog(IntPtr bytes, int len, IntPtr kcp, IntPtr user)
        {
            try
            {
                Marshal.Copy(bytes, logBuffer, 0, len);
                SDebug.LogFormat(logBuffer.ToStr(0, len));
            }
            catch (Exception e)
            {
                SDebug.LogError(e);
            }
        }

#if ENABLE_IL2CPP
		[AOT.MonoPInvokeCallback(typeof(KcpOutput))]
#endif
        private static int KcpOutput(IntPtr bytes, int len, IntPtr kcp, IntPtr user)
        {
            try
            {
                if (kcp == IntPtr.Zero)
                {
                    return 0;
                }

                if (!KChannel.KcpPtrChannels.TryGetValue(kcp, out KChannel kChannel))
                {
                    return 0;
                }

                kChannel.Output(bytes, len);
            }
            catch (Exception e)
            {
                SDebug.LogError(e);
                return len;
            }

            return len;
        }

        #endregion

        public KService(ThreadSynchronizationContext threadSynchronizationContext, IPEndPoint ipEndPoint, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
            this.ThreadSynchronizationContext = threadSynchronizationContext;
            this.startTime = TimeHelper.ClientNow();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.socket.SendBufferSize = Kcp.OneM * 64;
                this.socket.ReceiveBufferSize = Kcp.OneM * 64;
            }

            this.socket.Bind(ipEndPoint);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                this.socket.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
            }
        }

        public KService(ThreadSynchronizationContext threadSynchronizationContext, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
            this.ThreadSynchronizationContext = threadSynchronizationContext;
            this.startTime = TimeHelper.ClientNow();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // ��Ϊ�ͻ��˲���Ҫ�޸ķ��͸����ջ�������С
            this.socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                this.socket.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
            }
        }

        public void ChangeAddress(long id, IPEndPoint address)
        {
            KChannel kChannel = this.Get(id);
            if (kChannel == null)
            {
                return;
            }

            SDebug.LogFormat($"channel change address: {id} {address}");
            kChannel.RemoteAddress = address;
        }


        // �������е�channel
        private readonly Dictionary<long, KChannel> idChannels = new Dictionary<long, KChannel>();
        private readonly Dictionary<long, KChannel> localConnChannels = new Dictionary<long, KChannel>();
        private readonly Dictionary<long, KChannel> waitConnectChannels = new Dictionary<long, KChannel>();

        private readonly byte[] cache = new byte[8192];
        private EndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // ��֡Ҫ���µ�channel
        private readonly HashSet<long> updateChannels = new HashSet<long>();

        // �´�ʱ����µ�channel
        private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();

        private readonly List<long> timeOutTime = new List<long>();

        // ��¼��Сʱ�䣬����ÿ�ζ�ȥMultiMapȡ��һ��ֵ
        private long minTime;

        private List<long> waitRemoveChannels = new List<long>();

        private IPEndPoint CloneAddress()
        {
            IPEndPoint ip = (IPEndPoint)this.ipEndPoint;
            return new IPEndPoint(ip.Address, ip.Port);
        }

        private void Recv()
        {
            if (this.socket == null)
            {
                return;
            }

            while (socket != null && this.socket.Available > 0)
            {
                int messageLength = this.socket.ReceiveFrom(this.cache, ref this.ipEndPoint);

                // ����С��1��������������Ϣ
                if (messageLength < 1)
                {
                    continue;
                }

                // accept
                byte flag = this.cache[0];

                // conn��100��ʼ�����Ϊ1��2��3���������
                uint remoteConn = 0;
                uint localConn = 0;

                try
                {
                    KChannel kChannel = null;
                    switch (flag)
                    {
#if NOT_UNITY
                        case KcpProtocalType.SYN: // accept
                            {
                                // ����!=5������SYN��Ϣ
                                if (messageLength < 9)
                                {
                                    break;
                                }

                                string realAddress = null;
                                remoteConn = BitConverter.ToUInt32(this.cache, 1);
                                if (messageLength > 9)
                                {
                                    realAddress = this.cache.ToStr(9, messageLength - 9);
                                }

                                remoteConn = BitConverter.ToUInt32(this.cache, 1);
                                localConn = BitConverter.ToUInt32(this.cache, 5);

                                this.waitConnectChannels.TryGetValue(remoteConn, out kChannel);
                                if (kChannel == null)
                                {
                                    localConn = CreateRandomLocalConn();
                                    // �Ѵ���ͬ����localConn���򲻴����ȴ��´�sync
                                    if (this.localConnChannels.ContainsKey(localConn))
                                    {
                                        break;
                                    }
                                    long id = this.CreateAcceptChannelId(localConn);
                                    if (this.idChannels.ContainsKey(id))
                                    {
                                        break;
                                    }

                                    kChannel = new KChannel(id, localConn, remoteConn, this.socket, this.CloneAddress(), this);
                                    this.idChannels.Add(kChannel.Id, kChannel);
                                    this.waitConnectChannels.Add(kChannel.RemoteConn, kChannel); // �������˻��߳�ʱ���ɾ��
                                    this.localConnChannels.Add(kChannel.LocalConn, kChannel);

                                    kChannel.RealAddress = realAddress;

                                    IPEndPoint realEndPoint = kChannel.RealAddress == null ? kChannel.RemoteAddress : NetworkHelper.ToIPEndPoint(kChannel.RealAddress);
                                    this.OnAccept(kChannel.Id, realEndPoint);
                                }
                                if (kChannel.RemoteConn != remoteConn)
                                {
                                    break;
                                }

                                // ��ַ���ϴεĲ�һ��������
                                if (kChannel.RealAddress != realAddress)
                                {
                                    SDebug.LogError($"kchannel syn address diff: {kChannel.Id} {kChannel.RealAddress} {realAddress}");
                                    break;
                                }

                                try
                                {
                                    byte[] buffer = this.cache;
                                    buffer.WriteTo(0, KcpProtocalType.ACK);
                                    buffer.WriteTo(1, kChannel.LocalConn);
                                    buffer.WriteTo(5, kChannel.RemoteConn);
                                    SDebug.LogFormat($"kservice syn: {kChannel.Id} {remoteConn} {localConn}");
                                    this.socket.SendTo(buffer, 0, 9, SocketFlags.None, kChannel.RemoteAddress);
                                }
                                catch (Exception e)
                                {
                                    SDebug.LogError(e);
                                    kChannel.OnError(ErrorCore.ERR_SocketCantSend);
                                }

                                break;
                            }
#endif
                        case KcpProtocalType.ACK: // connect����
                            // ����!=9������connect��Ϣ
                            if (messageLength != 9)
                            {
                                break;
                            }

                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            kChannel = this.GetByLocalConn(localConn);
                            if (kChannel != null)
                            {
                                SDebug.LogFormat($"kservice ack: {kChannel.Id} {remoteConn} {localConn}");
                                kChannel.RemoteConn = remoteConn;
                                kChannel.HandleConnnect();
                            }

                            break;
                        case KcpProtocalType.FIN: // �Ͽ�
                            // ����!=13������DisConnect��Ϣ
                            if (messageLength != 13)
                            {
                                break;
                            }

                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            int error = BitConverter.ToInt32(this.cache, 9);

                            // ����chanel
                            kChannel = this.GetByLocalConn(localConn);
                            if (kChannel == null)
                            {
                                break;
                            }

                            // У��remoteConn����ֹ����������
                            if (kChannel.RemoteConn != remoteConn)
                            {
                                break;
                            }

                            SDebug.LogFormat($"kservice recv fin: {kChannel.Id} {localConn} {remoteConn} {error}");
                            kChannel.OnError(ErrorCore.ERR_PeerDisconnect);

                            break;
                        case KcpProtocalType.MSG: // �Ͽ�
                            // ����<9������Msg��Ϣ
                            if (messageLength < 9)
                            {
                                break;
                            }
                            // ����chanel
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);

                            kChannel = this.GetByLocalConn(localConn);
                            if (kChannel == null)
                            {
                                // ֪ͨ�Է��Ͽ�
                                this.Disconnect(localConn, remoteConn, ErrorCore.ERR_KcpNotFoundChannel, (IPEndPoint)this.ipEndPoint, 1);
                                break;
                            }

                            // У��remoteConn����ֹ����������
                            if (kChannel.RemoteConn != remoteConn)
                            {
                                break;
                            }

                            kChannel.HandleRecv(this.cache, 5, messageLength - 5);
                            break;
                    }
                }
                catch (Exception e)
                {
                    SDebug.LogError($"kservice error: {flag} {remoteConn} {localConn}\n{e}");
                }
            }
        }

        public KChannel Get(long id)
        {
            KChannel channel;
            this.idChannels.TryGetValue(id, out channel);
            return channel;
        }

        private KChannel GetByLocalConn(uint localConn)
        {
            KChannel channel;
            this.localConnChannels.TryGetValue(localConn, out channel);
            return channel;
        }


        private void Disconnect(uint localConn, uint remoteConn, int error, IPEndPoint address, int times)
        {
            try
            {
                if (this.socket == null)
                {
                    return;
                }

                byte[] buffer = this.cache;
                buffer.WriteTo(0, KcpProtocalType.FIN);
                buffer.WriteTo(1, localConn);
                buffer.WriteTo(5, remoteConn);
                buffer.WriteTo(9, (uint)error);
                for (int i = 0; i < times; ++i)
                {
                    this.socket.SendTo(buffer, 0, 13, SocketFlags.None, address);
                }
            }
            catch (Exception e)
            {
                SDebug.LogError($"Disconnect error {localConn} {remoteConn} {error} {address} {e}");
            }

            SDebug.LogFormat($"channel send fin: {localConn} {remoteConn} {address} {error}");
        }

        // �������Ҫ��channel��updateʱ���Ƿ��ѵ�
        public void AddToUpdateNextTime(long time, long id)
        {
            if (time == 0)
            {
                this.updateChannels.Add(id);
                return;
            }
            if (time < this.minTime)
            {
                this.minTime = time;
            }
            this.timeId.Add(time, id);
        }

        private void RemoveConnectTimeoutChannels()
        {
            waitRemoveChannels.Clear();
            foreach (long channelId in this.waitConnectChannels.Keys)
            {
                this.waitConnectChannels.TryGetValue(channelId, out KChannel kChannel);
                if (kChannel == null)
                {
                    SDebug.LogError($"RemoveConnectTimeoutChannels not found kchannel: {channelId}");
                    continue;
                }

                // ��������Ҫ����ɾ��
                if (kChannel.IsConnected)
                {
                    waitRemoveChannels.Add(channelId);
                }

                // 10�����ӳ�ʱ
                if (this.TimeNow > kChannel.CreateTime + 10 * 1000)
                {
                    waitRemoveChannels.Add(channelId);
                }
            }

            foreach (long channelId in waitRemoveChannels)
            {
                this.waitConnectChannels.Remove(channelId);
            }
        }

        // ���㵽����Ҫupdate��channel
        private void TimerOut()
        {
            if (this.timeId.Count == 0)
            {
                return;
            }

            uint timeNow = this.TimeNow;

            if (timeNow < this.minTime)
            {
                return;
            }

            this.timeOutTime.Clear();

            foreach (KeyValuePair<long, List<long>> kv in this.timeId)
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    minTime = k;
                    break;
                }

                this.timeOutTime.Add(k);
            }

            foreach (long k in this.timeOutTime)
            {
                foreach (long v in this.timeId[k])
                {
                    this.updateChannels.Add(v);
                }

                this.timeId.Remove(k);
            }
        }


        public override bool IsDispose()
        {
            return this.socket == null;
        }

        public override void Dispose()
        {
            foreach (long channelId in this.idChannels.Keys.ToArray())
            {
                this.Remove(channelId);
            }

            this.socket.Close();
            this.socket = null;
        }

        protected override void Get(long id, IPEndPoint address)
        {
            if (this.idChannels.TryGetValue(id, out KChannel kChannel))
            {
                return;
            }

            try
            {
                // ��32bit��localConn
                uint localConn = (uint)((ulong)id & uint.MaxValue);
                kChannel = new KChannel(id, localConn, this.socket, address, this);
                this.idChannels.Add(id, kChannel);
                this.localConnChannels.Add(kChannel.LocalConn, kChannel);
            }
            catch (Exception e)
            {
                SDebug.LogError($"kservice get error: {id}\n{e}");
            }
        }

        public override void Remove(long id)
        {
            if (!this.idChannels.TryGetValue(id, out KChannel kChannel))
            {
                return;
            }
            SDebug.LogFormat($"kservice remove channel: {id} {kChannel.LocalConn} {kChannel.RemoteConn}");
            this.idChannels.Remove(id);
            this.localConnChannels.Remove(kChannel.LocalConn);
            if (this.waitConnectChannels.TryGetValue(kChannel.RemoteConn, out KChannel waitChannel))
            {
                if (waitChannel.LocalConn == kChannel.LocalConn)
                {
                    this.waitConnectChannels.Remove(kChannel.RemoteConn);
                }
            }
            kChannel.Dispose();
        }

        protected override void Send(long channelId, long actorId, MemoryStream stream)
        {
            KChannel channel = this.Get(channelId);
            if (channel == null)
            {
                return;
            }
            channel.Send(actorId, stream);
        }

        public override void Update()
        {
            this.Recv();

            this.TimerOut();

            foreach (long id in updateChannels)
            {
                KChannel kChannel = this.Get(id);
                if (kChannel == null)
                {
                    continue;
                }

                if (kChannel.Id == 0)
                {
                    continue;
                }

                kChannel.Update();
            }

            this.updateChannels.Clear();

            this.RemoveConnectTimeoutChannels();
        }
    }
}