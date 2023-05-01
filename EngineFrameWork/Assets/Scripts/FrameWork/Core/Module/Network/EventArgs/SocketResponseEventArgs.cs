

using System;
using Core.Interface.Network;

namespace Core.Network
{
    /// <summary>
    /// Socket响应参数
    /// </summary>
    public class SocketResponseEventArgs : EventArgs, IResponse
    {
        /// <summary>
        /// 响应数据
        /// </summary>
        public byte[] Response { get; protected set; }

        /// <summary>
        /// 构建一个Socket响应参数
        /// </summary>
        /// <param name="bytes">字节数据</param>
        public SocketResponseEventArgs(byte[] bytes)
        {
            Response = bytes;
        }
    }
}