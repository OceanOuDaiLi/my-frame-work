using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace UI
{
    public class LogInService : BaseService, ILogInService
    {
        public void LogIn(string username, string password, Action<string> callback)
        {
            //Dictionary<string, object> user = new Dictionary<string, object>();
            //user.Add("username", username);
            //user.Add("password", password);

            //Dictionary<string, object> param = new Dictionary<string, object>();
            //param.Add("user", user);
            //param.Add("d_id", SystemInfo.deviceUniqueIdentifier);
            //param.Add("channel_code", "googleplay");

            //globalData.httpService.Request("/acc/login", param, (jsonDict) =>
            //{
            //    if (jsonDict.ContainsKey("token"))
            //    {
            //        string accessToken = jsonDict["token"].ToString();
            //        tcpService.SetAccessToken(accessToken);
            //        callback(accessToken);
            //    }
            //    else callback(string.Empty);
            //});

            callback?.Invoke("test");
        }

        public void Register(string username, string password, string email, Action<string> callback)
        {

        }
    }
}
