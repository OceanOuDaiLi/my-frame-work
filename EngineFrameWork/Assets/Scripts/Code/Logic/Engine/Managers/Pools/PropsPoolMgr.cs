using UnityEngine;

namespace GameEngine
{
    public class PropsPoolMgr : PrefabManager
    {
        public PropsPoolMgr()
        {
            pool = PoolManager.Pools[GameConfig.PROP_POOL_NAME];
        }

        ~PropsPoolMgr()
        {
            Dispose();
        }

        /// <summary>
        /// 指定道具放回缓存池
        /// </summary>
        /// <param name="trans"></param>
        public void DespawnProp(Transform trans)
        {
            if (trans == null) { return; }
            trans.SetParent(pool.group);

            Despawn(trans);
        }

        public Transform SpawnProp(string prefabName)
        {
            var prefabFrom = pool.prefabs[prefabName];
            if (prefabFrom == null)
            {
                CDebug.LogError($"Can't find prefab: {prefabName} on PropSpawnPool.");
                return null;
            }

            //创建预设缓存池
            Transform prefab = CreatePrefabPoolWithCache(prefabFrom);
            //实例化
            Transform inst = pool.Spawn(prefab, Vector3.zero, new Quaternion().normalized);

            if (inst == null)
            {
                CDebug.LogError($"Spawn  {prefabName} failed from PropSpawnPool.");

                return null;
            }

            return inst;
        }
    }
}