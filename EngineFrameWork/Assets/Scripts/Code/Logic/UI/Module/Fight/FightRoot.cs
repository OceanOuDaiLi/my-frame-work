using strange.extensions.context.impl;

namespace UI
{
    public class FightRoot : ContextView
    {
        private void Awake()
        {
            context = new FightContext(this);
        }
    }
}