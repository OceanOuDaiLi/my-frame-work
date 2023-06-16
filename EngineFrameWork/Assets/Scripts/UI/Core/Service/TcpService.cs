using System;
using FrameWork;
using Core.Network;
using Core.Interface;
using System.Collections;
using Core.Interface.Event;
using Core.Interface.Network;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	TcpStateException.cs
	Author:		DaiLi.Ou

	Descriptions: TCP/IP socket service for client.
*********************************************************************/
namespace UI
{
    public delegate void MessageEventCallback(Dictionary<string, object> dict);

    class TcpStateException : Exception
    {
        public TcpStateException(string message) : base(message) { }
    }

    class RequestTask
    {
        internal string Action { get; set; }
        internal Dictionary<string, object> Parameters { get; set; }
        internal MessageEventCallback Callback { get; set; }
    }

    public enum TcpState : byte
    {
        Connecting,
        Connected,
        Closed
    }

    /// <summary>
    /// TCP连接
    /// </summary>
    public class TcpService : IDisposable
    {
        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public virtual IEventDispatcher dispatcher { get; set; }
        //[Inject]
        //public virtual GlobalData dataVO { get; set; }

        public event Action onTcpConnected = null;
        public event Action onTcpDisconnected = null;
        public event Action onTcpRestartGame = null;

        TcpRequest tcp;
        Dictionary<string, List<RequestTask>> requestQueue = new Dictionary<string, List<RequestTask>>();
        Dictionary<string, List<RequestTask>> resendQueue = new Dictionary<string, List<RequestTask>>();
        Dictionary<string, List<MessageEventCallback>> listenQueue = new Dictionary<string, List<MessageEventCallback>>();
        MessageEventCallback globalMessageEventCallback = null;

        string accessToken = string.Empty;
        IEventHandler eHandlerOnConnect = null;
        IEventHandler eHandlerOnMessage = null;
        IEventHandler eHandlerOnError = null;
        IEventHandler eHandlerOnClose = null;

        const float APP_PAUSE_TIME_MAX = 60f;
        const float HEARTBEAT_INTERVAL = 20f;
        const float HEARTBEAT_INTERVAL_SHORT = 10f;
        const int HEARTBEAT_MAX = 2;
        const string RECONNECT_ACTION = "/reconn";

        float heartbeatInterval = HEARTBEAT_INTERVAL;
        int heartbeat = 0;
        bool reconnecting = false;
        DateTime appPauseTime;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public TcpState State { get; private set; }

        UnityEngine.Coroutine heartbeatCoroutine = null;

        public TcpService()
        {
            eHandlerOnConnect = App.Instance.On(SocketRequestEvents.ON_CONNECT, OnConnect);
            eHandlerOnMessage = App.Instance.On(SocketRequestEvents.ON_MESSAGE, OnMessage);
            eHandlerOnError = App.Instance.On(SocketRequestEvents.ON_ERROR, OnError);
            eHandlerOnClose = App.Instance.On(SocketRequestEvents.ON_CLOSE, OnClose);

            //GameManager.Instance.onApplicationPause += OnApplicationPause;
            //GameManager.Instance.onApplicationQuit += OnApplicationQuit;

            appPauseTime = DateTime.Now;
            State = TcpState.Closed;
        }

        ~TcpService()
        {
            Dispose();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) appPauseTime = DateTime.Now;
            else
            {
                if ((DateTime.Now - appPauseTime).Seconds > APP_PAUSE_TIME_MAX)
                {
                    OnError(tcp, new ExceptionEventArgs(new TcpStateException("Client deactive too long")));
                }
            }
        }

        void OnApplicationQuit()
        {
            OnError(tcp, new ExceptionEventArgs(new TcpStateException("Client close")));
        }

        public void Dispose()
        {
            if (GameMgr.Ins != null)
            {
                //GameManager.Instance.onApplicationPause -= OnApplicationPause;
                //GameManager.Instance.onApplicationQuit -= OnApplicationQuit;
            }

            App.Net.Destroy(Network.NetworkRequestType.Tcp);

            eHandlerOnConnect.Cancel();
            eHandlerOnMessage.Cancel();
            eHandlerOnError.Cancel();
            eHandlerOnClose.Cancel();

            requestQueue.Clear();
            listenQueue.Clear();
            if (resendQueue != null) resendQueue.Clear();

            onTcpConnected = null;
            onTcpDisconnected = null;
            onTcpRestartGame = null;
        }

        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;

