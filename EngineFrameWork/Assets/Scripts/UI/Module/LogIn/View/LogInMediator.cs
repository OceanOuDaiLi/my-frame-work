using Model;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace UI
{
    public class LogInMediator : EventMediator
    {
        [Inject]
        public LogInView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        //LogInModel loginModel = null;

        #region Events Binding.

        public override void OnRegister()
        {
            CDebug.Log("LogIn Mediator :  OnRegister");
            view.BindMediator(this);

            view.dispatcher.AddListener(LogInEvent.ON_CLICK_LOGIN, OnClickLogIn);

            dispatcher.AddListener(LogInEvent.RESPONSE_LOGIN_SUCCESS, OnResponseLoginSuccess);
        }

        public override void OnRemove()
        {
            CDebug.Log("LogIn Mediator :  OnRemove");
            view.dispatcher.RemoveListener(LogInEvent.ON_CLICK_LOGIN, OnClickLogIn);

            dispatcher.RemoveListener(LogInEvent.RESPONSE_LOGIN_SUCCESS, OnResponseLoginSuccess);
        }
        #endregion

        #region Unity Calls

        private void OnEnable()
        {
            CDebug.Log("LogIn Mediator :  OnEnable");
        }

        private void Awake()
        {
            CDebug.Log("LogIn Mediator :  Awake");
        }

        private void Start()
        {
            CDebug.Log("LogIn Mediator :  Start");

            CDebug.Log("LogIn Mediator :  globalData == null ; " + globalData == null);
        }

        #endregion

        public void OnClickLogIn(LogInModel logInModel)
        {
            CDebug.Log("OnClickLogIn Mediator 1");

            // todo check account/password/mail_regex_check len.
            //loginModel = (evt as TmEvent).data as LogInModel;
            dispatcher.Dispatch(LogInEvent.REQUEST_CLICK_LOGIN, logInModel);
        }

        private void OnClickLogIn(object evt)
        {
            CDebug.Log("OnClickLogIn Mediator 2");

            // todo check account/password/mail_regex_check len.
            //loginModel = (evt as TmEvent).data as LogInModel;
            dispatcher.Dispatch(LogInEvent.REQUEST_CLICK_LOGIN, evt);
        }

        private void OnResponseLoginSuccess(object evt)
        {
            string token = (evt as TmEvent).data as string;
            CDebug.Log("OnResponseLoginSuccess: result = > " + token);
        }
    }
}
