

namespace Core.Interface.Network
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 响应字节流
        /// </summary>
        byte[] Response { get; }
    }
}