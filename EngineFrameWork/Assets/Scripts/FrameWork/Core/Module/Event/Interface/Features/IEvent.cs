

namespace Core.Interface.Event
{
    /// <summary>
    /// 事件接口
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 事件实现
        /// </summary>
        IEventImpl Event { get; }
    }
}
