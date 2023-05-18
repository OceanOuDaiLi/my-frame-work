using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using strange.extensions.mediation.impl;

namespace UI
{
    public class LoadingView : EventView
    {
        [SerializeField] GameObject view = null;

        [SerializeField] Slider slider = null;
        [SerializeField] Text tips = null;
        [SerializeField] Text loadingText = null;
        [SerializeField] Text progressNum = null;

        [SerializeField] GameObject dataLoadingView = null;     //透明遮罩层
        [SerializeField] GameObject dataLoadingImg = null;      //菊花

        // Private Variables
        private float dataLoadingTime;                          //开始dataLoading时间
        private System.Random random = new System.Random();
        private Coroutine dotAnimationCRHandler = null;

        public float showLoadImgTime = 1.5f;

        private void Update()
        {
            if (dataLoadingTime > 0.0f && Time.time - dataLoadingTime > showLoadImgTime)
            {
                ShowDataLoadimgImg();
            }
        }

        public void SetLoadingProg(float value)
        {
            if (value >= 1.0f)
            {
                slider.value = 1.0f;
                view.SetActive(false);
                return;
            }

            if (value >= 0.0f && value < 1.0f)
            {
                slider.value = value;
                if (!view.activeSelf)
                {
                    view.SetActive(true);
                }
            }

            progressNum.text = string.Format("{0}%", Convert.ToInt32(value * 100).ToString());
        }

        public float GetLoadingProg()
        {
            return 0;
        }

        public void SetLoadingTips(string text)
        {
            tips.text = string.Empty;
            tips.text = text;
        }

        #region dataLoadingView 菊花view

        /// <summary>
        /// 显示透明遮罩层
        /// </summary>
        public void ShowDataLoadingView()
        {
            if (dataLoadingView.activeInHierarchy == false)
            {
                dataLoadingView.SetActive(true);
                dataLoadingTime = Time.time;
            }
        }

        /// <summary>
        /// 隐藏透明遮罩层
        /// </summary>
        public void HidenDataLoadingView()
        {
            if (dataLoadingView.activeInHierarchy == true)
            {
                dataLoadingImg.SetActive(false);
                dataLoadingView.SetActive(false);
                dataLoadingTime = 0.0f;
            }
        }

        /// <summary>
        /// 显示菊花
        /// </summary>
        public void ShowDataLoadimgImg()
        {
            dataLoadingImg.SetActive(true);
        }

        /// <summary>
        /// 立刻显示菊花
        /// </summary>
        public void ShowLoading()
        {
            dataLoadingImg.SetActive(true);
            dataLoadingView.SetActive(true);
        }
        #endregion

        //正在初始化...     delay 为间隔时间0.5f
        IEnumerator DotAnimationCoroutine(float delay = 0.3f)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                loadingText.text = "正在初始化.";
                yield return new WaitForSeconds(delay);
                loadingText.text = "正在初始化..";
                yield return new WaitForSeconds(delay);
                loadingText.text = "正在初始化...";

                if (!view.activeSelf) yield break;
            }
        }

        void StartDotAnimation()
        {
            if (dotAnimationCRHandler == null) dotAnimationCRHandler = StartCoroutine(DotAnimationCoroutine());
        }

        void StopDotAnimation()
        {
            if (dotAnimationCRHandler == null) return;

            StopCoroutine(dotAnimationCRHandler);
            dotAnimationCRHandler = null;
        }
    }
}