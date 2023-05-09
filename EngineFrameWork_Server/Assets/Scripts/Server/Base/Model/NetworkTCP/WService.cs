using System;
using FrameWork;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Collections.Generic;

namespace ET
{
    public class WService : AService
    {
        private long idGenerater = 200000000;

        private HttpListener httpListener;

        private readonly Dictionary<long, WChannel> channels = new Dictionary<long, WChannel>();

        public WService(ThreadSynchronizationContext threadSynchronizationContext, IEnumerable<string> prefixs)
        {
            this.ThreadSynchronizationContext = threadSynchronizationContext;

            this.httpListener = new HttpListener();
            this.httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;           //指定身份验证 Anonymous匿名访问

            StartAccept(prefixs).Coroutine();
        }

        public WService(ThreadSynchronizationContext threadSynchronizationContext)
        {
            this.ThreadSynchronizationContext = threadSynchronizationContext;
        }

        private long GetId
        {
            get
            {
                return ++this.idGenerater;
            }
        }

        public WChannel Create(string address, long id)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            WChannel channel = new WChannel(id, webSocket, address, this);
            this.channels[channel.Id] = channel;
            return channel;
        }

        public override void Remove(long id)
        {
            WChannel channel;
            if (!this.channels.TryGetValue(id, out channel))
            {
                return;
            }

            this.channels.Remove(id);
            channel.Dispose();
        }

        public override bool IsDispose()
        {
            return this.ThreadSynchronizationContext == null;
        }

        protected void Get(long id, string address)
        {
            if (!this.channels.TryGetValue(id, out _))
            {
                this.Create(address, id);
            }
        }

        public override void Dispose()
        {
            this.ThreadSynchronizationContext = null;
            this.httpListener?.Close();
            this.httpListener = null;
        }

        private async ETTask StartAccept(IEnumerable<string> prefixs)
        {
            try
            {
                foreach (string prefix in prefixs)
                {
                    this.httpListener.Prefixes.Add(prefix);
                }

                httpListener.Start();

                while (true)
                {
                    try
                    {
                        HttpListenerContext httpListenerContext = await this.httpListener.GetContextAsync();

                        HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

                        WChannel channel = new WChannel(this.GetId, webSocketContext, this);

                        this.channels[channel.Id] = channel;

                        this.OnAccept(channel.Id, channel.RemoteAddress);

                    }
                    catch (Exception e)
                    {
                        SDebug.LogError("服务启动失败: " + "\n\n" + e);
                    }

                    SDebug.Info("服务器启动成功 \n\n 等待客户连接...");
                }

            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    throw new Exception($"CMD管理员中输入: netsh http add urlacl url=http://*:8080/ user=Everyone", e);
                }

                SDebug.LogError(e);
            }
            catch (Exception e)
            {
                SDebug.LogError(e);
            }
        }

        protected override void Get(long id, IPEndPoint address)
        {
            throw new NotImplementedException();
        }

        protected override void Send(long channelId, long actorId, MemoryStream stream)
        {
            this.channels.TryGetValue(channelId, out WChannel channel);
            if (channel == null)
            {
                return;
            }
            channel.Send(stream);
        }

        public override void Update()
        {
        }
    }
}