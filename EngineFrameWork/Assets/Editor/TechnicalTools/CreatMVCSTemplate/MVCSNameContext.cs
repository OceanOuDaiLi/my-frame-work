using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class MVCSNameContext : MVCSContext
    {
        public MVCSNameContext(MonoBehaviour view) : base(view) { }

        public MVCSNameContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<MVCSNameView>().To<MVCSNameMediator>();
        }
    }
}