using Model;
using strange.extensions.mediation.impl;

namespace UI
{
    public class NamePanelMediator : EventMediator
    {
        [Inject]
        public NamePanelView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            view.BindMediator(this);

            // binds global events below.
        }

        public override void OnRemove()
        {
            // remove global events' below.
        }

    }
}
