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
        /// ָ����Ч�Żػ����
        /// </summary>
        /// <param name="trans"></param>
        public void DespawnMapCell(Transform trans)
        {
            if (trans == null) { return; }
            trans.SetParent(pool.group);

            Despawn(trans);
        }

        /// <summary>
        /// �ӻ�����л�ȡ��Ч���󣬻�����һ��ʵ��
        /// </summary>
        /// <param name="prefabFrom">���ɶ���</param>
        /// <param name="autoDespawn">�Զ��ӻ�����ͷ�</param>
        /// <returns></returns>
        public Transform SpawnMapCell(string prefabName)
        {
            var prefabFrom = pool.prefabs[prefabName];
            if (prefabFrom == null)
            {
                CDebug.LogError($"Can't find prefab: {prefabName} on EffectSpawnPool.");
                return null;
            }

            //����Ԥ�軺���
            Transform prefab = CreatePrefabPoolWithCache(prefabFrom);

            //ʵ����
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


