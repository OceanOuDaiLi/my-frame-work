using UnityEngine;
using System.Collections.Generic;
using System;

namespace GameEngine
{
    public class PrefabManager : BundleMgr
    {
        protected string poolName = string.Empty;
        //预实例化的数量
        protected int preloadAmount = 20;
        //是否限制可实例化的数量
        protected bool limitInstances = false;
        //limitInstances为true时，表示限制的最大数量
        protected int limitAmount = 2;
        //当缓存池变大时，是否把despawn的gameobject剔除
        protected bool cullDespawned = false;
        //cullDespawned为true时，位置大于cullAbove的被剔除
        protected int cullAbove = 20;
        //超过cullAbove时，多少秒后被剔除
        protected int cullDelay = 60;
        //每cullDelay后剔除多少个实例
        protected int cullMaxPerPass = 5;

        public SpawnPool pool = null;
        protected Dictionary<string, Transform> cachePrefabs = new Dictionary<string, Transform>();

        protected void CreatePool(Transform parent)
        {
            pool = PoolManager.Pools.Create(poolName);
            pool.group.parent = parent;
        }

        ~PrefabManager()
        {
            Dispose();
        }
#if UNITY_EDITOR
        public string JudgePoolCorrect()
        {
            foreach (var t in cachePrefabs)
            {
                if (t.Key == null || !t.Value.gameObject.activeInHierarchy)
                {
                    return t.Key;

                }
            }

            return string.Empty;
        }
#endif
        public override void Dispose()
        {
            base.Dispose();
            if (cachePrefabs != null)
            {
                cachePrefabs.Clear();
                cachePrefabs = null;
            }
            pool = null;
        }

        /// <summary>
        /// 从resource加载预设，并创建预设缓存池
        /// </summary>
        /// <param name="name">预设名称路径</param>
        /// <param name="callback">回调，transform：已缓存的预设，用于pool.spawn；bool：是否已缓存过</param>
        /// <param name="createInstance">是否创建instance实例</param>
        protected void CreatePrefabPool(string name, System.Action<Transform> callback)
        {
            Transform prefab = null;

            bool condition = cachePrefabs.TryGetValue(name, out prefab);
            if (condition)
            {
                callback(prefab);
            }
            else
            {
                // 注意，这里是异步加载，会导致callback存在已经被缓存的对象而前面的判断没有过滤
                GameMgr.Ins.LoadGameAssets(name, "path", (obj) =>
                {
                    // 因为异步操作有可能同时几个预设加载完成，如果同一个预设已经在异步中加载完成，这里要先过滤掉已经缓存的预设
                    if (!cachePrefabs.ContainsKey(name))
                    {
                        Transform p = obj.transform;
                        cachePrefabs.Add(name, p);
                        CreatePrefabPool(p);
                        callback(p);
                    }
                    else
                    {
                        callback(cachePrefabs[name]);
                    }
                });
            }
        }

        private void CreatePrefabPool(Transform prefab)
        {
            if (pool.GetPrefab(prefab) == null)
            {
                PrefabPool prefabPool = new PrefabPool(prefab);
                prefabPool.preloadAmount = preloadAmount;
                prefabPool.limitInstances = limitInstances;
                prefabPool.limitAmount = limitAmount;
                prefabPool.cullDespawned = cullDespawned;
                prefabPool.cullAbove = cullAbove;
                prefabPool.cullDelay = cullDelay;
                prefabPool.cullMaxPerPass = cullMaxPerPass;

                pool.CreatePrefabPool(prefabPool);
            }
        }

        /// <summary>
        /// 使用已知的预设去创建一个预设预存池
        /// </summary>
        /// <param name="prefab">已知的预设</param>
        /// <returns>是否已缓存</returns>
        protected Transform CreatePrefabPoolWithCache(Transform prefab)
        {
            Transform t = null;
            //如果缓存里面已经存在同一个名字的预设，则返回已经缓存了的预设
            if (cachePrefabs.TryGetValue(prefab.name, out t))
            {
                return t;
            }
            cachePrefabs.Add(prefab.name, prefab);
            CreatePrefabPool(prefab);
            return prefab;
        }

        public virtual void Despawn(Transform transform)
        {
            pool.Despawn(transform);
        }

        public virtual void DefireAll()
        {
            pool.DespawnAll();
        }
    }
}