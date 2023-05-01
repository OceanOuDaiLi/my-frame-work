using strange.extensions.context.impl;

namespace UI
{
    public class MVCSNameRoot : ContextView
    {
        private void Awake()
        {
            context = new MVCSNameContext(this);
        }
    }
}