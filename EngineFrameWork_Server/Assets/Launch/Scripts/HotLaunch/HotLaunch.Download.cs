namespace FrameWork.Launch
{
    public partial class HotLaunch
    {

        void PrepareDownload()
        {
            //1.get local updatelist
            //2.get server updatelist
            //3.compare updatelist


            //downloadData = new DownloadData();

            //int subVersion = !_hasLocalVer ? Utility.SubVersion("0.0.0", _serverVerCode) : Utility.SubVersion(_localVerCode, _serverVerCode);
            //downloadData.downType = subVersion > 1 ? DownloadData.DownloadType.TOTAL_FILE : DownloadData.DownloadType.ZIP;
            //downloadData.zipFile = _serverVer.Get("Version", "ZipFile");
            //downloadData.updateFile = _serverVer.Get("Version", "UpdateFile");
        }

        //async ETTask StartDownload()
        //{
        //    ETTask task = ETTask.Create(true);
        //    switch (downloadData.downType)
        //    {
        //        case DownloadData.DownloadType.TOTAL_FILE:

        //            DownloadTotalFile(() =>
        //            {
        //                task.SetResult();
        //                task = null;

        //            }).Coroutine();

        //            break;
        //        case DownloadData.DownloadType.ZIP:

        //            DownloadZip(() =>
        //            {
        //                task.SetResult();
        //                task = null;

        //            }).Coroutine();

        //            break;
        //        default:
        //            break;
        //    }

        //    await task;
        //}

        //async ETTask DownloadTotalFile(Action complete)
        //{

        //}

        //async ETTask DownloadZip(Action complete)
        //{

        //}

    }
}