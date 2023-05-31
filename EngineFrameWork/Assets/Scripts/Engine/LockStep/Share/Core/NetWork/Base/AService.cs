using System;
using System.IO;
using System.Net;

namespace FrameWork.Service
{
    public abstract class AService : IDisposable
    {
        public Action<long, int> ErrorCallback;
        public Action<long, IPEndPoint> AccetpCallback;
        public Action<long, MemoryStream> ReadCallback;

        public ServiceType ServiceType { get; protected set; }
        public ThreadSynchronizationContext ThreadSynchronizationContext;       // 线程同步上下文


        private long acceptIdGenerater = 1;                                     // localConn放在低32bit
        private long connectIdGenerater = int.MaxValue;

        public long CreateConnectChannelID(uint localConn)
        {
            return (--connectIdGenerater << 32) | localConn;
        }

        public uint CreateRandomLocalConn()
        {
            return (1u << 30) | RandomHelper.RandUInt32();
        }

        public long CreateAccetpChannelId(uint localConn)
        {
            return (++acceptIdGenerater << 32) | localConn;
        }




        public abstract void Update();

        public abstract void Dispose();

        public abstract bool IsDispose();

        public abstract void Remove(long channelId);

        protected abstract void Get(long id, IPEndPoint adress);

        protected abstract void Send(long channelId, long actorId, MemoryStream stream);




        public void OnError(long channelId, int e)
        {
            Remove(channelId);

            ErrorCallback?.Invoke(channelId, e);
        }

        public void OnRead(long channelId, MemoryStream memoryStream)
        {
            ReadCallback?.Invoke(channelId, memoryStream);
        }

        protected void OnAccept(long channelId, IPEndPoint iPEndPoint)
        {
            AccetpCallback?.Invoke(channelId, iPEndPoint); ;
        }




        public void OnDetory() { Dispose(); }

        public void SendStream(long channelId, long actorId, MemoryStream memoryStream) { Send(channelId, actorId, memoryStream); }

        public void GetOrCreate(long id, IPEndPoint adress) { Get(id, adress); }

        public void RemoveChannel(long channelId) { Remove(channelId); }

    }
}