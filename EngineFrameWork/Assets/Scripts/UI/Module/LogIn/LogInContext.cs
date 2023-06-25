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
            // bing service.
            injectionBinder.Bind<ILogInService>().To<LogInService>().ToSingleton();

            // bind command.from event to cmd.
            commandBinder.Bind(LogInEvent.REQUEST_CLICK_LOGIN).To<LoginCommand>();

            // view & mediator binding.
            mediationBinder.Bind<LogInView>().To<LogInMediator>();
        }
    }
}