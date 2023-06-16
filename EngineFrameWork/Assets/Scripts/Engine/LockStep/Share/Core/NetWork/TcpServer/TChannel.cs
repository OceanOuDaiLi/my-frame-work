using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FrameWork.Service
{
    /// <summary>
    /// 封装Socket,将回调push回主线程处理
    /// </summary>
    public class TChannel : AChannel
    {
        private bool isSending;
        private bool isConnected;


        private Socket socket;
        private readonly TService Service;
        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly PacketParser parser;
        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();
        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorILength];

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    Service.ThreadSynchronizationContext.Post(() =>
                    {
                        OnConnectComplete(e);
                    });
                    break;
                case SocketAsyncOperation.Receive:
                    Service.ThreadSynchronizationContext.Post(() =>
                    {
                        OnRecvComplete(e);
                    });
                    break;
                case SocketAsyncOperation.Send:
                    Service.ThreadSynchronizationContext.Post(() =>
                    {
                        OnSendComplete(e);
                    });
                    break;
                case SocketAsyncOperation.Disconnect:
                    Service.ThreadSynchronizationContext.Post(() =>
                    {
                        OnDisconnectComplete(e);
                    });
                    break;
                //case SocketAsyncOperation.Accept:
                //    break;
                //case SocketAsyncOperation.None:
                //    break;
                //case SocketAsyncOperation.ReceiveFrom:
                //    break;
                //case SocketAsyncOperation.ReceiveMessageFrom:
                //    break;
                //case SocketAsyncOperation.SendPackets:
                //    break;
                //case SocketAsyncOperation.SendTo:
                //    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        #region 网络线程

        public TChannel(long id, IPEndPoint iPEndPoint, TService service)
        {
            base.Id = id;
            base.RemoteAddress = iPEndPoint;
            base.ChannelType = ChannelType.Connect;

            this.Service = service;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;

            this.parser = new PacketParser(recvBuffer, Service);
            this.innArgs.Completed += OnComplete;
            this.outArgs.Completed += OnComplete;

            this.RemoteAddress = iPEndPoint;
            this.isConnected = false;
            this.isSending = false;

            this.Service.ThreadSynchronizationContext.Post(this.ConnectAsync);
        }

        public TChannel(long id, Socket socket, TService service)
        {
            base.Id = id;
            base.ChannelType = ChannelType.Accept;
            base.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;

            this.Service = service;
            this.socket = socket;
            this.socket.NoDelay = true;

            this.parser = new PacketParser(recvBuffer, Service);
            this.innArgs.Completed += OnComplete;
            this.outArgs.Completed += OnComplete;

            this.isConnected = true;
            this.isSending = false;

            // 下一帧开始读写
            Service.ThreadSynchronizationContext.PostNext(() =>
            {
                StartRecv();
                StartSend();
            });
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            SDebug.LogFormat($"channel dispose: {base.Id} {RemoteAddress}");

            long id = base.Id;
            base.Id = 0;
            Service.Remove(id);
            socket.Close();

            innArgs.Dispose();
            outArgs.Dispose();

            socket = null;
            innArgs = null;
            outArgs = null;
        }

        public void Update()
        {
            StartSend();
        }

        public void Send(long actorId, MemoryStream stream)
        {
            if (IsDisposed)
            {
                throw new Exception("TChannel 已经被Dispose，不能发送信息");
            }

            switch (Service.ServiceType)
            {
                case ServiceType.Inner:
                    int messageSize = (int)(stream.Length - stream.Position);
                    if (messageSize > ushort.MaxValue * 16)
                    {
                        throw new Exception($"send packet too large=> len: {stream.Length} pos: {stream.Position}");
                    }

                    sendCache.WriteTo(0, messageSize);
                    sendBuffer.Write(sendCache, 0, PacketParser.InnerPacketSizeLength);

                    // actorId
                    stream.GetBuffer().WriteTo(0, actorId);
                    sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                    break;

                case ServiceType.Outer:

                    stream.Seek(Packet.ActorILength, SeekOrigin.Begin);         //外网不需要actorId

                    ushort msgSize = (ushort)(stream.Length - stream.Position);

                    sendCache.WriteTo(0, msgSize);
                    sendBuffer.Write(sendCache, 0, PacketParser.OuterPacketSizeLength);

                    sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                    break;
            }

            if (!isSending)
            {
                Service.NeedStartSend.Add(base.Id);
            }
        }




        private void StartSend()
        {
            if (!this.isConnected)
            {
                return;
            }

            if (this.isSending)
            {
                return;
            }

            while (true)
            {
                try
                {
                    if (socket == null)
                    {
                        isSending = false;
                        return;
                    }

                    // 没用数据需要发送
                    if (sendBuffer.Length == 0)
                    {
                        isSending = false;
                        return;
                    }

                    isSending = true;

                    int sendSize = sendBuffer.ChunkSize - sendBuffer.FirstIndex;
                    if (sendSize > sendBuffer.Length)
                    {
                        sendSize = (int)sendBuffer.Length;
                    }

                    outArgs.SetBuffer(sendBuffer.First, sendBuffer.FirstIndex, sendSize);

                    if (socket.SendAsync(outArgs))
                    {
                        return;
                    }

                    HandleSend(outArgs);
                }
                catch (Exception e)
                {
                    throw new Exception($"socket set buffer error: {sendBuffer.First.Length}, {sendBuffer.FirstIndex}", e);
                }
            }
        }

        private void HandleSend(object o)
        {
            if (socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                OnError((int)e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                OnError(ErrorCore.ERR_PeerDisconnect);
                return;
            }

            sendBuffer.FirstIndex += e.BytesTransferred;
            if (sendBuffer.FirstIndex == sendBuffer.ChunkSize)
            {
                sendBuffer.FirstIndex = 0;
                sendBuffer.RemoveFirst();
            }
        }




        private void StartRecv()
        {
            while (true)
            {
                try
                {
                    if (socket == null)
                    {
                        return;
                    }

                    int size = recvBuffer.ChunkSize - recvBuffer.LastIndex;
                    innArgs.SetBuffer(recvBuffer.Last, recvBuffer.LastIndex, size);
                }
                catch (Exception e)
                {
                    SDebug.LogError($"[TChannel] OnError: {base.Id}\n{e}");
                    OnError(ErrorCore.ERR_TChannelRecvError);

                    return;
                }

                if (this.socket.ReceiveAsync(this.innArgs))
                {
                    return;
                }
                this.HandleRecv(innArgs);
            }
        }

        private void HandleRecv(object o)
        {
            if (socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                OnError((int)e.SocketError);
                return;
            }

            recvBuffer.LastIndex += e.BytesTransferred;
            if (recvBuffer.LastIndex == recvBuffer.ChunkSize)
            {
                recvBuffer.AddLast();
                recvBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                // 这里循环解析消息执行，有可能，执行消息的过程中断开了 session
                if (socket == null)
                {
                    return;
                }

                try
                {
                    bool ret = parser.Parse();
                    if (!ret)
                    {
                        break;
                    }

                    OnRead(parser.MemoryStream);
                }
                catch (Exception exc)
                {
                    SDebug.LogError($"ip: {RemoteAddress} {exc}");
                    OnError(ErrorCore.ERR_SocketError);
                    return;
                }
            }
        }

        private void OnRead(MemoryStream memoryStream)
        {
            try
            {
                long channelId = base.Id;
                Service.OnRead(channelId, memoryStream);
            }
            catch (Exception e)
            {
                SDebug.LogError($"{RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                OnError(ErrorCore.ERR_PacketParserError);
            }
        }




        private void ConnectAsync()
        {
            outArgs.RemoteEndPoint = RemoteAddress;
            if (socket.ConnectAsync(outArgs))
            {
                return;
            }
            OnConnectComplete(outArgs);
        }

        private void OnRecvComplete(object o)
        {
            HandleRecv(o);

            if (socket == null)
            {
                return;
            }

            StartRecv();
        }

        private void OnSendComplete(object o)
        {
            HandleSend(o);

            isSending = false;

            StartSend();
        }

        private void OnConnectComplete(object o)
        {
            if (socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }

            e.RemoteEndPoint = null;
            this.isConnected = true;
            this.StartRecv();
            this.StartSend();
        }

        private void OnDisconnectComplete(object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            OnError((int)e.SocketError);
        }

        private void OnError(int error)
        {
            SDebug.LogError($"[TChannel] OnError: {error} {RemoteAddress}");

            long channelId = base.Id;
            this.Service.Remove(channelId);
            this.Service.OnError(channelId, error);
        }

        #endregion
    }
}
