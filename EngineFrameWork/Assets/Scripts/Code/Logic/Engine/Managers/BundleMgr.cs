using System;
using FrameWork;
using Core.Interface;
using System.Collections;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	BundleMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/
namespace Engine
{
    public class BundleMgr : IDisposable
    {
        List<string> loadedBundleNames = new List<string>();

        ~BundleMgr()
        {
            Dispose();
        }

        public IEnumerator PreloadBundle(string[] bundles, string[] files)
        {
#if UNITY_EDITOR
            if (App.Env.DebugLevel == DebugLevels.Auto || App.Env.DebugLevel == DebugLevels.Develop)
            {
                yield break;
            }
#endif
            for (int i = 0; i < files.Length; i++)
            {
                for (int j = 0; j < bundles.Length; j++)
                {
                    string path = string.Format(bundles[j], files[i].ToLower());

                    if (loadedBundleNames.Contains(path)) { continue; }
                    yield return App.AssetBundleLoader.LoadAssetBundleAsync(path, (assetBundle) =>
                    {
                        loadedBundleNames.Add(path);
                    });
                }
            }
        }

        public void UnloadBundle()
        {
            int count = loadedBundleNames.Count;
            for (int i = 0; i < count; i++)
            {
                App.AssetBundleLoader.UnloadAssetBundle(loadedBundleNames[i]);
            }
        }

        public virtual void Dispose()
        {
            if (loadedBundleNames != null)
            {
                loadedBundleNames.Clear();
                loadedBundleNames = null;
            }
        }
    }
}