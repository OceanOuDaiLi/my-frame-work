
namespace Core.Interface.IO
{
    /// <summary>
    /// IO
    /// </summary>
    public interface IIOFactory
    {
        /// <summary>
        /// 获取磁盘驱动器
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>驱动器</returns>
		IDisk Disk(string name = null, IIOCrypt ioCrypt = null);
    }
}