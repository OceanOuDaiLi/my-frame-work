using UnityEngine;

namespace GameEngine
{
    public class MapPoolMgr : PrefabManager
    {
        public MapPoolMgr()
        {
            pool = PoolManager.Pools[GameConfig.MAP_POOL_NAME];
        }

        /// <summary>
        /// 指定特效放回缓存池
        /// </summary>
        /// <param name="trans"></param>
        public void DespawnMapCell(Transform trans)
        {
            if (trans == null) { return; }
            trans.SetParent(pool.group);

            Despawn(trans);
        }

        /// <summary>
        /// 从缓存池中获取特效对象，或生成一个实例
        /// </summary>
        /// <param name="prefabFrom">生成对象</param>
        /// <param name="autoDespawn">自动从缓存池释放</param>
        /// <returns></returns>
        public Transform SpawnMapCell(string prefabName)
        {
            var prefabFrom = pool.prefabs[prefabName];
            if (prefabFrom == null)
            {
                CDebug.LogError($"Can't find prefab: {prefabName} on EffectSpawnPool.");
                return null;
            }

            //创建预设缓存池
            Transform prefab = CreatePrefabPoolWithCache(prefabFrom);

            //实例化
            Transform inst = pool.Spawn(prefab, Vector3.zero, new Quaternion().normalized);
            if (inst == null)
            {
                CDebug.LogError($"Spawn  {prefabName} failed from EffectSpawnPool.");

                return null;
            }

            return inst;
        }
    }
}


