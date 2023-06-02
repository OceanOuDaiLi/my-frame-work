using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;

namespace FrameWork.Service
{
    public class TService : AService
    {
        private Socket acceptor;
        private readonly Action<long> foreachAction;
        private readonly SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private readonly Dictionary<long, TChannel> idChannels = new Dictionary<long, TChannel>();

        public HashSet<long> NeedStartSend = new HashSet<long>();

        public TService(ThreadSynchronizationContext threadSynchronizationContext, ServiceType serviceType)
        {
            foreachAction = channelId =>
            {
                TChannel tChannel = Get(channelId);
                tChannel?.Update();
            };

            base.ServiceType = serviceType;
            base.ThreadSynchronizationContext = threadSynchronizationContext;
        }

        public TService(ThreadSynchronizationContext threadSynchronizationContext, IPEndPoint iPEndPoint, ServiceType serviceType)
        {
            foreachAction = channelId =>
            {
                TChannel tChannel = Get(channelId);
                tChannel?.Update();
            };

            base.ServiceType = serviceType;
            base.ThreadSynchronizationContext = threadSynchronizationContext;

            this.acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.acceptor.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, this);

            innArgs.Completed += OnComplete;

            this.acceptor.Bind(iPEndPoint);
            this.acceptor.Listen(1000);

            this.ThreadSynchronizationContext.PostNext(AcceptAsync);
        }

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    SocketError socketError = e.SocketError;
                    Socket acceptSocket = e.AcceptSocket;
                    base.ThreadSynchronizationContext.Post(() =>
                    {
                        OnAcceptComplete(socketError, acceptSocket);
                    });
                    break;
                //case SocketAsyncOperation.Connect:
                //    break;
                //case SocketAsyncOperation.Disconnect:
                //    break;
                //case SocketAsyncOperation.None:
                //    break;
                //case SocketAsyncOperation.Receive:
                //    break;
                //case SocketAsyncOperation.ReceiveFrom:
                //    break;
                //case SocketAsyncOperation.ReceiveMessageFrom:
                //    break;
                //case SocketAsyncOperation.Send:
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

        ///////////////////////////////////////
        ///////////Private Methods////////////
        /////////////////////////////////////
        private void AcceptAsync()
        {
            innArgs.AcceptSocket = null;
            if (acceptor.AcceptAsync(innArgs))
            {
                return;
            }
            OnAcceptComplete(innArgs.SocketError, innArgs.AcceptSocket);
        }

        private void OnAcceptComplete(SocketError socketError, Socket acceptSocket)
        {
            if (acceptor == null)
            {
                return;
            }

            if (socketError != SocketError.Success)
            {
                SDebug.LogError($"accept error {socketError}");
                return;
            }

            try
            {
                long id = base.CreateAccetpChannelId(0);
                TChannel channel = new TChannel(id, acceptSocket, this);
                idChannels.Add(channel.Id, channel);
                long channelId = channel.Id;

                base.OnAccept(channelId, channel.RemoteAddress);
            }
            catch (Exception e)
            {
                SDebug.LogError(e);
            }

            // 开始新的accept
            AcceptAsync();
        }

        private TChannel Get(long id)
        {
            TChannel channel = null;
            idChannels.TryGetValue(id, out channel);
            return channel;
        }

        private TChannel Create(IPEndPoint iPEndPoint, long id)
        {
            TChannel channel = new TChannel(id, iPEndPoint, this);
            idChannels.Add(channel.Id, channel);
            return channel;
        }

        ///////////////////////////////////////
        ///////////Override Methods///////////
        /////////////////////////////////////
        public override void Update()
        {
            NeedStartSend.Foreach(foreachAction);
            NeedStartSend.Clear();
        }

        public override void Dispose()
        {
            acceptor?.Close();
            acceptor = null;

            innArgs.Dispose();
            base.ThreadSynchronizationContext = null;
            foreach (long id in idChannels.Keys.ToArray())
            {
                TChannel channel = idChannels[id];
                channel.Dispose();
            }

            idChannels.Clear();
        }

        public override bool IsDispose()
        {
            return base.ThreadSynchronizationContext == null;
        }

        public override void Remove(long id)
        {
            if (idChannels.TryGetValue(id, out TChannel channel))
            {
                channel.Dispose();
            }

            idChannels.Remove(id);
        }

        protected override void Get(long id, IPEndPoint adress)
        {
            if (idChannels.TryGetValue(id, out TChannel _))
            {
                return;
            }
            Create(adress, id);
        }

        protected override void Send(long channelId, long actorId, MemoryStream stream)
        {
            try
            {
                TChannel aChannel = Get(channelId);
                if (aChannel == null)
                {
                    base.OnError(channelId, ErrorCore.ERR_SendMessageNotFoundTChannel);
                    return;
                }
                aChannel.Send(actorId, stream);
            }
            catch (Exception e)
            {

                SDebug.LogError(e);
            }
        }

        #endregion

    }
}