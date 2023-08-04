﻿using System.IO;
using FrameWork;
using UnityEngine;
using Core.Interface;
using System.Collections;
using Core.Interface.Resources;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	ResourcesHosted.cs
	Author:		DaiLi.Ou
	Descriptions: Resources Hosting Class.
*********************************************************************/

namespace Core.Resources
{
    /// <summary>
    /// 资源托管
    /// </summary>
    public sealed class ResourcesHosted : IUpdate, IResourcesHosted
    {
        /// <summary>
        /// 自检间隔
        /// </summary>
        private const int SELF_CHECK_INTERVAL = 5;

        /// <summary>
        /// 多少时间卸载1个资源包
        /// </summary>
        private const float UNLOAD_INTERVAL = 1;

        /// <summary>
        /// 引用字典（查询）
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, ObjectInfo>> refDict = new Dictionary<string, Dictionary<string, ObjectInfo>>();

        /// <summary>
        /// 遍历用列表（冗余数据）
        /// </summary>
        private readonly List<ObjectInfo> refTraversal = new List<ObjectInfo>();

        /// <summary>
        /// 释放队列
        /// </summary>
        private readonly Queue<ObjectInfo> destroyQueue = new Queue<ObjectInfo>();

        /// <summary>
        /// 检测时钟
        /// </summary>
        private float time = SELF_CHECK_INTERVAL;

        /// <summary>
        /// 构建一个资源托管
        /// </summary>
        public ResourcesHosted()
        {
            App.Instance.StartCoroutine(UnLoad());
        }

        /// <summary>
        /// 获取一个引用
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>包装对象</returns>
        public IObject Get(string path)
        {
            var assetPath = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);
            if (!refDict.ContainsKey(assetPath) || !refDict[assetPath].ContainsKey(name))
            {
                return null;
            }
            var info = refDict[assetPath][name];
            info.IsDestroy = false;
            return info;
        }

        /// <summary>
        /// 托管内容
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="obj">生成的对象</param>
        /// <returns>包装对象</returns>
        public IObject Hosted(string path, Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var assetPath = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (!refDict.ContainsKey(assetPath))
            {
                refDict.Add(assetPath, new Dictionary<string, ObjectInfo>());
            }

            if (refDict[assetPath].ContainsKey(name))
            {
                //如果已经缓存了相同名称，但不是同一个对象时，抛出异常
                if (!refDict[assetPath][name].Original.Equals(obj))
                    throw new System.Exception("Can't host resources" + obj.name);
                return refDict[assetPath][name];
            }
            else
            {
                var info = new ObjectInfo(assetPath, name, obj);
                refDict[assetPath].Add(name, info);
                refTraversal.Add(info);
                return info;
            }
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public void Update()
        {
            if (!((time -= App.Time.DeltaTime) <= 0))
            {
                return;
            }
            for (var i = 0; i < refTraversal.Count; i++)
            {
                if (refTraversal[i].IsDestroy)
                {
                    continue;
                }
                refTraversal[i].Check();
                if (!refTraversal[i].IsDestroy)
                {
                    continue;
                }
                //防止处理卸载来不及不停的加入队列
                if (!destroyQueue.Contains(refTraversal[i]))
                {
                    destroyQueue.Enqueue(refTraversal[i]);
                }
            }
            time += SELF_CHECK_INTERVAL;
        }

        /// <summary>
        /// 卸载不使用的资源包
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator UnLoad()
        {
            ObjectInfo info;
            Dictionary<string, ObjectInfo> tmpDict;
            bool needDestroy;

            while (true)
            {
                if (destroyQueue.Count <= 0)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                info = destroyQueue.Dequeue();
                if (!info.IsDestroy)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                needDestroy = true;
                if (refDict.ContainsKey(info.AssetBundle))
                {
                    tmpDict = refDict[info.AssetBundle];
                    foreach (var val in tmpDict.Values)
                    {
                        if (!val.IsDestroy)
                        {
                            needDestroy = false;
                        }
                    }
                    if (needDestroy)
                    {
                        var isSuccess = App.AssetBundleLoader.UnloadAssetBundle(info.AssetBundle);
                        if (isSuccess)
                        {
                            foreach (var val in tmpDict.Values)
                            {
                                refTraversal.Remove(val);
                            }
                            refDict.Remove(info.AssetBundle);
                            yield return new WaitForSeconds(UNLOAD_INTERVAL);
                        }
                        else
                        {
                            //如果释放失败则重新丢入队尾
                            destroyQueue.Enqueue(info);
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}