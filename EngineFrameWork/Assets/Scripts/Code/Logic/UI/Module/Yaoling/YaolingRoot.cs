using strange.extensions.context.impl;

namespace UI
{
    public class YaolingRoot : ContextView
    {
        private void Awake()
        {
            context = new YaolingContext(this);
        }
    }
}