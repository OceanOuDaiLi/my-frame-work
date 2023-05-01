
using Object = UnityEngine.Object;
using Core.Interface.Resources;
using FrameWork;

namespace Core.Resources
{
    /// <summary>
    /// 资源托管
    /// </summary>
    public interface IResourcesHosted
    {
        /// <summary>
        /// 从托管系统中获取一个托管
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>包装对象</returns>
        IObject Get(string path);

        /// <summary>
        /// 托管内容一个内容
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="obj">托管对象</param>
        /// <returns>包装对象</returns>
        IObject Hosted(string path, Object obj);

        ETTask UnLoad();
    }
}