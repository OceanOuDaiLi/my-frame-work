using UnityEditor;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	ClearStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker Clear Strategy -> Clear Cache Files & AssetBundle Flags.
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class ClearStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Clear;

        public void Build(IBuildContext context)
        {
            if (context.ClearOldAssetBundleFlag)
            {
                ClearAssetBundleFlag();
            }
            ClearReleaseDir(context);

            AssetDatabase.Refresh();
        }

        private void ClearReleaseDir(IBuildContext context)
        {
            EditorUtility.DisplayProgressBar("Delete Old AssetBundle", "", 0f);
            var releaseDir = context.Disk.Directory(context.ReleasePath, PathTypes.Absolute);
            releaseDir.Delete();
            releaseDir.Create();
            EditorUtility.DisplayProgressBar("Delete AssetBundle", "Complete", 1f);
            EditorUtility.ClearProgressBar();
        }

        private void ClearAssetBundleFlag()
        {
            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            int count = assetBundleNames.Length;
            for (int i = 0; i < count; i++)
            {
                AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);
                EditorUtility.DisplayProgressBar("Clear AssetBundle Flags", "Clear Asset's ：" + assetBundleNames[i], 1f * i / count);
            }
        }
    }
}