﻿using FrameWork;
using System.IO;
using UnityEngine;
using Core.Interface;
using Core.Interface.IO;
using Core.Interface.Resources;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	AssetBundleLoader.cs
	Author:		DaiLi.Ou

	Descriptions: AssetBundleLoader Base Class.
*********************************************************************/
namespace Core.Resources
{
    /// <summary>
    /// AssetBundle加载器
    /// </summary>
    public sealed class AssetBundleLoader : IAssetBundle
    {
        #region Private Variables

        private IDisk disk;
        /// <summary>
        /// 磁盘
        /// </summary>
        private IDisk Disk
        {
            get { return disk ?? (disk = /*App.Env.IsAssetCrypt ? App.AssetCryptDisk :*/ App.AssetDisk); }
        }

        /// <summary>
        /// 主依赖文件
        /// </summary>
        private AssetBundleManifest assetBundleManifest;

        /// <summary>
        /// 被加载的主资源包
        /// </summary>
        private readonly Dictionary<string, MainBundle> loadAssetBundles = new Dictionary<string, MainBundle>();

        /// <summary>
        /// 依赖的资源包
        /// </summary>
        private readonly Dictionary<string, DependenciesBundle> dependenciesBundles = new Dictionary<string, DependenciesBundle>();

        /// <summary>
        /// 加载中的资源包
        /// </summary>
        private readonly List<string> onLoadingAssetBundles = new List<string>();

        /// <summary>
        /// 被保护的资源包列表（不能被卸载）
        /// </summary>
        private readonly Dictionary<string, int> protectedList = new Dictionary<string, int>();

        private string loadPath = string.Empty;
        #endregion

        #region 异步加载

