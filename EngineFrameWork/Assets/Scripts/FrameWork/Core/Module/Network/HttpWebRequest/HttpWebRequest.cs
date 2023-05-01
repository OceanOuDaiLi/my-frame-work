using System;
using FrameWork;
using System.Net;
using UnityEngine;
using System.Collections;
using Core.Interface.Network;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	HttpWebRequest.cs
	Author:		DaiLi.Ou

	Descriptions: Abstract class of Http Web Request.
*********************************************************************/
namespace Core.Network
{
    public sealed class HttpWebRequest : IConnectorHttp
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Cookie容器
        /// </summary>
        private readonly CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// 停止标记
        /// </summary>
        private bool stopMark;

        /// <summary>
        /// 服务器地址
        /// </summary>
        private string url;

        /// <summary>
        /// 超时
        /// </summary>
        private int timeout;

        /// <summary>
        /// 发送队列
        /// </summary>
        private readonly Queue<HttpWebRequestEntity> queue = new Queue<HttpWebRequestEntity>();

        /// <summary>
        /// 事件等级
        /// </summary>
        private Hashtable triggerLevel;

        /// <summary>
        /// 头信息
        /// </summary>
        private Dictionary<string, string> headers;

        /// <summary>
        /// 头信息
        /// </summary>
        public Dictionary<string, string> Headers { get { return headers; } }

        /// <summary>
        /// 请求内容类型
        /// </summary>
        private string contentType;

        /// <summary>
        /// 接收内容类型
        /// </summary>
        private string accept;

        /// <summary>
        /// 设定配置
        /// </summary>
        /// <param name="config">配置信息</param>
        public void SetConfig(Hashtable config)
        {
            if (config.ContainsKey(Network.NetworkConfigKey.Host))
            {
                url = config[Network.NetworkConfigKey.Host].ToString().TrimEnd('/');
            }

            if (config.ContainsKey(Network.NetworkConfigKey.Timeout))
            {
                timeout = Convert.ToInt32(config[Network.NetworkConfigKey.Timeout].ToString());
            }

            if (config.ContainsKey(Network.NetworkConfigKey.ContentType))
            {
                contentType = Convert.ToString(config[Network.NetworkConfigKey.ContentType]);
            }
            else
            {
                contentType = "application/octet-stream";
            }

            if (config.ContainsKey(Network.NetworkConfigKey.Accept))
            {
                accept = Convert.ToString(config[Network.NetworkConfigKey.Accept]);
            }
            else
            {
                accept = "application/octet-stream";
            }

            if (config.ContainsKey("event.level"))
            {
                if (config["event.level"] is Hashtable)
                {
                    triggerLevel = config["event.level"] as Hashtable;
                }
            }
        }

        /// <summary>
        /// 设定头
        /// </summary>
        /// <param name="headers">头信息</param>
        /// <returns>当前实例</returns>
        public IConnectorHttp SetHeader(Dictionary<string, string> headers)
        {
            this.headers = headers;
            return this;
        }

        /// <summary>
        /// 追加头
        /// </summary>
        /// <param name="header">头</param>
        /// <param name="val">值</param>
        /// <returns>当前实例</returns>
        public IConnectorHttp AppendHeader(string header, string val)
        {
            if (headers == null) { headers = new Dictionary<string, string>(); }
            headers.Remove(header);
            headers.Add(header, val);
            return this;
        }

        /// <summary>
        /// 以Restful请求
        /// </summary>
        /// <param name="method">方法类型</param>
        /// <param name="action">请求行为</param>
        public object Restful(Restfuls method, string action)
        {
            var request = new HttpWebRequestEntity(url + action, method);
            queue.Enqueue(request);
            return request;
        }

        /// <summary>
        /// 以Restful请求
        /// </summary>
        /// <param name="method">方法类型</param>
        /// <param name="action">请求行为</param>
        /// <param name="form">包体数据</param>
        public object Restful(Restfuls method, string action, WWWForm form)
        {
            var request = new HttpWebRequestEntity(url + action, method);
            request.SetBody(form.data).SetContentType("application/x-www-form-urlencoded");
            queue.Enqueue(request);
            return request;
        }

        /// <summary>
        /// 以Restful进行请求
        /// </summary>
        /// <param name="method">方法类型</param>
        /// <param name="action">请求行为</param>
        /// <param name="body">包体数据</param>
        public object Restful(Restfuls method, string action, byte[] body)
        {
            var request = new HttpWebRequestEntity(url + action, method);
            request.SetBody(body).SetContentType(contentType).SetAccept(accept);
            queue.Enqueue(request);
            return request;
        }

        ///// <summary>
        ///// 以Restful进行请求
        ///// </summary>
        ///// <param name="method">方法类型</param>
        ///// <param name="action">请求行为</param>
        ///// <param name="body">包体数据</param>
        //public object Restful(Restfuls method, string action, byte[] body)
        //{
        //    var request = new HttpWebRequestEntity(url + action, method);
        //    request.SetBody(body).SetContentType("application/octet-stream");
        //    queue.Enqueue(request);
        //    return request;
        //}

        /// <summary>
        /// 以Get请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        public object Get(string action)
        {
            return Restful(Restfuls.Get, action);
        }

        /// <summary>
        /// 以Head请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        public object Head(string action)
        {
            return Restful(Restfuls.Head, action);
        }

        /// <summary>
        /// 以Post请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        /// <param name="form">post数据</param>
        public object Post(string action, WWWForm form)
        {
            return Restful(Restfuls.Post, action, form);
        }

        /// <summary>
        /// 以Post请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        /// <param name="body">post数据</param>
        public object Post(string action, byte[] body)
        {
            return Restful(Restfuls.Post, action, body);
        }

        /// <summary>
        /// 以Put请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        /// <param name="form">post数据</param>
        public object Put(string action, WWWForm form)
        {
            return Restful(Restfuls.Put, action, form);
        }

        /// <summary>
        /// 以Put请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
        /// <param name="body">post数据</param>
        public object Put(string action, byte[] body)
        {
            return Restful(Restfuls.Put, action, body);
        }

        /// <summary>
        /// 以Delete请求数据
        /// </summary>
        /// <param name="action">请求行为</param>
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
            queue.Clear();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>协程</returns>
        public IEnumerator StartServer()
        {
            while (true)
            {
                if (stopMark)
                {
                    break;
                }
                if (queue.Count > 0)
                {
                    HttpWebRequestEntity request;
                    while (queue.Count > 0)
                    {
                        if (stopMark)
                        {
                            break;
                        }
                        request = queue.Dequeue();
                        request.SetContainer(cookieContainer);
                        request.SetHeader(headers);
                        request.SetTimeOut(timeout).SetReadWriteTimeOut(timeout);

                        yield return request.Send();

                        var args = new HttpRequestEventArgs(request);

                        App.Instance.Trigger(HttpRequestEvents.ON_MESSAGE, this, args);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}