using strange.extensions.mediation.impl;
using UnityEngine;

namespace UI
{
    public class YaolingView : EventView
    {
        // private SerializeField Components
        [SerializeField] GameObject proxyScrollView;
        [SerializeField] RectTransform avatarRect;
        [SerializeField] RectTransform detailRect;

        // private Variables
        private float _scrollHeight = 0;
        private float _avatarInitHeight = 0;
        private float _detailInitHeight = 0;
        private YaolingMediator mediator;

        public void BindMediator(YaolingMediator _mediator)
        {
            mediator = _mediator;
        }

        protected override void Awake()
        {
            base.Awake();
            _scrollHeight = proxyScrollView.transform.Find("Content").GetComponent<RectTransform>().rect.height - proxyScrollView.GetComponent<RectTransform>().rect.height;
            _avatarInitHeight = avatarRect.rect.height;
            _detailInitHeight = detailRect.rect.height;
        }

        public void Move(Vector2 value)
        {
            float offsetY = (value[1] - 1.0f) * _scrollHeight;
            avatarRect.sizeDelta = new Vector2(avatarRect.sizeDelta.x, _avatarInitHeight + offsetY);
            detailRect.sizeDelta = new Vector2(detailRect.sizeDelta.x, _detailInitHeight - offsetY);
        }

        public void Back()
        {
            UIMgr.Ins.CloseUI();
        }
    }
}