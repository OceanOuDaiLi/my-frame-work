using UI;
using Model;
using System;
using HybridCLR;
using GameEngine;
using UnityEngine;
using System.Collections;

namespace FrameWork.Application
{
    public class Main
    {
        public static void HotFixStart(string aotDllPath)
        {
            CDebug.EnableLog = true;

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

            // FrameWork.Launch.Utils.Utility.ResolveVolumeManager();

            GC.Collect();

            yield return Launch();
        }

        private static IEnumerator Launch()
        {
            GameMgr.Ins.Startup();
            while (!GameMgr.Ins.Inited) { yield return Yielders.EndOfFrame; }

            InputCatcher.Ins.Startup();

            UIMgr.Ins.Startup();
            while (!UIMgr.Ins.Inited()) { yield return Yielders.EndOfFrame; }

            while (GlobalData.instance == null) { yield return Yielders.EndOfFrame; }
            GlobalData.instance.Initialize();

            yield return GameMgr.Ins.LoadUICommonCanvas();
            while (!GameMgr.Ins.LoadedCommonCanvas) { yield return Yielders.EndOfFrame; }

            SceneLoadMgr.Ins.Startup();
            // Do PreLoad AseetBundle.
            yield return SceneLoadMgr.Ins.PreLoadPoolAssets();

            string path = $"ui/prefabs/login/login";
            yield return GameMgr.Ins.LoadGameAssets("LogIn", path, (tar) =>
            {
                GameObject parent = GameObject.Find("Canvas");

                for(int i=0;i< parent.transform.childCount;i++)
                {
                    parent.transform.GetChild(i).gameObject.SetActive(false);
                }
                tar.transform.SetParent(parent.transform, false);
            });
        }
        #endregion
    }
}