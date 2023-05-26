using System;
using UnityEngine.Networking;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {

        async ETTask UnityWebRequestGet(string url, Action<byte[]> response, string errorTips = "", Action failed = null)
        {
            LogProgress(string.Format($"UnityWebRequest url： {url} "));
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation webRequestAsync = webRequest.SendWebRequest();
                ETTask waitDown = ETTask.Create(true);
                webRequestAsync.completed += (asyncOperation) =>
                {
                    waitDown.SetResult();
                };

                await waitDown;
                waitDown = null;

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    OnNetError(string.Format($"请求失败 \n  URL: {url} "));
                    return;
                }
                response?.Invoke(webRequest.downloadHandler.data);
                webRequest.downloadHandler.Dispose();
            }

            void OnNetError(string info)
            {
                LogError(info);
                // todo: show error ui.
                //  "网络错误,重新开始?"
            }
        }
    }
}