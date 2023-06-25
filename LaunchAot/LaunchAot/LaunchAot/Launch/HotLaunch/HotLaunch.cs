using System;
using UnityEngine;
using System.Linq;


namespace FrameWork.Launch
{
    public partial class HotLaunch : MonoBehaviour
    {
        // 跳过热更流程
        public bool UnityEditor = false;
        // 跳过资源下载
        public bool SkipDownLoadAsset = false;

        public System.Reflection.Assembly GameUIAsset { get; private set; }
        public System.Reflection.Assembly CSharpAsset { get; private set; }
        public System.Reflection.Assembly FrameWorkAsset { get; private set; }
        public System.Reflection.Assembly TechArtistAsset { get; private set; }

        public static HotLaunch Instance { get; private set; }


        public void Launch()
        {
            if (UnityEditor)
            {
                SkipCsharpHotFix();
            }
            else
            {
                Init();

                OnStart().Coroutine();
            }
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        async ETTask OnStart()
        {
            // 资源解压检测
            bool decompressPass = CheckDecompress();
            if (!decompressPass)
            {
                await CopyStreamingAssets();
            }

            // 资源更新检测
            if (!SkipDownLoadAsset)
            {
                bool needUpdate = await CheckHotFixVersion();
                if (!needUpdate)
                {
                    await PrepareDownload();

                    await DeleteOldAssets();

                    await DownLoadAssets();
                }
            }

            // 热更启动
            await HybridClrStart();
        }


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


        async ETTask HybridClrStart()
        {
            ETTask tcs = ETTask.Create(true);

            var request = AssetBundle.LoadFromFileAsync(_hotFixFile.FullName);

            request.completed += (opt) =>
            {
                /* Tips:
                 * 加载顺序遵循规则：先加载依赖的Assembly，再加载本体的Assembly。
                 * 例如：
                 * 你有A, B, C, D四个dll，
                 * A需要B，D
                 * C需要D
                 * 那么，加载顺序就是D, C, B, A
                 * 
                */

                AssetBundle dllAB = request.assetBundle;
                // load bytes.
                byte[] csBytes = dllAB.LoadAsset<TextAsset>("Assembly-CSharp.bytes").bytes;
                byte[] techArtistBytes = dllAB.LoadAsset<TextAsset>("TechArtist.bytes").bytes;
                byte[] frameWorkBytes = dllAB.LoadAsset<TextAsset>("FrameWotk.bytes").bytes;
                byte[] gameUIBytes = dllAB.LoadAsset<TextAsset>("GameUI.bytes").bytes;

                // load assembly.
                FrameWorkAsset = System.Reflection.Assembly.Load(frameWorkBytes);
                LogProgress("[HotLaunch:FrameWork Assembly Loaded] : " + (FrameWorkAsset != null).ToString());

                GameUIAsset = System.Reflection.Assembly.Load(gameUIBytes);
                LogProgress("[HotLaunch: GameUI Assembly Loaded] : " + (GameUIAsset != null).ToString());

                TechArtistAsset = System.Reflection.Assembly.Load(techArtistBytes);
                LogProgress("[HotLaunch:TechArtist Assembly Loaded] : " + (TechArtistAsset != null).ToString());

                CSharpAsset = System.Reflection.Assembly.Load(csBytes);
                LogProgress("[HotLaunch:CSharp Assembly Loaded] : " + (CSharpAsset != null).ToString());


                var appType = CSharpAsset.GetType("FrameWork.Application.Main");
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
            };

            await tcs;
            tcs = null;

            OnDispose();
        }

        void OnDispose()
        {
            cdnHosts = string.Empty;

            _assetDisk = null;
            _streamingDisk = null;
            updateFileStore = null;
            _assetReleaseDir = null;
            _streamingReleaseDir = null;


            localLst = null;
            serverLst = null;
            needUpdateLst = null;
            needDeleteLst = null;
            needUpdateFields = null;
            needDeleteFields = null;

            _hostIni = null;
            _localVer = null;
            _serverVer = null;

            _aotFile = null;
            _keyFile = null;
            _hostsFile = null;
            _hotFixFile = null;
            _updateFile = null;
            _versionFile = null;

            GC.Collect();
        }
    }
}