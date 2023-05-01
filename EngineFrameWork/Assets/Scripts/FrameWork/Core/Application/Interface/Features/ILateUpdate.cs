

namespace Core.Interface
{
    /// <summary>
    /// 在Update之后调用
    /// </summary>
    public interface ILateUpdate
    {
        /// <summary>
        /// LateUpdate时调用
        /// </summary>
        void LateUpdate();
    }
}