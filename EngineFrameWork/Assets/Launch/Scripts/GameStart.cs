
using UnityEngine;
using FrameWork.Launch;
using System.Collections;

public class GameStart : MonoBehaviour
{
    [Header("�����ȸ�����")]
    public bool UnityEditor = false;
    [Header("������Դ����")]
    public bool SkipDownLoadAsset = false;

    private void Awake()
    {
        GameObject hotLaunchObj = new GameObject("HotLaunch");
        hotLaunchObj.AddComponent<HotLaunch>();
    }

    private IEnumerator Start()
    {
        while (HotLaunch.Instance == null)
        {
            yield return null;
        }
#if UNITY_EDITOR
        HotLaunch.Instance.UnityEditor = UnityEditor;
        HotLaunch.Instance.SkipDownLoadAsset = SkipDownLoadAsset;
#else
        HotLaunch.Instance.UnityEditor = false;
        HotLaunch.Instance.SkipDownLoadAsset = true;
#endif

        HotLaunch.Instance.Launch();
    }
}
