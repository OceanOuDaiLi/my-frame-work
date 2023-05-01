using System;
using FrameWork;
using Core.Network;
using System.Collections;
using Core.Interface.Event;
using Core.Interface.Network;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	HttpResponseExtraInfo.cs
	Author:		DaiLi.Ou

	Descriptions: Http service for client.
*********************************************************************/
internal class HttpResponseExtraInfo
{
    public string action;
    public Action<Dictionary<string, object>> callback;
    public Action<byte[]> resCallBack;

    public HttpResponseExtraInfo(string action, Action<Dictionary<string, object>> callback)
    {
        this.action = action;
        this.callback = callback;
    }

    public HttpResponseExtraInfo(string action, Action<byte[]> callback)
    {
        this.action = action;
        resCallBack = callback;
    }
}

public class HttpService : IDisposable
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    IConnectorHttp http;
    List<string> actionQueue = new List<string>();
    Dictionary<object, HttpResponseExtraInfo> callbackQueue = new Dictionary<object, HttpResponseExtraInfo>();

    IEventHandler eHandlerOnMessage = null;

    public delegate void ManagerErrorEventHandler();

#if UNITY_EDITOR || __LOG__
    string url;
#endif

    public HttpService(string url)
    {
#if UNITY_EDITOR || __LOG__
        this.url = url;
#endif
        eHandlerOnMessage = App.Instance.On(HttpRequestEvents.ON_MESSAGE, ResponseMessage);

        App.Net.Destroy(Network.NetworkRequestType.Http);
        http = App.Net.Create<IConnectorHttp>(Network.NetworkRequestType.Http, new Hashtable()
        {
            { Network.NetworkConfigKey.Host, url  },
            { Network.NetworkConfigKey.Timeout, 10000 },
            { Network.NetworkConfigKey.ContentType, "application/json" },
            { Network.NetworkConfigKey.Accept, "*/*" }
        });
    }

    ~HttpService()
    {
        Dispose();
    }

    public void Dispose()
    {
        App.Net.Destroy(Network.NetworkRequestType.Http);
        if (eHandlerOnMessage != null)
        {
            eHandlerOnMessage.Cancel();
            eHandlerOnMessage = null;
        }

        callbackQueue.Clear();
        actionQueue.Clear();
    }


    /// <summary>
    /// 发起请求
    /// </summary>
    /// <param name="action">请求的api</param>
    /// <param name="param">请求的参数</param>
    /// <param name="succeed">成功的回调</param>
    /// <param name="filed">失败的回调</param>
    public void Request(string action, Dictionary<string, object> param, Action<Dictionary<string, object>> succeed)
    {
        if (actionQueue.Contains(action)) return;
        actionQueue.Add(action);

        object request = null;
        if (param == null)
        {
#if UNITY_EDITOR || __LOG__
            UnityEngine.Debug.Log("Http Get 发起请求: " + url + action);
#endif
            request = http.Get(action);
        }
        else
        {
            // http.Post
        }

        callbackQueue.Add(request, new HttpResponseExtraInfo(action, succeed));
    }

    public void RequestRes(string action, Action<byte[]> succeed)
    {
        if (actionQueue.Contains(action)) return;
        actionQueue.Add(action);

#if UNITY_EDITOR|| __LOG__
        UnityEngine.Debug.Log("Http Get Res 发起请求: " + url + action);
#endif
        object request = http.Get(action);

        callbackQueue.Add(request, new HttpResponseExtraInfo(action, succeed));
    }

    void ResponseMessage(object Binder, EventArgs e)
    {
        try
        {
            HttpRequestEventArgs response = e as HttpRequestEventArgs;
            HttpResponseExtraInfo extraInfo = null;

            if (callbackQueue.TryGetValue(response.Request, out extraInfo))
            {
                if (extraInfo.callback != null)
                {

#if UNITY_EDITOR || __LOG__
                    UnityEngine.Debug.Log("Http response text: " + response.Text);
#endif

                    Dictionary<string, object> result = new Dictionary<string, object>();
                    result["data"] = response.Text;
                    extraInfo.callback(result);
                }
                else if (extraInfo.resCallBack != null)
                {
#if UNITY_EDITOR || __LOG__
                    UnityEngine.Debug.Log("Http response Bytes len: " + response.Bytes.Length);
#endif
                    extraInfo.resCallBack(response.Bytes);
                }
                else
                {
                    // unKnowable callback
                }

                callbackQueue.Remove(response.Request);
                actionQueue.Remove(extraInfo.action);
            }
        }
        catch (Exception excep)
        {
            UnityEngine.Debug.Log("Exception : " + excep);
        }
    }
}
