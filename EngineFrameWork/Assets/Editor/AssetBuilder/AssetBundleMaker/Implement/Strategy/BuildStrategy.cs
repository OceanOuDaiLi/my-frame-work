using FrameWork;
using UnityEditor;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	BuildStrategy.cs
Author:		DaiLi.Ou
Descriptions: Build Assetbundles.
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class BuildStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Build;

        public void Build(IBuildContext context)
        {
            /*
            -- BuildAssetBundleOptions.None：
               默认构建AssetBundle的方式。
               使用LZMA算法压缩，此算法压缩包小，但是加载时间长，而且使用之前必须要整体解压。
               解压以后，这个包又会使用LZ4算法重新压缩，这样这种包就不要对其整体解压了。（也就是第一次解压很慢，之后就变快了。）

            -- BuildAssetBundleOptions.UncompressedAssetBundle：
               不压缩数据，包大，但是加载很快。

            -- BuildAssetBundleOptions.ChunkBasedCompression：
               使用LZ4算法压缩，压缩率没有LZMA高，但是加载资源不必整体解压。
               这种方法中规中矩，官方推荐。
            */


            // 方案一：
            // 资源使用LZ4的方式打包成AssetBundle.
            //拆分或热更资源在Zip Bundle Strategy时,使用7Zip LZMA 进行二次压缩成Zip包.
            BuildPipeline.BuildAssetBundles("Assets" + context.ReleasePath.Substring(App.Env.DataPath.Length),
                                                BuildAssetBundleOptions.ChunkBasedCompression,
                                                context.BuildTarget);

            // 【Curent Build】
            // 方案二：
            // 1.资源使用 UncompressedAssetBundle。不压缩数据，包大，但是加载很快。
            // 2.拆分或热更资源在Zip Bundle Strategy时,使用7Zip的LZMA算法进行资源的压缩。
            // 3.需要注意跟包资源的大小。尽量保证跟包资源只有代码资产AB(需要额外进行LZ4压缩)，和配置相关的文件
            //BuildPipeline.BuildAssetBundles("Assets" + context.ReleasePath.Substring(App.Env.DataPath.Length),
            //                        BuildAssetBundleOptions.UncompressedAssetBundle,
            //                        context.BuildTarget);

            // ## Todo ##
            // 原生进行磁盘空间检测。若空间不足，应提示，并不进行资源下载操作。
            // ## Todo ##
            // 兼容云测（性能测试）的整包加载方式。适配不能进行网络资源下载操作。【如果云测支持，进行资源下载操作。就不需要做此兼容操作了】
            // 资源直接build到StreamingAsset
            // 使用BuildAssetBundleOptions.None
        }
    }
}
