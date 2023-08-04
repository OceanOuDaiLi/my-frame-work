using TMPro;
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
        [SerializeField] TMP_Text tips = null;
        [SerializeField] TMP_Text loadingText = null;

        bool showInit = false;

        public void SetLoadingProg(float value)
        {
            if (value >= 1.0f)
            {
                slider.value = 1f;
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

            //progressNum.text = string.Format("{0}%", Convert.ToInt32(value * 100).ToString());
        }

        public void ShowLoading(bool value)
        {
            view.SetActive(value);
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

        public void ShowInit(bool value)
        {
            showInit = value;
            if (value)
            {
                StartCoroutine(DotAnimationCoroutine());
            }
        }

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

                if (!showInit) yield break;
            }
        }

    }
}