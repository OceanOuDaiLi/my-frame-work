

using System.Collections;

namespace Core.Interface.Network
{
    /// <summary>
    /// 连接器
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// 名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 设定配置
        /// </summary>
        /// <param name="hashtable">配置信息</param>
        void SetConfig(Hashtable hashtable);

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>协程</returns>
        IEnumerator StartServer();

        /// <summary>
        /// 释放链接
        /// </summary>
        void Destroy();
    }
}
