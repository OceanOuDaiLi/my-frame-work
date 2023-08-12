
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/08
	Filename: 	PropPool.cs
	Author:		DaiLi.Ou
	Descriptions: 道具缓冲池。
*********************************************************************/

namespace GameEngine
{
    public class PropPool : MonoBehaviour
    {
        PropsPoolMgr mgr = null;
        public static PropPool Ins = null;

        private void Awake()
        {
            mgr = new PropsPoolMgr();
            Ins = this;
        }

        private void OnDestroy()
        {
            Ins = null;
        }

        //#if UNITY_EDITOR
        //        private void Update()
        //        {
        //            if (Time.frameCount % (600) == 0)
        //            {
        //                string key = mgr.JudgePoolCorrect();
        //                if (!key.Equals(string.Empty))
        //                {
        //                    Debug.LogErrorFormat("PropPoolManager: There are some prop elements did not put back into EffectPool ({0})", key);
        //                }
        //            }
        //        }
        //#endif

        public Transform SpawnProp(string prefabName)
        {
            return mgr.SpawnProp(prefabName);
        }

        public void DespawnProp(Transform trans)
        {
            mgr.DespawnProp(trans);
        }
    }
}