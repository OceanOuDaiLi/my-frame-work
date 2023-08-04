using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;


namespace UI
{
    public class UserInfoView : EventView
    {

        public void Back()
        {
            UIMgr.Ins.CloseUI();
        }
    }
}