using UnityEngine;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        async ETTask AccompanyFilesDecompress()
        {
            LogProgress("Decompressing ...");

            // get sources files.
            await RequestStreamingFiles();

            // copy sources file to dest file.
            await CopyFiles();
        }

        async ETTask CopyFiles()
        {
            ETTask copyTask = ETTask.Create(true);
            int downCount = 0;
            foreach (var item in decompressInfo.DecompressDic)
            {
                if (item.Key.Exists)
                {
                    item.Key.Delete();
                }
                decompressInfo.CurentDecompressSize += item.Value.LongLength;
                //LogProgress(string.Format($"Decompress : {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));
                if (item.Key.FullName.Contains("launch"))
                {
                    _assetReleaseDir.Create("ui/prefabs");
                }
                item.Key.Create(item.Value);
                downCount++;
                if (downCount.Equals(decompressInfo.DecompressDic.Count))
                {
                    LogProgress("Copy End");

                    copyTask.SetResult();
                }
            }

            await copyTask;
            decompressInfo.Dispose();
            copyTask = null;
        }

        async ETTask RequestStreamingFiles()
        {
            // prepare decompress
            decompressInfo = new DecompressInfo();
            int copyCount = 5;
            string _aotStreamingFile = GetStreamingFilePath(_streamingReleaseDir.File(AOT_FILE).FullName);
            string _keyStreamingFile = GetStreamingFilePath(_streamingReleaseDir.File(KEY_FILE).FullName);
            string _hostsStreamingFile = GetStreamingFilePath(_streamingReleaseDir.File(HOSTS_FILE).FullName);
            string _updateStreamingFile = GetStreamingFilePath(_streamingReleaseDir.File(UPDATE_FILE).FullName);
            string _hotFixStreamingFile = GetStreamingFilePath(_streamingReleaseDir.File(HOT_FIX_FILE).FullName);

            // get source files.
            ETTask getTask = ETTask.Create(true);
            UnityWebRequestGet(_aotStreamingFile, (data) =>
            {
                decompressInfo.CollectDecompressSize(data.LongLength);
                decompressInfo.DecompressDic[_aotFile] = data;

                if (decompressInfo.DecompressDic.Count == copyCount)
                    getTask.SetResult();

            }).Coroutine();
            UnityWebRequestGet(_keyStreamingFile, (data) =>
            {
                decompressInfo.CollectDecompressSize(data.LongLength);
                decompressInfo.DecompressDic[_keyFile] = data;

                if (decompressInfo.DecompressDic.Count == copyCount)
                    getTask.SetResult();
            }).Coroutine();
            UnityWebRequestGet(_hostsStreamingFile, (data) =>
            {
                decompressInfo.CollectDecompressSize(data.LongLength);
                decompressInfo.DecompressDic[_hostsFile] = data;

                if (decompressInfo.DecompressDic.Count == copyCount)
                    getTask.SetResult();
            }).Coroutine();
            UnityWebRequestGet(_hotFixStreamingFile, (data) =>
            {
                decompressInfo.CollectDecompressSize(data.LongLength);
                decompressInfo.DecompressDic[_hotFixFile] = data;

                if (decompressInfo.DecompressDic.Count == copyCount)
                    getTask.SetResult();
            }).Coroutine();
            UnityWebRequestGet(_updateStreamingFile, (data) =>
            {
                decompressInfo.CollectDecompressSize(data.LongLength);
                decompressInfo.DecompressDic[_updateFile] = data;

                if (decompressInfo.DecompressDic.Count == copyCount)
                    getTask.SetResult();
            }).Coroutine();
            await getTask;
            getTask = null;

            LogProgress(string.Format($"Decompress : {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));
        }

        string GetStreamingFilePath(string path)
        {
            if (UnityEngine.Application.isMobilePlatform || UnityEngine.Application.isConsolePlatform)
            {
                if (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return string.Format("file://{0}", path);
                }
                else if (UnityEngine.Application.platform == RuntimePlatform.Android)
                {
                    return path;
                }
                else
                {
                    return path;
                }
            }
            else
            {
                return string.Format("file:///{0}", path);
            }
        }
    }
}