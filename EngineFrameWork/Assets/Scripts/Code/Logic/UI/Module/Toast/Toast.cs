using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Toast : MonoBehaviour
    {
        public GameObject toastPrefab;
        public float fadeDuration = 0.3f;
        public Vector2 initVector = new Vector2(0, 0);
        private float toastHeight = 0f;
        private Queue<GameObject> toastQueue = new Queue<GameObject>();
        private IEventDispatcher dispatcher { get; set; }

        private void Start()
        {
            dispatcher = GameMgr.Ins.CrossDispatcher;
            toastHeight = toastPrefab.GetComponent<RectTransform>().rect.height;

            dispatcher.AddListener(ToastEvent.SHOW, OnShowToast);
        }

        private void OnDestroy()
        {
            dispatcher.RemoveListener(ToastEvent.SHOW, OnShowToast);
            dispatcher = null;
            toastQueue.Clear();
            toastQueue = null;
        }

        private void OnShowToast(IEvent payload)
        {
            ShowToast(payload.data as string);
        }

        public void ShowToast(string message)
        {
            GameObject toastObject = Instantiate(toastPrefab, transform);
            toastObject.GetComponent<RectTransform>().localPosition = initVector;
            toastObject.GetComponentInChildren<Text>().text = message;
            toastQueue.Enqueue(toastObject);

            MoveToNextPosition();
            StartCoroutine(ShowToastCoroutine(toastObject));
        }

        private IEnumerator ShowToastCoroutine(GameObject toastObject)
        {
            CanvasGroup canvasGroup = toastObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            toastObject.SetActive(true);

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

        private IEnumerator HideToastCoroutine(GameObject toastObject)
        {
            CanvasGroup canvasGroup = toastObject.GetComponent<CanvasGroup>();

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            toastQueue.Dequeue();
            Destroy(toastObject, 0.1f);
        }

        private void MoveToNextPosition()
        {
            GameObject[] toastObjArr = toastQueue.ToArray();
            for (int len = toastObjArr.Length, i = 0; i < len; ++i)
            {
                GameObject toastObject = toastObjArr[i];
                RectTransform transform = toastObject.GetComponent<RectTransform>();
                transform.localPosition = new Vector2(initVector.x, initVector.y + (len - 1 - i) * toastHeight);
            }
        }
    }
}
