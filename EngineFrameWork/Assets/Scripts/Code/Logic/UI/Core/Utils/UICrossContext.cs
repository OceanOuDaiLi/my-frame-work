using Model;
using UnityEngine;
using strange.extensions.context.impl;

namespace UI
{
    public class UICrossContext : MVCSContext
    {
        public bool Inited { get; private set; }
        public UICrossContext(MonoBehaviour view) : base(view) { }

        protected override void mapBindings()
        {
            WebSocketService websocketService = new WebSocketService();
            injectionBinder.Bind<WebSocketService>().ToValue(websocketService).ToSingleton().CrossContext();

            // data.
            GlobalData globalData = new GlobalData();
            globalData.websocketService = websocketService;
            injectionBinder.Bind<GlobalData>().ToValue(globalData).ToSingleton().CrossContext();

            // proto response event bind.
            BindUIProtoEvent();

            Inited = true;
        }

        private void BindUIProtoEvent()
        {
            var dicData = ProtoData.GetS2CProtoDic();

            foreach (var data in dicData)
            {
                crossContextBridge.Bind(data.Value.FullName);
            }
        }
    }
}