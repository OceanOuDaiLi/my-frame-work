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

using Model;
using ProtoBuf;
using System.IO;
using Core.Buffer;
using System.Reflection;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	WebSocketService.cs
	Author:		DaiLi.Ou

	Descriptions: Web socket service for client.
*********************************************************************/
namespace UI
{
    public enum WebSocketState : byte
    {
        Connecting,
        Connected,
        Closed
    }

    public class WebSocketService : IDisposable
    {
        class WebSocketStateException : Exception
        {
            public WebSocketStateException(string message) : base(message) { }
        }

        class RequestTask
        {
            internal string Action { get; set; }
            internal Dictionary<string, object> Parameters { get; set; }
            internal MessageEventCallback Callback { get; set; }
        }

        WebSocketRequest websocket;
        IEventHandler eHandlerOnError = null;
        IEventHandler eHandlerOnClose = null;
        IEventHandler eHandlerOnConnect = null;
        IEventHandler eHandlerOnMessage = null;



        BufferBuilder recvBuffer = new BufferBuilder();
        BufferBuilder sendBuffer = new BufferBuilder();

        //const int SENF_BUFFER_LEN = 64 * 1024;
        //const int REVIVE_BUFFER_LEN = 128 * 1024;
        //const int DATA_BYTE_LENGTH = 40;//假设一个字段4个字节，共10个字段，已经远远超过游戏实际情况了
        //MemoryStream msSend = new MemoryStream(new byte[SENF_BUFFER_LEN], 0, SENF_BUFFER_LEN, true, true);
        //MemoryStream msRecive = new MemoryStream(new byte[REVIVE_BUFFER_LEN], 0, REVIVE_BUFFER_LEN, true, true);


        public int Port { get; private set; }
        public string Host { get; private set; }

        public WebSocketState State { get; private set; }
        public event Action onWebSocketConnected = null;
        public event Action onWebSocketDisconnected = null;
        public event Action onWebSocketRestartGame = null;

        public delegate void MessageEventCallback(Dictionary<string, object> dict);

        public WebSocketService()
        {
            eHandlerOnConnect = App.Instance.On(SocketRequestEvents.ON_CONNECT, OnConnect);
            eHandlerOnMessage = App.Instance.On(SocketRequestEvents.ON_MESSAGE, OnMessage);
            eHandlerOnError = App.Instance.On(SocketRequestEvents.ON_ERROR, OnError);
            eHandlerOnClose = App.Instance.On(SocketRequestEvents.ON_CLOSE, OnClose);

            State = WebSocketState.Closed;
        }

        ~WebSocketService()
        {
            Dispose();
        }

        public void Dispose()
        {
            App.Net.Destroy(Network.NetworkRequestType.WebSocket);

            eHandlerOnConnect.Cancel();
            eHandlerOnMessage.Cancel();
            eHandlerOnError.Cancel();
            eHandlerOnClose.Cancel();

            onWebSocketConnected = null;
            onWebSocketDisconnected = null;
            onWebSocketRestartGame = null;
        }

        public void Disconnect()
        {
            App.Net.Destroy(Network.NetworkRequestType.WebSocket);
            RestartGame();
        }

        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;


            App.Net.Destroy(Network.NetworkRequestType.WebSocket);
            websocket = (WebSocketRequest)App.Net.Create<IConnectorWebSocket>(Network.NetworkRequestType.WebSocket, new Hashtable()
                {
                    { Network.NetworkConfigKey.Host, host },
                    { Network.NetworkConfigKey.Port, port },
                    { Network.NetworkConfigKey.Packing, new Core.NetPackage.WebSocketFramePacking() },
                    //{ Network.NetworkConfigKey.Protocol, new Core.NetPackage.ByteProtocol() }
                });

            this.State = WebSocketState.Connecting;

            // show loading rotate ui.
#if UNITY_EDITOR
            CDebug.Log("WebSocket 服务器开始连接");
#endif
            websocket.Connect();
        }

        void OnConnect(object sender, EventArgs e)
        {

#if UNITY_EDITOR
            CDebug.Log("WebSocket 服务器连接成功");
#endif

            State = WebSocketState.Connected;
            // hide loading rotate ui.
            if (onWebSocketConnected != null) onWebSocketConnected();
        }

        void RestartGame()
        {
            // hide loading rotate ui.
            onWebSocketRestartGame?.Invoke();
        }

        void OnError(object sender, EventArgs e)
        {
            ExceptionEventArgs args = e as ExceptionEventArgs;
#if UNITY_EDITOR
            if (!args.Exception.GetType().Equals(typeof(WebSocketStateException)))
            {
                CDebug.LogError(args.Exception);
            }
            else
            {
                CDebug.Log(args.Exception);
            }
#endif

            //show restart ui.
        }

        void OnClose(object sender, EventArgs e)
        {
#if UNITY_EDITOR
            CDebug.Log("服务器连接关闭");
#endif

            State = WebSocketState.Closed;
            if (onWebSocketDisconnected != null) onWebSocketDisconnected();
        }

        // Send/On Msg 
        #region ProtoBuf Send/On Msg Methods

        public void SendMessage(IExtensible msg)
        {
            short nMsgID = ProtoData.GetC2SMsgID(msg.GetType());
            if (nMsgID == 0)
            {
                CDebug.LogError("not found proto ID!!");
                return;
            }

            byte[] val = Encode(msg);

            sendBuffer.Clear();

            sendBuffer.Push(new byte[] { (byte)(nMsgID >> 8), (byte)(nMsgID & 0xFF) });
            sendBuffer.Push(BitConverter.GetBytes(val.Length));
            sendBuffer.Push(val);

            websocket.Send(sendBuffer.Byte);
        }

        public void OnMessage(object sender, EventArgs e)
        {
            SocketResponseEventArgs args = e as SocketResponseEventArgs;
            byte[] encryptBytes = args.Response;

            recvBuffer.Clear();
            recvBuffer.Push(encryptBytes);
            int by1 = recvBuffer.Shift()[0];
            int by2 = recvBuffer.Shift()[0];
            int mainId = (by1 << 8) + by2;

            // ProcessPacketHandle
            Type tClass = ProtoData.GetProtoClass((short)mainId);
            if (tClass != null)
            {
                object insMsg = Decode(tClass, recvBuffer, 0, recvBuffer.Length);
                if (insMsg == null)
                {
                    CDebug.LogError($"Proto协议号: {mainId} 返回为空");
                }

                string eventName = tClass.FullName;
                if (!string.IsNullOrEmpty(eventName))
                {
                    GameMgr.Ins.CrossDispatcher.Dispatch(eventName, insMsg);
                }
            }
        }

        #endregion

        // Serialize/DeSerialize PB Data
        #region  ProtoBuf Serialize/DeSerialize PB Data

        protected byte[] Encode(IExtensible msgBase)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Serializer.Serialize(memory, msgBase);
                return memory.GetBuffer();//memory.ToArray();
            }
        }

        protected object Decode(Type t, byte[] bytes, int offset, int count)
        {
            using (MemoryStream memory = new MemoryStream(bytes, offset, count))
            {
                return Serializer.Deserialize(t, memory);
            }
        }

        #endregion
    }
}
