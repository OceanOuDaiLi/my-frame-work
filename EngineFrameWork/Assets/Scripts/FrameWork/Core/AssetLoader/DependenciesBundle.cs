
namespace Core.Resources
{
    /// <summary>
    /// 依赖资源包
    /// </summary>
    internal sealed class DependenciesBundle
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; set; }

        /// <summary>
        /// AssetBundle
        /// </summary>
        public UnityEngine.AssetBundle Bundle { get; set; }

        /// <summary>
        /// 构建一个依赖资源包
        /// </summary>
        /// <param name="assetBundle">AssetBundle</param>
        public DependenciesBundle(UnityEngine.AssetBundle assetBundle)
        {
            Bundle = assetBundle;
            RefCount = 1;
        }
    }
}