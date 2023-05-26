using System;
using Core.Interface.IO;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        async ETTask DeleteOldAssets()
        {
            needDeleteFields = needDeleteLst.Fields;
            // get delete total size.
            long totalSize = 0;
            foreach (UpdateFileField field in needDeleteFields)
            {
                totalSize += field.Size;
            }
            LogProgress(string.Format($"needDelete Total Size:  {GetBytesString(totalSize)} "));

            // do delete.
            ETTask deleteTask = ETTask.Create(true);
            DoDeleteOldAsset(() =>
            {
                deleteTask.SetResult();
            });

            await deleteTask;
            deleteTask = null;
        }

        void DoDeleteOldAsset(Action down)
        {
            LogProgress("## Start cleaning old asset ... ##");

            IFile file;
            foreach (UpdateFileField field in needDeleteFields)
            {
                file = _assetReleaseDir.File(field.Path);
                LogProgress(string.Format($"Cleaning  asset: {field.Path} size: {field.Size} "));

                if (!file.Exists)
                {
                    localLst.Delete(field);
                    continue;
                }

                file.Delete();
                localLst.Delete(field);
            }
            LogProgress("## Cleaning old asset Success ... ##");
            down?.Invoke();
        }
    }
}
