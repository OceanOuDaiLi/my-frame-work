using System;
using FrameWork;
using System.IO;
using UnityEditor;
using UnityEngine;
using Core.AutoUpdate;
using HybridCLR.Editor;
using Core.Interface.IO;
using System.Reflection;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	AssetBundlesMaker.cs
	Author:		DaiLi.Ou
	Descriptions: Integrated packaging unity's Assetbundle & Application
*********************************************************************/
namespace Core.AssetBuilder
{
    public static class AssetBundlesMaker
    {
        #region Public Variables
        public static bool BuildFirstPkg = false;

        //------------------------------------------------------------------------
        //---------------------Build Application Information----------------------
        //------------------------------------------------------------------------
        public static string _releaseDirPath;
        public static string _curPlatformName;
        public static string[] _accompanyFiles;
        public static string[] _accompanyDirectorys;
        public static AssetBundleBuildSetting _totalInfo;
        public static AssetBundleBuildInfo _curBuildInfo;
        public static AssetBundleSplitSetting _splitSetting;
        public static ApplicationBuildInfo _applicationBuildInfo;

        public static string _rootPath = string.Empty;
        public static string _cdnDirName = string.Empty;
        public static string _chanelDirName = string.Empty;
        public static string _outPutFloderName = string.Empty;
        public static string _verFileName = "Version.ini";
        public static string _upLoadFloderName = "Res_UpLoad";
        public static string _rootFloderName = "Package_Release";

        public static IDisk _rootDisk;
        public static IDisk _streamingDisk;

        public static IDirectory _cdnDir;
        public static IDirectory _upLoadDir;
        public static IDirectory _chanelDir;
        public static IDirectory _curBuildDir;
        public static IDirectory _platformDir;
        public static IDirectory _souceAssetDir;
        public static IDirectory _projectRootDir;

        private static Core.IO.IO _io;
        public static Core.IO.IO IO
        {
            get
            {
                return _io ?? (_io = new Core.IO.IO());
            }
        }
        //------------------------------------------------------------------------
        //------------------------------------------------------------------------
        //------------------------------------------------------------------------
        #endregion

        #region Build Flag Methodes

        public static void BuildAssetBundlesFlag()
        {
            Type[] filter = new Type[]
            {
                typeof(BuildStrategy),
                typeof(ScanningStrategy),
                typeof(EncryptionStrategy),
                typeof(GenTableStrategy),
                typeof(AutoUpdateGenPathStrategy),
                typeof(SplitBundleStrategy),
                typeof(ZipBundleStrategy),
                typeof(GenVersionStrategy),
                typeof(CompleteStrategy)
            };
            ExcuteBuildStrategy(filter, false);
        }

        public static void ClearAssetBundlesFlag()
        {
            Type[] filter = new Type[]
            {
                typeof(PrecompiledStrategy),
                typeof(BuildStrategy),
                typeof(ScanningStrategy),
                typeof(EncryptionStrategy),
                typeof(GenTableStrategy),
                typeof(AutoUpdateGenPathStrategy),
                typeof(SplitBundleStrategy),
                typeof(ZipBundleStrategy),
                typeof(GenVersionStrategy),
                typeof(CompleteStrategy) };
            ExcuteBuildStrategy(filter);
        }

        public static void BuildAssetBundlesFlagWithoutClearOldFlag()
        {
            Type[] filter = new Type[] {
                typeof(BuildStrategy),
                typeof(ScanningStrategy),
                typeof(EncryptionStrategy),
                typeof(GenTableStrategy),
                typeof(AutoUpdateGenPathStrategy),
                typeof(SplitBundleStrategy),
                typeof(ZipBundleStrategy),
                typeof(GenVersionStrategy),
                typeof(CompleteStrategy)
            };
            ExcuteBuildStrategy(filter, false);
        }

        #endregion

        #region Start Build Strategy

        private static void BuildAssetBundle()
        {
            BuildAssetBundlesFlagWithoutClearOldFlag();

            BuildNotAssetBundles();

            ExcuteBuildStrategy(null, false);
        }

        private static void BuildNotAssetBundles()
        {
            Type[] filter = new Type[]
            {
                typeof(PrecompiledStrategy),
                typeof(BuildStrategy),
                typeof(ScanningStrategy),
                typeof(EncryptionStrategy),
                typeof(SplitBundleStrategy),
                typeof(ZipBundleStrategy),
                typeof(GenVersionStrategy),
                typeof(CompleteStrategy)
            };
            ExcuteBuildStrategy(filter, false);
        }

        [MenuItem("打包工具/Editor快速构建AOT", priority = 220)]
        /// <summary>
        ///  Gen Supplementary AOT Metadata 
        /// </summary>
        public static void BuildAot()
        {
            int csProjectNum = GetCsProjectNum();
            PlayerPrefs.SetInt("CSProject_Count", csProjectNum);
            HybridCLR.Editor.BuildHCLRCommand.BuildHybridByTarget();
        }

