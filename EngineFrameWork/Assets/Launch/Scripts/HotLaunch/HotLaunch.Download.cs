using System;
using System.IO;
using System.Text;
using Core.Interface.IO;
using UnityEngine.Networking;
using System.Collections.Generic;


namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        int downTask = 0;
        int totalTask = 0;
        Action downloadSuccess;
        Queue<UpdateFileField[]> downLoadTaskQueue;

        async ETTask PrepareDownload()
        {
            updateFileStore = IOHelper.UpdateFileStore;
            //1. get local update-list.
            localLst = new UpdateFile(Encoding.Default.GetString(_updateFile.Read()));
            //2. get server update-list.
            string _serverUptateFileURL = string.Format($"{hostsUrl}/{UPDATE_FILE}");
            await UnityWebRequestGet(_serverUptateFileURL, (data) =>
            {
                serverLst = new UpdateFile(Encoding.Default.GetString(data));
            });
            //3. get need update-list & delete-list.
            localLst.Comparison(serverLst, out needUpdateLst, out needDeleteLst);
        }

        async ETTask DownLoadAssets()
        {
            // get donwload total size.
            needUpdateFields = needUpdateLst.Fields;
            long totalSize = 0;
            foreach (UpdateFileField field in needUpdateFields)
            {
                totalSize += field.Size;
            }

            // do download.
            ResetDownloadTask(totalSize);
            ETTask downloadTask = ETTask.Create(true);
            MultiThreadDownLoad(() =>
            {
                downloadTask.SetResult();
            });

            await downloadTask;
            downloadTask = null;
            //updateFileStore.Save(_assetReleaseDir.Path, localLst);
        }

        void ResetDownloadTask(long bytes)
        {
            long tmpSize = 0;
            long unitSize = 1048576L * TaskDownLoadSize;
            List<UpdateFileField> tmpFiled = new List<UpdateFileField>();
            downLoadTaskQueue = new Queue<UpdateFileField[]>();

            for (int i = 0; i < needUpdateFields.Length; i++)
            {
                tmpSize += needUpdateFields[i].Size;
                if (tmpSize < unitSize)
                {
                    tmpFiled.Add(needUpdateFields[i]);
                }
                else
                {
                    tmpSize = 0;
                    downLoadTaskQueue.Enqueue(tmpFiled.ToArray());
                    tmpFiled.Clear();
                }
            }

            tmpFiled.Clear();
            totalTask = downLoadTaskQueue.Count;
            LogProgress(string.Format($"HotFix Total Size: {0} / {GetBytesString(bytes)} & DownLoadTask Num: {totalTask}"));
        }

        public void MultiThreadDownLoad(Action finished)
        {
            downloadSuccess = finished;
            foreach (var item in downLoadTaskQueue)
            {
                ExcuteDownLoadTask(item).Coroutine();
            }
        }

        public async ETTask ExcuteDownLoadTask(UpdateFileField[] fileFields)
        {
            List<UpdateFileField> errorLs = new List<UpdateFileField>();
            for (int i = 0; i < fileFields.Length; i++)
            {
                await ExcuteDownLoadOneData(fileFields[i], (field) =>
                {
                    errorLs.Add(field);
                });
            }
            if (errorLs.Count > 0)
            {
                // FBIWarning: asset download error
                // ExcuteDownLoadTask(errorLs.ToArray()).Coroutine();
                // errorLs.Clear();
                LogError("FBIWarning: Unbelieveable asset download error !!!!!!");
            }
            else
            {
                errorLs = null;
                downTask++;
            }

            if (downTask.Equals(totalTask))
            {
                downloadSuccess?.Invoke();
            }
        }

        public async ETTask ExcuteDownLoadOneData(UpdateFileField field, Action<UpdateFileField> failed)
        {
            // warning:
            // There are serveral sring combine & io memory used.
            // Need to profiler and opt.
            string downloadUrl, savePath, saveDir;
            downloadUrl = string.Format($"{hostsUrl}{Path.AltDirectorySeparatorChar}{field.Path}");
            savePath = string.Format($"{_assetReleaseDir.Path}{Path.AltDirectorySeparatorChar}{field.Path}");
            saveDir = savePath.Substring(0, savePath.LastIndexOf(Path.AltDirectorySeparatorChar));

            // 1.request get.
            using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
            {
                UnityWebRequestAsyncOperation webRequestAsync = webRequest.SendWebRequest();
                ETTask waitDown = ETTask.Create(true);
                webRequestAsync.completed += (asyncOperation) =>
                {
                    waitDown.SetResult();
                };

                await waitDown;
                waitDown = null;

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    LogError(string.Format($"[Asset Update] -> Download failed. url: {downloadUrl}"));
                    failed?.Invoke(field);
                }

                // 2.IO Create.
                _assetDisk.Directory(saveDir, PathTypes.Absolute).Create();
                var saveFile = _assetDisk.File(savePath, PathTypes.Absolute);
                if (saveFile.Exists) saveFile.Delete();
                saveFile.Create(webRequest.downloadHandler.data);

                // 3.Refresh FileStore.
                localLst.Append(field);
                updateFileStore.Append(_updateFile, field);

                webRequest.downloadHandler.Dispose();
            }

        }
    }
}
