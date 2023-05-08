using System;
using System.IO;
using FrameWork;
using System.Text;
using UnityEngine;
using Core.Interface;
using Core.Interface.IO;
using System.Collections;
using UnityEngine.Networking;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	AutoUpdate.cs
	Author:		DaiLi.Ou

	Descriptions: Resources Auto Update.
*********************************************************************/
namespace Core.AutoUpdate
{
    public sealed class AutoUpdate
    {
        /// <summary>
        /// 磁盘
        /// </summary>
        private IDisk disk;

        /// <summary>
        /// 磁盘
        /// </summary>
        private IDisk Disk
        {
            get
            {
                return disk ?? (disk = App.AssetDisk);
            }
        }

        /// <summary>
        /// 是否开始更新
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// 获取更新目录的api地址
        /// </summary>
        private string updateAPI;

        /// <summary>
        /// 更新的请求url
        /// </summary>
        private string updateURL;

        /// <summary>
        /// 是否自动下载
        /// </summary>
        private bool autoDownload = false;

        /// <summary>
        /// 设定一个api请求地址用于获取更新目录
        /// </summary>
        /// <param name="api">请求api</param>
        public void SetUpdateAPI(string api)
        {
            if (!string.IsNullOrEmpty(api))
            {
                updateAPI = api;
            }
        }

