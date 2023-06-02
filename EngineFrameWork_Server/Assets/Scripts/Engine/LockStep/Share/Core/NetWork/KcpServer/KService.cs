
using System.IO;
using System.Net;

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