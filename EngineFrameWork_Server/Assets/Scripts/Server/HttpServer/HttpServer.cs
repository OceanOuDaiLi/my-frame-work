
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

            // 异步方法全部会回掉到主线程
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            /*
             * 管理员 cmd
             * cmd -> ipconfig 获取 IPv4
             * netsh http add urlacl url=http://+:8080/  user=Everyone
             * netsh http add urlacl url=http://127.0.0.1:8080/  user=Everyone
             * netsh http add urlacl url=http://192.168.12.155:8080/  user=Everyone
             * 
             * CMD配置防火墙
             * netsh advfirewall firewall Add rule name=\"命令行Web访问8080\" dir=in protocol=tcp localport=8080 action=allow
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
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;              //指定身份验证 Anonymous匿名访问
                    listerner.Prefixes.Add("http://192.168.12.155:8080/");
                    listerner.Start();
                }
                catch (Exception ex)
                {
                    SDebug.Info("服务启动失败..." + ex);
                    break;
                }
                SDebug.Info("服务器启动成功.......");

                //线程池
                int minThreadNum;
                int portThreadNum;
                int maxThreadNum;
                ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
                ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
                SDebug.LogFormat("最大线程数：{0}", maxThreadNum);
                SDebug.LogFormat("最小空闲线程数：{0}", minThreadNum);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc1), x);

                SDebug.Info("\n\n等待客户连接中。。。。");
                while (true)
                {
                    //等待请求连接
                    //没有请求则GetContext处于阻塞状态
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

            //接收POST参数
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


            //使用Writer输出http响应代码,UTF8格式
            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write(resultJson);
                writer.Close();

                ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");      //跨域？
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
            //    dicValue["msg"] = "注册成功";

            //    dic["data"] = dicValue;
            //}
            //else
            //{
            //    //ToDo:用户名已经存在，返回错误码
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
            //    dicValue["msg"] = "登录成功";

            //    dic["data"] = dicValue;
            //}
            //else
            //{
            //    //ToDo:用户名或密码错误
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