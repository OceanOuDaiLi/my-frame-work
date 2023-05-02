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
            Debug.Log("AccompanyFilesDecompress ...1");

            // get source decompress files.
            IFile sourceAot = _streamingReleaseDir.File(AOT_FILE);
            IFile sourceKey = _streamingReleaseDir.File(KEY_FILE);
            IFile sourceHosts = _streamingReleaseDir.File(HOSTS_FILE);
            IFile sourceHotFix = _streamingReleaseDir.File(HOT_FIX_FILE);
            IFile sourceUpdate = _streamingReleaseDir.File(UPDATE_FILE);

            if (decompressInfo != null) { decompressInfo.Dispose(); }
            decompressInfo = new DecompressInfo();
            decompressInfo.DecompressDic = new System.Collections.Generic.Dictionary<IFile, IFile>();
            Debug.Log("AccompanyFilesDecompress ... 2");

            // get need decompress files.
            if (JudgeFileNeedDecompress(sourceAot, _aotFile)) { decompressInfo.DecompressDic[sourceAot] = _aotFile; }
            if (JudgeFileNeedDecompress(sourceKey, _keyFile)) { decompressInfo.DecompressDic[sourceKey] = _keyFile; }
            if (JudgeFileNeedDecompress(sourceHosts, _hostsFile)) { decompressInfo.DecompressDic[sourceHosts] = _hostsFile; }
            if (JudgeFileNeedDecompress(sourceHotFix, _hotFixFile)) { decompressInfo.DecompressDic[sourceHotFix] = _hotFixFile; }
            if (JudgeFileNeedDecompress(sourceUpdate, _updateFile)) { decompressInfo.DecompressDic[sourceUpdate] = _updateFile; }

            Debug.Log("AccompanyFilesDecompress ... 3 dic cnt:" + decompressInfo.DecompressDic.Count);

            if (decompressInfo.DecompressDic.Count < 1)
            {
                PlayerPrefs.SetInt(DECOMPRESS_SUCCESS, 1);
                return;
            }

            Debug.Log("AccompanyFilesDecompress ... 4");
            decompressInfo.ReSetDecompressSize();
            Debug.Log("AccompanyFilesDecompress ... 5");
            ShowTips(string.Format($"资源解压中 {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));
            Debug.Log("AccompanyFilesDecompress ... 6");

            // multi thread decompress.
            ETTask task = ETTask.Create(true);
            foreach (var item in decompressInfo.DecompressDic)
            {
                CopyAccompanyFiles(item.Key, item.Value, () =>
                {
                    decompressInfo.CurentDecompressSize += item.Key.Length;

                    ShowTips(string.Format($"资源解压中 {GetBytesString(decompressInfo.CurentDecompressSize)} / {GetBytesString(decompressInfo.TotalDecompressSize)}"));

                    if (decompressInfo.CurentDecompressSize.Equals(decompressInfo.TotalDecompressSize))
                    {
                        Debug.Log("AccompanyFilesDecompress ... 7");
                        PlayerPrefs.SetInt(DECOMPRESS_SUCCESS, 1);
                        task.SetResult();
                    }
                });
            }

            await task;

            decompressInfo.Dispose();
        }

        void CopyAccompanyFiles(IFile source, IFile dest, Action complete)
        {
            byte[] sourceByte = source.Read();
            dest = source.CopyTo(dest.Directory);

            complete?.Invoke();
        }

        //async ETTask HotFixFilesDecompress() { }
    }
}