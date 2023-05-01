

namespace Core.Interface.Network
{
    /// <summary>
    /// 协议解析器
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 协议反序列化
        /// </summary>
        /// <param name="bytes">需要反序列化的数据</param>
        /// <returns>协议包</returns>
        IPackage Decode(byte[] bytes);

        /// <summary>
        /// 协议序列化
        /// </summary>
        /// <param name="package">协议包</param>
        /// <returns>序列化后的数据</returns>
        byte[] Encode(IPackage package);
    }
}