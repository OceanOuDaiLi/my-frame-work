using strange.extensions.mediation.impl;


namespace UI
{
    public class MVCSNameView : EventView
    {
        private MVCSNameMediator mediator;

        public void BindMediator(MVCSNameMediator _mediator)
        {
            mediator = _mediator;
        }
    }
}