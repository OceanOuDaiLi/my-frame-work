using System;
using System.IO;
using System.Net;

namespace FrameWork.Service
{
    public enum ServiceType
    {
        Outer,
        Inner,
    }

    public enum ChannelType
    {
        Connect,
        Accept,
    }

    public struct Packet
    {
        public const int MinPacketSize = 2;

        public const int OpcodeIndex = 8;
        public const int OpcodeLength = 2;

        public const int KcpOpcodeIndex = 0;

        public const int ActorIdLength = 0;
        public const int ActorILength = 8;

        public const int MessageIndex = 10;


        public ushort Opcode;
        public long ActorId;
        public MemoryStream MemoryStream;
    }

    public abstract class AChannel : IDisposable
    {
        public long Id;

        public int Error { get; set; }

        public IPEndPoint RemoteAddress { get; set; }

        public ChannelType ChannelType { get; protected set; }

        public bool IsDisposed
        {
            get => Id == 0;
        }

        public abstract void Dispose();
    }
}