using System;
using System.Threading;
using System.Collections.Concurrent;

namespace FrameWork.Service
{
    public class ThreadSynchronizationContext : SynchronizationContext
    {
        public static ThreadSynchronizationContext Instance { get; } = new ThreadSynchronizationContext(Thread.CurrentThread.ManagedThreadId);

        private readonly int threadId;

        // �߳�ͬ�����У����ͽ���socket�ص����ŵ��ö��У���poll�ֳ�ͳһִ��
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private Action a;

        public ThreadSynchronizationContext(int threadId)
        {
            this.threadId = threadId;
        }


        public void Update()
        {
            while (true)
            {
                if (!queue.TryDequeue(out a))
                {
                    return;
                }

                try
                {
                    a();
                }
                catch (Exception e)
                {
                    SDebug.LogError(e);
                    throw;
                }
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            Post(() => { callback(state); });
        }

        public void Post(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == this.threadId)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {

                    SDebug.LogError(e);
                }

                return;
            }
            queue.Enqueue(action);
        }

        public void PostNext(Action action)
        {
            queue.Enqueue(action);
        }
    }
}