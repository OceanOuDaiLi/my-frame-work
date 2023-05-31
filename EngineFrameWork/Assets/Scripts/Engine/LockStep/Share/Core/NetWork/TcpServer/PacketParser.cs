
using System;
using System.IO;

namespace FrameWork.Service
{
    public enum ParserState
    {
        PacketSize,
        PacketBocy,
    }

    public class PacketParser
    {
        public AService service;
        public MemoryStream MemoryStream;
        public const int InnerPacketSizeLength = 4;
        public const int OuterPacketSizeLength = 2;

        private int packetSize;
        private ParserState state;
        private readonly CircularBuffer buffer;
        private readonly byte[] cache = new byte[8];

        public PacketParser(CircularBuffer buffer, AService service)
        {
            this.buffer = buffer;
            this.service = service;
        }

        public bool Parse()
        {
            while (true)
            {
                switch (state)
                {
                    case ParserState.PacketSize:
                        {

                            if (service.ServiceType.Equals(ServiceType.Inner))
                            {
                                if (buffer.Length < InnerPacketSizeLength)
                                {
                                    return false;
                                }

                                buffer.Read(cache, 0, InnerPacketSizeLength);
                                packetSize = BitConverter.ToInt32(cache, 0);

                                if (packetSize > ushort.MaxValue * 16 || packetSize < Packet.MinPacketSize)
                                {
                                    throw new Exception($"recv packet size error, 可能是外网探测端口: {packetSize}");
                                }
                            }
                            else
                            {
                                if (buffer.Length < OuterPacketSizeLength)
                                {
                                    return false;
                                }

                                buffer.Read(cache, 0, OuterPacketSizeLength);
                                packetSize = BitConverter.ToUInt16(cache, 0);

                                if (packetSize < Packet.MinPacketSize)
                                {
                                    throw new Exception($"recv packet size error, 可能是外网探测端口: {packetSize}");
                                }

                                state = ParserState.PacketBocy;
                            }

                        }
                        break;
                    case ParserState.PacketBocy:
                        {
                            if (buffer.Length < packetSize)
                            {
                                return false;
                            }

                            MemoryStream memoryStream = new MemoryStream(packetSize);
                            buffer.Read(memoryStream, packetSize);
                            this.MemoryStream = memoryStream;

                            if (service.ServiceType.Equals(ServiceType.Inner))
                            {
                                memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
                            }
                            else
                            {
                                memoryStream.Seek(Packet.OpcodeLength, SeekOrigin.Begin);
                            }

                            state = ParserState.PacketSize;
                            return true;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

    }
}