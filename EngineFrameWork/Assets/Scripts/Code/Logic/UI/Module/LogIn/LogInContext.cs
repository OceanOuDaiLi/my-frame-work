using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class LogInContext : MVCSContext
    {
        public LogInContext(MonoBehaviour view) : base(view) { }

        public LogInContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            // view & mediator binding.
            mediationBinder.Bind<LogInView>().To<LogInMediator>();
        }
    }
}