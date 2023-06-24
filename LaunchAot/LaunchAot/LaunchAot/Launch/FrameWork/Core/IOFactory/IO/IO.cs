
using Core.Interface.IO;


namespace Core.IO
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public sealed class IO : IIOFactory
    {
        /// <summary>
        /// 获取磁盘驱动器
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>驱动器</returns>
        public IDisk Disk(string root = null, IIOCrypt ioCrypt = null)
        {
            return new LocalDisk(root, ioCrypt);
        }
    }
}