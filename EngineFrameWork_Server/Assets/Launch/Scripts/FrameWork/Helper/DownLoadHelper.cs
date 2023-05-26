using System;
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
        Action finished;
        Queue<UpdateFileField[]> downLoadTaskQueue;

        public DownLoadHelper(Queue<UpdateFileField[]> downLoadTaskQueue)
        {
            this.downLoadTaskQueue = downLoadTaskQueue;
        }

        public void MultiThreadDownLoad(Action finished)
        {
            this.finished = finished;

            foreach (var item in downLoadTaskQueue)
            {
                ExcuteOneDownLoadTask(item).Coroutine();
            }
        }

        public async ETTask ExcuteOneDownLoadTask(UpdateFileField[] fileFields)
        {

        }

        public void OnDispose()
        {
            downLoadTaskQueue.Clear();
        }
    }
}
