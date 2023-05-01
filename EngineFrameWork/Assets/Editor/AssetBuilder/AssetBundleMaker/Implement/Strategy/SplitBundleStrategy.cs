using System.IO;
using UnityEngine;
using Core.Interface.IO;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	SplitBundleStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker Split Bundle Strategy - > Auto Split Builded AssetBundle.
*********************************************************************/
namespace Core.AssetBuilder
{
    public class SplitBundleStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.SplitBundle;

        public void Build(IBuildContext context)
        {
            if (context.FirstPkg)
            {
                // Checking file before spliting.
                if (CheckPass())
                {
                    StartSpliting();
                }
                else
                {
                    AssetBundlesMaker._souceAssetDir.CopyTo(AssetBundlesMaker._storageDir.Path);
                    DeletaMateFiles(AssetBundlesMaker._storageDir);
                }
            }
            else
            {
                SplitHotFixAssets();
            }
        }

        #region Spliting assets for build first package.

        #region Begin Check

        private bool CheckPass()
        {
            if (!Directory.Exists(AssetBundlesMaker._releaseDirPath))
            {
                Debug.LogError("There are not contains release floder.Please build asset bundle at first.");
                return false;
            }

            if (FliterTotalPkg(AssetBundlesMaker._releaseDirPath))
            {
                Debug.Log("Don't need split AssetsBundle for first pkg. Building total package.");
                return false;
            }

            if (JudgeHasWarningSplitFile())
            {
                return false;
            }

            return true;
        }

        // 若StreamingAsset整包，则跳过。
        private bool FliterTotalPkg(string releaseDirPath)
        {
            for (int i = 0; i < AssetBundlesMaker._accompanyDirectorys.Length; i++)
            {
                if (AssetBundlesMaker._accompanyDirectorys[i].Equals(releaseDirPath))
                {
                    return true;
                }
            }

            return false;
        }

