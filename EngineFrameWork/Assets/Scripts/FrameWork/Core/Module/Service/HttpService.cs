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

#if UNITY_EDITOR
    string url;
#endif

    public HttpService(string url)
    {
#if UNITY_EDITOR
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
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Http Get 发起请求: " + url + action);
#endif
            request = http.Get(action);
        }
        else
        {
            string jsonData = App.Json.Encode(param);
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Http Post 发起请求: " + url + action + "\n" + "ParamData:" + "\n" + jsonData);
#endif
        }

        if (callbackQueue.Count == 0)
        {
            // show rotateing img.
        }
        callbackQueue.Add(request, new HttpResponseExtraInfo(action, succeed));
    }

    void ResponseMessage(object Binder, EventArgs e)
    {
        HttpRequestEventArgs response = e as HttpRequestEventArgs;

#if UNITY_EDITOR
        ZDebug.Log("Http 服务器返回: " + response.Text);
#endif

        HttpResponseExtraInfo extraInfo = null;
        if (callbackQueue.TryGetValue(response.Request, out extraInfo))
        {
            Dictionary<string, object> dict = App.Json.Decode<Dictionary<string, object>>(response.Text);

            bool err = CheckLogicError(dict);

            if (!err)
            {
                object data = null;
                dict.TryGetValue("data", out data);
                if (data == null) data = dict;
                extraInfo.callback((Dictionary<string, object>)data);
            }

            callbackQueue.Remove(response.Request);
            actionQueue.Remove(extraInfo.action);
        }

        if (callbackQueue.Count == 0)
        {
            // dispare rotateing img.
        }
    }

    bool CheckLogicError(Dictionary<string, object> dict)
    {
        if (dict == null) return true;

        object tmp = null;
        dict.TryGetValue("code", out tmp);
        if (tmp != null)
        {
            int code = (int)tmp;

            dict.TryGetValue("msg", out tmp);
            string msg = tmp as string;
            //Todo: Show Error Msg Alert .
            switch (code)
            {
                case 404:
                    return false;
                case 400:
                    return true;
            }
        }

        return true;
    }
}
