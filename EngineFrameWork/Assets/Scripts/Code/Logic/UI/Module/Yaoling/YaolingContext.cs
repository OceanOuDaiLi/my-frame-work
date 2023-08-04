using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class YaolingContext : MVCSContext
    {
        public YaolingContext(MonoBehaviour view) : base(view) { }

        public YaolingContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<YaolingView>().To<YaolingMediator>();
        }
    }
}