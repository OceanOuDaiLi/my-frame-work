using strange.extensions.context.impl;

namespace UI
{
    public class NamePanelRoot : ContextView
    {
        private void Awake()
        {
            context = new NamePanelContext(this);
        }
    }
}