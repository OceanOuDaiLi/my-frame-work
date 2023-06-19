using Model;
using UnityEngine;
using strange.extensions.context.impl;

namespace UI
{

    public class UICrossContext : MVCSContext
    {
        public UICrossContext(MonoBehaviour view) : base(view) { }

        protected override void mapBindings()
        {
            CDebug.Log("UICrossContext Bind");
            TcpService tcpService = new TcpService();
            injectionBinder.Bind<TcpService>().ToValue(tcpService).ToSingleton().CrossContext();

            // data.
            GlobalData globalData = new GlobalData();
            globalData.tcpService = tcpService;
            injectionBinder.Bind<GlobalData>().ToValue(globalData).ToSingleton().CrossContext();

            // global event bind.
            //crossContextBridge.Bind(HeraldryEvent.UI_SHOW_HERALDRY_E_DESCRIPTION);

        }
    }
}