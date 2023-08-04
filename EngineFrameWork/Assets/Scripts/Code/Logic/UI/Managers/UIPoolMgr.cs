
using System;
using UnityEngine;
using System.Collections;
using Core.Interface.Event;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	UIPool.cs
	Author:		DaiLi.Ou

	Descriptions:
*********************************************************************/
public class UIPoolMgr : MonoBehaviour
{
    private Dictionary<Transform, List<Transform>> dictSpawnCache = new Dictionary<Transform, List<Transform>>();
    private SpawnPool uiPool = null;
    private IEventHandler eHandlerSceneChange = null;

    private static UIPoolMgr _ins = null;

    public static UIPoolMgr Ins
    {
        get
        {
            return _ins;
        }
    }

    private void Awake()
    {
        _ins = this;

        // command by daili.ou .  while reset pool.
        // eHandlerSceneChange = App.Instance.On(Config.APP_EVENT_SCENE_CHANGE, OnSceneChange);
    }

    private void OnDestroy()
    {
        if (eHandlerSceneChange != null)
        {
            eHandlerSceneChange.Cancel();
            eHandlerSceneChange = null;
        }
        _ins = null;
    }

    void OnSceneChange(object sender, EventArgs e)
    {
#if UNITY_EDITOR
        if (dictSpawnCache.Count > 0) Debug.LogError("UIPoolMgr: More than one view did not clear UIPool elements");
#endif

        ICollection keys = dictSpawnCache.Keys;
        foreach (Transform k in keys)
        {
            List<Transform> ls = dictSpawnCache[k];
            foreach (Transform t in ls)
            {
                t.SetParent(uiPool.group);
                uiPool.Despawn(t);
            }
            ls.Clear();
        }
        dictSpawnCache.Clear();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Time.frameCount % 600 == 0)
        {
            foreach (Transform t in dictSpawnCache.Keys)
            {
                if (t == null || !t.gameObject.activeInHierarchy)
                {
                    Debug.LogErrorFormat("UIPoolMgr: There are some ui elements did not put back into uipool ({0})", t.name);
                }
            }
        }
    }
#endif

    SpawnPool UIPool
    {
        get
        {
            if (uiPool == null) uiPool = PoolManager.Pools[GameConfig.UI_POOL_NAME];
            return uiPool;
        }
    }

    public Transform Spawn(Transform owner, string prefabName)
    {
        List<Transform> list = null;
        if (dictSpawnCache.ContainsKey(owner)) list = dictSpawnCache[owner];
        else
        {
            list = new List<Transform>();
            dictSpawnCache.Add(owner, list);
        }

        Transform prefab = UIPool.prefabs[prefabName];
        if (prefab == null)
        {
#if UNITY_EDITOR
            Debug.LogErrorFormat("Prefab ({0}) not exist", prefabName);
#endif
            return null;
        }
        Transform inst = UIPool.Spawn(prefab);
        list.Add(inst);
        return inst;
    }

    public T Spawn<T>(Transform owner, string prefabName)
    {
        return Spawn(owner, prefabName).GetComponent<T>();
    }

    public void Despawn(Transform owner)
    {
        List<Transform> list = null;
        dictSpawnCache.TryGetValue(owner, out list);
        if (list != null)
        {
            foreach (Transform t in list)
            {
                t.SetParent(uiPool.group);
                uiPool.Despawn(t);
            }
            list.Clear();
        }
        dictSpawnCache.Remove(owner);
    }

    public void Despawn<T>(Transform owner, Action<T> clearAction)
    {
        List<Transform> list = null;
        dictSpawnCache.TryGetValue(owner, out list);
        if (list != null)
        {
            foreach (Transform t in list)
            {
                T cell = t.GetComponent<T>();
                if (cell != null)
                {
                    clearAction(cell);
                    t.SetParent(uiPool.group);
                    uiPool.Despawn(t);
                }
                continue;
            }
        }
        dictSpawnCache.Remove(owner);
    }

    /*
     * Example:
     * 
     * Desapwn Target Pool Cells.
     * 
    public void DespawnXXXCell(Transform owner)
    {
        Despawn<XXXCell>(owner, (c) =>
        {
            c.onClickCell = null;
            c.ClearData();
        });
    }
    */

}
