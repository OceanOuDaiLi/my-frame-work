using strange.extensions.command.impl;

namespace UI
{
    public class LoginCommand : EventCommand
    {
        [Inject]
        public ILogInService loginService { get; set; }

        public override void Execute()
        {
            LogInModel loginModel = evt.data as LogInModel;

            if (loginModel.newUser)
            {
                loginService.Register(loginModel.userName, loginModel.password, loginModel.email, succeed);
            }
            else
            {
                loginService.LogIn(loginModel.userName, loginModel.password, succeed);
            }
        }

        void succeed(string token)
        {
            dispatcher.Dispatch(LogInEvent.RESPONSE_LOGIN_SUCCESS, token);
        }
    }
}
