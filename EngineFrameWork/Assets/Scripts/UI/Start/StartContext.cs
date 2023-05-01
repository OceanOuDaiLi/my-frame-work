using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class StartContext : MVCSContext
    {

        public StartContext(MonoBehaviour view) : base(view) { }

        public StartContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {

            mediationBinder.Bind<StartView>().To<StartMediator>();

        }
    }
}