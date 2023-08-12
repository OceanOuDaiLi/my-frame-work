using UnityEngine;

namespace GameEngine
{
    public class UIPoolMgr : PrefabManager
    {

        public UIPoolMgr()
        {
            pool = PoolManager.Pools[GameConfig.UI_POOL_NAME];
        }

        ~UIPoolMgr()
        {
            Dispose();
        }

        public void DespawnUI(Transform trans)
        {
            if (trans == null) { return; }
            trans.SetParent(pool.group);

            Despawn(trans);
        }

        public Transform Spawn(string prefabName)
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
                CDebug.LogError($"Spawn  {prefabName} failed from UISpawnPool.");

                return null;
            }

            return inst;
        }
    }
}