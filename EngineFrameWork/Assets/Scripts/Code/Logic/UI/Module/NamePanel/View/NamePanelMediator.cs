using Model;
using System;
using FrameWork;
using GameEngine;
using Core.Interface.Event;
using strange.extensions.mediation.impl;

namespace UI
{
    public class NamePanelMediator : EventMediator
    {
        [Inject]
        public NamePanelView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        private IEventHandler eventHandlerGameFightStart = null;

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            view.BindMediator(this);

            // binds global events below.
            eventHandlerGameFightStart = App.Instance.On(FightEvent.FIGHT_EVENT_GAME_FIGHT_STATE_CHANGE, OnGameFightStateChange);
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


        private void OnGameFightStateChange(object sender, EventArgs e)
        {
            FightState state = (FightState)sender;
            if (state.Equals(FightState.Initialize))
                view.OnGameFightStart();
        }
    }
}
