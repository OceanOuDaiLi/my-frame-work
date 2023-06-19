using strange.extensions.context.impl;

namespace UI
{
    public class UICrossRoot : ContextView
    {
        protected void Awake()
        {
            CDebug.Log("Cross Root Awake");
            context = new UICrossContext(this);
            GameMgr.Ins.CrossDispatcher = (context as UICrossContext).dispatcher;
        }
    }
}