            requestQueue.Clear();
            listenQueue.Clear();
            if (resendQueue != null) resendQueue.Clear();

            App.Net.Destroy(Network.NetworkRequestType.Tcp);
            tcp = (TcpRequest)App.Net.Create<IConnectorTcp>(Network.NetworkRequestType.Tcp, new Hashtable()
                {
                    { Network.NetworkConfigKey.Host, host },
                    { Network.NetworkConfigKey.Port, port },
                    { Network.NetworkConfigKey.Packing, new Core.NetPackage.FramePacking() },
                    { Network.NetworkConfigKey.Protocol, new Core.NetPackage.ByteProtocol() }
                });

            this.State = TcpState.Connecting;

            // show loading rotate ui.
#if UNITY_EDITOR
            ZDebug.Log("Tcp 服务器开始连接");
#endif
            tcp.Connect();
        }

        public void Disconnect()
        {
            App.Net.Destroy(Network.NetworkRequestType.Tcp);
            RestartGame();
        }

        void RestartGame()
        {
            requestQueue.Clear();
            listenQueue.Clear();
            resendQueue.Clear();

            // hide loading rotate ui.

            //tcp restart.
            //if (onTcpRestartGame != null) onTcpRestartGame();

            //change scene
            //if (!dataVO.CurrentScene.Equals(Config.Scene_Start))
            //{

            //    LoadingStructure loadscene = LoadingStructure.LoadingDirective(Config.Scene_Start, true, true, 1.0f);
            //    dispatcher.Dispatch(LoadEvent.LOAD_SCENE, loadscene);
            //}
        }

        /// <summary>
        /// 设置AccessToken，登录完之后设置
        /// </summary>
        /// <param name="token"></param>
        public void SetAccessToken(string token)
        {
            this.accessToken = token;
        }

        public void AddListener(string action, MessageEventCallback callback)
        {
            if (listenQueue.ContainsKey(action))
            {
                List<MessageEventCallback> ls = listenQueue[action];
                ls.Add(callback);
            }
            else
            {
                List<MessageEventCallback> ls = new List<MessageEventCallback>();
                ls.Add(callback);
                listenQueue.Add(action, ls);
            }
        }

        public void RemoveListener(string action, MessageEventCallback callback)
        {
            if (listenQueue.ContainsKey(action))
            {
                List<MessageEventCallback> ls = listenQueue[action];
                ls.Remove(callback);
                if (ls.Count == 0) listenQueue.Remove(action);
            }
        }

        public void SetGlobalMessageEventCallback(MessageEventCallback callback)
        {
            globalMessageEventCallback = callback;
        }

        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="action">请求的api</param>
        /// <param name="param">请求的参数</param>
        /// <param name="callback">成功的回调</param>
        public void Request(string action, Dictionary<string, object> param, MessageEventCallback callback = null)
        {
            if (State != TcpState.Connected) return;

            if (callback != null)
            {
                if (requestQueue.ContainsKey(action))
                {
                    List<RequestTask> ls = requestQueue[action];
                    ls.Add(new RequestTask { Action = action, Parameters = param, Callback = callback });
                }
                else
                {
                    List<RequestTask> ls = new List<RequestTask>
                    {
                        new RequestTask { Action = action, Parameters = param, Callback = callback }
                    };
                    requestQueue.Add(action, ls);
                }
            }

            Dictionary<string, object> parameters = param ?? new Dictionary<string, object>();
            string jsonData = App.Json.Encode(parameters);
            string request = string.Format("{0} {1}", action, jsonData);

            byte[] originalBytes = System.Text.Encoding.UTF8.GetBytes(request);
            byte[] encryptBytes = App.Crypt.Encrypt(originalBytes);                 //Error warning: Todo =》 SetCrypy Iv & key. by daili.ou 2023.03.08
#if UNITY_EDITOR
            ZDebug.LogFormat("Tcp 服务器请求：{0}", request);
#endif
            tcp.Send(encryptBytes);

            if (requestQueue.Count > 0)
            {
                // show loading rotate ui.
            }
        }

        IEnumerator HeartbeatCoroutine()
        {
            while (true)
            {
                if (heartbeat <= 1) heartbeatInterval = HEARTBEAT_INTERVAL;
                else heartbeatInterval = HEARTBEAT_INTERVAL_SHORT;

                yield return Yielders.GetWaitForSeconds(heartbeatInterval);

                if (heartbeat >= HEARTBEAT_MAX)
                {
                    Reconnect();
                    break;
                }

                heartbeat++;

#if UNITY_EDITOR
                ZDebug.Log("Tcp 服务器心跳请求 :" + heartbeat);
#endif
                tcp.Send(new byte[1] { 1 });
            }
        }

        void Reconnect()
        {
            reconnecting = true;
            resendQueue = new Dictionary<string, List<RequestTask>>(requestQueue);
            requestQueue.Clear();
            Connect(this.Host, this.Port);
        }

        void OnConnect(object sender, EventArgs e)
        {
#if UNITY_EDITOR
            ZDebug.Log("Tcp 服务器连接成功");
#endif

            State = TcpState.Connected;
            // hide loading rotate ui.
            if (onTcpConnected != null) onTcpConnected();

            if (heartbeatCoroutine != null)
            {
                App.Instance.StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
            }
            heartbeatCoroutine = App.Instance.StartCoroutine(HeartbeatCoroutine());

            if (reconnecting)
            {
                Request(RECONNECT_ACTION, new Dictionary<string, object>() { { "token", accessToken } }, OnReconnect);
            }
        }

        void OnReconnect(Dictionary<string, object> data)
        {
            reconnecting = false;
            heartbeat = 0;
            ResendRequestTask();
        }

        void ResendRequestTask()
        {
            if (resendQueue.Count > 0)
            {
                foreach (string action in resendQueue.Keys)
                {
                    List<RequestTask> ls = resendQueue[action];
                    resendQueue.Remove(action);
                    foreach (RequestTask t in ls)
                    {
                        Request(action, t.Parameters, t.Callback);
                    }
                    break;
                }
            }
        }

        void OnMessage(object sender, EventArgs e)
        {
            PackageResponseEventArgs args = e as PackageResponseEventArgs;
            byte[] encryptBytes = args.Response.ToByte();
            if (encryptBytes.Length <= 1)
            {
#if UNITY_EDITOR
                ZDebug.Log("Tcp 服务器心跳响应");
#endif
                //心跳有回应的时候直接清零
                heartbeat = 0;
                return;
            }

            byte[] decryptBytes = App.Crypt.Decrypt(encryptBytes);                  //Error warning: Todo =》 SetCrypy Iv & key. by daili.ou 2023.03.08
            string response = System.Text.Encoding.UTF8.GetString(decryptBytes);

            int seperateIdx = response.IndexOf(' ');
            string action = string.Empty;
            if (seperateIdx > -1) action = response.Substring(0, seperateIdx);
            string json = response.Substring(seperateIdx + 1, response.Length - seperateIdx - 1);

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(action))
            {
                ZDebug.LogError(string.Format("Tcp 服务器返回: {0} {1}", action, json));
            }
            else
            {
                ZDebug.LogFormat("Tcp 服务器返回: {0} {1}", action, json);
            }
