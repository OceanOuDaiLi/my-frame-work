using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace FrameWork.Launch
{
    public partial class HotLaunch : MonoBehaviour
    {
        string TxtTips = string.Empty;

        //先默认不加密
        bool IsCodeCrypt = false;
        bool IsAssetCrypt = false;

        void Start()
        {

#if UNITY_EDITOR
            SkipCsharpHotFix();
#else
            Init();

            OnStart().Coroutine();
#endif
        }

        async ETTask OnStart()
        {
            bool decompressPass = CheckDecompress();
            if (!decompressPass)
            {
                await AccompanyFilesDecompress();
            }

            /* 
             * test code.
             * 首包全部使用StreamingAsset路径加载
             * 跳过热更新流程
                bool versionPass = await CheckVersionPass();
                if (!versionPass)
                {
                    PrepareDownload();

                    await StartDownload();
                }
            */

            await HybridClrStart();
        }

#if UNITY_EDITOR
        void SkipCsharpHotFix()
        {
            System.Reflection.Assembly cSharp = null;
            cSharp = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            System.Type appType = cSharp.GetType("FrameWork.Application.Main");


            var mainMethod = appType.GetMethod("HotFixStart");
            if (mainMethod == null)
            {
                UnityEngine.Debug.LogError($"[HotLaunch::SkipCsharpHotFix] Main is null");
                return;
            }

            mainMethod.Invoke(null, new object[] { "" });

            OnDispose();
        }
#endif

        async ETTask HybridClrStart()
        {
            ETTask tcs = ETTask.Create(true);

            Debug.Log("HybridClrStart: " + _hotFixFile.Exists);

            var request = AssetBundle.LoadFromFileAsync(_hotFixFile.FullName);

            request.completed += (opt) =>
            {
                AssetBundle dllAB = request.assetBundle;
                byte[] csBytes = dllAB.LoadAsset<TextAsset>("Assembly-CSharp.bytes").bytes;

                // 先加载依赖的，再加载本体
                //你有A, B, C, D四个dll
                //A需要B，D
                //C需要D
                //那么，加载顺序就是D, C, B, A
                System.Reflection.Assembly GameAsset = System.Reflection.Assembly.Load(csBytes);

                var appType = GameAsset.GetType("FrameWork.Application.Main");
                if (appType == null)
                {
                    LogError("[HotLaunch::HotFixStart] appType is null");
                }

                var mainMethod = appType.GetMethod("HotFixStart");
                if (mainMethod == null)
                {
                    LogError("[HotLaunch::RunDll] HotFixStart is null");
                }

                mainMethod.Invoke(null, new object[] { _aotFile.FullName });

                tcs.SetResult();
                tcs = null;
            };

            await tcs;
        }

        void ShowTips(string tips)
        {
            TxtTips = tips;
        }

        void OnDispose()
        {
            //_codeDisk = null;
            //_assetDisk = null;
            //_streamingDisk = null;
            //_codeReleaseDir = null;
            //_assetReleaseDir = null;
            //_streamingReleaseDir = null;

            _hostIni = null;
            _localVer = null;
            _serverVer = null;
        }
    }
}