using System;
using FrameWork;
using UnityEngine;
using UnityEngine.UI;
using Core.Interface.Event;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace UI
{
    public class NpcChatView : EventView
    {
        private NpcChatMediator mediator;

        public GameObject Panel_root;
        public GameObject Panel_button;
        public GameObject Panel_just_option;
        public GameObject Panel_just_option_button;
        public GameObject Panel_hero_name;
        public GameObject Panel_npc_name;
        public Button Button_auto;
        public Button Button_skip;
        public Text Text_chat;
        public Text Text_npc_name;
        public Text Text_hero_name;
        public GameObject Live2D_Npc;
        public GameObject Live2D_Hero;


        List<Action> _testAction;
        int _testIndex = 0;
        Dictionary<string, IEventHandler> _eventHandlerInputSingleClick = null;


        private void SetClick(object obj, string szKey, EventHandler evnHandler)
        {
            if (_eventHandlerInputSingleClick == null)
            {
                _eventHandlerInputSingleClick = new Dictionary<string, IEventHandler>();
            }
            if (_eventHandlerInputSingleClick.ContainsKey(szKey))
            {
                _eventHandlerInputSingleClick[szKey].Cancel();
            }
            _eventHandlerInputSingleClick[szKey] = App.Instance.On(GameEvent.INPUT_EVENT_SINGLE_CLICK, (sender, e) =>
            {
                var clickData = (InputCatcher.SingleClickData)sender;
                var objClick = clickData.clickTarget;

                if (clickData.targetType == InputCatcher.SingleClickData.TargetType.None)
                    return;

                if (!objClick.Equals(obj))
                    return;

                evnHandler(sender, e);
            });

        }

        public void BindMediator(NpcChatMediator _mediator)
        {
            mediator = _mediator;
        }

        private void Start()
        {
            base.Start();

            _testAction = new List<Action>();
            _testAction.Add(TestStatus1);
            _testAction.Add(TestStatus2);
            _testAction.Add(TestStatus3);
            _testAction.Add(TestStatus4);
            _testAction.Add(TestStatus5);

            UpdateUI();
        }

        private void UpdateUI()
        {
            SetClick(Panel_root, "Panel_root", OnCloseUI);
            _testIndex = 0;
            NextAction(null, null);
        }

        private void OnEnable()
        {
            UpdateUI();
        }



        private void OnCloseUI(object sender, EventArgs e)
        {
            _testIndex = 0;
            UIConfig npcChatView = new UIConfig();
            npcChatView.floaderName = "npcchat";
            npcChatView.prefabName = "npcchat";
            UIMgr.Ins.CloseUI(npcChatView);
        }

        private void TestStatus1()
        {
            NpcSay("游行商人", "有什么需要帮助的吗?");

            Panel_button.SetActive(true);
            Button_auto.gameObject.SetActive(false);
            Button_skip.gameObject.SetActive(false);

            var obj = Panel_button.transform.Find("Button_1").gameObject;
            SetClick(obj, "Panel_button_Button_1", NextAction);

            obj = Panel_button.transform.Find("Button_2").gameObject;
            SetClick(obj, "Panel_button_Button_2", OnCloseUI);

            obj = Panel_button.transform.Find("Button_3").gameObject;
            SetClick(obj, "Panel_button_Button_3", OnCloseUI);
        }
        private void TestStatus2()
        {
            NpcSay("游行商人",
                "祭祀花神的会场尚未布置完成,还需在周围的树\r\n木新添一些<color=#800080>绫罗飘带</color>的装饰,可否麻烦你们...\r\n"
                );

            SetClick(Panel_root, "Panel_root", NextAction);
        }

        private void TestStatus3()
        {
            Panel_just_option.SetActive(true);
            Panel_just_option_button.transform.Find("Button_1").GetComponentInChildren<Text>().text = "我尝试去寻找";
            Panel_just_option_button.transform.Find("Button_2").GetComponentInChildren<Text>().text = "没时间";

            var obj = Panel_just_option_button.transform.Find("Button_1").gameObject;
            SetClick(obj, "Panel_just_option_button_Button_1", NextAction);

            obj = Panel_just_option_button.transform.Find("Button_2").gameObject;
            SetClick(obj, "Panel_just_option_button_Button_2", OnCloseUI);

            Panel_just_option_button.transform.Find("Button_3").gameObject.SetActive(false);
        }

        private void TestStatus4()
        {
            HeroSay("东西已经帮你找到了");
        }

        private void TestStatus5()
        {
            NpcSay("游行商人", "非常感谢");
        }

        private void HeroSay(string szMsg)
        {
            Panel_hero_name.SetActive(true);
            Panel_npc_name.SetActive(false);
            Text_hero_name.text = mediator.globalData.characterModelMgr.GetMapPlayerCharacter().GetName();
            Text_chat.text = szMsg;

            Panel_just_option.SetActive(false);
            Panel_button.SetActive(false);

            Button_auto.gameObject.SetActive(true);
            Button_skip.gameObject.SetActive(true);

            Live2D_Npc.SetActive(false);
            Live2D_Hero.SetActive(true);
        }

        private void NpcSay(string szNpcName, string szMsg)
        {
            Panel_hero_name.SetActive(false);
            Panel_npc_name.SetActive(true);
            Text_npc_name.text = szNpcName;
            Text_chat.text = szMsg;

            Panel_just_option.SetActive(false);
            Panel_button.SetActive(false);


            Button_auto.gameObject.SetActive(true);
            Button_skip.gameObject.SetActive(true);

            Live2D_Npc.SetActive(true);
            Live2D_Hero.SetActive(false);
        }

        private void NextAction(object sender, EventArgs e)
        {
            if (_testAction == null)
                return;

            if (_testIndex < _testAction.Count)
            {
                _testAction[_testIndex]();
                _testIndex++;
            }
            else
            {
                OnCloseUI(sender, e);
            }

        }
    }
}