﻿using System;
using FrameWork;
using System.IO;
using UnityEditor;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	PrecompiledStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker Precompiled Strategy -> Scane Files & Building AssetBundle Flags .
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class PrecompiledStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Precompiled;

        public void Build(IBuildContext context)
        {
            BuildAssetBundleName(context);
            EditorUtility.ClearProgressBar();
        }

        private void BuildAssetBundleName(IBuildContext context)
        {
            var directory = context.Disk.Directory(context.BuildPath, PathTypes.Absolute);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);

            int count = files.Length;
            for (var i = 0; i < count; i++)
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    EditorUtility.DisplayProgressBar("Build AssetBundle Flag", "File Path：" + files[i].FullName, 1f * (i + 1) / count);
                    BuildFileBundleName(files[i], context.BuildPath);
                }
            }
        }

        private void BuildFileBundleName(IFile file, string basePath)
        {
            string extension = file.Extension;
            string fullName = AssetBundlesMaker.StandardPath(file.FullName);
            string fileName = file.Name;
            string baseFileName = fileName.Substring(0, fileName.Length - extension.Length);
            string assetName = fullName.Substring(basePath.Length);
            assetName = assetName.Substring(0, assetName.Length - fileName.Length).TrimEnd(Path.AltDirectorySeparatorChar);

            if (baseFileName + extension == ".DS_Store")
            {
                return;
            }

            int variantIndex = baseFileName.LastIndexOf(".", StringComparison.Ordinal);
            string variantName = string.Empty;

            //while Aot dlls named like: a.b.c.xxx,Fliteing assetbundle variant.
            bool dllFilter = fileName.Equals("System.Core.bytes");
            if (variantIndex > 0 && !dllFilter)
            {
                variantName = baseFileName.Substring(variantIndex + 1);
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath("Assets" + App.Env.ResourcesBuildPath + assetName + Path.AltDirectorySeparatorChar + baseFileName + extension);
            if (assetImporter == null || assetImporter.assetBundleName == null)
            {
                UnityEngine.Debug.LogError("Asset not exist:" + assetName + Path.AltDirectorySeparatorChar + baseFileName + extension);
                return;
            }

            assetImporter.assetBundleName = assetName.TrimStart(Path.AltDirectorySeparatorChar);

            if (!string.IsNullOrEmpty(variantName))
            {
                assetImporter.assetBundleVariant = variantName;
            }

            if (!string.IsNullOrEmpty(assetImporter.assetBundleVariant) && dllFilter)
            {
                assetImporter.assetBundleVariant = string.Empty;
            }
        }
    }
}