using Model;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;

namespace UI
{
    public class LogInMediator : EventMediator
    {
        [Inject]
        public LogInView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        LogInModel loginModel = null;

        #region Events Binding.

        public override void OnRegister()
        {
            ZDebug.Log("LogIn Mediator :  OnRegister");
            view.dispatcher.AddListener(LogInEvent.ON_CLICK_LOGIN, OnClickLogIn);

            dispatcher.AddListener(LogInEvent.RESPONSE_LOGIN_SUCCESS, OnResponseLoginSuccess);
        }

        public override void OnRemove()
        {
            ZDebug.Log("LogIn Mediator :  OnRemove");
            view.dispatcher.RemoveListener(LogInEvent.ON_CLICK_LOGIN, OnClickLogIn);
        }
        #endregion

        #region Unity Calls

        private void OnEnable()
        {
            ZDebug.Log("LogIn Mediator :  OnEnable");
        }

        private void Awake()
        {
            ZDebug.Log("LogIn Mediator :  Awake");
        }

        private void Start()
        {
            ZDebug.Log("LogIn Mediator :  Start");

            ZDebug.Log("LogIn Mediator :  globalData == null ; " + globalData == null);
        }

        #endregion

        private void OnClickLogIn(object evt)
        {
            ZDebug.Log("OnClickLogIn Mediator");

            // todo check account/password/mail_regex_check len.
            //loginModel = (evt as TmEvent).data as LogInModel;
            dispatcher.Dispatch(LogInEvent.REQUEST_CLICK_LOGIN, evt);
        }

        private void OnResponseLoginSuccess(object evt)
        {
            string token = (evt as TmEvent).data as string;
            ZDebug.Log("OnResponseLoginSuccess: result = > " + token);
        }
    }
}
