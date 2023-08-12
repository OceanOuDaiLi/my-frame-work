using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/08
	Filename: 	UIPool.cs
	Author:		DaiLi.Ou
	Descriptions: UI缓冲池。
*********************************************************************/
namespace GameEngine
{
    public class UIPool : MonoBehaviour
    {
        public static UIPool Ins;

        public UIPoolMgr mgr = null;


        private void Awake()
        {
            mgr = new UIPoolMgr();
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
        //                    //Debug.LogErrorFormat("UIPoolManager: There are some ui elements did not put back into UIPool ({0})", key);
        //                }
        //            }
        //        }
        //#endif

        /// <summary>
        /// 根据预制体名称，从缓冲池获取一个实例。
        /// </summary>
        /// <param name="prefabName">预制体名</param>
        /// <returns></returns>
        public Transform SpawnUI(string prefabName)
        {
            return mgr.Spawn(prefabName);
        }

        /// <summary>
        /// 将从缓存池取出来的UI预制体，放回缓存池
        /// </summary>
        /// <param name="trans">放回缓冲池的预制体实例</param>
        public void DespawnUI(Transform trans)
        {
            mgr.DespawnUI(trans);
        }
    }
}
