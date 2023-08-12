using strange.extensions.context.impl;

namespace UI
{
    public class NpcChatRoot : ContextView
    {
        private void Awake()
        {
            context = new NpcChatContext(this);
        }
    }
}