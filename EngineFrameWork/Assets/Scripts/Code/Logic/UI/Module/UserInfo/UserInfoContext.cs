using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace UI
{
    public class UserInfoContext : MVCSContext
    {
        public UserInfoContext(MonoBehaviour view) : base(view) { }

        public UserInfoContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

        protected override void mapBindings()
        {
            mediationBinder.Bind<UserInfoView>().To<UserInfoMediator>();
        }
    }
}