using FrameWork;
using System;
using UnityEngine;
using Core.Interface.IO;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        async ETTask AccompanyFilesDecompress()
        {
            // get source decompress files.
            IFile sourceAot = _streamingReleaseDir.File(AOT_FILE);
            IFile sourceKey = _streamingReleaseDir.File(KEY_FILE);
            IFile sourceHosts = _streamingReleaseDir.File(HOSTS_FILE);
            IFile sourceHotFix = _streamingReleaseDir.File(HOT_FIX_FILE);
            IFile sourceUpdate = _streamingReleaseDir.File(UPDATE_FILE);

            if (decompressInfo != null) { decompressInfo.Dispose(); }
            decompressInfo = new DecompressInfo();
            // get need decompress files.
            if (JudgeFileNeedDecompress(sourceAot, _aotFile)) { decompressInfo.DecompressDic[sourceAot] = _aotFile; }
            if (JudgeFileNeedDecompress(sourceKey, _keyFile)) { decompressInfo.DecompressDic[sourceKey] = _keyFile; }
            if (JudgeFileNeedDecompress(sourceHosts, _hostsFile)) { decompressInfo.DecompressDic[sourceHosts] = _hostsFile; }
            if (JudgeFileNeedDecompress(sourceHotFix, _hotFixFile)) { decompressInfo.DecompressDic[sourceHotFix] = _hotFixFile; }
            if (JudgeFileNeedDecompress(sourceUpdate, _updateFile)) { decompressInfo.DecompressDic[sourceUpdate] = _updateFile; }

            if (decompressInfo.DecompressDic.Count < 1)
            {
                PlayerPrefs.SetInt(DECOMPRESS_SUCCESS, 1);
                return;
            }

            decompressInfo.ReSetDecompressSize();
            ShowTips(string.Format($"资源解压中 {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));

            // multi thread decompress.
            ETTask task = ETTask.Create(true);
            foreach (var item in decompressInfo.DecompressDic)
            {
                CopyAccompanyFiles(item.Key, item.Value, () =>
                {
                    decompressInfo.CurentDecompressSize += item.Key.Length;

                    if (decompressInfo.CurentDecompressSize.Equals(decompressInfo.TotalDecompressSize))
                    {
                        PlayerPrefs.SetInt(DECOMPRESS_SUCCESS, 1);
                        task.SetResult();
                        task = null;
                    }
                }).Coroutine();
            }

            await task;
        }

        async ETTask CopyAccompanyFiles(IFile source, IFile dest, Action complete)
        {
            ETTask task = ETTask.Create(true);
            byte[] sourceByte = null;
            source.ReadAsync((bytes) =>
            {
                sourceByte = bytes;
            });
            dest.CreateAsync(sourceByte, () =>
            {
                task.SetResult();
                task = null;
            });

            await task;

            complete?.Invoke();
        }


        //async ETTask HotFixFilesDecompress() { }
    }
}