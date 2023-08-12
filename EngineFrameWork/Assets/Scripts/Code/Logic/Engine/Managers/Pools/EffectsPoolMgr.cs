using UnityEngine;
using System.Collections.Generic;

namespace GameEngine
{
    public class EffectsPoolMgr : PrefabManager
    {
        Dictionary<string, float> durations = new Dictionary<string, float>();

        public EffectsPoolMgr()
        {
            pool = PoolManager.Pools[GameConfig.EFFECT_POOL_NAME];
        }

        /// <summary>
        /// 指定特效放回缓存池
        /// </summary>
        /// <param name="trans"></param>
        public void DespawnEffect(Transform trans)
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
        public Transform SpawnEffect(string prefabName, bool autoDespawn = true)
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

            //duraction秒后把实例despawn
            if (inst != null && autoDespawn)
            {
                float duration = GetMaxDurationFromCache(prefabFrom.name, prefabFrom);
                pool.Despawn(inst, duration);
            }

            return inst;
        }

        float GetMaxDurationFromCache(string name, Transform prefab)
        {
            float maxDuration = 0;
            if (!durations.TryGetValue(name, out maxDuration))
            {
                maxDuration = GetMaxDuration(prefab);
                durations.Add(name, maxDuration);
            }
            return maxDuration;
        }

        float GetMaxDuration(Transform prefab)
        {
            float maxDuration = 0;
            ParticleSystem[] particles = prefab.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                float duration = particles[i].main.duration + particles[i].main.startDelay.constantMax + particles[i].main.startLifetime.constantMax;
                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }
            }
            return maxDuration;
        }
    }
}

