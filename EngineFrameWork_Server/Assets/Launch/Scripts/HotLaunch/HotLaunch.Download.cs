using System.Text;
using System.Collections.Generic;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        async ETTask PrepareDownload()
        {
            UpdateFile oldList = null;
            UpdateFile newList = null;

            //1. get local update-list.
            oldList = new UpdateFile(Encoding.Default.GetString(_updateFile.Read()));
            //2. get server update-list.
            string _serverUptateFileURL = string.Format($"{hostsUrl}/{UPDATE_FILE}");
            await UnityWebRequestGet(_serverUptateFileURL, (data) =>
            {
                newList = new UpdateFile(Encoding.Default.GetString(data));
            });
            //3. get need update-list.
            oldList.Comparison(newList, out needUpdateLst, out needDeleteLst);

        }

        async ETTask StartDownload()
        {
            //await DeleteOldAssets();

            await DownLoadAssets();
        }

        //async ETTask DeleteOldAssets()
        //{

        //}

        async ETTask DownLoadAssets()
        {
            // distribute donwload task.
            needUpdateFields = needUpdateLst.Fields;
            long totalSize = 0;
            foreach (UpdateFileField field in needUpdateFields)
            {
                totalSize += field.Size;
            }

            DownLoadHelper downLoadHelper = new DownLoadHelper(DistributeDownloadTask(totalSize));
            await downLoadHelper.DownLoadUpdate();
        }

        List<UpdateFileField[]> DistributeDownloadTask(long bytes)
        {

            long tmpSize = 0;
            long unitSize = 1048576L * TaskDownLoadSize;
            List<UpdateFileField> tmpFiled = new List<UpdateFileField>();
            List<UpdateFileField[]> result = new List<UpdateFileField[]>();

            for (int i = 0; i < needDeleteFields.Length; i++)
            {
                tmpSize += needDeleteFields[i].Size;
                if (tmpSize < unitSize)
                {
                    tmpFiled.Add(needDeleteFields[i]);
                }
                else
                {
                    tmpSize = 0;
                    result.Add(tmpFiled.ToArray());
                    tmpFiled.Clear();
                }
            }

            LogProgress(string.Format($"HotFix Total Size: {0} / {GetBytesString(bytes)} & DownLoadTask Num: {result.Count}"));

            return result;
        }
    }


}