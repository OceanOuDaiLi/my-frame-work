﻿

namespace Core.Interface.Network
{
    /// <summary>
    /// 连接器
    /// </summary>
    public interface IConnectorSocket : IConnector
    {
        /// <summary>
        /// 发送内容
        /// </summary>
        /// <param name="data">发送数据</param>
        void Send(byte[] data);

        /// <summary>
        /// 发送内容
        /// </summary>
        /// <param name="package">数据包</param>
        void Send(IPackage package);

        /// <summary>
        /// 连接
        /// </summary>
        void Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
    }
}
