using UI;
using Proto.Login;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace Model
{
    public class LoginModelMgr : BaseModelMgr
    {
        // Todo: Cache DB Config Data 
        // Todo: Cache User Model Data

        ///////////////////////////////////////////////
        ////////////LoginModelMgr Initialize///////////
        ///////////////////////////////////////////////

        public LoginModelMgr() : base()
        {
            OnRegister();

            // 获取 DB数据
        }

        public override void OnDispose()
        {
            OnRemove();
            base.OnDispose();
        }

        #region Auto Event Binding.
        private void OnRegister()
        {
            //  (工具动态生成)
            dispatcher.AddListener("Proto.Login.RspRegisterAccount", RspRegisterAccount);
        }

        private void OnRemove()
        {
            //  (工具动态生成)
            dispatcher.RemoveListener("Proto.Login.RspRegisterAccount", RspRegisterAccount);

        }
        #endregion


        ///////////////////////////////////////////////
        ////////////Proto Response Event///////////////
        ////////////动态生成接口，实现自定义///////////

        public void RspLogin(object evt)
        {
            // test code.
            //var data = (evt as TmEvent).data;
            //CDebug.Log(data.ToString());

            RspLogin msg = (RspLogin)(evt as TmEvent).data;
            dispatcher.Dispatch(LogInEvent.RESPONSE_LOGIN, msg);
        }

        public void RspRegisterAccount(object insMsg)
        {
            RspRegisterAccount msg = (RspRegisterAccount)(insMsg as TmEvent).data;
            if (msg.errorID == 0)
            {
                dispatcher.Dispatch(LogInEvent.RESPONSE_REG, msg);              // todo: global event send & listener.
            }
            else
            {
                CDebug.LogError("RegisterAccount failed : " + msg.errorID);
            }
        }

        public void RspCreateRole(object insMsg)
        {

        }

        public void RspEnterGame(object insMsg)
        {

        }

        public void RspVerifyIDCard(object insMsg)
        {

        }
    }
}