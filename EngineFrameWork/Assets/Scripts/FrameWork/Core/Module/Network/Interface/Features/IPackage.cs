

namespace Core.Interface.Network
{
    /// <summary>
    /// 数据包
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// 数据包
        /// </summary>
        object Package { get; }

        /// <summary>
        /// 数据包字节流
        /// </summary>
        byte[] ToByte();
    }
}
