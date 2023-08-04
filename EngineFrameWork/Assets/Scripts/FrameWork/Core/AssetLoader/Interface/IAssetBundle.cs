﻿
using UnityEngine;

namespace Core.Interface.Resources
{
    /// <summary>
    /// Assetbundle
    /// </summary>
    public interface IAssetBundle
    {
        //string Variant{ get; set; }

        /// <summary>
        /// 获取AssetBundle资源包
        /// </summary>
        /// <param name="path">资源包路径</param>
        /// <returns>AssetBundle资源包</returns>
        AssetBundle LoadAssetBundle(string path);

        /// <summary>
        /// 获取AssetBundle资源包
        /// </summary>
        /// <param name="path">资源包路径</param>
        /// <param name="callback">加载完成的回调</param>
        /// <returns>协程</returns>
        UnityEngine.Coroutine LoadAssetBundleAsync(string path, System.Action<AssetBundle> callback);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <returns>加载的对象</returns>
        Object LoadAsset(string path, System.Type type);

        /// <summary>
        /// 加载资源（异步） 
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调</param>
        /// <returns>协程</returns>
        UnityEngine.Coroutine LoadAssetAsync(string path, System.Type type, System.Action<Object> callback);

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