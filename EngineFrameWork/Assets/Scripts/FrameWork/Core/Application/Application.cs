using System.Threading;
using Core.Interface;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Library程序
    /// </summary>
    public sealed class Application : Driver, IApplication
    {
        /// <summary>
        /// 全局唯一自增
        /// </summary>
        private long guid;

        /// <summary>
        /// 构建一个Library实例
        /// </summary>
        public Application()
        {
        }

        /// <summary>
        /// 构建一个Library实例
        /// </summary>
        /// <param name="behaviour">驱动脚本</param>
        public Application(Component behaviour)
            : base(behaviour)
        {
        }

        /// <summary>
        /// 获取一个唯一id
        /// </summary>
        /// <returns>应用程序内唯一id</returns>
        public long GetGuid()
        {
            return Interlocked.Increment(ref guid);
        }
    }
}