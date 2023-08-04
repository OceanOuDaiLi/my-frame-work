using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class NamePanelContext : MVCSContext
    {
        public NamePanelContext(MonoBehaviour view) : base(view) { }

        public NamePanelContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<NamePanelView>().To<NamePanelMediator>();
        }
    }
}