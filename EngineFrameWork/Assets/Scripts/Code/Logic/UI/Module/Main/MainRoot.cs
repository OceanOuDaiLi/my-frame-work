using strange.extensions.context.impl;

namespace UI
{
    public class MainRoot : ContextView
    {
        private void Awake()
        {
            context = new MainContext(this);
        }
    }
}