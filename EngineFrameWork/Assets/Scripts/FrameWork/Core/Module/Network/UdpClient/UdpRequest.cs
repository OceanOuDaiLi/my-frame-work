using System;
using FrameWork;
using System.Net;
using UnityEngine;
using Core.Buffer;
using Core.Interface;
using System.Collections;
using Core.Interface.Network;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	UdpRequest.cs
	Author:		DaiLi.Ou

	Descriptions: Abstract class of Udp Request.
*********************************************************************/
namespace Core.Network
{
    public class UdpRequest : IConnectorUdp
    {
        public string Name { get; set; }

        public BufferBuilder DecodeRenderBuffer { get; set; }

        public BufferBuilder EncodeRenderBuffer { get; set; }

        private Queue<object[]> queue = new Queue<object[]>();

        private string host;
        private int port;

        private UdpConnector udpConnector;

        private IPacking packer;
        private IRender[] render;
        private IProtocol protocol;

        private bool stopMark = false;

        private object locker = new object();

        public void SetConfig(Hashtable config)
        {
            if (packer == null && config.ContainsKey(Network.NetworkConfigKey.Packing))
            {
                packer = config[Network.NetworkConfigKey.Packing] as IPacking;
            }

            if (protocol == null && config.ContainsKey(Network.NetworkConfigKey.Protocol))
            {
                protocol = config[Network.NetworkConfigKey.Protocol] as IProtocol;
            }

            if (config.ContainsKey(Network.NetworkConfigKey.Host))
            {
                host = config[Network.NetworkConfigKey.Host].ToString();
            }

            if (config.ContainsKey(Network.NetworkConfigKey.Port))
            {
                port = Convert.ToInt32(config[Network.NetworkConfigKey.Port].ToString());
            }
        }

        /// <summary>
        /// 链接
        /// </summary>
        public void Connect()
        {
            lock (locker)
            {
                Disconnect();
                if (packer != null) { packer.Clear(); }

                udpConnector = new UdpConnector();
                udpConnector.OnConnect += OnConnect;
                udpConnector.OnClose += OnClose;
                udpConnector.OnError += OnError;
                udpConnector.OnMessage += OnMessage;

                if (string.IsNullOrEmpty(host) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    udpConnector.Connect();
                }
                else
                {
                    udpConnector.Connect(host, port);
                }
            }

        }

        /// <summary>
        /// 加入发送队列
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(IPackage package)
        {
            lock (locker)
            {
                byte[] data;
                if (protocol != null)
                {
                    data = protocol.Encode(package);
                }
                else
                {
                    data = package.ToByte();
                }
                Send(data);
            }
        }

        /// <summary>
        /// 加入发送队列
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            lock (locker)
            {
                bytes = SendEncode(bytes);
                if (bytes == null) { return; }
                queue.Enqueue(new object[] { bytes });
            }
        }

        /// <summary>
        /// 发送到目标
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Send(IPackage package, string host, int port)
        {
            lock (locker)
            {
                byte[] data;
                if (protocol != null)
                {
                    data = protocol.Encode(package);
                }
                else
                {
                    data = package.ToByte();
                }
                Send(data, host, port);
            }
        }

        /// <summary>
        /// 发送到目标
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Send(byte[] bytes, string host, int port)
        {
            lock (locker)
            {
                bytes = SendEncode(bytes);
                if (bytes == null) { return; }
                queue.Enqueue(new object[] { bytes, host, port });
            }
        }

        private byte[] SendEncode(byte[] bytes)
        {
            try
            {
                if (render != null && render.Length > 0)
                {
                    EncodeRenderBuffer.Byte = bytes;
                    for (int n = render.Length - 1; n >= 0; n--)
                    {
                        render[n].Encode(EncodeRenderBuffer);
                    }
                    bytes = EncodeRenderBuffer.Byte;
                }

                if (packer != null)
                {
                    bytes = packer.Encode(bytes);
                }
                return bytes;
            }
            catch (Exception ex)
            {
                Trigger(SocketRequestEvents.ON_ERROR, new ExceptionEventArgs(ex));
                return null;
            }
        }

        public IEnumerator StartServer()
        {
            while (true)
            {
                if (stopMark) { break; }
                while (queue.Count > 0)
                {
                    if (udpConnector != null && udpConnector.CurrentStatus == UdpConnector.Status.Establish)
                    {
                        object[] data = queue.Dequeue();
                        if (data.Length == 1)
                        {
                            udpConnector.Send(data[0] as byte[]);
                        }
                        else if (data.Length == 3)
                        {
                            udpConnector.SendTo(data[0] as byte[], data[1] as string, int.Parse(data[2].ToString()));
                        }

                    }
                    else { break; }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void Destroy()
        {
            stopMark = true;
            Disconnect();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (udpConnector != null)
            {
                udpConnector.Dispose();
            }
            udpConnector = null;
        }

        /// <summary>
        /// 当链接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnect(object sender, EventArgs args)
        {
            Trigger(SocketRequestEvents.ON_CONNECT, args);
        }

        /// <summary>
        /// 当关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnClose(object sender, EventArgs args)
        {
            Trigger(SocketRequestEvents.ON_CLOSE, args);
        }

        /// <summary>
        /// 当错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnError(object sender, EventArgs args)
        {
            Trigger(SocketRequestEvents.ON_ERROR, args);
        }

        /// <summary>
        /// 当接受到消息时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessage(object sender, EventArgs args)
        {

            if (packer == null)
            {
                Trigger(SocketRequestEvents.ON_MESSAGE, args);
                return;
            }

            try
            {
                byte[][] data = packer.Decode((args as SocketResponseEventArgs).Response);
                if (data != null)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        try
                        {

                            if (render != null && render.Length > 0)
                            {
                                DecodeRenderBuffer.Byte = data[i];
                                for (int n = 0; n < render.Length; n++)
                                {
                                    render[n].Decode(DecodeRenderBuffer);
                                }
                                data[i] = DecodeRenderBuffer.Byte;
                            }

                            if (protocol == null)
                            {
                                args = new SocketResponseEventArgs(data[i]);
                            }
                            else
                            {
                                args = new PackageResponseEventArgs(protocol.Decode(data[i]));
                            }
                            Trigger(SocketRequestEvents.ON_MESSAGE, args);

                        }
                        catch (Exception ex)
                        {
                            Trigger(SocketRequestEvents.ON_ERROR, new ExceptionEventArgs(ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trigger(SocketRequestEvents.ON_ERROR, new ExceptionEventArgs(ex));
                Disconnect();
            }

        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名字</param>
        /// <param name="args">参数</param>
        private void Trigger(string eventName, EventArgs args)
        {
            App.Instance.Trigger(eventName, this, args);
        }
    }
}