        // 若有异常的跟包文件资源，提示，并自动处理。
        private bool JudgeHasWarningSplitFile()
        {
            // Key: splitIndex  Value: List<tarIndex>
            string contents = string.Empty;
            Dictionary<int, List<int>> warningDic = new Dictionary<int, List<int>>();
            for (int i = 0; i < AssetBundlesMaker._splitSetting.SplitInfos.Length; i++)
            {
                var info = AssetBundlesMaker._splitSetting.SplitInfos[i];
                for (int j = 0; j < info.AccompanyFiles.Length; j++)
                {
                    string recordDir = string.Empty;
                    bool warning = AssetBundlesMaker._splitSetting.JudgeAccompanyFileContainsToDirectory(i, j, out recordDir);
                    if (warning)
                    {
                        if (!warningDic.ContainsKey(i))
                        {
                            warningDic[i] = new List<int>();
                        }
                        warningDic[i].Add(j);
                        contents += recordDir + "\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(contents))
            {
                AssetBundleBrowser.TipsAlertWindow.ShowAlertWithBtn("警告: 有错误引用文件未处理", contents + "\n" + "点击确定，自动删除", () =>
                {
                    AssetBundlesMaker._splitSetting.RemoveFilesGroupInfo(warningDic);
                });
                return true;
            }

            return false;
        }

        #endregion

        #region Split Pkg

        private void CopyUpdateListFile()
        {
            IFile source = AssetBundlesMaker._storageDir.File("update-list.ini");
            source.CopyTo(AssetBundlesMaker._upLoadCachedDir);
        }

        private void DeletaMateFiles(IDirectory sourceDir)
        {
            IFile[] allFiles = sourceDir.GetFiles(SearchOption.AllDirectories);
            List<IFile> deleteList = new List<IFile>();
            // search all meta files and delete it.
            foreach (IFile file in allFiles)
            {
                if (file.Extension.Contains(".meta"))
                {
                    deleteList.Add(file);
                }
            }
            foreach (IFile file in deleteList)
            {
                file.Delete();
            }
        }

        private void GetAccompanyFilesAndDirectory(out List<string> dirList, out List<string> filesList)
        {
            string fullPath = string.Empty;
            string replaceHeader = "Assets/StreamingAssets/" + AssetBundlesMaker._curPlatformName + Path.AltDirectorySeparatorChar;

            dirList = new List<string>();
            filesList = new List<string>();
            for (int i = 0; i < AssetBundlesMaker._accompanyFiles.Length; i++)
            {
                if (string.IsNullOrEmpty(AssetBundlesMaker._accompanyFiles[i])) { continue; }

                fullPath = AssetBundlesMaker._accompanyFiles[i].Replace(replaceHeader, "");
                filesList.Add(fullPath);
            }

            for (int i = 0; i < AssetBundlesMaker._accompanyDirectorys.Length; i++)
            {
                if (string.IsNullOrEmpty(AssetBundlesMaker._accompanyDirectorys[i])) { continue; }

                fullPath = AssetBundlesMaker._accompanyDirectorys[i].Replace(replaceHeader, "");
                dirList.Add(fullPath);
            }
        }

        private void DeleteSplitFloderAccFiles(List<string> filesList, IFile[] targetFiles, bool deletaCache)
        {
            List<IFile> deleteCachedFileList = new List<IFile>();
            foreach (IFile file in targetFiles)
            {
                var tmpFile = filesList.Find((f) => { return f.Equals(file.Name); });
                bool contidion = deletaCache ? tmpFile != null : string.IsNullOrEmpty(tmpFile);
                if (contidion)
                {
                    deleteCachedFileList.Add(file);
                }
            }
            foreach (IFile file in deleteCachedFileList)
            {
                file.Delete();
            }
            deleteCachedFileList.Clear();
        }

        private void DeleteSplitFloderAccDirectoryes(List<string> dirList, string rootPath)
        {
            List<IDirectory> deleteDirList = new List<IDirectory>();
            foreach (var item in dirList)
            {
                IDirectory tmpDir = AssetBundlesMaker._rootDisk.Directory(rootPath + Path.AltDirectorySeparatorChar + item);

                if (tmpDir.Exists())
                {
                    deleteDirList.Add(tmpDir);
                }
                else
                {
                    Debug.LogErrorFormat($"#### Split Bundle Error[Delete Accompany Floder Error]. Please Check it. Fload not Exits on path {tmpDir.Path}  ####");
                }
            }

            foreach (var item in deleteDirList)
            {
                item.Delete();
            }
            deleteDirList.Clear();
        }

        private void DeleteStreamingFloderUnAccFiles(List<string> filesList)
        {
            IFile[] releaseFiles = AssetBundlesMaker._souceAssetDir.GetFiles(SearchOption.AllDirectories);
            DeleteSplitFloderAccFiles(filesList, releaseFiles, false);
        }

        private void DeleteStreamingFloderUnAccDirectoryes(List<string> dirList)
        {
            IDirectory[] childrenDirs = AssetBundlesMaker._souceAssetDir.ChildrenDirectorys;
            List<IDirectory> deleteList = new List<IDirectory>();
            for (int i = 0; i < childrenDirs.Length; i++)
            {
                var str = dirList.Find((x) =>
                {
                    return childrenDirs[i].Path.Contains(x);
                });

                if (string.IsNullOrEmpty(str))
                {
                    deleteList.Add(childrenDirs[i]);
                }
            }

            if (deleteList.Count > 0)
            {
                foreach (var item in deleteList)
                {
                    //可能父目录已经被删除,判空处理一下
                    if (Directory.Exists(item.Path))
                        item.Delete();
                }
                deleteList.Clear();
            }

        }

        private void DeleteAccompanyAssets(List<string> dirList, List<string> filesList)
        {
            // Delete accompany files on split floder.
            IFile[] cachedFiles = AssetBundlesMaker._splitDir.GetFiles(SearchOption.AllDirectories);
            DeleteSplitFloderAccFiles(filesList, cachedFiles, true);

            // Delete accompany directory on split floder.
            string rootPath = AssetBundlesMaker._splitDir.Path.Replace(AssetBundlesMaker._rootDisk.Root.Path, "");
            DeleteSplitFloderAccDirectoryes(dirList, rootPath);

            //While Jenkins Build, First package => delete accompany directory on release floder.
            if (AssetBundlesMaker._curBuildInfo.JenkinsBuildId > 1)
            {
                // First package => delete accompany files on release floder.
                DeleteStreamingFloderUnAccFiles(filesList);

                // First package => delete directory files on release floder.
                DeleteStreamingFloderUnAccDirectoryes(dirList);
            }
        }

        private void StartSpliting()
        {
            List<string> dirList;
            List<string> filesList;
            GetAccompanyFilesAndDirectory(out dirList, out filesList);

            // copy total floder to cached floder & delete files on storage floder.
            AssetBundlesMaker._souceAssetDir.CopyTo(AssetBundlesMaker._storageDir.Path);
            DeletaMateFiles(AssetBundlesMaker._storageDir);
            // copy total floder to splited floder.
            AssetBundlesMaker._souceAssetDir.CopyTo(AssetBundlesMaker._splitDir.Path);
            // delete meta files on splited floder.
            DeletaMateFiles(AssetBundlesMaker._splitDir);
            // delete accompany pkg assets on splited floder.
            DeleteAccompanyAssets(dirList, filesList);
            // copy update-list.ini to first_package root.
            CopyUpdateListFile();
            Debug.Log("### Split First Package AssetBundle Success ###");
        }

        #endregion

        #endregion

        #region Spliting assets for build hot-fix package.

        private void SplitHotFixAssets()
        {
            // copy total floder to cached floder & delete files on storage floder.
            AssetBundlesMaker._souceAssetDir.CopyTo(AssetBundlesMaker._upLoadCachedDir.Path);
            DeletaMateFiles(AssetBundlesMaker._upLoadCachedDir);
            AssetBundlesMaker._splitDir.Delete();
            AssetBundlesMaker._storageDir.Delete();
            Debug.Log("### Split Hot Fix Assets Success ###");
        }

        #endregion
    }
}
