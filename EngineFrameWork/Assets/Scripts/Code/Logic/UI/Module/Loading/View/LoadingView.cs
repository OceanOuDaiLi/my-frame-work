using TMPro;
using GameEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

        #region toast variables
        [Space(5)]
        [Header("Toast Variables")]
        public float fadeDuration = 0.3f;
        public Vector2 initVector = new Vector2(0, 0);
        private float toastHeight = 0f;
        private Queue<Transform> toastQueue = new Queue<Transform>();

        #endregion

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

        #region Toast
#warning This is must rewrite code.unreasonable implement func.

        public void ShowToast(string message)
        {
            Transform toastTrans = UIPool.Ins.SpawnUI(GameConfig.UI_TOAST);
            toastTrans.SetParent(transform);

            toastTrans.GetComponent<RectTransform>().localPosition = initVector;
            toastTrans.GetComponentInChildren<Text>().text = message;
            toastQueue.Enqueue(toastTrans);

            MoveToNextPosition();
            StartCoroutine(ShowToastCoroutine(toastTrans));
        }

        private IEnumerator ShowToastCoroutine(Transform toastObject)
        {
            CanvasGroup canvasGroup = toastObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(2f);
            StartCoroutine(HideToastCoroutine(toastObject));
        }

        private IEnumerator HideToastCoroutine(Transform toastTrans)
        {
            CanvasGroup canvasGroup = toastTrans.GetComponent<CanvasGroup>();

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0f;

            var tar = toastQueue.Dequeue();
            UIPool.Ins.DespawnUI(tar);
        }

        private void MoveToNextPosition()
        {
            Transform[] toastObjArr = toastQueue.ToArray();
            for (int len = toastObjArr.Length, i = 0; i < len; ++i)
            {
                GameObject toastObject = toastObjArr[i].gameObject;
                RectTransform transform = toastObject.GetComponent<RectTransform>();
                transform.localPosition = new Vector2(initVector.x, initVector.y + (len - 1 - i) * toastHeight);
            }
        }

        #endregion

    }
}