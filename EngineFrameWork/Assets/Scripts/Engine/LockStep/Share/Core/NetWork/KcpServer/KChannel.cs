using System;
using System.IO;
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

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}