        /// <summary>
        /// Build Apk/Ipa
        /// </summary>
        public static void BuildApplication()
        {
            string outputPath = AssetBundlesMaker._curBuildDir.Path;
            var buildOptions = BuildOptions.CompressWithLz4;
            BuildPlayerOptions buildPlayerOptions;

            PlayerSettings.companyName = _applicationBuildInfo.CompanyName;
            PlayerSettings.productName = _applicationBuildInfo.ProductName;
            // todo. add more playersettings info.

            if (EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.Android))
            {
                string location = outputPath + Path.AltDirectorySeparatorChar + string.Format($"Original_{_cdnDirName}_{_chanelDirName}.apk");

                //PlayerSettings
                PlayerSettings.Android.keystorePass = "123456";
                PlayerSettings.Android.keyaliasPass = "123456";

                buildPlayerOptions = new BuildPlayerOptions()
                {
                    scenes = new string[]
                    {
                        "Assets/Scenes/Launch.unity" ,
                        "Assets/Scenes/LogIn.unity" ,
                        "Assets/Scenes/Hall.unity" ,
                        "Assets/Scenes/Game.unity" ,
                    },
                    locationPathName = location,
                    options = buildOptions,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                };

                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
            else if (EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.iOS))
            {
                outputPath += +Path.AltDirectorySeparatorChar + string.Format($"Original_XCode_{_cdnDirName}_{_chanelDirName}");
                Directory.CreateDirectory(outputPath);
                buildPlayerOptions = new BuildPlayerOptions()
                {
                    scenes = new string[]
                    {
                        "Assets/Scenes/Launch.unity" ,
                        "Assets/Scenes/LogIn.unity" ,
                        "Assets/Scenes/Hall.unity" ,
                        "Assets/Scenes/Game.unity" ,
                    },
                    locationPathName = outputPath,
                    options = buildOptions,
                    target = BuildTarget.iOS,
                    targetGroup = BuildTargetGroup.iOS,
                };

                BuildPipeline.BuildPlayer(buildPlayerOptions);

                // todo: add to jenkins. Cmake bash IL2CPP for hybridclr.
            }
            else
            {
                Debug.LogError("UnKnow build target");
            }
            Debug.Log("### Application Build Completed ###");

            CompleteStrategy.OpenBuildFloder(_upLoadDir.Path);
        }

        [MenuItem("打包工具/Editor快速构建pkg", priority = 320)]
        /// <summary>
        /// Building First Package By Platform.
        /// </summary>
        public static void BuildFirstPackage()
        {
            BuildFirstPkg = true;

            if (JudgeCsProjectChanged())
            {
                return;
            }

            ClearAssetBundlesFlag();

            BuildHotFixPackapge();

            BuildApplication();

            //end:
            BuildFirstPkg = false;
        }

        [MenuItem("打包工具/Editor快速构建热更", priority = 420)]
        /// <summary>
        /// Building Application hot-fix.
        /// </summary>
        public static void BuildHotFixPackapge()
        {
            BuildUIAltas();

            if (JudgeCsProjectChanged())
            {
                Debug.LogError("Serious Error: CSProject Changed -> Can't Build Code HotFixed. Please Checked it.");
                return;
            }

            BuildHCLRCommand.BuildHybridCLRHotRes(EditorUserBuildSettings.activeBuildTarget);

            BuildAssetBundle();

            if (!BuildFirstPkg)
                CompleteStrategy.OpenBuildFloder(_upLoadDir.Path);
        }

        //[MenuItem("test/setup")]
        //public static void TestSetUp()
        //{
        //    Type[] filter = new Type[] {
        //        typeof(ClearStrategy),
        //        typeof(BuildStrategy),
        //        typeof(ScanningStrategy),
        //        typeof(EncryptionStrategy),
        //        typeof(GenTableStrategy),
        //        typeof(AutoUpdateGenPathStrategy),
        //        typeof(SplitBundleStrategy),
        //        typeof(ZipBundleStrategy),
        //        typeof(GenVersionStrategy),
        //        typeof(CompleteStrategy)
        //    };
        //    ExcuteBuildStrategy(filter, false);
        //}

        #endregion

        #region Excute Build Strategy

