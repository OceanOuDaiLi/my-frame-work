using strange.extensions.mediation.impl;

namespace UI
{
    public class StartMediator : EventMediator
    {
        [Inject]
        public StartView view { get; set; }

        public override void OnRegister()
        {
        }

        public override void OnRemove()
        {
        }
    }
}
