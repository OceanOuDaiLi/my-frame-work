using UnityEngine;
using System.Collections;
using Core.Interface.IO;
using System.Collections.Generic;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        private UpdateInfo aotUpdateInfo;
        private UpdateInfo hotUpdateInfo;
        private List<UpdateInfo> needUpdateInfos;
        private int downloadedSize { get; set; }
        private int downloadTotalSize { get; set; }

        private IFile _remoteFile { get; set; }
        private IFile _aotDllFile { get; set; }
        private IFile _hotDllFile { get; set; }

        private IDisk StreamingDisk { get; set; }
        private IDisk PersistentDisk { get; set; }

        private IDirectory rootDir { get; set; }

        void WriteClrHotFixFile()
        {
            LogInfo("[HotLaunch.File] => WriteCsharpHotFixTxt..");

            if (rootDir == null) { rootDir = PersistentDisk.Directory(); }
        }

        void WriteFiles()
        {
            LogInfo("[HotLaunch.File] => WriteDlls..");

            //write aot dll
            GetClrFile();

            if (aotDllBytes != null)
            {
                if (_aotDllFile.Exists)
                {
                    _aotDllFile.Delete();
                }
                _aotDllFile.Create(aotDllBytes);
            }
            if (hotDllBytes != null)
            {
                if (_hotDllFile.Exists)
                {
                    _hotDllFile.Delete();
                }
                _hotDllFile.Create(hotDllBytes);
            }
        }

        void GetClrFile()
        {
            if (_aotDllFile == null || _hotDllFile == null)
            {
                GetLocalDisk();

                rootDir = PersistentDisk.Directory();

                _aotDllFile = rootDir.File(aotFileName);
                _hotDllFile = rootDir.File(hotFileName);
            }
        }

        void GetLocalDisk()
        {
            if (PersistentDisk == null)
                PersistentDisk = IOHelper.IO.Disk(Application.persistentDataPath);

            if (StreamingDisk == null)
                StreamingDisk = IOHelper.IO.Disk(GetStreamingAssetsPath());
        }

        IEnumerator CopyStreamingAssets()
        {
            LogInfo("[HotLaunch.File] => CopyStreamingAssets .. Start");

            GetLocalDisk();

            IDirectory sourceDir = StreamingDisk.Directory();
            IDirectory destDir = PersistentDisk.Directory();

            if (!destDir.Exists())
            {
                destDir.Create();
            }

            _aotDllFile = destDir.File(aotFileName);
            using (WWW www = new WWW(sourceDir.File(aotFileName).FullName))
            {
                yield return www;
                if (_aotDllFile.Exists)
                {
                    _aotDllFile.Delete();
                }

                _aotDllFile.Create(www.bytes);
                LogInfo("[HotLaunch.File] => CopyStreamingAssets .. _aotDllFile finished.");
            }


            _hotDllFile = destDir.File(hotFileName);
            using (WWW www = new WWW(sourceDir.File(hotFileName).FullName))
            {
                yield return www;
                if (_hotDllFile.Exists)
                {
                    _hotDllFile.Delete();
                }
                _hotDllFile.Create(www.bytes);
                LogInfo("[HotLaunch.File] => CopyStreamingAssets .. _hotDllFile finished.");
            }


            _remoteFile = destDir.File(remoteFileName);
            using (WWW www = new WWW(sourceDir.File(remoteFileName).FullName))
            {
                yield return www;
                if (_remoteFile.Exists)
                {
                    _remoteFile.Delete();
                }
                _remoteFile.Create(www.bytes);
                LogInfo("[HotLaunch.File] => CopyStreamingAssets .. _remoteFile finished.");
            }


            while (!_aotDllFile.Exists || !_hotDllFile.Exists || !_remoteFile.Exists)
            {
                yield return null;
            }

            LogInfo("[HotLaunch.File] => CopyStreamingAssets .. End");
        }

        string GetStreamingAssetsPath()
        {

            if (isSkipHotFix)
            {
                return Application.streamingAssetsPath;
            }
            else
            {
                if (Application.isMobilePlatform || Application.isConsolePlatform)
                {
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        return "file://" + Application.streamingAssetsPath;
                    }
                    else if (Application.platform == RuntimePlatform.Android)
                    {
                        return Application.streamingAssetsPath;
                    }
                    else
                    {
                        return Application.streamingAssetsPath;
                    }
                }
                else
                {
                    return "file:///" + Application.streamingAssetsPath;
                }
            }
        }
    }
}