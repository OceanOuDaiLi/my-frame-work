using Core.Interface.IO;
using Core.IO;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        bool _gotNeedUpdateTxt { get; set; }
        bool _gotServerVersion { get; set; }

        IEnumerator RequestGet(string downloadPath, Action<byte[]> response)
        {
            LogProgress("Web Request: " + downloadPath);
            using (var request = UnityWebRequest.Get(downloadPath))
            {
                UnityWebRequestAsyncOperation webRequestOp = request.SendWebRequest();

                yield return webRequestOp;

                while (!webRequestOp.isDone) { yield return null; }

                if (!request.result.Equals(UnityWebRequest.Result.Success))
                {
                    LogProgress("[HotLaunch.Net] => Request download error. with Path " + downloadPath);
                    response?.Invoke(null);
                }
                else
                {
                    response?.Invoke(request.downloadHandler.data);
                }
            }
        }

        private void ResponseGetClrResHost(byte[] result)
        {
            _resUrl = System.Text.Encoding.UTF8.GetString(result);
            _resUrl = _resUrl.Trim();
            getResUrl = true;
        }

        private void ResponseGetServerVersion(byte[] result)
        {
            if (_gotServerVersion) { return; }

            //cached result
            clrVerBytes = result;

            /*
             * Kings2 不对配置相关的文件进行加密
            //temporary decrypted
            LocalDisk disk = new LocalDisk("", IOHelper.IOCrypt);
            byte[] data = disk.IOCrypt.Decrypted(result);
            */
            _serverVer = System.Text.Encoding.UTF8.GetString(result);
            _gotServerVersion = true;
        }


        private void ResponseDownloadAotDlls(byte[] result)
        {
            downloadedSize += aotUpdateInfo.fileByteSize;
            SetDownloadText();

            aotDllBytes = result;
            _downLoadList.Remove(_aotDllsUrl);
        }

        void ResponseDownloadHotDlls(byte[] result)
        {
            downloadedSize += hotUpdateInfo.fileByteSize;
            SetDownloadText();

            hotDllBytes = result;
            _downLoadList.Remove(_hotDllsUrl);
        }


        UpdateInfo ParseString(string tar)
        {
            string[] firstValue = tar.Split(',');

            UpdateInfo updateInfo = new UpdateInfo();
            for (int i = 0; i < firstValue.Length; i++)
            {
                string[] result = firstValue[i].Split(':');
                if (result[0].Contains("name"))
                {
                    updateInfo.fileName = result[1];
                }
                else if (result[0].Contains("size"))
                {
                    updateInfo.fileByteSize = int.Parse(result[1]);
                }
                else if (result[0].Contains("md5"))
                {
                    updateInfo.fileMd5 = result[1];
                }
                else
                {
                    LogError("Parse HotList.txt error");
                }
            }
            return updateInfo;
        }
    }
}