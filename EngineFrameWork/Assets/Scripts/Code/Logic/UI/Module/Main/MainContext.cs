using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class MainContext : MVCSContext
    {
        public MainContext(MonoBehaviour view) : base(view) { }

        public MainContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<MainView>().To<MainMediator>();
        }
    }
}