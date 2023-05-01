using UnityEditor;
using Core.Interface.AssetBuilder;
using Core.Interface.IO;

namespace Core.AssetBuilder
{
    /// <summary>
    /// AssetBuilder编译上下文
    /// </summary>
    public class BuildContext : IBuildContext
    {
        /// <summary>
        /// 是否代码资产加密
        /// </summary>
        public bool IsCodeCrypt { get; set; }

        /// <summary>
        /// 是否游戏资产加密
        /// </summary>
        public bool IsAssetCrypt { get; set; }

        /// <summary>
        /// 是否配置资产加密
        /// </summary>
        public bool IsConfigCrypt { get; set; }

        /// <summary>
        /// 是否清除旧的AssetBundle标识
        /// </summary>
        public bool ClearOldAssetBundleFlag { get; set; }

        /// <summary>
        /// 当前编译目标平台
        /// </summary>
        public BuildTarget BuildTarget { get; set; }

        /// <summary>
        /// 目标平台的名字
        /// </summary>
        public string PlatformName { get; set; }

        /// <summary>
        /// 需要编译的文件路径
        /// </summary>
        public string BuildPath { get; set; }

        /// <summary>
        /// 不需要编译的文件路径
        /// </summary>
        public string NoBuildPath { get; set; }

        /// <summary>
        /// 最终发布的路径
        /// </summary>
        public string ReleasePath { get; set; }

        /// <summary>
        /// 最终加密的路径
        /// </summary>
        public string EncryptionPath { get; set; }

        /// <summary>
        /// 程序可更改路径
        /// </summary>
        public string PersistentDataPath { get; set; }

        /// <summary>
        /// 最终发布的文件列表
        /// </summary>
        public string[] ReleaseFiles { get; set; }

        /// <summary>
        /// 被加密的文件列表
        /// </summary>
        public string[] EncryptionFiles { get; set; }

        /// <summary>
        /// 文件系统中对应的磁盘
        /// </summary>
        public IDisk Disk { get; set; }

        /// <summary>
        /// 首包 or 热更包
        /// </summary>
        public bool FirstPkg { get; set; }

        /// <summary>
        /// 终止构建
        /// </summary>
        public bool StopBuild { get; set; }
    }
}