        public async ETTask TaskLoadAssetAsync(string path, System.Type type, System.Action<Object> callback)
        {
            LoadManifest();
            string relPath, objName;
            ParsePath(path, out relPath, out objName);

            AssetBundle assetTarget = null;

            //加入保护的列表
            if (!protectedList.ContainsKey(relPath))
            {
                protectedList.Add(relPath, 1);
            }
            else
            {
                protectedList[relPath]++;
            }

            ETTask tcs = ETTask.Create(true);
            await TaskLoadAssetBundleAsync(App.Env.AssetPath, relPath, (ab) =>
            {
                assetTarget = ab;
                tcs.SetResult();
            });
            await tcs;
            tcs = null;

            tcs = ETTask.Create(true);
            AssetBundleRequest targetAssetRequest = assetTarget.LoadAssetAsync(objName, type);
            targetAssetRequest.completed += operation =>
             {
                 callback.Invoke(targetAssetRequest != null ? targetAssetRequest.asset : null);
                 tcs.SetResult();
             };
            await tcs;
            tcs = null;

            protectedList[relPath]--;
            if (protectedList[relPath] <= 0)
            {
                protectedList.Remove(relPath);
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="envPath">环境路径</param>
        /// <param name="relPath">相对路径</param>
        /// <param name="complete">完成时的回调用</param>
        /// <returns>迭代器</returns>
        private async ETTask TaskLoadAssetBundleAsync(string envPath, string relPath, System.Action<AssetBundle> complete)
        {
            MainBundle mb = null;
            DependenciesBundle db = null;

            //如果处于其他请求在处理的依赖包
            if (onLoadingAssetBundles.Contains(relPath))
            {
                while (true)
                {
                    await TimeAwaitHelper.AwaitTime(ResourcesHosted.PER_FRAME_TIME);
                    if (onLoadingAssetBundles.Contains(relPath))
                    {
                        continue;
                    }
                    break;
                }
            }

            //如果被别的主包加载请求进行了加载的话直接返回
            if (loadAssetBundles.TryGetValue(relPath, out mb))
            {
                //保证bundle是有效的
                if (mb.Bundle != null)
                {
                    complete(mb.Bundle);
                    return;
                }
                loadAssetBundles.Remove(relPath);
            }

            //如果是其他依赖包建立的依赖加载,那么将依赖包加入主包列表
            if (dependenciesBundles.TryGetValue(relPath, out db))
            {
                //保证bundle是有效的
                if (db.Bundle != null)
                {
                    //将 asset bundle 加入加载中列表
                    onLoadingAssetBundles.Add(relPath);

                    //加载依赖资源
                    await TaskLoadDependenciesAssetBundleAsync(envPath, relPath);

                    loadAssetBundles.Add(relPath, new MainBundle(db.Bundle));

                    onLoadingAssetBundles.Remove(relPath);

                    complete(db.Bundle);
                    return;
                }
            }

            //将 asset bundle 加入加载中列表
            onLoadingAssetBundles.Add(relPath);

            //加载依赖资源
            await TaskLoadDependenciesAssetBundleAsync(envPath, relPath);

            //创建加载主包请求
            AssetBundleCreateRequest assetTargetBundleRequest;

            /*
             * Changed by daili.ou on 2023.5.2
             * 1.暂时不考虑加密。
             * 2.适配首包StreamingAsset加载 => persistentData Disk 不存在文件， 则 使用 StreamingDisk 进行加载。
             * 
            if (Disk.IsCrypt)
            {
                var file = Disk.File(envPath + Path.AltDirectorySeparatorChar + relPath, PathTypes.Absolute);
                assetTargetBundleRequest = AssetBundle.LoadFromMemoryAsync(file.Read());
            }
            else
            {
                assetTargetBundleRequest = AssetBundle.LoadFromFileAsync(envPath + Path.AltDirectorySeparatorChar + relPath);
            }
            */

            loadPath = Path.Combine(envPath, App.Env.PlatformToName(App.Env.SwitchPlatform), relPath);
            Debug.Log(string.Format($"### 加载主包 start . loadPath: {loadPath} ###"));
            if (!File.Exists(loadPath))
            {
                loadPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, App.Env.PlatformToName(App.Env.SwitchPlatform), relPath);
            }
            Debug.Log(string.Format($"### 加载主包 end . loadPath: {loadPath} ###"));
            assetTargetBundleRequest = AssetBundle.LoadFromFileAsync(loadPath);

            //等待主包加载完成
            ETTask tcs = ETTask.Create(true);
            assetTargetBundleRequest.completed += operation =>
            {
                var assetTarget = assetTargetBundleRequest.assetBundle;
                if (assetTarget != null)
                {
                    loadAssetBundles.Remove(relPath);
                    loadAssetBundles.Add(relPath, new MainBundle(assetTarget));
                }
                //从加载列表中移除
                onLoadingAssetBundles.Remove(relPath);

                complete(assetTarget);
                tcs.SetResult();
                tcs = null;
            };
            await tcs;
        }

        /// <summary>
        /// 异步加载资源依赖
        /// </summary>
        /// <param name="envPath">环境路径</param>
        /// <param name="relPath">相对路径</param>
        /// <returns>迭代器</returns>
        private async ETTask TaskLoadDependenciesAssetBundleAsync(string envPath, string relPath)
        {
            var dependenciesAssetBundles = assetBundleManifest.GetAllDependencies(relPath);

            for (var i = 0; i < dependenciesAssetBundles.Length; i++)
            {
                var dependencies = dependenciesAssetBundles[i];

                //如果处于其他请求在处理的依赖包
                if (onLoadingAssetBundles.Contains(dependencies))
                {
                    //挂起等待其他程序的加载完成
                    while (true)
                    {
                        await TimeAwaitHelper.AwaitTime(ResourcesHosted.PER_FRAME_TIME);
                        if (onLoadingAssetBundles.Contains(dependencies))
                        {
                            continue;
                        }
                        break;
                    }
                }

                //如果是其他依赖包发起的加载，那么直接开始下一个依赖包的加载
                DependenciesBundle db = null;
                if (dependenciesBundles.TryGetValue(dependencies, out db))
                {
                    if (db.Bundle != null)
                    {
                        db.RefCount++;
                        continue;
                    }
                    dependenciesBundles.Remove(dependencies);
                }

                //如果是主包发起的加载同时保证asset bundle是有效的，那么这次请求只需要将主包拷贝入依赖列表
                if (loadAssetBundles.ContainsKey(dependencies) && loadAssetBundles[dependencies].Bundle != null)
                {
                    dependenciesBundles.Add(dependencies, new DependenciesBundle(loadAssetBundles[dependencies].Bundle));
                    continue;
                }

                //将 asset bundle 加入加载中列表
                onLoadingAssetBundles.Add(dependencies);

                //建立创建请求
                AssetBundleCreateRequest assetBundleDependencies;

                /*
                 * Changed by daili.ou on 2023.5.2
                 * 1.暂时不考虑加密。
                 * 2.适配首包StreamingAsset加载 => persistentData Disk 不存在文件， 则 使用 StreamingDisk 进行加载。
                 * 
                if (Disk.IsCrypt)
                {
                    var file = Disk.File(envPath + Path.AltDirectorySeparatorChar + dependencies, PathTypes.Absolute);
                    assetBundleDependencies = AssetBundle.LoadFromMemoryAsync(file.Read());
                }
                else
                {
                    assetBundleDependencies = AssetBundle.LoadFromFileAsync(envPath + Path.AltDirectorySeparatorChar + dependencies);
                }
                */

                loadPath = Path.Combine(envPath, App.Env.PlatformToName(App.Env.SwitchPlatform), dependencies);
                Debug.Log(string.Format($"### 加载资源依赖 start . loadPath: {loadPath} ###"));
                if (!File.Exists(loadPath))
                {
                    loadPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, dependencies);
                }
                Debug.Log(string.Format($"### 加载资源依赖 end . loadPath: {loadPath} ###"));
                assetBundleDependencies = AssetBundle.LoadFromFileAsync(loadPath);

                //等待请求完成
                ETTask tcs = ETTask.Create(true);
                assetBundleDependencies.completed += operation =>
                 {
                     //在依赖包中增加请求的asset bundle
                     if (assetBundleDependencies.assetBundle != null)
                     {
                         dependenciesBundles.Remove(dependencies);
                         dependenciesBundles.Add(dependencies, new DependenciesBundle(assetBundleDependencies.assetBundle));
                     }

                     //将 asset bundle 从加载中列表移除
                     onLoadingAssetBundles.Remove(dependencies);
                     tcs.SetResult();
                 };
                await tcs;
                tcs = null;
            }
        }

