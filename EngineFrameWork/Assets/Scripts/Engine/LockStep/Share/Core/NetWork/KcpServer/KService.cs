
using System;
using System.IO;
using System.Net;
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
        // KService的创建时间
        private readonly long startTime;

        public uint TimeNow
        {
            get
            {
                return (uint)(TimeHelper.ClientNow() - this.startTime);
            }
        }

        private Socket socket;

        #region 回调函数
        static KService()
        {
            // Kcp.KcpSetLog(KcpLog);
            Kcp.KcpSetoutput(KcpOutput);
        }

        private static readonly byte[] logBuffer = new byte[1024];

#if ENABLE_IL2CPP
        [AOT.MonoPInvokeCallback(KcpOutput)]
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
                throw;
            }
        }

#if ENABLE_IL2CPP
        [AOT.MonoPInvokeCallback(KcpOutput)]
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

        public KService(ThreadSynchronizationContext threadSynchronizationContext, IPEndPoint iPEndPoint, ServiceType serviceType)
        {
            base.ServiceType = serviceType;
            base.ThreadSynchronizationContext = threadSynchronizationContext;

            startTime = TimeHelper.ClientNow();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.socket.SendBufferSize = Kcp.OneM * 64;
                this.socket.ReceiveBufferSize = Kcp.OneM * 64;
            }

            socket.Bind(iPEndPoint);
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
            base.ServiceType = serviceType;
            base.ThreadSynchronizationContext = threadSynchronizationContext;
            this.startTime = TimeHelper.ClientNow();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 作为客户端不需要修改发达跟接收缓冲区大小
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
            KChannel kChannel = Get(id);
            if (kChannel == null)
            {
                return;
            }

            SDebug.LogFormat($"channel change address: {id} {address}");
            kChannel.RemoteAddress = address;
        }


        // 保存所有的channel
        private readonly Dictionary<long, KChannel> idChannels = new Dictionary<long, KChannel>();
        private readonly Dictionary<long, KChannel> localConnChannels = new Dictionary<long, KChannel>();
        private readonly Dictionary<long, KChannel> waitConnectChannels = new Dictionary<long, KChannel>();

        private readonly byte[] cache = new byte[8192];
        private EndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // 下帧更新的channel
        private readonly HashSet<long> updateChannels = new HashSet<long>();

        // 下次时间更新的channel
        private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();

        private readonly List<long> timeOutTime = new List<long>();

        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTIme;

        private List<long> waitRemoveChannels = new List<long>();

        private IPEndPoint CloneAddress()
        {
            IPEndPoint ip = (IPEndPoint)this.ipEndPoint;
            return new IPEndPoint(ip.Address, ip.Port);
        }

        private void Recv()
        {
            if (socket == null)
            {
                return;
            }

            while (socket != null && socket.Available > 0)
            {
                int messageLength = socket.ReceiveFrom(cache, ref this.ipEndPoint);

                // 长度小于1，不是正常的消息
                if (messageLength < 1)
                {
                    continue;
                }

                // accept
                byte flag = this.cache[0];

                // conn从100开始，如果为1，2，3则是特殊包
                uint remoteConn = 0;
                uint localConn = 0;

                try
                {
                    KChannel kChannel = null;

                    switch (flag)
                    {
                        case KcpProtocalType.SYN:
                            {
                                if (messageLength < 9)
                                {
                                    break;
                                }

                                string realAddress = null;
                                remoteConn = BitConverter.ToUInt32(cache, 1);
                                if (messageLength > 9)
                                {

                                }
                            }
                            break;
                    }

                }
                catch (Exception e)
                {

                    throw;
                }
            }
        }

        public KChannel Get(long id)
        {
            return null;
        }

        public KChannel GetByLocalConn(uint localConn)
        {
            return null;
        }

        private void Disconnet(uint localConn, uint remoteConn, int error, IPEndPoint adress, int times)
        {

        }
        // 服务端需要看channel的update时间是否已到
        public void AddToUpdateNextTime(long time, long id)
        {

        }

        private void RemoveConnectTimeoutChannels()
        {

        }

        // 计算到期需要update的channel
        private void TimerOut()
        {

        }



        ///////////////////////////////////////
        ///////////Override Methods///////////
        /////////////////////////////////////
        public override void Dispose()
        {

        }

        public override bool IsDispose()
        {
            return false;
        }

        public override void Remove(long channelId)
        {

        }

        public override void Update()
        {

        }

        protected override void Get(long id, IPEndPoint adress)
        {

        }

        protected override void Send(long channelId, long actorId, MemoryStream stream)
        {

        }
    }
}