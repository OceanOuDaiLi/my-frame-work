using System;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace FrameWork.Launch
{
    public partial class HotLaunch : MonoBehaviour
    {
        //[SerializeField] UnityEngine.UI.Text text;

        public System.Reflection.Assembly GameAsset { get; private set; }

        [Header("跳过热更流程")]
        public bool isSkipHotFix = true;

        public IEnumerator Start()
        {
            enableLog = true;

#if !UNITY_EDITOR
            isSkipHotFix = false;
#endif

            if (isSkipHotFix)
            {
                yield return SkipCsharpHotFix();
            }
            else
            {
                // step 1: Get local version file.
                bool localFileCheckPass = GetLocalVersion();

                // step 2: Checking Version file is existsed.
                if (!localFileCheckPass)
                {
                    //if not existsed,
                    //copyed from streaming assset data path to persistentDataPath
                    yield return CopyStreamingAssets();

                    yield return Start();

                    yield break;
                }

                // step 3: Get Urls.
                yield return GetUrls();

                while (!getUrlsDown) { yield return null; }

                // step 4: Get server version file.
                yield return GetServerVersion();

                while (!_gotServerVersion) { yield return null; }

                // step 5: Version Compareing
                if (VersionCompare())
                {
                    //version compare passed, Hot Starting Game.
                    yield return HybridClrStart();
                }
                else
                {
                    ///version compare not passed, Download hot fix dlls.
                    yield return DownloadAndWriteFiles();

                    yield return HybridClrStart();
                }
            }
        }


        private IEnumerator HybridClrStart()
        {
            GetClrFile();

            var request = AssetBundle.LoadFromFileAsync(_hotDllFile.FullName);

            while (!request.isDone) { yield return null; }

            AssetBundle dllAB = request.assetBundle;

            byte[] csBytes = dllAB.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes").bytes;
            //byte[] sfloatBytes = dllAB.LoadAsset<TextAsset>("sfloat.dll.bytes").bytes;
            //byte[] smathBytes = dllAB.LoadAsset<TextAsset>("smath.dll.bytes").bytes;
            //byte[] aiMathBytes = dllAB.LoadAsset<TextAsset>("AIMath.dll.bytes").bytes;
            //byte[] aiConfigBytes = dllAB.LoadAsset<TextAsset>("AIConfig.dll.bytes").bytes;
            //byte[] aiCommonBytes = dllAB.LoadAsset<TextAsset>("AICommon.dll.bytes").bytes;
            //byte[] aiTreeBytes = dllAB.LoadAsset<TextAsset>("AITree.dll.bytes").bytes;
            LogProgress("Load Assembly Bytes...OK");


            // 先加载依赖的，再加载本体
            //你有A, B, C, D四个dll
            //A需要B，D
            //C需要D
            //那么，加载顺序就是D, C, B, A
            //var aiConfigAsset = System.Reflection.Assembly.Load(aiConfigBytes);
            //var sfloatAsset = System.Reflection.Assembly.Load(sfloatBytes);
            //var smathAsset = System.Reflection.Assembly.Load(smathBytes);
            //var aiMathAsset = System.Reflection.Assembly.Load(aiMathBytes);
            //var aiCommonAsset = System.Reflection.Assembly.Load(aiCommonBytes);
            //var aiTreeAsset = System.Reflection.Assembly.Load(aiTreeBytes);

            GameAsset = System.Reflection.Assembly.Load(csBytes);
            LogProgress("Load Assembly Asset...OK");


            var appType = GameAsset.GetType("FTX.Application.Main");
            if (appType == null)
            {
                LogError("[HotLaunch::HotFixStart] appType is null");
                yield break;
            }

            var mainMethod = appType.GetMethod("HotFixStart");
            if (mainMethod == null)
            {
                LogError("[HotLaunch::RunDll] HotFixStart is null");
                yield break;
            }

            mainMethod.Invoke(null, new object[] { _aotDllFile.FullName });
        }

        private IEnumerator SkipCsharpHotFix()
        {
            System.Reflection.Assembly cSharp = null;
            cSharp = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            System.Type appType = cSharp.GetType("FrameWork.Application.Main");

            yield return null;

            var mainMethod = appType.GetMethod("HotFixStart");
            if (mainMethod == null)
            {
                LogError("[HotLaunch::SkipCsharpHotFix] Main is null");
                yield break;
            }

            mainMethod.Invoke(null, new object[] { "" });

        }

        private void OnDisable()
        {
            OnDispose();
        }

        private void OnDispose()
        {
            if (_downLoadList != null)
            {
                _downLoadList.Clear();
                _downLoadList = null;
            }

            //if (text != null)
            //{
            //    text = null;
            //}
        }
    }
}