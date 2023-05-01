using UnityEditor;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	CompleteStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker Complete Strategy - > On Build AssetBundle Complete.
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class CompleteStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Complete;

        public void Build(IBuildContext context)
        {
            if (context.StopBuild) { return; }

            // Copy AssetBundle To PersistenData Path for Pc Sim Test.
            IDirectory copyDir = context.Disk.Directory(context.ReleasePath, PathTypes.Absolute);
            copyDir.CopyTo(context.PersistentDataPath);

            AssetBundleBuildInfo curBuildInfo = AssetBundlesMaker._curBuildInfo;
            string title = "{0} ########## AssetBundle BuildInfo ########## {1}";
#if UNITY_EDITOR
            title = string.Format(title, "<color=#FFFF00>", "</color>");
#else
            title = string.Format(title, "", "");
#endif

            UnityEngine.Debug.Log("### Build AssetBundle Complete ###");
            UnityEngine.Debug.LogFormat($"{title} \n" +
                                        $"Summary： {curBuildInfo.TipsName} \n  " +
                                        $"Chanel： {curBuildInfo.ChanelName} \n  " +
                                        $"BuildTarget： {curBuildInfo.BuildTarget} \n  " +
                                        $"BuildLanguage： {curBuildInfo.buildLanguage}");

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }
}
