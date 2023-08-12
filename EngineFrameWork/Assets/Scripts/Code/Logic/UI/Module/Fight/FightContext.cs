using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class FightContext : MVCSContext
    {
        public FightContext(MonoBehaviour view) : base(view) { }

        public FightContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<FightView>().To<FightMediator>();
        }
    }
}