using TMPro;
using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace UI
{
    public class LogInView : EventView
    {
        // Private SerializeField Components.
        [SerializeField] TMP_Text tostTxt;
        [SerializeField] Animator logInAnimator;

        [SerializeField] GameObject loginBg;
        [SerializeField] GameObject[] viewObjs;                // 0: login 1:register
        [SerializeField] GameObject[] newbieObjs;              // len = 4.

        // Private Variables.
        private int _newbieIdx = 0;
        private bool _isNewbie = false;
        private bool _logInSuccess = false;
        private const string PLAYERPREFS_NEW_GUY = "new_guy";

        private LogInMediator mediator;

        public void BindMediator(LogInMediator _mediator)
        {
            mediator = _mediator;
        }

        protected override void Awake()
        {
            base.Awake();
            _isNewbie = PlayerPrefs.GetInt(PLAYERPREFS_NEW_GUY) <= 0;
        }

        protected override void Start()
        {
            base.Start();

            StartCoroutine(FadeLogInView());
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mediator != null)
                mediator = null;
        }



        #region LogIn & Register Methods

        IEnumerator FadeLogInView()
        {
            yield return Yielders.GetWaitForSeconds(1.0f);
            logInAnimator.enabled = true;

            ShowOrHideLogInView();
        }

        private void ShowOrHideLogInView()
        {
            viewObjs[0].SetActive(!_isNewbie);
            viewObjs[1].SetActive(_isNewbie);
        }

        public void OnClickLogIn()
        {
            // fake demo login
            OnClickCloseLogIn();
            tostTxt.text = "登陆成功";
            logInAnimator.Play("tost", 0);
            _logInSuccess = true;

#warning todo: login CS - Logic.
            //mediator.OnClickLogIn();
        }

        public void OnClickRegister()
        {
            // fake demo register.
            OnClickCloseLogIn();
            tostTxt.text = "注册成功";
            logInAnimator.Play("tost", 0);
            _logInSuccess = true;

#warning todo: register CS - Logic.
            //mediator.OnClickReg();
        }

        public void OnClickCloseLogIn()
        {
            loginBg.SetActive(false);
        }

        #endregion

        #region NewBie Methods

        public void ShowNewBieByIdx(int idx)
        {
            if (idx > 0)
            {
                newbieObjs[_newbieIdx].SetActive(false);
            }
            _newbieIdx = idx;

            if (_newbieIdx >= newbieObjs.Length)
            {
                mediator.LoadGameScene();
                PlayerPrefs.SetInt(PLAYERPREFS_NEW_GUY, 1);
            }
            else
            {
                newbieObjs[_newbieIdx].SetActive(true);
            }
        }

        public void ShowNextNewBie()
        {
            int idx = _newbieIdx;
            idx++;
            ShowNewBieByIdx(idx);
        }

        #endregion

        public void AninationCallEnterGame()
        {
            OnClickEnterGame();
        }

        public void OnClickEnterGame()
        {
            _isNewbie = PlayerPrefs.GetInt(PLAYERPREFS_NEW_GUY) <= 0;

            if (!_logInSuccess)
            {
                loginBg.SetActive(true);
                viewObjs[0].SetActive(!_isNewbie);
                viewObjs[1].SetActive(_isNewbie);
                return;
            }

            //_isNewbie 新手执行
            if (_isNewbie)
            {
                _newbieIdx = 0;
                ShowNewBieByIdx(_newbieIdx);
            }
            //!_isNewbie 进入主城
            else
            {
                mediator.LoadGameScene();
            }
        }
    }
}