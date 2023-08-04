using FrameWork;
using Model;
using strange.extensions.mediation.impl;
using System;
using UnityEngine.UI;

namespace UI
{
    public class MainMediator : EventMediator
    {


        [Inject]
        public MainView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            view.BindMediator(this);

            // binds global events below.

            App.Instance.On("OnHeroPosChanged", OnHeroMove);
        }

        private void OnHeroMove(object sender, EventArgs e)
        {
            view.SetPositionText(globalData.heroMgr.m_vServerPos.ToString());
        }

        public override void OnRemove()
        {
            // remove global events' below.
        }
    }
}
