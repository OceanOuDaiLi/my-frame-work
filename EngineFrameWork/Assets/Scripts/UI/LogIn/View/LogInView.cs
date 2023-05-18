using strange.extensions.mediation.impl;


namespace UI
{
    public class LogInView : EventView
    {
        private LogInMediator mediator;

        public void BindMediator(LogInMediator _mediator)
        {
            mediator = _mediator;
        }

        protected override void Awake()
        {
            base.Awake();
            ZDebug.Log("LogInView Awake");
        }

        public void OnClickLogIn()
        {
            ZDebug.Log("OnClickLogIn View");
            LogInModel logInModel = new LogInModel();
            // test data.
            logInModel.password = "password";
            logInModel.userName = "10001";
            logInModel.email = "chinanumone@163.com";
            mediator.OnClickLogIn(logInModel);

            //dispatcher.Dispatch(LogInEvent.ON_CLICK_LOGIN, logInModel);
        }
    }
}