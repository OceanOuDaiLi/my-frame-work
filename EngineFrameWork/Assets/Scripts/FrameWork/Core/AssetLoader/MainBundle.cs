
using UnityEngine;

namespace Core.Resources
{
    /// <summary>
    /// 主资源包
    /// </summary>
    internal sealed class MainBundle
    {
        /// <summary>
        /// 资源包
        /// </summary>
		public AssetBundle Bundle { get; set; }

        /// <summary>
        /// 构建一个主资源包
        /// </summary>
        /// <param name="bundle">资源包</param>
        public MainBundle(AssetBundle bundle)
        {
            Bundle = bundle;
        }
    }
}