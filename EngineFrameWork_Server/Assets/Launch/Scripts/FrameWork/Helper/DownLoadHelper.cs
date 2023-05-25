using System.Collections.Generic;

namespace FrameWork.Launch
{

    public class DownLoadData
    {
        public byte[] Data;

        private static readonly Queue<DownLoadData> DownLoadDataPool = new Queue<DownLoadData>();

        public static DownLoadData Get()
        {
            if (DownLoadDataPool.Count > 0)
            {
                return DownLoadDataPool.Dequeue();
            }
            else
            {
                return new DownLoadData();
            }
        }

        public static void Recovery(DownLoadData downLoadData)
        {

        }

        public static void ClearPool()
        {
            DownLoadData.ClearPool();
        }
    }

    public class DownLoadHelper
    {
        List<UpdateFileField[]> distributeList;
        Queue<UpdateFileField[]> downLoadTaskQueue;

        public DownLoadHelper(List<UpdateFileField[]> distributeList)
        {
            this.distributeList = distributeList;
            downLoadTaskQueue = new Queue<UpdateFileField[]>();
        }

        public async ETTask DownLoadUpdate()
        {

        }

        public void OnDispose()
        {
            distributeList.Clear();
            downLoadTaskQueue.Clear();
        }
    }
}
