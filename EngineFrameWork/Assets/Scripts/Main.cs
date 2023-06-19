using UI;
using System;
using HybridCLR;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FrameWork.Application
{
    public class Main
    {
        public static void HotFixStart(string aotDllPath)
        {
            CDebug.EnableLog = true;

            UIMgr.Ins.Startup();
            GameMgr.Ins.Startup();

#if !UNITY_EDITOR
            GameMgr.Ins.StartCoroutine(LoadMetadataForAOTAssemblies(aotDllPath));
#else
            GameMgr.Ins.StartCoroutine(Launch());
#endif
        }

        #region Game Launch

        private static IEnumerator LoadMetadataForAOTAssemblies(string aotDllPath)
        {
            var request = AssetBundle.LoadFromFileAsync(aotDllPath);
            while (!request.isDone) { yield return null; }

            AssetBundle dllAB = request.assetBundle;

            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessors里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in dllAB.GetAllAssetNames())
            {
                byte[] dllBytes = dllAB.LoadAsset<TextAsset>(aotDllName).bytes;
                try
                {
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                    if (!err.Equals(LoadImageErrorCode.OK))
                    {
                        CDebug.Log($"[Main::LoadMetadataForAotAssembly error] load {aotDllName} ret:{err.ToString()}");
                    }
                    else
                    {
                        CDebug.Log($"[Main::LoadMetadataForAotAssembly] load {aotDllName} ret:{err}");
                    }
                }
                catch (Exception ex)
                {
                    CDebug.Log($"[Main::LoadMetadataForAotAssembly error] load {aotDllName} exception");
                    CDebug.LogError(ex);
                    throw;
                }
            }

            CDebug.Log($"[Main::LoadMetadataForAotAssembly] finished!");

            FrameWork.Launch.Utils.Utility.ResolveVolumeManager();

            GC.Collect();

            yield return Launch();
        }

        private static IEnumerator Launch()
        {
            while (!GameMgr.Ins.Inited && !UIMgr.Ins.Inited)
            {
                yield return Yielders.EndOfFrame;
            }

            UIMgr.Ins.BindUICrossRoot();

            AsyncOperation sc = SceneManager.LoadSceneAsync("Scenes/LogIn", new LoadSceneParameters(LoadSceneMode.Single));
            while (!sc.isDone)
            {
                yield return Yielders.EndOfFrame;
            }

            //UIConfig loadingView = new UIConfig();
            //loadingView.floaderName = "loading";
            //loadingView.prefabName = "loading";
            //UIMgr.Ins.OpenUI(loadingView, (s) =>
            //{
            //    CCDebug.Log("Loading View Opened");
            //});

            UIConfig launchView = new UIConfig();
            launchView.floaderName = "login";
            launchView.prefabName = "login";
            UIMgr.Ins.OpenUI(launchView, (s) =>
            {


            });

        }
        #endregion
    }
}