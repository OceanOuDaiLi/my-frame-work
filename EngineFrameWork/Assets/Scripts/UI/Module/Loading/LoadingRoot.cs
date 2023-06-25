using strange.extensions.context.impl;

namespace UI {
    public class LoadingRoot : ContextView {
        private void Awake() {
            context = new LoadingContext(this);
        }
    }
}