

namespace Core.Interface
{
    /// <summary>
    /// 当被释放时
    /// </summary>
    public interface IDestroy
    {
        /// <summary>
        /// 当被释放时调用
        /// </summary>
        void OnDestroy();
    }
}