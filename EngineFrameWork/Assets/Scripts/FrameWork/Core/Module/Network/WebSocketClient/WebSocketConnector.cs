using System;
using UnityEngine;
using Core.Interface;
using UnityWebSocket;
using System.Net.Sockets;

namespace Core.Network
{

    public class WebSocketConnector : IDisposable
    {

        public enum Status
        {
            Initial = 1,
            Connecting = 2,
            Establish = 3,
            Closed = 4,
        }

        protected WebSocket socket;
        protected string remoteAddress;
        protected int remotePort;
        protected int readBufferLength = 1024;
        protected volatile Status status = Status.Initial;

        public Status CurrentStatus
        {
            get { return status; }
        }

        protected NetworkStream networkStream;

        private byte[] readBuffer;

        public EventHandler OnConnect;
        public EventHandler OnClose;
        public EventHandler OnError;
        public EventHandler OnMessage;

        public WebSocketConnector(string host, int port)
        {
            remoteAddress = host;
            remotePort = port;
        }

        public void Connect()
        {
            if (status != Status.Initial && status != Status.Closed)
            {
                return;
            }
            status = Status.Connecting;
            try
            {
                //var ipAddress = Dns.EndGetHostAddresses(result);
                socket = new WebSocket(remoteAddress);

                socket.OnOpen += OnConnectComplete;
                socket.OnMessage += OnReadCallBack;
                socket.OnClose += OnCloseCallBack;
                socket.OnError += OnErrorCallBack;

                socket.ConnectAsync();

                //Dns.BeginGetHostAddresses(remoteAddress, OnDnsGetHostAddressesComplete, null);
            }
            catch (Exception e)
            {
                OnError(this, new ExceptionEventArgs(e));
                Dispose();
            }
        }

        public void Send(byte[] bytes)
        {
            if (status != Status.Establish)
            {
                return;
            }
            try
            {
                //networkStream.BeginWrite(bytes, 0, bytes.Length, OnSendCallBack, socket);

                socket.SendAsync(bytes);
            }
            catch (Exception ex)
            {
                OnError(this, new ExceptionEventArgs(ex));
                Dispose();
            }
        }

        public void Dispose()
        {
            if (status == Status.Closed)
            {
                return;
            }
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream = null;
            }
            if (socket != null)
            {
                socket.CloseAsync();
                socket = null;
            }
            status = Status.Closed;
            OnClose(this, EventArgs.Empty);
        }

        private void OnErrorCallBack(object sender, UnityWebSocket.ErrorEventArgs e)
        {

            Debug.LogWarning(string.Format("Error: {0}", e.Message));
        }

        private void OnCloseCallBack(object sender, CloseEventArgs e)
        {
            Debug.Log(string.Format("Closed: StatusCode: {0}, Reason: {1}", e.StatusCode, e.Reason));
            OnClose(this, EventArgs.Empty);
        }

        protected void OnConnectComplete(object sender, OpenEventArgs e)
        {
            try
            {
                //socket.EndConnect(result);

                //networkStream = socket.GetStream();
                //readBuffer = new byte[readBufferLength];
                //networkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallBack, readBuffer);

                status = Status.Establish;
                OnConnect(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                OnError(this, new ExceptionEventArgs(ex));
                Dispose();
            }
        }

        protected void OnReadCallBack(object sender, MessageEventArgs e)
        {
            if (socket == null || status != Status.Establish)
            {
                return;
            }

            var read = e.RawData.Length;
            if (read < 1)
            {
                //status = Status.Closed;
                Dispose();
                return;
            }

            if (read > 2)
            {
                var args = new SocketResponseEventArgs(e.RawData);
                OnMessage(this, args);
            }


            //readBuffer = new byte[readBufferLength];
            //networkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallBack, readBuffer);
        }

        protected void OnSendCallBack(IAsyncResult result)
        {
            try
            {
                networkStream.EndWrite(result);

            }
            catch (Exception ex)
            {
                OnError(this, new ExceptionEventArgs(ex));
                Dispose();
            }
        }
    }
}