        #endregion

        #region Util Methods

        /// <summary>
        /// 加载依赖文件
        /// </summary>
        private void LoadManifest()
        {
            if (assetBundleManifest != null)
            {
                return;
            }
#if UNITY_EDITOR
            if (App.Env.DebugLevel == DebugLevels.Auto)
            {
                return;
            }
#endif
            var assetBundle = LoadAssetBundle(App.Env.AssetPath, App.Env.PlatformToName(App.Env.SwitchPlatform));
            assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// <param name="envPath">环境路径</param>
        /// <param name="relPath">相对路径</param>
        private AssetBundle LoadAssetBundle(string envPath, string relPath)
        {
            if (onLoadingAssetBundles.Contains(relPath))
            {
                throw new System.Exception("this asset bundle already async loading.");
            }

            LoadDependenciesAssetBundle(envPath, relPath);

            AssetBundle assetTarget;
            if (!loadAssetBundles.ContainsKey(relPath) || loadAssetBundles[relPath].Bundle == null)
            {
                /*
                 * Changed by daili.ou on 2023.5.2
                 * 1.暂时不考虑加密。
                 * 2.适配首包StreamingAsset加载 => persistentData Disk 不存在文件， 则 使用 StreamingDisk 进行加载。
                 * This is crypt code.
                 * 
                if (Disk.IsCrypt)
                {
                    var file = Disk.File(envPath + Path.AltDirectorySeparatorChar + relPath, PathTypes.Absolute);
                    assetTarget = AssetBundle.LoadFromMemory(file.Read());
                }
                else
                {
                    assetTarget = AssetBundle.LoadFromFile(envPath + Path.AltDirectorySeparatorChar + relPath);
                }
                */

                loadPath = Path.Combine(envPath, App.Env.PlatformToName(App.Env.SwitchPlatform), relPath);
                Debug.Log(string.Format($"### start . loadPath: {loadPath} ###"));
                if (!File.Exists(loadPath))
                {
                    loadPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, relPath, relPath);
                }
                Debug.Log(string.Format($"### end . loadPath: {loadPath} ###"));
                assetTarget = AssetBundle.LoadFromFile(loadPath);

                loadAssetBundles.Remove(relPath);
                loadAssetBundles.Add(relPath, new MainBundle(assetTarget));
            }
            else
            {
                assetTarget = loadAssetBundles[relPath].Bundle;
            }

            return assetTarget;
        }

