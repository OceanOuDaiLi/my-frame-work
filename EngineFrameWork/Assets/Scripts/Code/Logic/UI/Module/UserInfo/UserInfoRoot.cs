using strange.extensions.context.impl;

namespace UI
{
    public class UserInfoRoot : ContextView
    {
        private void Awake()
        {
            context = new UserInfoContext(this);
        }
    }
}