
using UnityEngine;
using FrameWork.Launch;
using System.Collections;

public class GameStart : MonoBehaviour
{
    [Header("跳过热更流程")]
    public bool UnityEditor = false;
    [Header("跳过资源下载")]
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
