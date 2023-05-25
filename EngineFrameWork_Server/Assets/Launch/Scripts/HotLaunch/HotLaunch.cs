﻿using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace FrameWork.Launch
{

    public partial class HotLaunch : MonoBehaviour
    {

#if __CLIENT__
        [Header("跳过热更流程")]
        public bool SkipHotFix = true;

        /// <summary>
        /// 每个线程下载资源大小(Mb)
        /// </summary>
        [HideInInspector]
        public int TaskDownLoadSize = 200;

        void Start()
        {
            if (SkipHotFix)
            {
#if UNITY_EDITOR
                SkipCsharpHotFix();
#endif
            }
            else
            {
                Init();

                OnStart().Coroutine();
            }
        }

        /// <summary>
        /// Tips:首包全部使用StreamingAsset路径加载
        /// </summary>
        /// <returns></returns>
        async ETTask OnStart()
        {
            // 资源解压检测
            bool decompressPass = CheckDecompress();
            if (!decompressPass)
            {
                await CopyStreamingAssets();
            }

            // 资源更新检测
            bool needUpdate = await CheckHotFixVersion();
            if (!needUpdate)
            {
                await PrepareDownload();

                await StartDownload();
            }

            // 热更启动
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
        }
#endif

        async ETTask HybridClrStart()
        {
            ETTask tcs = ETTask.Create(true);

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

        void OnDispose()
        {
            cdnHosts = string.Empty;

            _assetDisk = null;
            _streamingDisk = null;
            _assetReleaseDir = null;
            _streamingReleaseDir = null;

            _hostIni = null;
            _localVer = null;
            _serverVer = null;

            _aotFile = null;
            _keyFile = null;
            _hostsFile = null;
            _hotFixFile = null;
            _updateFile = null;
            _versionFile = null;
        }
#endif

#if __SERVER__
        void Start()
        {
#if UNITY_EDITOR
            System.Reflection.Assembly cSharp = null;
            cSharp = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            System.Type appType = cSharp.GetType("Server.Http.HttpServer");

            var mainMethod = appType.GetMethod("OnStart");
            if (mainMethod == null)
            {
                UnityEngine.Debug.LogError($"[HotLaunch::HttpServer] OnStart is null");
                return;
            }

            mainMethod.Invoke(null, null);
#endif
        }
#endif
    }
}