using Model;
using strange.extensions.mediation.impl;

namespace UI
{
    public class UserInfoMediator : EventMediator
    {
        [Inject]
        public UserInfoView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.

            // binds global events below.
        }

        public override void OnRemove()
        {
            // remove global events' binding below.
        }
    }
}
