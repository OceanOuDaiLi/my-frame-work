using System.IO;
using UnityEditor;
using UnityEngine;
using FrameWork.Launch;
using Core.Interface.IO;
using HybridCLR.Editor.Commands;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	BuildHCLRCommand.cs
	Author:		DaiLi.Ou

	Descriptions: Build HybridCLR Commands.
*********************************************************************/
namespace HybridCLR.Editor
{
    public class BuildHCLRCommand
    {
        private static IDisk HybridDisk
        {
            get => IOHelper.IO.Disk(Application.dataPath + "/ABAssets/AssetBundle");
        }

        private static IDirectory AotSourceDataDir
        {
            get => HybridDisk.Directory("aotdlls");
        }

        private static IDirectory HotFixSourceDataDir
        {
            get => HybridDisk.Directory("hotupdatedlls");
        }

        private static void CreateDirectory()
        {
            if (AotSourceDataDir.Exists())
            {
                AotSourceDataDir.Delete();
            }
            AotSourceDataDir.Create();

            if (HotFixSourceDataDir.Exists())
            {
                HotFixSourceDataDir.Delete();
            }
            HotFixSourceDataDir.Create();
        }

        public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib",
            "System",
            "System.Core",
            //"SimpleDB",
            //"Animancer",
            //"MagicaCloth",
            //"AIAOT",
            //....
            //....
            //AOT相关DLL的构建
        };

        public static string AotPkgFloderNameWithTime
        {
            get
            {
                string floderName = System.DateTime.Now.Year.ToString()
                   + System.DateTime.Now.Month.ToString()
                   + System.DateTime.Now.Day.ToString();

                return floderName;
            }
        }

        /// <summary>
        /// Gen Supplement AOT MetaDatas' Dlls
        /// </summary>
        public static void BuildHybridByTarget()
        {
            CreateDirectory();

            string streamingPath = Application.streamingAssetsPath;
            if (Directory.Exists(streamingPath))
            {
                Directory.Delete(streamingPath, true);
            }
            Directory.CreateDirectory(streamingPath);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            PrebuildCommand.GenerateAll();
            CompileDllCommand.CompileDllActiveBuildTarget();

            // Get filename.
            string outputPath = $"{SettingsUtil.ProjectDir}/AOT-Release/Android/" + AotPkgFloderNameWithTime;
            var buildOptions = BuildOptions.CompressWithLz4;
            BuildPlayerOptions buildPlayerOptions;
            if (EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.Android))
            {
                string location = outputPath + "/AotBuild.apk";
                PlayerSettings.Android.keystorePass = "123456";
                PlayerSettings.Android.keyaliasPass = "123456";
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                buildPlayerOptions = new BuildPlayerOptions()
                {
                    scenes = new string[] { "Assets/Scenes/Launch.unity" },
                    locationPathName = location,
                    options = buildOptions,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                };

                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
            else if (EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.iOS))
            {
                outputPath = $"{SettingsUtil.ProjectDir}/AOT-Release/IOS/" + AotPkgFloderNameWithTime;
                buildPlayerOptions = new BuildPlayerOptions()
                {
                    scenes = new string[] { "Assets/Scenes/Launch.unity" },
                    locationPathName = outputPath,
                    options = buildOptions,
                    target = BuildTarget.iOS,
                    targetGroup = BuildTargetGroup.iOS,
                };

                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
            else
            {
                Debug.LogError("UnKnow build target");
            }

            Debug.Log("########## Aot Build Success ##########");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        /// <summary>
        /// Hot Package Build CLR Informations.
        /// </summary>
        /// <param name="target">build target</param>
        /// <param name="firstPkg">is first build pkg</param>
        public static void BuildHybridCLRHotRes(BuildTarget target)
        {
            Debug.Log("Hybrid CLR Build ====> HotFix dlls");
            CompileDllCommand.CompileDllActiveBuildTarget();
            PrebuildCommand.GenerateAll();

            // step 1: 文件夹创建
            CreateDirectory();

            // step 2: hotfix.dll拷贝
            CopyHotFixDlls(HotFixSourceDataDir.Path, target);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            // setp 3: aot.dll拷贝
            CopyAotDlls(AotSourceDataDir.Path, target);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static List<string> CopyHotFixDlls(string destDir, BuildTarget target)
        {
            var hotUpdateDllAssets = new List<string>();
            string hotfixDllDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            foreach (var fileName in SettingsUtil.HotUpdateAssemblyNamesExcludePreserved)
            {
                string surDllPath = $"{hotfixDllDir}/{fileName}.dll";
                string destDllPath = $"{destDir}/{fileName}.bytes";
                if (!File.Exists(surDllPath))
                {
                    Debug.LogError($"ab中添加HotFix DLL:{surDllPath} 时发生错误,文件不存在。");
                    continue;
                }

                //crypt sour dll file.
                //byte[] cryptByte = File.ReadAllBytes(surDllPath);
                //cryptByte = GameMain.Utils.XXTEA.Encrypt(cryptByte, "password_hotfix_dll");
                //File.WriteAllBytes(destDllPath, cryptByte);

                //uncrypt sour dll file.
                File.Copy(surDllPath, destDllPath, true);

                hotUpdateDllAssets.Add(destDllPath);
                Debug.Log($"[BuildHybridCLRHotRes] copy hotfix dll {surDllPath} -> {destDllPath}");
            }

            return hotUpdateDllAssets;
        }

        private static List<string> CopyAotDlls(string destDir, BuildTarget target)
        {
            var aotDllAssets = new List<string>();
            string aotDllDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);

            foreach (var fileName in AOTMetaAssemblyNames)
            {
                string surDllPath = $"{aotDllDir}/{fileName}.dll";
                string destDllPath = $"{destDir}/{fileName}.bytes";
                if (!File.Exists(surDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{surDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }

                //crypt sour dll file.
                //byte[] cryptByte = File.ReadAllBytes(surDllPath);
                //cryptByte = GameMain.Utils.XXTEA.Encrypt(cryptByte, "password_aot_dll");
                //File.WriteAllBytes(destDllPath, cryptByte);

                //uncrypt sour dll file.
                File.Copy(surDllPath, destDllPath, true);

                aotDllAssets.Add(destDllPath);
            }

            return aotDllAssets;
        }

    }
}
