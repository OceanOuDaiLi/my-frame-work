using System;
using UnityEngine;
using System.Linq;


namespace FrameWork.Launch
{
    public partial class HotLaunch : MonoBehaviour
    {
        [Header("跳过热更流程")]
        public bool UnityEditor = true;

        public static HotLaunch Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        void Start()
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

                await DeleteOldAssets();

                await DownLoadAssets();
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
                */

                AssetBundle dllAB = request.assetBundle;
                // load bytes.
                byte[] csBytes = dllAB.LoadAsset<TextAsset>("Assembly-CSharp.bytes").bytes;
                byte[] techArtistBytes = dllAB.LoadAsset<TextAsset>("TechArtist.bytes").bytes;

                // load assembly.
                System.Reflection.Assembly GameAsset = System.Reflection.Assembly.Load(csBytes);
                LogProgress("[HotLaunch:CSharp Assembly Loaded] : " + (GameAsset != null).ToString());

                System.Reflection.Assembly TechArtisttAsset = System.Reflection.Assembly.Load(techArtistBytes);
                LogProgress("[HotLaunch:TechArtist Assembly Loaded] : " + (TechArtisttAsset != null).ToString());

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