#endif

            Dictionary<string, object> dict = App.Json.Decode<Dictionary<string, object>>(json);
            bool err = CheckLogicError(action, dict);

            object data = null;
            dict.TryGetValue("data", out data);
            if (data == null) data = dict;

            HandleRequestTask(action, requestQueue, err, (Dictionary<string, object>)data, true);

            if (requestQueue.Count == 0 && resendQueue.Count == 0)
            {
                // hide loading rotate ui.
            }
            else if (resendQueue.Count > 0) ResendRequestTask();

            HandleListeningAction(action, err, (Dictionary<string, object>)data, false);
        }

        void HandleRequestTask(string action, Dictionary<string, List<RequestTask>> queue, bool err, Dictionary<string, object> data, bool clear)
        {
            List<RequestTask> ls = null;
            if (queue.TryGetValue(action, out ls))
            {
                try
                {
                    if (!err)
                    {
                        for (int i = 0; i < ls.Count; i++)
                        {
                            RequestTask c = ls[i];
                            c.Callback(data);
                        }
                    }
                }
                catch (Exception e) //捕捉callback里面的异常，避免网络卡死游戏进程
                {
#if UNITY_EDITOR
                    ZDebug.LogError(e);
#endif
                }
                if (clear)
                {
                    ls.Clear();
                    queue.Remove(action);
                }
            }
            else if (globalMessageEventCallback != null)
            {
                globalMessageEventCallback(data);
            }
        }

        void HandleListeningAction(string action, bool err, Dictionary<string, object> data, bool clear)
        {
            List<MessageEventCallback> ls = null;
            listenQueue.TryGetValue(action, out ls);
            if (ls != null)
            {
                try
                {
                    if (!err)
                    {
                        for (int i = 0; i < ls.Count; i++)
                        {
                            MessageEventCallback c = ls[i];
                            c(data);
                        }
                    }
                }
                catch (Exception e) //捕捉callback里面的异常，避免网络卡死游戏进程
                {
#if UNITY_EDITOR
                    ZDebug.LogError(e);
#endif
                }
                if (clear) ls.Clear();
            }
            if (clear) listenQueue.Remove(action);
        }

        void OnError(object sender, EventArgs e)
        {
            ExceptionEventArgs args = e as ExceptionEventArgs;
#if UNITY_EDITOR
            if (!args.Exception.GetType().Equals(typeof(TcpStateException)))
            {
                ZDebug.LogError(args.Exception);
            }
            else
            {
                ZDebug.Log(args.Exception);
            }
#endif

            //show restart ui.
            //MessageModel msgModel = new MessageModel("网络连接丢失，重启游戏吗？", "重启游戏", Disconnect);
            //dispatcher.Dispatch(MessageEvent.SHOW_MESSAGE, msgModel);
        }

        void OnClose(object sender, EventArgs e)
        {
#if UNITY_EDITOR
            ZDebug.Log("服务器连接关闭");
#endif

            State = TcpState.Closed;
            if (onTcpDisconnected != null) onTcpDisconnected();

            if (heartbeatCoroutine != null)
            {
                App.Instance.StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
            }
        }

        bool CheckLogicError(string action, Dictionary<string, object> dict)
        {
            object tmp = null;
            dict.TryGetValue("code", out tmp);
            if (tmp != null)
            {
                int code = (int)tmp;
                if (code == 0) return false;

                dict.TryGetValue("msg", out tmp);
                string msg = tmp as string;

                //    MessageModel msgModel = null;

                //    if (action.Equals(RECONNECT_ACTION))
                //    {
                //        reconnecting = false;
                //        heartbeat = 0;
                //        msgModel = new MessageModel("网络连接丢失，重启游戏吗？", "重启游戏", Disconnect);
                //        dispatcher.Dispatch(MessageEvent.SHOW_MESSAGE, msgModel);
                //        return true;
                //    }

                //    switch (code)
                //    {
                //        case 3:
                //            {
                //                msgModel = new MessageModel("您的账号已在别处登录，现在注销吗？", "注销", Disconnect);
                //            } break;
                //        case 5:
                //            {
                //                msgModel = new MessageModel("离线时间太长，请重启游戏", "重启", Disconnect);
                //            } break;
                //        default:
                //            {
                //                msgModel = new MessageModel(msg, "确定");
                //            } break;
                //    }
                //    dispatcher.Dispatch(MessageEvent.SHOW_MESSAGE, msgModel);
                //}
                //else {
                //    MessageModel msgModel = new MessageModel("服务器发生错误，重启游戏吗？", "重启游戏", Disconnect);
                //    dispatcher.Dispatch(MessageEvent.SHOW_MESSAGE, msgModel);
            }
            return true;
        }
    }
}
