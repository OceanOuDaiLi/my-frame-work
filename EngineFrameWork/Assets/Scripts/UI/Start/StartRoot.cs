using strange.extensions.context.impl;

namespace UI
{
    public class StartRoot : ContextView
    {
        private void Awake()
        {
            context = new StartContext(this);
        }
    }
}