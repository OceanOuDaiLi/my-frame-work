using System;

namespace FrameWork.Service
{
    public class TimeInfo : IDisposable
    {
        public static TimeInfo Instance = new TimeInfo();

        private int timeZone;
        public int TimeZone
        {
            get => timeZone;
            set
            {
                timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }

        public long FrameTime;
        public long ServerMinusClientTime { private get; set; }

        private readonly DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        private TimeInfo()
        {
            FrameTime = ClientNow();
        }

        public void Update()
        {
            FrameTime = ClientNow();
        }

        /// <summary>
        /// 根据时间戳获取时间
        /// </summary>
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        /// <returns></returns>
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - dt1970.Ticks) / 1000;
        }

        public long ServerNow()
        {
            return ClientNow() + Instance.ServerMinusClientTime;
        }

        public long ClientFrameTime()
        {
            return FrameTime;
        }

        public long ServerFrameTime()
        {
            return FrameTime + Instance.ServerMinusClientTime;
        }

        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 1000;
        }

        public void Dispose()
        {
            Instance = null;
        }
    }
}
