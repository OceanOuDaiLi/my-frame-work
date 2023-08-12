using Model;
using System;
using FrameWork;
using GameEngine;
using UnityEngine;
using Core.Interface.Event;
using strange.extensions.mediation.impl;

namespace UI
{
    public class MainMediator : EventMediator
    {
        [Inject]
        public MainView view { get; set; }
        [Inject]
        public GlobalData globalData { get; set; }

        private IEventHandler eventHandlerHeroMove = null;

        private IEventHandler eventHandlerGameFightStart = null;

        private IEventHandler eventHandlerInputSingleClick = null;

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            // binds global events below.

            eventHandlerHeroMove = App.Instance.On(HeroModelMgr.ON_HERO_POS_CHANGED, OnHeroMove);
            eventHandlerGameFightStart = App.Instance.On(FightEvent.FIGHT_EVENT_GAME_FIGHT_STATE_CHANGE, OnGameFightStartEffect);

            if (eventHandlerInputSingleClick == null)
                eventHandlerInputSingleClick = App.Instance.On(GameEvent.INPUT_EVENT_SINGLE_CLICK, OnHasAnybodyClick);
        }

        public override void OnRemove()
        {
            // remove global events' below.
            if (eventHandlerHeroMove != null)
            {
                eventHandlerHeroMove.Cancel();
            }
            eventHandlerHeroMove = null;

            if (eventHandlerGameFightStart != null)
            {
                eventHandlerGameFightStart.Cancel();
            }
            eventHandlerGameFightStart = null;

            if (eventHandlerInputSingleClick != null)
            {
                eventHandlerInputSingleClick.Cancel();
                eventHandlerInputSingleClick = null;
            }
        }

        private void OnHeroMove(object sender, EventArgs e)
        {
            Vector2Int message = (Vector2Int)sender;
            view.SetPositionText(message.ToString());
        }

        private void OnGameFightStartEffect(object sender, EventArgs e)
        {
            FightState state = (FightState)sender;

            if (state.Equals(FightState.Initialize))
                view.OnGameFightStart();
        }

        private void OnHasAnybodyClick(object sender, EventArgs e)
        {
            var target = (InputCatcher.SingleClickData)sender;
            view.OnHasAnybodyClick(target);
        }

    }
}
