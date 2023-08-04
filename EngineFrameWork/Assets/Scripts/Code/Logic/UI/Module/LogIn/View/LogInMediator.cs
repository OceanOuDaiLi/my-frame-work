
using Model;
using System;
using FrameWork;
using Proto.Login;
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


        #region Events Binding.

        public override void OnRegister()
        {
            // 'OnRegister' excused order on this mono scripts: After -> 'Awake'、'OnEnable' && Before 'Start'.
            view.BindMediator(this);

            dispatcher.AddListener(LogInEvent.RESPONSE_LOGIN, OnResponseLogin);
            dispatcher.AddListener(LogInEvent.RESPONSE_REG, OnResponseReg);
        }

        public override void OnRemove()
        {

            dispatcher.RemoveListener(LogInEvent.RESPONSE_LOGIN, OnResponseLogin);
            dispatcher.RemoveListener(LogInEvent.RESPONSE_REG, OnResponseReg);
        }

        #endregion

        private void OnDestroy()
        {
            globalData = null;
            if (view != null)
                view = null;
        }

        private void ConnectServer(Action cbFunc)
        {
            var websocketService = globalData.websocketService;
            if (websocketService.State != WebSocketState.Connected)
            {
                websocketService.Connect("ws://192.168.1.86:8010", 8010);
                websocketService.onWebSocketConnected += cbFunc;
            }
            else
            {
                cbFunc?.Invoke();
            }
        }

        private void OnReqLogin()
        {

            //var websocketService = globalData.websocketService;
            //var szUserName = view.GetUserName();
            //var szPassword = view.GetPassword();

            //PlayerPrefs.SetString(view.szSaveUserName, szUserName);
            //PlayerPrefs.SetString(view.szSavePwd, szPassword);
            //PlayerPrefs.Save();


            //ReqLogin msg = new ReqLogin();
            //msg.userName = szUserName;
            //msg.Password = szPassword;
            //msg.loginType = 1;

            //msg.Info = new clientInfo();
            //msg.Info.AppUdid = "11111";
            //msg.Info.Channel = "1";
            //msg.Info.Device = "22222";
            //msg.Info.DeviceUdid = "222222";

            //msg.curVer = "10.1.1";
            //msg.engineVer = 100;
            //msg.isSetup = 1;

            //websocketService.SendMessage(msg);

            //view.ShowToast("欢迎贵宾 " + msg.userName);
            //view.showServerPanel(true);
            //view.showAccountPanel(false);
        }

        private void OnReqReg()
        {
            var websocketService = globalData.websocketService;
            var szUserName = "123456781";       //view.GetUserName();
            var szPassword = "123456";          //view.GetPassword();

            ReqRegisterAccount msg = new ReqRegisterAccount();
            msg.Account = szUserName;
            msg.Password = szPassword;
            msg.Info = new clientInfo();
            msg.Info.AppUdid = "11111";
            msg.Info.Channel = "1";
            msg.Info.Device = "22222";
            msg.Info.DeviceUdid = "222222";

            websocketService.SendMessage(msg);
        }

        //        msg.userName = "test101";
        //        msg.Password = "123456";
        public void OnClickLogIn()
        {
            ConnectServer(OnReqLogin);
        }

        public void OnClickReg()
        {
            ConnectServer(OnReqReg);
        }

        private void OnResponseLogin(object evt)
        {
            string token = (evt as TmEvent).data as string;
            CDebug.Log($"LoginSuccess -> Start WebSocket  ");
        }
        private void OnResponseReg(object evt)
        {
            string token = (evt as TmEvent).data as string;
            CDebug.Log($"OnResponseReg -> OnResponseReg  ");
        }

        public void LoadGameScene()
        {
            App.Instance.Trigger(LoadEvent.LOAD_GAME_SCENE);
        }
    }
}
