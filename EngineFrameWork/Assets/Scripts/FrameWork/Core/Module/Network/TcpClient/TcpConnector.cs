

using System;
using System.Net;
using System.Net.Sockets;
using Core.Interface;

namespace Core.Network
{

    public class TcpConnector : IDisposable
    {

        public enum Status
        {
            Initial = 1,
            Connecting = 2,
            Establish = 3,
            Closed = 4,
        }

        protected TcpClient socket;
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

        public TcpConnector(string host, int port)
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
                Dns.BeginGetHostAddresses(remoteAddress, OnDnsGetHostAddressesComplete, null);
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
                networkStream.BeginWrite(bytes, 0, bytes.Length, OnSendCallBack, socket);
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
                socket.Close();
                socket = null;
            }
            status = Status.Closed;
            OnClose(this, EventArgs.Empty);
        }

        protected void OnDnsGetHostAddressesComplete(IAsyncResult result)
        {
            try
            {
                var ipAddress = Dns.EndGetHostAddresses(result);
                socket = new TcpClient();
                socket.BeginConnect(ipAddress, remotePort, OnConnectComplete, socket);
            }
            catch (Exception ex)
            {
                OnError(this, new ExceptionEventArgs(ex));
                Dispose();
            }
        }

        protected void OnConnectComplete(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);

                networkStream = socket.GetStream();
                readBuffer = new byte[readBufferLength];
                networkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallBack, readBuffer);

                status = Status.Establish;
                OnConnect(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                OnError(this, new ExceptionEventArgs(ex));
                Dispose();
            }
        }

        protected void OnReadCallBack(IAsyncResult result)
        {
            var read = networkStream.EndRead(result);
            if (read <= 0)
            {
                //status = Status.Closed;
                Dispose();
                return;
            }

            var callbackBuff = new byte[read];
            System.Buffer.BlockCopy(readBuffer, 0, callbackBuff, 0, read);
            var args = new SocketResponseEventArgs(callbackBuff);
            OnMessage(this, args);

            readBuffer = new byte[readBufferLength];
            networkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallBack, readBuffer);
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