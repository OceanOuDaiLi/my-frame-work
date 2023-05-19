using System;
using FrameWork;
using System.IO;
using UnityEngine;
using System.Text;
using FrameWork.Launch;
using Core.Interface.IO;
using Core.Interface.INI;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	    ZipBundleStrategy.cs
Author:		    DaiLi.Ou
Descriptions:   AssetBundleMaker Split Bundle Strategy - > Zip Splited AssetBundle.
Tips:           首包才做分包的Zip。
                热更包为了做跨版本下载支持，不做Zip.
*********************************************************************/
namespace Core.AssetBuilder
{
    public class ZipBundleStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.ZipBundle;

        // dont zip asset. command bu daili.ou 2023/05/04
        public void Build(IBuildContext context)
        {
            //if (context.FirstPkg)
            //{
            //    FirstPkgZip();
            //}
            //else
            //{
            //    HotPkgZip(context);
            //}

            //// copy to upload floder.
            //if (!context.StopBuild)
            //    AssetBundlesMaker._upLoadCachedDir.CopyTo(AssetBundlesMaker._curBuildDir.Path);
        }

        //        #region First Pkg Zip

        //        private void InitZipFirstPkg(out string srcFullDir, out List<string> fileList)
        //        {
        //            srcFullDir = AssetBundlesMaker.StandardPath(AssetBundlesMaker._splitDir.Path + Path.AltDirectorySeparatorChar);

        //            if (!srcFullDir.EndsWith("/"))
        //            {
        //                srcFullDir += "/";
        //            }

        //            fileList = new List<string>();
        //            if (fileList == null || fileList.Count == 0)
        //            {
        //                var files = Directory.GetFiles(srcFullDir, "*", SearchOption.AllDirectories);
        //                fileList = new List<string>(files.Length);
        //                foreach (var file in files)
        //                {
        //                    if (file.EndsWith(".meta"))
        //                    {
        //                        continue;
        //                    }

        //                    var tmp = file.Replace('\\', '/').Replace(srcFullDir, "");
        //                    fileList.Add(tmp);
        //                }
        //            }
        //        }

        //        private void FirstPkgZip()
        //        {
        //            string srcFullDir;
        //            List<string> fileList;
        //            InitZipFirstPkg(out srcFullDir, out fileList);
        //            if (fileList.Count > 0)
        //            {
        //                var dstFile = AssetBundlesMaker._upLoadCachedDir.Path + Path.AltDirectorySeparatorChar + AssetBundlesMaker._zipName;
        //                CompressSplitBundle(srcFullDir, fileList, dstFile);
        //            }
        //            else
        //            {
        //                Debug.Log("### Don't contains any assets for First-Package Zip.  ###");
        //                AssetBundlesMaker._splitDir.Delete();
        //            }

        //            Debug.Log("### Zip First Package Successed ###");
        //        }

        //        #endregion

        //        #region Hot Pkg Zip

        //        private void InitZipHotPkg(out UpdateFile needUpdateLst)
        //        {
        //            IFile oldVersionFile = AssetBundlesMaker._platformDir.File(AssetBundlesMaker._verFileName);
        //            IIniResult result = App.Ini.Load(oldVersionFile);
        //            Dictionary<string, string> dic = result.Get("Version");
        //            string pastUpdateFile = dic["UpdateFile"];

        //            IFile newUpdateFile = AssetBundlesMaker._upLoadCachedDir.File(UpdateFileStore.FILE_NAME);
        //            IFile oldUpdateFile = AssetBundlesMaker._upLoadDir.File(pastUpdateFile);

        //            UpdateFile newList = new UpdateFile(Encoding.Default.GetString(newUpdateFile.Read()));
        //            if (oldUpdateFile.Exists)
        //            {
        //                UpdateFile oldList = new UpdateFile(Encoding.Default.GetString(oldUpdateFile.Read()));

        //                UpdateFile needDeleteLst;
        //                oldList.Comparison(newList, out needUpdateLst, out needDeleteLst);
        //            }
        //            else
        //            {
        //                needUpdateLst = newList;
        //            }
        //        }

        //        private void HotPkgZip(IBuildContext context)
        //        {
        //            UpdateFile needUpdateLst;
        //            InitZipHotPkg(out needUpdateLst);

