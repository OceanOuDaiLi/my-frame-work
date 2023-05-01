

namespace Core.Interface.Event
{
    /// <summary>
    /// 事件句柄
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// 取消注册的事件
        /// </summary>
        bool Cancel();

        /// <summary>
        /// 剩余的调用次数，当为0时事件会被释放
        /// </summary>
        int Life { get; }

        /// <summary>
        /// 事件是否是有效的
        /// </summary>
        bool IsLife { get; }
    }
}