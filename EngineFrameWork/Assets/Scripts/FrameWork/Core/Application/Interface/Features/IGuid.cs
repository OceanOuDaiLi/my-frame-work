

namespace Core.Interface
{
    /// <summary>
    /// 唯一标识符接口
    /// </summary>
    public interface IGuid
    {
        /// <summary>
        /// 获取当前类的全局唯一标识符
        /// </summary>
        long Guid { get; }
    }
}