        //            //Tips: C端下载Zip时,可根据needDeleteLst删除本地资源, 或写入更新资源时覆盖写入(推荐)
        //            UpdateFileField[] needUpdateFields = needUpdateLst.Fields;
        //            if (needUpdateFields.Length < 1)
        //            {
        //                Debug.LogError("## FBIWARNING ## There are not contains any hot update files,Willing to delete this hot package building and stopping build pipeline...");
        //                //todo: 删除本次热更构建，并终止。
        //                AssetBundlesMaker._upLoadCachedDir.Delete();
        //                AssetBundlesMaker._outPutCacheDir.Delete();
        //                Debug.LogError("## FBIWARNING ## Build Hot Update Failed...");
        //                context.StopBuild = true;
        //                return;
        //            }

        //            string content = string.Empty;
        //            List<string> fileList = new List<string>();
        //            for (int i = 0; i < needUpdateFields.Length; i++)
        //            {
        //                string fieldName = needUpdateFields[i].Path;
        //                content += fieldName + "\n";
        //                fileList.Add(Path.AltDirectorySeparatorChar + fieldName);
        //            }

        //            var dstFile = AssetBundlesMaker._curBuildDir.Path + Path.AltDirectorySeparatorChar + AssetBundlesMaker._zipName;
        //            CompressSplitBundle(AssetBundlesMaker._upLoadCachedDir.Path, fileList, dstFile);

        //            string title = "{0} ########## Hot Update Files Detail ########## {1}";
        //#if UNITY_EDITOR
        //            title = string.Format(title, "<color=#FFFF00>", "</color> \n");
        //#else
        //            title = string.Format(title, "", "");
        //#endif

        //            Debug.Log("### Zip Hot Update Successed ###");
        //            UnityEngine.Debug.LogFormat($"{title}" + content);
        //        }

        //        #endregion

        //        /// <summary>
        //        /// # 7Zip LZMA # Compress
        //        /// </summary>
        //        /// <param name="srcFullDir"></param>
        //        /// <param name="fileList"></param>
        //        private void CompressSplitBundle(string srcFullDir, List<string> fileList, string destFile)
        //        {
        //            switch (AssetBundlesMaker._curBuildInfo.assetZipTool)
        //            {
        //                //case AssetZipTool.SHARP_ZIP_LIB:
        //                //    ShzrpZipBundle(srcFullDir, fileList, destFile);
        //                //    break;
        //                case AssetZipTool.SEVEN_ZIP:
        //                    SevenZipBundle(srcFullDir, fileList, destFile);
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }

        //        private void ShzrpZipBundle(string srcFullDir, List<string> fileList, string destFile)
        //        {

        //        }

        //        private void SevenZipBundle(string srcFullDir, List<string> fileList, string destFile)
        //        {
        //            var zipMb = AssetBundlesMaker._applicationBuildInfo.subZipSize;
        //            // 解析：
        //            // 多个zip，进行下载 & 解压。
        //            // n条线程，进行n个zip的下载。zip粒度越小，网络带宽消耗越低(续传导致的重复请求)。
        //            // 每个线程，记录自己的下载/解压进度。
        //            // 可支持【Socket + Http】 / 【Http】下载。
        //            // 可根据机型，自定义下载/解压的线程数。

        //            try
        //            {
        //                using var ms = new MemoryStream();
        //                using var bw = new BinaryWriter(ms);
        //                bw.Write(fileList.Count);
        //                foreach (var file in fileList)
        //                {
        //                    var filePath = srcFullDir + file;

        //                    if (!File.Exists(filePath))
        //                    {
        //                        Debug.LogError($"Zip Error : 文件 {filePath} 不存在!");
        //                        continue;
        //                    }

        //                    bw.Write(file);
        //                    var bytes = File.ReadAllBytes(filePath);
        //                    bw.Write(bytes.Length);
        //                    bw.Write(bytes);
        //                }

        //                using var output = new MemoryStream((int)ms.Length);

        //                if (!ZipHelper.Compress(ms, output))
        //                {
        //                    Debug.LogError($"Zip Error : 压缩失败 ？？？ ");
        //                    return;
        //                }

        //                var outBuffer = output.ToArray();
        //                PathHelper.EnsureExistFileDirectory(destFile);
        //                File.WriteAllBytes(destFile, outBuffer);
        //            }
        //            catch (Exception e)
        //            {
        //                throw e;
        //            }
        //        }

    }
}