        static Type[] FindTypesWithInterface(Type interfaceType)
        {
            var lstType = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    Type[] interfaces = type.GetInterfaces();
                    foreach (var t in interfaces)
                    {
                        if (t == interfaceType)
                        {
                            lstType.Add(type);
                        }
                    }
                }
            }
            return lstType.ToArray();
        }

        static void ExcuteBuildStrategy(Type[] filterStrategy, bool clearOldABFlag = true)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var strategys = new List<IBuildStrategy>();
            foreach (var t in FindTypesWithInterface(typeof(IBuildStrategy)))
            {
                if (filterStrategy != null)
                {
                    bool filter = false;
                    foreach (Type ft in filterStrategy)
                    {
                        if (t.Equals(ft))
                        {
                            filter = true;
                            break;
                        }
                    }
                    if (filter) continue;
                }

                strategys.Add(assembly.CreateInstance(t.FullName) as IBuildStrategy);
            }

            strategys.Sort((left, right) => ((int)left.Process).CompareTo((int)right.Process));

            var context = new BuildContext();
            context.IsAssetCrypt = false;//_applicationBuildInfo == null ? false : _applicationBuildInfo.EncryptAsset;
            context.IsCodeCrypt = false;//_applicationBuildInfo == null ? false : _applicationBuildInfo.EncryptCode;
            context.IsConfigCrypt = false;//_applicationBuildInfo == null ? false : _applicationBuildInfo.EncryptCfgAsset;
            context.ClearOldAssetBundleFlag = clearOldABFlag;

            foreach (var buildStrategy in strategys.ToArray())
            {
                buildStrategy.Build(context);
            }
        }

        #endregion

        #region Build_Utils_Command
        public static void BuildUIAltas()
        {
            BuildUIAltasCommand.StartBuildAtlas();
        }

        public static int GetCsProjectNum()
        {
            string projectRootPath = UnityEngine.Application.dataPath.Replace("/Assets", "");
            int idx = projectRootPath.LastIndexOf("/");
            string projectNmae = projectRootPath.Substring(idx + 1);
            projectRootPath = projectRootPath.Remove(idx);

            IDisk projectRootDisk = AssetBundlesMaker.IO.Disk(projectRootPath);
            IDirectory projectDir = projectRootDisk.Directory(projectNmae);

            IFile[] files = projectDir.GetFiles();
            int csProjectNum = 0;
            foreach (var item in files)
            {
                if (item.Extension.Equals(".csproj"))
                {
                    csProjectNum++;
                }
            }

            return csProjectNum;
        }

        public static bool JudgeCsProjectChanged()
        {
            // todo. 将Aot构建 接入首包构建流程
            int csProjectNum = GetCsProjectNum();
            int pastCsProjectNum = PlayerPrefs.GetInt("CSProject_Count");

            if (pastCsProjectNum != csProjectNum)
            {
                Debug.Log("########## CSProject Changed. Please build aot at first. ##########");
                return true;
            }

            return false;
        }
        #endregion

        public static int GetDefaultBuildPlatForm()
        {
            string ptName = App.Env.PlatformToName(App.Env.SwitchPlatform);
            int info;
            switch (ptName)
            {
                case "Android":
                    info = 0;
                    break;
                case "IOS":
                    info = 1;
                    break;
                default:
                    info = 0;
                    break;
            }
            return info;
        }

        public static string StandardPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string GetCommandLineArg(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name)
                {
                    if (args.Length > i + 1)
                    {
                        return args[i + 1];
                    }
                }
            }

            return "";
        }

        public static AssetBundleBuildSetting LoadAssetBundleSetting()
        {
            string path = "Editor/AssetBuilder/AppBuildResources/AssetBuildSettings.asset";
            UnityEngine.Object _asset = AssetDatabase.LoadAssetAtPath(App.Env.AssetPath.Substring(App.Env.AssetPath.IndexOf("Assets"))
                + Path.AltDirectorySeparatorChar
                + path, typeof(UnityEngine.Object));

            return (AssetBundleBuildSetting)_asset;
        }

        public static AssetBundleSplitSetting LoadAssetBundleSplitSetting()
        {
            string path = "Editor/AssetBuilder/AppBuildResources/AssetSplitSettings.asset";
            UnityEngine.Object _asset = AssetDatabase.LoadAssetAtPath(App.Env.AssetPath.Substring(App.Env.AssetPath.IndexOf("Assets"))
                + Path.AltDirectorySeparatorChar
                + path, typeof(UnityEngine.Object));

            return (AssetBundleSplitSetting)_asset;
        }

        public static void GetTotalBuildApplicationInfomation(out AssetBundleBuildInfo curBuildInfo, out AssetBundleBuildSetting totalInfo, out ApplicationBuildInfo applicationBuildInfo)
        {
            // init bundle build info.
            totalInfo = AssetBundlesMaker.LoadAssetBundleSetting();
            applicationBuildInfo = totalInfo.applicationBuildInfo;
            // Default Build
            int defaultIdx = AssetBundlesMaker.GetDefaultBuildPlatForm();
            curBuildInfo = totalInfo.AssetBundleBuildInfos[defaultIdx];
            // Jenkins Build
            string chanelName = AssetBundlesMaker.GetCommandLineArg("JenkinsBuildId");
            if (!string.IsNullOrEmpty(chanelName))
            {
                int chanelId;
                int.TryParse(chanelName, out chanelId);

                // Get Build Info
                for (int i = 0; i < totalInfo.AssetBundleBuildInfos.Length; i++)
                {
                    var info = totalInfo.AssetBundleBuildInfos[i];
                    if (info.JenkinsBuildId.Equals(chanelId))
                    {
                        curBuildInfo = info;
                        break;
                    }
                }
            }
        }
    }
}
