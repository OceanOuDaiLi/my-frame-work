using System;

namespace UI
{
    public interface ILogInService
    {
        void LogIn(string username, string password, Action<string> callback);

        void Register(string username, string password, string email, Action<string> callback);
    }
}