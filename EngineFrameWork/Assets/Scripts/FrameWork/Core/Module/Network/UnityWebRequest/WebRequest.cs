using System;
using FrameWork;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Core.Interface.Network;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	WebRequest.cs
	Author:		DaiLi.Ou

	Descriptions: Abstract class of Unity WebRequest.
*********************************************************************/
namespace Core.Network
{
    public class WebRequest : IConnectorHttp
    {
        public string Name { get; set; }

        /// <summary>
        /// 终止标记
        /// </summary>
        private bool stopMark = false;

        /// <summary>
        /// 服务器地址
        /// </summary>

        private string url;

        /// <summary>
        /// 发送队列
        /// </summary>
        private Queue<UnityWebRequest> queue = new Queue<UnityWebRequest>();

        private Hashtable triggerLevel;

        private Dictionary<string, string> headers;
        public Dictionary<string, string> Headers { get { return headers; } }

        public void SetConfig(Hashtable config)
        {

            if (config.ContainsKey(Network.NetworkConfigKey.Host))
            {
                url = config[Network.NetworkConfigKey.Host].ToString().TrimEnd('/');
            }

            if (config.ContainsKey(Network.NetworkConfigKey.Timeout))
            {
                throw new Exception(this.GetType().ToString() + " is not support [timeout] config");
            }

            if (config.ContainsKey("event.level"))
            {
                if (config["event.level"] is Hashtable)
                {
                    triggerLevel = config["event.level"] as Hashtable;
                }
            }
        }

        public IConnectorHttp SetHeader(Dictionary<string, string> headers)
        {
            this.headers = headers;
            return this;
        }

        public IConnectorHttp SetTimeOut(int timeout)
        {
            throw new Exception("this component is not support this features");
        }

        public IConnectorHttp AppendHeader(string header, string val)
        {
            if (headers == null) { headers = new Dictionary<string, string>(); }
            headers.Remove(header);
            headers.Add(header, val);
            return this;
        }

        public object Restful(Restfuls method, string action)
        {
            UnityWebRequest request = new UnityWebRequest(url + action, method.ToString());
            queue.Enqueue(request);
            return request;
        }

        public object Restful(Restfuls method, string action, WWWForm form)
        {
            if (method == Restfuls.Post)
            {
                UnityWebRequest request = UnityWebRequest.Post(url + action, form);
                queue.Enqueue(request);
                return request;
            }
            else
            {
                return Restful(method, action, form.data);
            }
        }

        public object Restful(Restfuls method, string action, byte[] body)
        {
            UnityWebRequest request = null;
            switch (method)
            {
                case Restfuls.Get: request = UnityWebRequest.Get(url + action); break;
                case Restfuls.Put: request = UnityWebRequest.Put(url + action, body); break;
                case Restfuls.Delete: request = UnityWebRequest.Delete(url + action); break;
                case Restfuls.Head: request = UnityWebRequest.Head(url + action); break;
                default: throw new Exception("this component is not support [" + method.ToString() + "] restful");
            }
            queue.Enqueue(request);
            return request;
        }

        public object Get(string action)
        {
            return Restful(Restfuls.Get, action);
        }

        public object Head(string action)
        {
            return Restful(Restfuls.Head, action);
        }

        public object Post(string action, WWWForm form)
        {
            return Restful(Restfuls.Post, action, form);
        }

        public object Post(string action, byte[] body)
        {
            return Restful(Restfuls.Post, action, body);
        }

        public object Put(string action, WWWForm form)
        {
            return Restful(Restfuls.Put, action, form);
        }

        public object Put(string action, byte[] body)
        {
            return Restful(Restfuls.Put, action, body);
        }

        public object Delete(string action)
        {
            return Restful(Restfuls.Delete, action);
        }

        /// <summary>
        /// 释放链接
        /// </summary>
        public void Destroy()
        {
            stopMark = true;
        }

        /// <summary>
        /// 请求到服务器
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartServer()
        {
            while (true)
            {
                if (stopMark) { break; }
                if (queue.Count > 0)
                {
                    UnityWebRequest request;
                    while (queue.Count > 0)
                    {
                        if (stopMark) { break; }
                        request = queue.Dequeue();
                        if (headers != null)
                        {
                            foreach (var kv in headers)
                            {
                                request.SetRequestHeader(kv.Key, kv.Value);
                            }
                        }
                        yield return request.Send();

                        var args = new WebRequestEventArgs(request);

                        App.Instance.Trigger(HttpRequestEvents.ON_MESSAGE, this, args);
                    }
                }
                yield return new WaitForEndOfFrame();
            }

        }
    }
}