        /// <summary>
        /// 设定更新的请求url
        /// </summary>
        /// <param name="url">更新的url</param>
        public void SetUpdateURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                updateURL = url;
            }
        }

        public void StartDownload()
        {
            autoDownload = true;
        }

        public void StopDownload()
        {
            App.Instance.StopCoroutine(StartUpdate());
        }

        public string GetStreamingAssetsPath()
        {
            if (UnityEngine.Application.isMobilePlatform || UnityEngine.Application.isConsolePlatform)
            {
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
                {
                    return "file://" + UnityEngine.Application.streamingAssetsPath;
                }
                else if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    return UnityEngine.Application.streamingAssetsPath;
                }
                else
                {
                    return UnityEngine.Application.streamingAssetsPath;
                }
            }
            else
            {
                return "file:///" + UnityEngine.Application.streamingAssetsPath;
            }
        }

        public IEnumerator CopyStreamingAssetsCoroutine(Action<float> prog, IFile versionFile, bool force = false)
        {
            IDisk floderdDisk = null;
            var curPlatform = App.Env.Platform;
            string streamingAssetsPath = GetStreamingAssetsPath();

            //Pc using streaming Asset Path for resources Update Floder.
            //Mobile using Persistent Path for resources Update Floder.
            if (curPlatform == RuntimePlatform.WindowsPlayer)
            {
                prog?.Invoke(1.0f);
                yield break;
            }
            else if (curPlatform == RuntimePlatform.Android || curPlatform == RuntimePlatform.IPhonePlayer)
            {
                floderdDisk = App.IO.Disk(UnityEngine.Application.persistentDataPath);
            }
            else
            {
                ZDebug.LogError("Uknow Release Platform");
            }

            if (versionFile.Exists)
            {
                if (force) versionFile.Delete();
                else
                {
                    bool update = false;
                    byte[] versionOldBytes = versionFile.Read();
                    string versionOld = Encoding.UTF8.GetString(versionOldBytes);
                    update = UnityEngine.Application.version.VersionCompare(versionOld) > 0;
                    if (update) versionFile.Delete();
                    else yield break;
                }
            }

            string newUpdatePath = Path.Combine(Path.Combine(streamingAssetsPath, App.Env.PlatformToName(App.Env.SwitchPlatform)), UpdateFileStore.FILE_NAME);
            string newUpdateText = "";

            if (newUpdatePath.Contains("://"))
            {
                //纯文本获取, 使用 webrequest 需要将 byte[] 转 文件流。
                //www 更优
                using (WWW www = new WWW(newUpdatePath))
                {
                    newUpdateText = www.text;
                }
            }
            else
            {
                IFile updateFile = floderdDisk.File(newUpdatePath, PathTypes.Absolute);
                byte[] updateBytes = updateFile.Read();
                newUpdateText = Encoding.Default.GetString(updateBytes);
            }
            prog(0.01f);

            var fileStore = App.UpdateFileStore;
            var newLst = new UpdateFile(newUpdateText);
            var localLst = fileStore.LoadFromPath(App.Env.AssetPath);
            if (localLst == null) localLst = CreateUpdateFile();
            IFile localLstFile = fileStore.GetFile(App.Env.AssetPath);

            UpdateFile needUpdateLst, needDeleteLst;

            //比较新旧列表
            localLst.Comparison(newLst, out needUpdateLst, out needDeleteLst);

            //删除旧列表
            yield return DeleteOldAsset(localLst, needDeleteLst, false);

            IFile localDesFile = null;
            UpdateFileField[] needUpdateFields = needUpdateLst.Fields;
            int needUpdateLength = needUpdateFields.Length;


            if (newUpdatePath.Contains("://"))
            {
                for (int i = 0; i < needUpdateLength; i++)
                {
                    string file = needUpdateFields[i].Path;

                    //空字符过滤掉
                    if (string.IsNullOrEmpty(file)) continue;

                    localDesFile = floderdDisk.File(file, PathTypes.Relative);
                    if (localDesFile.Exists) localDesFile.Delete();

                    using (WWW www = new WWW(Path.Combine(Path.Combine(streamingAssetsPath, App.Env.PlatformToName(App.Env.SwitchPlatform)), file)))
                    {
                        yield return www;

                        localDesFile.Directory.Create();
                        localDesFile.Create(www.bytes);
                    }
                    fileStore.Append(localLstFile, needUpdateFields[i]);
                    prog((i + 1) * 1f / needUpdateLength);
                }
            }
            else
            {
                for (int i = 0; i < needUpdateLength; i++)
                {
                    string file = needUpdateFields[i].Path;

                    //空字符过滤掉
                    if (string.IsNullOrEmpty(file)) continue;

                    localDesFile = floderdDisk.File(file, PathTypes.Relative);
                    if (localDesFile.Exists) localDesFile.Delete();

                    IFile srcFile = floderdDisk.File(Path.Combine(Path.Combine(streamingAssetsPath, App.Env.PlatformToName(App.Env.SwitchPlatform)), file), PathTypes.Absolute);
                    byte[] srcBytes = srcFile.Read();
                    localDesFile.Directory.Create();
                    localDesFile.Create(srcBytes);

                    fileStore.Append(localLstFile, needUpdateFields[i]);
                    prog((i + 1) * 1f / needUpdateLength);
                }
            }

            prog(1f);

            versionFile.Create(Encoding.UTF8.GetBytes(UnityEngine.Application.version));
        }

        /// <summary>
        /// 更新Asset
        /// </summary>
        /// <returns>迭代器</returns>
        public void UpdateAsset(bool autoDownload = true)
        {
            this.autoDownload = autoDownload;
            //Staging模式下资源目录会被定位到发布文件目录所以不能进行热更新
#if UNITY_EDITOR
            if (App.Env.DebugLevel == DebugLevels.Staging)
            {
                return;
            }
            if (App.Env.DebugLevel == DebugLevels.Auto || App.Env.DebugLevel == DebugLevels.Develop)
            {
                return;
            }
#endif
            App.Instance.StartCoroutine(StartUpdate());
        }

        /// <summary>
        /// 启动更新
        /// </summary>
        /// <returns>迭代器</returns>
        private IEnumerator StartUpdate()
        {
            if (isUpdate)
            {
                yield break;
            }

            isUpdate = true;

            var resUrl = string.Empty;

            if (updateAPI != null)
            {
                var request = UnityWebRequest.Get(updateAPI);
                yield return request.SendWebRequest();
                if (!request.result.Equals(UnityWebRequest.Result.ConnectionError) && request.responseCode == 200)
                {
                    resUrl = request.downloadHandler.text;
                }
            }

            if (resUrl == string.Empty)
            {
                if (updateURL != null)
                {
                    resUrl = updateURL;
                }
                else
                {
                    App.Instance.Trigger(AutoUpdateEvents.ON_GET_UPDATE_URL_FAILD, this);
                    yield break;
                }
            }

            yield return UpdateList(resUrl);
        }

        /// <summary>
        /// 生成更新文件
        /// </summary>
        /// <returns></returns>
        UpdateFile CreateUpdateFile()
        {
            UpdateFile fileLst = new UpdateFile();
            Disk.Root.Create();
            Disk.Root.Walk((f) =>
            {
                if (StandardPath(f.FullName).EndsWith(".meta"))
                {
                    return;
                }
                var fullName = StandardPath(f.FullName);
                var assetName = fullName.Substring(App.Env.AssetPath.Length + 1);
                fileLst.Append(assetName, App.Hash.FileMd5(f.FullName), f.Length);
            });
            var fileStore = App.UpdateFileStore;
            fileStore.Save(App.Env.AssetPath, fileLst);
            return fileLst;
        }

        /// <summary>
        /// 获取文件更新列表
        /// </summary>
        /// <param name="resUrl">更新url</param>
        /// <returns>迭代器</returns>
        private IEnumerator UpdateList(string resUrl)
        {
            App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_START, this);

            resUrl = resUrl + Path.AltDirectorySeparatorChar + App.Env.PlatformToName(App.Env.SwitchPlatform);
            var request = UnityWebRequest.Get(resUrl + Path.AltDirectorySeparatorChar + UpdateFileStore.FILE_NAME);

            //log test.
            //string path = resUrl + Path.AltDirectorySeparatorChar + UpdateFileStore.FILE_NAME;
            //ZeusDebug.Log("Update Path: " + path);

            yield return request.SendWebRequest();

            //ZeusDebug.Log(" request.downloadHandler.data length: " + request.downloadHandler.data.Length);
            if (request.result.Equals(UnityWebRequest.Result.ConnectionError)) //|| request.responseCode != 200)
            {
                isUpdate = false;
                App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_LIST_FAILED, this);

                //ZeusDebug.Log(" request net work error: " + request.responseCode);
                yield break;
            }

            //WWW www = new WWW(path);
            //while (!www.isDone) yield return null;

            App.Instance.Trigger(AutoUpdateEvents.ON_SCANNING_DISK_FILE_HASH_START, this);
            var fileStore = App.UpdateFileStore;
            var newLst = fileStore.LoadFromBytes(request.downloadHandler.data);//(www.bytes);
            var localLst = fileStore.LoadFromPath(App.Env.AssetPath);
            if (localLst == null) localLst = CreateUpdateFile();

            App.Instance.Trigger(AutoUpdateEvents.ON_SCANNING_DISK_FILE_HASH_END, this);

            UpdateFile needUpdateLst, needDeleteLst;
            localLst.Comparison(newLst, out needUpdateLst, out needDeleteLst);

            yield return DeleteOldAsset(localLst, needDeleteLst);

            yield return UpdateAssetFromUrl(localLst, needUpdateLst, resUrl);

            //fileStore.Save(App.Env.AssetPath, localLst);

            if (isUpdate)
            {
                isUpdate = false;
                App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_COMPLETE, this);
            }
        }

        /// <summary>
        /// 删除旧的资源
        /// </summary>
        /// <param name="needDeleteLst">旧资源列表</param>
        /// <returns>迭代器</returns>
        private IEnumerator DeleteOldAsset(UpdateFile localLst, UpdateFile needDeleteLst, bool notify = true)
        {
            if (notify) App.Instance.Trigger(AutoUpdateEvents.ON_DELETE_DISK_OLD_FILE_START, this);
            IFile file;
            foreach (UpdateFileField field in needDeleteLst)
            {
                //ZeusDebug.Log(App.Env.AssetPath + Path.DirectorySeparatorChar + field.Path);
                file = Disk.File(App.Env.AssetPath + Path.DirectorySeparatorChar + field.Path, PathTypes.Absolute);
                if (!file.Exists)
                {
                    localLst.Delete(field);
                    continue;
                }
                if (notify) App.Instance.Trigger(AutoUpdateEvents.ON_DELETE_DISK_OLD_FILE_ACTION, this);
                //ZeusDebug.Log("Delete File: " + field);
                file.Delete();
                localLst.Delete(field);
            }

            yield return null;
            var fileStore = App.UpdateFileStore;
            fileStore.Save(App.Env.AssetPath, localLst);
            if (notify) App.Instance.Trigger(AutoUpdateEvents.ON_DELETE_DISK_OLD_FILE_END, this);
        }

        /// <summary>
        /// 通过url更新资源
        /// </summary>
        /// <param name="needUpdateLst">需要更新的列表</param>
        /// <param name="downloadUrl">下载列表</param>
        /// <returns>迭代器</returns>
        private IEnumerator UpdateAssetFromUrl(UpdateFile localLst, UpdateFile needUpdateLst, string downloadUrl)
        {
            string savePath, downloadPath, saveDir;

            App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_FILE_START, this, new UpdateFileStartEventArgs(needUpdateLst));

            while (!autoDownload) yield return null;

            var fileStore = App.UpdateFileStore;
            var localFile = fileStore.GetFile(App.Env.AssetPath);

            //ToDo:
            // 增加断点续传，支持到多线程下载策略。
            // 一、 整AB资源 不压缩方案：
            //      1.Update.File 文件内容 需要添加ID。
            //      2.基于Id 开启多线程去进行下载
            //二、  整AB资源压缩方案
            //      1.使用Shell脚本,将整包 压缩成 Zip， 并进行分块。
            //      2.分块大小，由项目实际情况觉得。
            //      3.新增UpdateId.ini 文件 记录分段Id。
            //      4.基于分段id 进行多线程下载。
            //      5.下载完成后，组装zip文件。并解压。
            //三、  字符串拼接GC优化
            UpdateFileField[] fields = needUpdateLst.Fields;
            for (int i = 0; i < fields.Length; i++)
            {
                downloadPath = downloadUrl + Path.AltDirectorySeparatorChar + fields[i].Path;
                savePath = App.Env.AssetPath + Path.AltDirectorySeparatorChar + fields[i].Path;
                saveDir = savePath.Substring(0, savePath.LastIndexOf(Path.AltDirectorySeparatorChar));

                using (var request = UnityWebRequest.Get(downloadPath))
                {
                    UnityWebRequestAsyncOperation webRequestOp = request.SendWebRequest();

                    //App.Trigger的事件要传递EventArgs，如果这里使用 while() yield return null;的方法实时反馈下载进度，会造成大量的内存碎片，一个文件提交一次事件就好
                    App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_FILE_ACTION, this, new UpdateFileActionEventArgs(fields[i], webRequestOp));

                    yield return webRequestOp;

                    if (request.result.Equals(UnityWebRequest.Result.ConnectionError) || request.responseCode != 200)
                    {
                        //下载错误，中断整个下载流程。
                        //根据上方断点续传方案做优化
                        isUpdate = false;
#if UNITY_EDITOR
                        ZDebug.Log("AutoUpdate: file download error. with Path " + downloadPath);
#endif
                        App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_FILE_FAILD, this);
                        yield break;
                    }

                    //careing file streaming operate
                    Disk.Directory(saveDir, PathTypes.Absolute).Create();

                    var saveFile = Disk.File(savePath, PathTypes.Absolute);
                    if (saveFile.Exists) saveFile.Delete();
                    saveFile.Create(request.downloadHandler.data);

                    localLst.Append(fields[i]);
                    fileStore.Append(localFile, fields[i]);
                }
            }

            App.Instance.Trigger(AutoUpdateEvents.ON_UPDATE_FILE_END, this);
        }

        private string StandardPath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}