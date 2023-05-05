using Model;
using strange.extensions.mediation.impl;

namespace UI
{
    public class LogInMediator : EventMediator
    {
        [Inject]
        public LogInView view { get; set; }

        [Inject]
        public GlobalData globalData { get; set; }

        public override void OnRegister()
        {
            ZDebug.Log("LogIn Mediator :  OnRegister");
        }

        public override void OnRemove()
        {
            ZDebug.Log("LogIn Mediator :  OnRemove");
        }

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
    }
}
