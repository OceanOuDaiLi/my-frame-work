using System;
using UnityEngine.Networking;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        /* 
         *  #### 
         *  1. WebRequest Task Supported.
         *  2. Mult
         *  ####
         *      
        */

        async ETTask UnityWebRequestGet(string url, Action<byte[]> response)
        {
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
                    OnNetError(string.Format($"«Î«Û ß∞‹ \n  URL: {url} "));
                    return;
                }
                response?.Invoke(webRequest.downloadHandler.data);
                webRequest.downloadHandler.Dispose();
            }

            void OnNetError(string info)
            {
                LogError(info);
                // todo: show error ui.
            }
        }
    }
}