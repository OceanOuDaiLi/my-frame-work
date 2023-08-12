using Model;
using System;
using FrameWork;
using GameEngine;
using Core.Interface.Event;
using strange.extensions.mediation.impl;

namespace UI
{
    public class FightMediator : EventMediator
    {
        [Inject]
        public FightView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        private FightModelMgr modelMgr { get; set; }
        private IEventHandler eventHandlerGameFightStart = null;


        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            view.BindMediator(this);

            // binds global events below.
            eventHandlerGameFightStart = App.Instance.On(FightEvent.FIGHT_EVENT_GAME_FIGHT_STATE_CHANGE, OnGameFightStartEffect);
        }

        public override void OnRemove()
        {
            // remove global events' below.
            if (eventHandlerGameFightStart != null)
            {
                eventHandlerGameFightStart.Cancel();
            }
            eventHandlerGameFightStart = null;
        }

        public void OnRoundPrepare()
        {
            FightControl.Ins.State = FightState.RoundStart;
        }

        int round = 1;
        public void OnDemoSkill()
        {
            if (modelMgr == null) modelMgr = globalData.fightModelMgr;

            switch (round)
            {
                case 1:
                    // 第一回合都普攻对位敌人
                    // 模拟服务器，编辑组装一个简单的普通攻击FSM。 [需预留fsm并行执行功能.]
                    FightControl.Ins.DemoSimulateServerNormalAttack();
                    break;
                case 2:
                    // 第二回合，技能释放。结束战斗
                    break;
            }
        }

        private void OnGameFightStartEffect(object sender, EventArgs e)
        {
            FightState state = (FightState)sender;
            if (state.Equals(FightState.Start))
            {
                view.OnGameFightStart();
            }
        }
    }
}
