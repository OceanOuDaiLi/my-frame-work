using UnityEngine;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

namespace UI
{
    public class FightView : EventView
    {
        [SerializeField] Animator animator;
        [SerializeField] CanvasGroup skillBg;

        [SerializeField] Image geImg;
        [SerializeField] Image shiImg;
        [SerializeField] Sprite[] timeSpArry;       // 0 ~ 9; need to opt.

        private int countTime = 60;
        private FightMediator mediator;


        public void BindMediator(FightMediator _mediator)
        {
            mediator = _mediator;
        }

        public void OnGameFightStart()
        {
            animator.enabled = true;
        }

        public void OnStartAnimationEnd()
        {
            // start client time count.
            mediator.OnRoundPrepare();

            StarCountTime();
        }

        public void StarCountTime()
        {
            countTime = 60;
            geImg.gameObject.SetActive(true);
            shiImg.gameObject.SetActive(true);
            InvokeRepeating("CountTime", 1, 1);
        }

        public void CountTime()
        {
            //show time
            int geWei = countTime % 10;
            int shiWei = (countTime % 1000) / 10;

            geImg.sprite = timeSpArry[geWei];
            shiImg.sprite = timeSpArry[shiWei];
            if (countTime == 0)
            {
                CancelInvoke("CountTime");  // 进入自动
                AutoFight();
            }
            countTime--;
        }

        public void AutoFight()
        {
            OnClickSkillDemo();
        }

        public void OnClickSkillDemo()
        {
            if (IsInvoking("CountTime"))
                CancelInvoke("CountTime");

            // 1. hide count img.
            geImg.gameObject.SetActive(false);
            shiImg.gameObject.SetActive(false);

            // 2. hide skillpanel.
            OnClickHideSkillInfo();

            // 3. notifle auto operator. or user operator.
            mediator.OnDemoSkill();
        }

        public void OnClickShowSkillInfo()
        {
            if (AnimatorStateInfo("showSkill")) { return; }
            animator.Play("showSkill", 0, 0);
        }

        public void OnClickHideSkillInfo()
        {
            if (AnimatorStateInfo("hideSkill") || skillBg.alpha == 0) { return; }
            animator.Play("hideSkill", 0, 0);
        }

        private bool AnimatorStateInfo(string name)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
        }
    }
}