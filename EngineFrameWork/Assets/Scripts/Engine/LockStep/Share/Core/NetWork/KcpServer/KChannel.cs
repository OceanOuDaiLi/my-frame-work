using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace FrameWork.Service
{
    public struct KcpWaitPacket
    {
        public long ActorId;
        public MemoryStream MemoryStream;
    }

    public class KChannel : AChannel
    {
        public static readonly Dictionary<IntPtr, KChannel> KcpPtrChannels = new Dictionary<IntPtr, KChannel>();

        public KService Service;

        private Socket socket;

        public IntPtr kcp { get; private set; }

        private readonly Queue<KcpWaitPacket> sendBuffer = new Queue<KcpWaitPacket>();

        private uint lastRecvTime;

        public readonly uint CreateTime;

        public uint LocalConn { get; set; }
        public uint RemoteConn { get; set; }

        private readonly byte[] sendCache = new byte[1024 * 2];

        public bool IsConnected { get; private set; }

        public string RealAdress { get; set; }

        private const int maxPacketSize = 10000;

        private MemoryStream ms = new MemoryStream(maxPacketSize);

        private MemoryStream readMemory;
        private int needReadSplitCount;

        private void InitKcp()
        {
            KcpPtrChannels.Add(kcp, this);
            switch (Service.ServiceType)
            {
                case ServiceType.Inner:
                    Kcp.KcpNodelay(kcp, 1, 10, 2, 1);
                    Kcp.KcpWndsize(kcp, ushort.MaxValue, ushort.MaxValue);
                    Kcp.KcpSetmtu(kcp, 1400);                                   //默认1400
                    Kcp.KcpSetminrto(kcp, 30);
                    break;
                case ServiceType.Outer:
                    Kcp.KcpNodelay(kcp, 1, 10, 2, 1);
                    Kcp.KcpWndsize(kcp, 256, 256);
                    Kcp.KcpSetmtu(kcp, 470);
                    Kcp.KcpSetminrto(kcp, 30);
                    break;
            }
        }

        // connect
        public KChannel(long id, uint localConn, Socket socket, IPEndPoint remoteEndPoint, KService kService)
        {
            //this.LocalConn = localConn;

            //base.Id = id;
            //base.ChannelType = ChannelType.Connect;

            //SDebug.LogFormat($"channel create: {base.Id} {this.LocalConn} {remoteEndPoint} {base.ChannelType}");

            //kcp = IntPtr.Zero;
            //Service = kService;
            //this.socket = socket;
            //RemoteAddress = remoteEndPoint;
            //lastRecvTime = kService.
        }

        // accept
        public KChannel(long id, uint localConn, uint remoteConn, Socket socket, IPEndPoint remoteEndPoint, KService kService)
        {

        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void HandleConnect() { }

        /// <summary>
        /// 发送请求连接消息
        /// </summary>
        private void Connect() { }

        public void Update() { }

        public void HandleRecv(byte[] data, int offset, int length) { }

        public void Output(IntPtr bytes, int count) { }

        private void KcpSend(KcpWaitPacket kcpWaitPacket) { }

        public void Send(long actorId, MemoryStream stream) { }

        private void OnRead(MemoryStream memoryStream) { }

        public void OnError(int error) { }
    }
}