        /// <summary>
        /// 加载依赖的AssetBundle
        /// </summary>
        /// <param name="envPath">环境路径</param>
        /// <param name="relPath">相对路径</param>
        private void LoadDependenciesAssetBundle(string envPath, string relPath)
        {
            if (assetBundleManifest == null)
            {
                return;
            }

            AssetBundle assetTarget;
            string[] dependencies = assetBundleManifest.GetAllDependencies(relPath);
            foreach (var _dependencies in dependencies)
            {
                if (!dependenciesBundles.ContainsKey(_dependencies) || dependenciesBundles[_dependencies].Bundle == null)
                {
                    /*
                     * Changed by daili.ou on 2023.5.2
                     * 1.暂时不考虑加密。
                     * 2.适配首包StreamingAsset加载 => persistentData Disk 不存在文件， 则 使用 StreamingDisk 进行加载。
                     * 
                    if (Disk.IsCrypt)
                    {
                        var file = Disk.File(envPath + Path.AltDirectorySeparatorChar + _dependencies, PathTypes.Absolute);
                        assetTarget = AssetBundle.LoadFromMemory(file.Read());
                    }
                    else
                    {
                        assetTarget = AssetBundle.LoadFromFile(envPath + Path.AltDirectorySeparatorChar + _dependencies);
                    }
                    */

                    loadPath = Path.Combine(envPath, App.Env.PlatformToName(App.Env.SwitchPlatform), _dependencies);
                    Debug.Log(string.Format($"Dependencies: ### start . loadPath: {loadPath} ###"));
                    if (!File.Exists(loadPath))
                    {
                        loadPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, App.Env.PlatformToName(App.Env.SwitchPlatform), _dependencies);
                    }
                    Debug.Log(string.Format($"Dependencies: ### end . loadPath: {loadPath} ###"));
                    assetTarget = AssetBundle.LoadFromFile(loadPath);

                    dependenciesBundles.Add(_dependencies, new DependenciesBundle(assetTarget));
                }
                else
                {
                    dependenciesBundles[_dependencies].RefCount++;
                }
            }
        }

        /// <summary>
        /// 解析文件路径
        /// </summary>
        /// <param name="path">传入路径</param>
        /// <param name="relPath">相对路径</param>
        /// <param name="objName">对象名</param>
        private void ParsePath(string path, out string relPath, out string objName)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            var dirPath = Path.GetDirectoryName(path);

            objName = name + extension;
            relPath = dirPath;
        }

        /// <summary>
        /// 卸载依赖资源包
        /// </summary>
        /// <param name="assetbundlePath">资源包路径</param>
        private void UnloadDependenciesAssetBundle(string assetbundlePath)
        {
            if (!dependenciesBundles.ContainsKey(assetbundlePath))
            {
                return;
            }
            dependenciesBundles[assetbundlePath].RefCount--;
            if (dependenciesBundles[assetbundlePath].RefCount > 0)
            {
                return;
            }
            //如果被依赖的资源包被当作主包，那么就不移除只删除依赖
            if (!loadAssetBundles.ContainsKey(assetbundlePath))
            {
                dependenciesBundles[assetbundlePath].Bundle.Unload(true);
            }
            dependenciesBundles.Remove(assetbundlePath);
        }

        /// <summary>
        /// 强制卸载全部资源包（一般情况请不要调用）
        /// </summary>
        public bool UnloadAll()
        {
            if (onLoadingAssetBundles.Count > 0)
            {
                return false;
            }
            foreach (var asset in loadAssetBundles)
            {
                if (asset.Value.Bundle != null)
                {
                    asset.Value.Bundle.Unload(true);
                }
            }
            foreach (var asset in dependenciesBundles)
            {
                if (asset.Value.Bundle != null)
                {
                    asset.Value.Bundle.Unload(true);
                }
            }
            loadAssetBundles.Clear();
            dependenciesBundles.Clear();
            protectedList.Clear();
            return true;
        }

        /// <summary>
        /// 卸载指定资源包
        /// </summary>
        /// <param name="assetbundlePath">资源包路径</param>
        public bool UnloadAssetBundle(string assetbundlePath)
        {
            if (protectedList.ContainsKey(assetbundlePath))
            {
                return false;
            }

            if (!loadAssetBundles.ContainsKey(assetbundlePath))
            {
                return true;
            }

            //如果除了作为主包外还被其他包引用那么就不释放
            if (!dependenciesBundles.ContainsKey(assetbundlePath))
            {
                loadAssetBundles[assetbundlePath].Bundle.Unload(true);
            }
            loadAssetBundles.Remove(assetbundlePath);

            //释放依赖
            foreach (var dependencies in assetBundleManifest.GetAllDependencies(assetbundlePath))
            {
                UnloadDependenciesAssetBundle(dependencies);
            }
            return true;
        }

        #endregion
    }
}
