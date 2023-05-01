

namespace Core.Interface.Network
{
    /// <summary>
    /// Udp连接
    /// </summary>
    public interface IConnectorUdp : IConnectorSocket
    {
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="package">数据包</param>
        /// <param name="host">host</param>
        /// <param name="port">端口</param>
        void Send(IPackage package, string host, int port);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="host">host</param>
        /// <param name="port">端口</param>
        void Send(byte[] data, string host, int port);
    }
}