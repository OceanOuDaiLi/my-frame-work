using strange.extensions.context.impl;

namespace UI
{
    public class UICrossRoot : ContextView
    {
        protected void Awake()
        {
            context = new UICrossContext(this);
            GameMgr.Ins.CrossDispatcher = (context as UICrossContext).dispatcher;
        }

        public bool Inited()
        {
            if (context == null)
            {
                return false;
            }

            return ((UICrossContext)context).Inited;
        }
    }
}