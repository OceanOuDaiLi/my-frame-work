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
            ZDebug.EnableLog = true;

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
                        Debug.Log($"[Main::LoadMetadataForAotAssembly error] load {aotDllName} ret:{err.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"[Main::LoadMetadataForAotAssembly] load {aotDllName} ret:{err}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"[Main::LoadMetadataForAotAssembly error] load {aotDllName} exception");
                    Debug.LogException(ex);
                    throw;
                }
            }

            Debug.Log($"[Main::LoadMetadataForAotAssembly] finished!");

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

            //UIConfig startView = new UIConfig();
            //startView.floaderName = "start";
            //startView.prefabName = "StartView";
            //bool opened = false;
            //UIMgr.Ins.OpenUI(startView, (s) => { opened = true; });

            AsyncOperation sc = SceneManager.LoadSceneAsync("Scenes/LogIn", new LoadSceneParameters(LoadSceneMode.Single));
            while (/*!opened ||*/ !sc.isDone)
            {
                yield return Yielders.EndOfFrame;
            }

            Debug.Log("## End ##");
        }


        #endregion
    }
}