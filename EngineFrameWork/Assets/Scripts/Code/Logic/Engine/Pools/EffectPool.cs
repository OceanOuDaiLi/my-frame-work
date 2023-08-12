using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/08
	Filename: 	EffectPool.cs
	Author:		DaiLi.Ou
	Descriptions: 特效缓存池。
*********************************************************************/
namespace GameEngine
{
    public class EffectPool : MonoBehaviour
    {
        public static EffectPool Ins;

        public EffectsPoolMgr mgr = null;


        private void Awake()
        {
            mgr = new EffectsPoolMgr();
            Ins = this;
        }

        private void OnDestroy()
        {
            Ins = null;
        }

        //#if UNITY_EDITOR
        //        private void Update()
        //        {
        //            if (Time.frameCount % (600 * 100) == 0)
        //            {
        //                string key = mgr.JudgePoolCorrect();
        //                if (!key.Equals(string.Empty))
        //                {
        //                    Debug.LogErrorFormat("EffectPoolManager: There are some effects elements did not put back into EffectPool ({0})", key);
        //                }
        //            }
        //        }
        //#endif

        /// <summary>
        /// 根据预制体名称，从缓冲池获取一个实例。
        /// </summary>
        /// <param name="prefabName">预制体名</param>
        /// <param name="autoDespawn">自动放回缓存池</param>
        /// <returns></returns>
        public Transform SpawnEffect(string prefabName, bool autoDespawn = true)
        {
            return mgr.SpawnEffect(prefabName, autoDespawn);
        }

        /// <summary>
        /// 将从缓存池取出来的特效预制体，放回缓存池
        /// </summary>
        /// <param name="trans">放回缓冲池的预制体实例</param>
        public void DespawnEffect(Transform trans)
        {
            mgr.DespawnEffect(trans);
        }
    }
}
