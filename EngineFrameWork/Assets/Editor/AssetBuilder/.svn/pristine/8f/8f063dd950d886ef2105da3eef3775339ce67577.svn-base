using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;
using System.Collections.Generic;

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
            //if (context.StopBuild) { return; }

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

        /// <summary>
        /// 打开指定路径的文件夹。
        /// </summary>
        /// <param name="folder">要打开的文件夹的路径。</param>
        public static void OpenBuildFloder(string folder)
        {
            folder = string.Format("\"{0}\"", folder);
            switch (UnityEngine.Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    System.Diagnostics.Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                    break;

                case RuntimePlatform.OSXEditor:
                    System.Diagnostics.Process.Start("open", folder);
                    break;

                default:
                    throw new Exception(string.Format("Not support open folder on '{0}' platform.",
                        UnityEngine.Application.platform.ToString()));
            }
        }
    }
}
