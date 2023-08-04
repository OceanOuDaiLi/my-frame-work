/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	BuildProcess.cs
	Author:		DaiLi.Ou
	Descriptions: AssetBundle Build Process PipeLine.
*********************************************************************/
namespace Core.Interface.AssetBuilder
{
    public enum BuildProcess
    {
        /// <summary>
        /// 打包启动设置(编译环境、输出路径、相关文件夹路径生成等)
        /// </summary>
        Setup = 1,

        /// <summary>
        /// 文件或打包标识清理
        /// </summary>
        Clear = 10,

        /// <summary>
        /// 构建打包资源AssetBundleName
        /// </summary>
        Precompiled = 20,

        /// <summary>
        /// 基于已构建的ABName构建AssetBundle
        /// </summary>
        Build = 30,

        /// <summary>
        /// 文件扫描，记录需要发布的文件列表
        /// </summary>
        Scanning = 40,

        /// <summary>
        /// 资产加密
        /// </summary>
        Encryption = 50,

        /// <summary>
        /// 配置文件生成
        /// </summary>
        GenConfig = 60,

        /// <summary>
        /// 更新文件生成
        /// </summary>
        GenUpdateFile = 70,

        /// <summary>
        /// 资产拆分
        /// </summary>
        SplitBundle = 80,

        /// <summary>
        /// 资产压缩(暂时跳过）
        /// </summary>
        ZipBundle = 90,

        /// <summary>
        /// 版本文件生成
        /// </summary>
        GenVersion = 100,

        Complete = 110,
    }
}
