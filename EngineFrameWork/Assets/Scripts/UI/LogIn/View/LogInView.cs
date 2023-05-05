using strange.extensions.mediation.impl;


namespace UI
{
    public class LogInView : EventView
    {
        public void OnClickLogIn()
        {
            ZDebug.Log("OnClickLogIn View");
            LogInModel logInModel = new LogInModel();
            // test data.
            logInModel.password = "password";
            logInModel.userName = "10001";
            logInModel.email = "chinanumone@163.com";
            dispatcher.Dispatch(LogInEvent.ON_CLICK_LOGIN, logInModel);
        }
    }
}