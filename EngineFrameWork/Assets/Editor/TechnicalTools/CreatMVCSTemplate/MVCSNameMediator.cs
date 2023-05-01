using strange.extensions.mediation.impl;

namespace UI
{
    public class MVCSNameMediator : EventMediator
    {
        [Inject]
        public MVCSNameView view { get; set; }

        public override void OnRegister()
        {
        }

        public override void OnRemove()
        {
        }
    }
}
