using strange.extensions.mediation.impl;

namespace UI
{
    public class LogInMediator : EventMediator
    {
        [Inject]
        public LogInView view { get; set; }

        public override void OnRegister()
        {
        }

        public override void OnRemove()
        {
        }
    }
}
