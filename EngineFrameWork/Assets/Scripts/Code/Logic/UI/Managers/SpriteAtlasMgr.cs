using System;
using FrameWork;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using TinyJson;
#endif

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	UIPool.cs
	Author:		DaiLi.Ou

	Descriptions:
*********************************************************************/
namespace UI
{
    public class SpriteAtlasMgr
    {
        private readonly List<string> _loadingList = new List<string>();
        private readonly Dictionary<string, SpriteAtlas> _atlasTable = new Dictionary<string, SpriteAtlas>();


        ~SpriteAtlasMgr()
        {
            OnDispose();
        }

        public bool IsDependenceAtlasLoaded(string prefabName)
        {
            if (!AtlasConfig.PrefabAtlasDependenceDic.ContainsKey(prefabName))
            {
                CDebug.LogError($"Can't find key name: {prefabName} on AtlasConfig.PrefabAtlasDependenceDic");
                return false;
            }

            bool loaded = true;
            foreach (var item in AtlasConfig.PrefabAtlasDependenceDic[prefabName])
            {
                if (!_atlasTable.ContainsKey(item))
                {
                    loaded = false;
                    break;
                }
            }

#if UNITY_EDITOR
            if (loaded)
            {
                CDebug.Log($"[{prefabName}] dependence atlas load success: {AtlasConfig.PrefabAtlasDependenceDic[prefabName].ToJson()}");
            }
#endif

            return loaded;
        }

        public void LoadSpriteAtlas(string bundleName, Action<SpriteAtlas> down)
        {
            string path = string.Format("ui/prefabs/{0}/{1}_atlas.spriteatlas", bundleName, bundleName);

            if (_atlasTable.TryGetValue(bundleName, out var atlas))
            {
                if (atlas != null)
                {
                    down?.Invoke(atlas);
                    return;
                }
            }

            if (!_loadingList.Contains(bundleName))
            {
                _loadingList.Add(bundleName);
            }
            else
            {
                GameMgr.Ins.StartCoroutine(WaitingLoadAsync(bundleName, down));
                return;
            }

            App.Res.LoadAsync<SpriteAtlas>(path, (obj) =>
            {
                SpriteAtlas spriteObj = obj.Get<SpriteAtlas>(this);
                if (spriteObj == null)
                {
                    CDebug.LogError($"Load Asset Bundle Error {path}");
                    return;
                }

                _atlasTable.Add(bundleName, spriteObj);
                _loadingList.Remove(bundleName);

                down?.Invoke(spriteObj);
            });
        }

        IEnumerator WaitingLoadAsync(string bundleName, Action<SpriteAtlas> down)
        {
            CDebug.Log($"Waiting load atlas: {bundleName}");
            while (!_atlasTable.ContainsKey(bundleName))
            {
                yield return Yielders.EndOfFrame;
            }

            if (_atlasTable.TryGetValue(bundleName, out var atlas))
            {
                if (atlas != null)
                {
                    down?.Invoke(atlas);
                }
            }
        }

        public virtual void OnDispose()
        {
            _atlasTable.Clear();
            // todo: unload _atlasTable or not.
            _loadingList.Clear();
        }
    }
}