using strange.extensions.context.impl;

namespace UI
{
    public class LogInRoot : ContextView
    {
        private void Awake()
        {
            context = new LogInContext(this);
        }
    }
}