

using Core.Interface.Buffer;

namespace Core.Interface.Network
{
    /// <summary>
    /// 数据渲染流
    /// </summary>
    public interface IRender
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bufferBuilder">流</param>
        void Decode(IBufferBuilder bufferBuilder);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="bufferBuilder">流</param>
        void Encode(IBufferBuilder bufferBuilder);
    }
}