using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;
namespace UI {

    public class LoadingContext : MVCSContext {
        public LoadingContext(MonoBehaviour view) : base(view) { }
        public LoadingContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings() {
            mediationBinder.Bind<LoadingView>().To<LoadingMediator>();
        }

    }
}