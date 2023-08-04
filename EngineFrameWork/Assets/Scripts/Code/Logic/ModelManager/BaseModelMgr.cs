
using strange.extensions.dispatcher.eventdispatcher.api;

namespace Model
{
    public class BaseModelMgr
    {
        public GlobalData globalData { get; set; }

        public IEventDispatcher dispatcher { get; set; }

        public BaseModelMgr()
        {
            // Profiler.Sample Constructor Cost.
            dispatcher = GameMgr.Ins.CrossDispatcher;
            globalData = GlobalData.instance;
        }

        public virtual void OnDispose()
        {
            // Profiler.Sample Dispose Cost.
            dispatcher = null;
            globalData = null;
        }
    }

}