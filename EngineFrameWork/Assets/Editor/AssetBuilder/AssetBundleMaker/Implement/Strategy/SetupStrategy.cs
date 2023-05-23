using System;
using FrameWork;
using System.IO;
using UnityEditor;
using UnityEngine;
using Core.Interface.AssetBuilder;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	SetupStrategy.cs
	Author:		DaiLi.Ou
	Descriptions: AssetBundleMaker StartUp Strategy -> File Setting Strategy.
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class SetupStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Setup;

        public void Build(IBuildContext context)
        {
            if (UnityEngine.Application.isPlaying)
            {
                throw new Exception("Please stop playing,Before building asset bundles.");
            }

            EditorUtility.DisplayProgressBar("开始设置", "", 0f);

            var switchPlatform = App.Env.SwitchPlatform;
            context.BuildTarget = PlatformToBuildTarget(switchPlatform);
            context.PlatformName = App.Env.PlatformToName(switchPlatform);
            context.ReleasePath = App.Env.DataPath + App.Env.ReleasePath + Path.AltDirectorySeparatorChar + context.PlatformName;
            context.BuildPath = App.Env.DataPath + App.Env.ResourcesBuildPath;
            context.NoBuildPath = App.Env.DataPath + App.Env.ResourcesNoBuildPath;
            context.PersistentDataPath = App.Env.PersistentDataPath;
            context.FirstPkg = AssetBundlesMaker.BuildFirstPkg;

            context.Disk = AssetBundlesMaker.IO.Disk(context.ReleasePath);

            PrepearedInitBuildInformation();

            EditorUtility.DisplayProgressBar("开始设置", "设置完成", 1f);
        }

        private void PrepearedInitBuildInformation()
        {
            //---------------------Build Application Information----------------------
            AssetBundlesMaker._curPlatformName = App.Env.PlatformToName(App.Env.SwitchPlatform);
            // init spliting info.
            AssetBundlesMaker._splitSetting = AssetBundlesMaker.LoadAssetBundleSplitSetting();
            SplitBundleInfo splitInfo = AssetBundlesMaker._splitSetting.GetSplitInfoByBuildTarget(AssetBundlesMaker._curPlatformName);
            AssetBundlesMaker._accompanyFiles = splitInfo.AccompanyFiles;
            AssetBundlesMaker._accompanyDirectorys = splitInfo.AccompanyDirectorys;

            // init bundle build info.
            AssetBundlesMaker.GetTotalBuildApplicationInfomation(out AssetBundlesMaker._curBuildInfo, out AssetBundlesMaker._totalInfo, out AssetBundlesMaker._applicationBuildInfo);

            // init unity project release directory path.
            AssetBundlesMaker._releaseDirPath = "Assets" + App.Env.ReleasePath + Path.AltDirectorySeparatorChar + AssetBundlesMaker._curPlatformName;
            AssetBundlesMaker._rootPath = UnityEngine.Application.dataPath.Replace("/Assets", "");
            AssetBundlesMaker._cdnDirName = AssetBundlesMaker._curBuildInfo.CDN;
            AssetBundlesMaker._chanelDirName = AssetBundlesMaker._curBuildInfo.ChanelName;

            // init disk.
            AssetBundlesMaker._rootDisk = AssetBundlesMaker.IO.Disk(AssetBundlesMaker._rootPath);
            AssetBundlesMaker._streamingDisk = AssetBundlesMaker.IO.Disk(App.Env.DataPath + App.Env.ReleasePath);
            AssetBundlesMaker._souceAssetDir = AssetBundlesMaker._streamingDisk.Directory(AssetBundlesMaker._curPlatformName);

            // init directory.
            // project directory.
            AssetBundlesMaker._projectRootDir = AssetBundlesMaker._rootDisk.Directory(AssetBundlesMaker._rootFloderName);
            if (!AssetBundlesMaker._projectRootDir.Exists()) { AssetBundlesMaker._projectRootDir.Create(); }

            // build chanel directory.
            string chanelDirPath = AssetBundlesMaker._rootFloderName + Path.AltDirectorySeparatorChar + AssetBundlesMaker._chanelDirName;
            AssetBundlesMaker._chanelDir = AssetBundlesMaker._rootDisk.Directory(chanelDirPath);
            if (!AssetBundlesMaker._chanelDir.Exists()) { AssetBundlesMaker._chanelDir.Create(); }

            // belong cdn directory.
            string cdnDirPath = chanelDirPath + Path.AltDirectorySeparatorChar + AssetBundlesMaker._cdnDirName;
            AssetBundlesMaker._cdnDir = AssetBundlesMaker._rootDisk.Directory(cdnDirPath);
            if (!AssetBundlesMaker._cdnDir.Exists())
            {
                AssetBundlesMaker._cdnDir.Create();
            }

            string ptName = App.Env.PlatformToName(App.Env.SwitchPlatform);
            string platformDirPath = cdnDirPath + Path.AltDirectorySeparatorChar + ptName;
            AssetBundlesMaker._platformDir = AssetBundlesMaker._rootDisk.Directory(platformDirPath);
            if (AssetBundlesMaker.BuildFirstPkg)
            {
                AssetBundlesMaker._platformDir.Delete();
            }
            if (!AssetBundlesMaker._platformDir.Exists())
            {
                AssetBundlesMaker._platformDir.Create();
            }

            var upLoadPth = platformDirPath + Path.AltDirectorySeparatorChar + AssetBundlesMaker._upLoadFloderName;
            AssetBundlesMaker._upLoadDir = AssetBundlesMaker._rootDisk.Directory(upLoadPth);
            if (!AssetBundlesMaker._upLoadDir.Exists())
            {
                AssetBundlesMaker._upLoadDir.Create();
            }

            // set out put floder name.
            string timeTips = UtilityExtension.GetYearMonthDayHour();
            AssetBundlesMaker._outPutFloderName = AssetBundlesMaker.BuildFirstPkg
                ?
                string.Format($"Original_{AssetBundlesMaker._cdnDirName}_{AssetBundlesMaker._chanelDirName}")
                :
                string.Format($"HotFix_{AssetBundlesMaker._cdnDirName}_{AssetBundlesMaker._chanelDirName}_{timeTips}");

            var curBuildPth = upLoadPth + Path.AltDirectorySeparatorChar + AssetBundlesMaker._outPutFloderName;
            AssetBundlesMaker._curBuildDir = AssetBundlesMaker._rootDisk.Directory(curBuildPth);
            if (!AssetBundlesMaker._curBuildDir.Exists())
            {
                AssetBundlesMaker._curBuildDir.Create();
            }
        }

        private BuildTarget PlatformToBuildTarget(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.LinuxPlayer:
                    return BuildTarget.StandaloneLinux64;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return BuildTarget.StandaloneWindows64;
                case RuntimePlatform.Android:
                    return BuildTarget.Android;
                case RuntimePlatform.IPhonePlayer:
                    return BuildTarget.iOS;
                default:
                    throw new ArgumentException("Undefined Platform");
            }
        }
    }
}