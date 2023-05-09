
using ET;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;

namespace Server.Http
{
    public class HttpServer
    {
        public static AService Service;
        public static void OnStart()
        {
            SDebug.EnableLog = true;
            SDebug.Info("### HttpServer Start ###");

            // �첽����ȫ����ص������߳�
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            /*
             * ����Ա cmd
             * cmd -> ipconfig ��ȡ IPv4
             * netsh http add urlacl url=http://+:8080/  user=Everyone
             * netsh http add urlacl url=http://127.0.0.1:8080/  user=Everyone
             * netsh http add urlacl url=http://192.168.12.155:8080/  user=Everyone
             * 
             * CMD���÷���ǽ
             * netsh advfirewall firewall Add rule name=\"������Web����8080\" dir=in protocol=tcp localport=8080 action=allow
            */

            // way 1
            //Service = new WService(ThreadSynchronizationContext.Instance, new List<string>()
            //{
            //    "http://192.168.12.155:8080/"
            //});
            //Service.ErrorCallback += (channelId, error) => OnError(channelId, error);
            //Service.ReadCallback += (channelId, Memory) => OnRead(channelId, Memory);
            //Service.AcceptCallback += (channelId, IPAddress) => OnAccept(channelId, IPAddress);

            // way 2

            HttpListener listerner = new HttpListener();
            while (true)
            {
                try
                {
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;              //ָ�������֤ Anonymous��������
                    listerner.Prefixes.Add("http://192.168.12.155:8080/");
                    listerner.Start();
                }
                catch (Exception ex)
                {
                    SDebug.Info("��������ʧ��..." + ex);
                    break;
                }
                SDebug.Info("�����������ɹ�.......");

                //�̳߳�
                int minThreadNum;
                int portThreadNum;
                int maxThreadNum;
                ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
                ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
                SDebug.LogFormat("����߳�����{0}", maxThreadNum);
                SDebug.LogFormat("��С�����߳�����{0}", minThreadNum);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc1), x);

                SDebug.Info("\n\n�ȴ��ͻ������С�������");
                while (true)
                {
                    //�ȴ���������
                    //û��������GetContext��������״̬
                    HttpListenerContext ctx = listerner.GetContext();

                    ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
                }
                //listerner.Stop();
            }
        }

        static void TaskProc(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;

            ctx.Response.StatusCode = 200;
            string url = ctx.Request.Url.ToString();

            //����POST����
            Stream stream = ctx.Request.InputStream;
            System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
            String body = reader.ReadToEnd();
            SDebug.LogFormat(string.Format("POST: {0}  Body: {1}", url, body));


            string resultJson = string.Empty;

            if (url.Contains("/register"))
            {
                resultJson = ResponseRegiste(body);
            }
            else if (url.Contains("/login"))
            {
                resultJson = ResponseLogin(body);
            }


            //ʹ��Writer���http��Ӧ����,UTF8��ʽ
            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write(resultJson);
                writer.Close();

                ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");      //����
                ctx.Response.Close();
            }
        }

        static string ResponseRegiste(string body)
        {
            //string account = HttpUtility.ParseQueryString(body).Get("account");
            //string password = HttpUtility.ParseQueryString(body).Get("password");

            //bool isNewFromDb = true;
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //Dictionary<string, object> dicValue = new Dictionary<string, object>();
            //if (isNewFromDb)
            //{
            //    dicValue["succeed"] = true;
            //    dicValue["code"] = 200;
            //    dicValue["msg"] = "ע��ɹ�";

            //    dic["data"] = dicValue;
            //}
            //else
            //{
            //    //ToDo:�û����Ѿ����ڣ����ش�����
            //}

            //return JsonConvert.SerializeObject(dic);

            return body;
        }

        static string ResponseLogin(string body)
        {
            //string username = HttpUtility.ParseQueryString(body).Get("username");
            //string password = HttpUtility.ParseQueryString(body).Get("password");

            //bool approved = true;
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //Dictionary<string, object> dicValue = new Dictionary<string, object>();
            //if (approved)
            //{
            //    dicValue["succeed"] = true;
            //    dicValue["code"] = 200;
            //    dicValue["msg"] = "��¼�ɹ�";

            //    dic["data"] = dicValue;
            //}
            //else
            //{
            //    //ToDo:�û������������
            //}

            //return JsonConvert.SerializeObject(dic);

            return body;
        }

        //private static void OnAccept(long channelId, IPEndPoint iPAddress)
        //{
        //    SDebug.LogFormat($"OnAccept: channelId: {channelId} iPAddress:{iPAddress}");

        //}

        //private static void OnRead(long channelId, MemoryStream memory)
        //{
        //    SDebug.LogFormat($"OnRead: channelId: {channelId} MemoryStream:{memory.Length}");
        //}

        //private static void OnError(long channelId, int error)
        //{
        //    SDebug.LogFormat($"OnError: channelId: {channelId} error:{error}");
        //}
    }
}