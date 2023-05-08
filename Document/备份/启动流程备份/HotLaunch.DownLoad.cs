using System.Collections;
using System.Collections.Generic;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        // download list.
        private List<string> _downLoadList;
        // downloaded bytes.
        private byte[] aotDllBytes;
        private byte[] hotDllBytes;
        // server clr version file bytes.
        private byte[] clrVerBytes;

        IEnumerator DownloadAndWriteFiles()
        {
            GetLocalDisk();

            yield return GetNeedUpdateFiles();

            yield return DownloadDlls();

            while (_downLoadList.Count > 0) { yield return null; }

            WriteFiles();

            WriteClrHotFixFile();
        }

        IEnumerator GetNeedUpdateFiles()
        {
            yield return null;
            //yield return RequestGet(_hotlistUrl, ResponseDownloadNeedUpdateList);
            //while (!_gotNeedUpdateTxt) { yield return null; }
        }

        IEnumerator DownloadDlls()
        {
            _downLoadList = new List<string>();

            for (int i = 0; i < needUpdateInfos.Count; i++)
            {
                if (needUpdateInfos[i].fileName.Contains(aotFileName))
                {
                    aotUpdateInfo = needUpdateInfos[i];
                    downloadTotalSize += aotUpdateInfo.fileByteSize;
                    _downLoadList.Add(_aotDllsUrl);
                    yield return null;
                    yield return RequestGet(_aotDllsUrl, ResponseDownloadAotDlls);
                }
                else if (needUpdateInfos[i].fileName.Contains(hotFileName))
                {
                    hotUpdateInfo = needUpdateInfos[i];

                    downloadTotalSize += hotUpdateInfo.fileByteSize;
                    _downLoadList.Add(_hotDllsUrl);
                    yield return null;
                    yield return RequestGet(_hotDllsUrl, ResponseDownloadHotDlls);
                }
                else
                {
                    LogError("Version Compare not pass, But don't have need update file..");
                }
            }

            SetDownloadText();
        }

        void SetDownloadText()
        {
            //text.text = string.Format("下载大小：{0}/{1}", GetBytesString(downloadedSize), GetBytesString(downloadTotalSize));
        }

        string GetBytesString(long bytes)
        {
            if (bytes >= 1073741824L)
            {
                return (bytes / 1073741824L) + "GB";
            }
            else if (bytes >= 1048576L)
            {
                return (bytes / 1048576L) + "MB";
            }
            else if (bytes >= 1024L)
            {
                return (bytes / 1024L) + "KB";
            }
            else
            {
                return bytes + "B";
            }
        }
    }
}