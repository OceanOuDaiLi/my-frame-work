using System;
using FrameWork;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

/*  
         *  #### 
         *  IOS首包，
         *  解压/下载操作，
         *  应该显示具体解压大小和下载大小，
         *  否则将影响过审。
         *  ####
         *  
         *  Process overview
         * 
         *  1. DeCompress Check
         *        No Pass： Do DeCompressing
         * 
         *  2. Do Version Check
         *        No Pass： Do Download & Unzip
         *  
         *  3. CLR Start
         *  
         *  4. Start/LogIn Scene
         *  
         *  
         *  流程简述
         * 
         *  1. 资源解压检测
         *      1.1.解压检测不通过。执行解压
         * 
         *  2.  资源版本号检测
         *       2.1.版本号检测不通过.
         *        2.1.1.资源下载.
         *         2.1.2 顺延版本下载或跨版本加载检测.
         *          2.1.3 顺延版本下载，直接下载Zip，并解压到PersistentDataPath.
         *           2.1.4 跨版本下载，按文件下载到PersistentDataPath.
         *           
         *       2.2 CLR启动
         *      
         *  3.  切换到登录或开启场景     
         *      
         *      
         *  功能支持：
         *          [1].兼容使用StreamingAsset加载(加载策略:持久化路径无文件，则加载StreamingAsset下文件)
         *          [2].支持整包替换。
         *      
        */
namespace FrameWork.Launch
{
    public partial class HotLaunch : MonoBehaviour
    {
        string TxtTips = string.Empty;
        [SerializeField] bool IsCodeCrypt = false;
        [SerializeField] bool IsAssetCrypt = false;
        [HideInInspector] bool DEBUG_MODEL = false;

        void Start()
        {
            Init();

            if (DEBUG_MODEL)
            {
                SkipCsharpHotFix();
            }
            else
            {
                OnStart().Coroutine();
            }
        }

        async ETTask OnStart()
        {
            bool decompressPass = CheckDecompressPass();
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

            OnDispose();
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
            var request = AssetBundle.LoadFromFileAsync(_hotFixFile.FullName);
            request.completed += (opt) =>
            {
                AssetBundle dllAB = request.assetBundle;
                byte[] csBytes = dllAB.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes").bytes;

                // 先加载依赖的，再加载本体
                //你有A, B, C, D四个dll
                //A需要B，D
                //C需要D
                //那么，加载顺序就是D, C, B, A
                System.Reflection.Assembly GameAsset = System.Reflection.Assembly.Load(csBytes);
                LogProgress("Load Asset --- gameAsset is null : " + (csBytes == null).ToString());


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
            _codeDisk = null;
            _assetDisk = null;
            _streamingDisk = null;
            _codeReleaseDir = null;
            _assetReleaseDir = null;
            _streamingReleaseDir = null;

            _hostIni = null;
            _localVer = null;
            _serverVer = null;
        }
    }
}