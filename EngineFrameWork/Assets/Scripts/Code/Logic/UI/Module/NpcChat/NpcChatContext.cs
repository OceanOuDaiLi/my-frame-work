using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class NpcChatContext : MVCSContext
    {
        public NpcChatContext(MonoBehaviour view) : base(view) { }

        public NpcChatContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<NpcChatView>().To<NpcChatMediator>();
        }
    }
}