
namespace Core.Interface.Resources
{
    /// <summary>
    /// Assetbundle
    /// </summary>
    public interface IAssetBundle
    {
        /// <summary>
        /// 强制卸载全部资源包（一般情况请不要调用）
        /// </summary>
        bool UnloadAll();

        /// <summary>
        /// 卸载指定资源包
        /// </summary>
        /// <param name="assetbundlePath">资源包路径</param>
        bool UnloadAssetBundle(string assetbundlePath);
    }
}