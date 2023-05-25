using UnityEditor;
using Core.Interface.IO;

namespace Core.Interface.AssetBuilder
{
    /// <summary>
    /// 编译上下文
    /// </summary>
    public interface IBuildContext
    {

        /// 是否代码资产加密
        /// </summary>
        bool IsCodeCrypt { get; set; }

        /// <summary>
        /// 是否AssetBundle加密
        /// </summary>
        bool IsAssetCrypt { get; set; }

        /// <summary>
        /// 是否配置资产加密
        /// </summary>
        bool IsConfigCrypt { get; set; }

        /// <summary>
        /// 是否清除旧的AssetBundle标识
        /// </summary>
        bool ClearOldAssetBundleFlag { get; set; }

        /// <summary>
        /// 当前编译目标平台
        /// </summary>
        BuildTarget BuildTarget { get; set; }

        /// <summary>
        /// 目标平台的名字
        /// </summary>
        string PlatformName { get; set; }

        /// <summary>
        /// 需要编译的文件路径
        /// </summary>
        string BuildPath { get; set; }

        /// <summary>
        /// 不需要编译的文件路径
        /// </summary>
        string NoBuildPath { get; set; }

        /// <summary>
        /// 最终发布的路径
        /// </summary>
        string ReleasePath { get; set; }

        ///// <summary>
        ///// 加密路径
        ///// </summary>
        //string EncryptionPath { get; set; }

        /// <summary>
        /// 程序可更改路径
        /// </summary>
        string PersistentDataPath { get; set; }

        /// <summary>
        /// 最终发布的文件列表
        /// </summary>
        string[] ReleaseFiles { get; set; }

        ///// <summary>
        ///// 被加密的文件列表
        ///// </summary>
        //string[] EncryptionFiles { get; set; }

        /// <summary>
        /// 文件系统磁盘
        /// </summary>
        IDisk Disk { get; set; }

        /// <summary>
        /// 首包 or 热更包
        /// </summary>
        bool FirstPkg { get; set; }

        /// <summary>
        /// 首包是否进行拆包
        /// </summary>
        bool SplitPkg { get; set; }
    }
}
