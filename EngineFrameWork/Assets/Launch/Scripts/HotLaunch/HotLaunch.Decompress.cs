using UnityEngine;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        async ETTask AccompanyFilesDecompress()
        {
            LogProgress("Decompressing ... Start");

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
                decompressInfo.CurentDecompressSize += item.Value.LongLength;
                LogProgress(string.Format($"Decompress : {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));

                if (item.Key.Exists)
                {
                    item.Key.Delete();
                }
                item.Key.Create(item.Value);

                downCount++;
                if (downCount.Equals(decompressInfo.DecompressDic.Count))
                {
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
            string _aotStreamingFile = FormatStreamingFilePath(_streamingReleaseDir.File(AOT_FILE).FullName);
            string _keyStreamingFile = FormatStreamingFilePath(_streamingReleaseDir.File(KEY_FILE).FullName);
            string _hostsStreamingFile = FormatStreamingFilePath(_streamingReleaseDir.File(HOSTS_FILE).FullName);
            string _updateStreamingFile = FormatStreamingFilePath(_streamingReleaseDir.File(UPDATE_FILE).FullName);
            string _hotFixStreamingFile = FormatStreamingFilePath(_streamingReleaseDir.File(HOT_FIX_FILE).FullName);

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
        }

        string FormatStreamingFilePath(string path)
        {
            if (Application.isMobilePlatform || Application.isConsolePlatform)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return string.Format("file://{0}", path);
                }
                else if (Application.platform == RuntimePlatform.Android)
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