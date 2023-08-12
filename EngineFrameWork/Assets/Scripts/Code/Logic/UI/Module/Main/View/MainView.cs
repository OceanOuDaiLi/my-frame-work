using FrameWork;
using GameEngine;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

namespace UI
{
    public class MainView : EventView
    {
        // private SerializeField Components
        [SerializeField] Animator sysPanelAnimator;
        [SerializeField] GameObject sysPanelObj;
        [SerializeField] GameObject toggleBtn;
        [SerializeField] GameObject[] taskAndTeamObjs;               //0:team,  1:task
        [SerializeField] Text posText;

        [SerializeField] GameObject[] showFightObjs;

        // private Variables
        private int _sysPanelState = 0;                              //0:关闭，1:打开

        protected override void Awake()
        {
            base.Awake();
            // 初始化
            sysPanelObj.SetActive(false);
            OnTeamToggleValueChange(1);
        }

        void OnDisable()
        {
            sysPanelAnimator.enabled = false;
            sysPanelObj.SetActive(false);
        }

        public void OnHasAnybodyClick(InputCatcher.SingleClickData clickData)
        {
            if (clickData.clickTarget == null ||
                (_sysPanelState == 1 &&
                toggleBtn != clickData.clickTarget &&
                !toggleBtn.transform.IsMyGrandson(clickData.clickTarget.transform) &&
                sysPanelObj != clickData.clickTarget &&
                !sysPanelObj.transform.IsMyGrandson(clickData.clickTarget.transform)))
            {
                ShowAndHideSysPanel(false);
            }
        }

        public void ToggleSysPanel()
        {
            if (!sysPanelAnimator.enabled)
            {
                sysPanelAnimator.enabled = true;
            }
            if (!sysPanelObj.activeSelf)
            {
                sysPanelObj.SetActive(true);
                _sysPanelState = 0;
            }
            ShowAndHideSysPanel(_sysPanelState == 0);
        }

        void ShowAndHideSysPanel(bool flag)
        {
            if ((flag && _sysPanelState == 1) || (!flag && _sysPanelState == 0)) return;
            sysPanelAnimator.Play(flag ? "show" : "hide", 0, 0);
            _sysPanelState = flag ? 1 : 0;
        }

        public void OnTeamToggleValueChange(int idx)
        {
            taskAndTeamObjs[0].SetActive(idx == 0);
            taskAndTeamObjs[1].SetActive(idx == 1);
        }

        public void OpenUserInfo()
        {
            UIConfig userInfo = new UIConfig();
            userInfo.floaderName = "userinfo";
            userInfo.prefabName = "userinfo";
            UIMgr.Ins.OpenUI(userInfo, (s) =>
            {

            });
            //mediator.Test();
        }

        public void OpenYaoling()
        {
            UIConfig yaoling = new UIConfig();
            yaoling.floaderName = "yaoling";
            yaoling.prefabName = "yaoling";
            UIMgr.Ins.OpenUI(yaoling, (s) =>
            {

            });
        }

        public void OnClickTestFight()
        {
            //Debug.Log("test fight");
            CameraMgr.Instance.ShowGlitchEffect();
        }

        public void NotFunction()
        {
            // 功能未实现
            App.Instance.Trigger(GameEvent.SHOW_TOAST, "功能未实现");
        }

        public void SetPositionText(string text)
        {
            posText.text = text;
        }

        public void OnGameFightStart()
        {
            foreach (var item in showFightObjs)
            {
                item.SetActive(false);
            }
        }

        public void OnGameFightEnd()
        {
            foreach (var item in showFightObjs)
            {
                item.SetActive(true);
            }
        }
    }
}