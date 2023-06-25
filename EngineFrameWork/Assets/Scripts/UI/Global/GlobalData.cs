using UI;

namespace Model
{
    public class GlobalData
    {
        public static GlobalData instance = null;

        public TcpService tcpService = null;
        public HttpService httpService = null;

        public GlobalData()
        {
            if (instance != null)
            {
                CDebug.LogError("More than one GlobalData existed");
                instance = null;
            }

            instance = this;
        }

        ~GlobalData()
        {
            instance = null;
        }

        public void SetHttpService(string url)
        {
            if (httpService != null)
            {
                httpService.Dispose();
                httpService = null;
            }
            httpService = new HttpService(url);
            httpService.dispatcher = GameMgr.Ins.CrossDispatcher;
        }
    }
}