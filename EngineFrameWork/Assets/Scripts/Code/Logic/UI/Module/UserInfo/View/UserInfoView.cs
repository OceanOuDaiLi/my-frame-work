using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;


namespace UI
{
    public class UserInfoView : EventView
    {

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void Back()
        {
            UIMgr.Ins.CloseUI();
        }
    }
}