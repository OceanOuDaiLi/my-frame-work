using FrameWork;
using System.IO;
using UnityEngine;
using Core.Interface;
using Core.Interface.Resources;
using System.Collections.Generic;


/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	Resources.cs
	Author:		DaiLi.Ou

	Descriptions: Resources Base Class.
*********************************************************************/
namespace Core.Resources
{
    public sealed class Resources
    {
        private readonly Dictionary<System.Type, string> extensionDict = new Dictionary<System.Type, string>();

        public IResourcesHosted ResourcesHosted { get; set; }

        public void SetHosted(IResourcesHosted hosted)
        {
            ResourcesHosted = hosted;
        }

        public Resources()
        {
            extensionDict.Add(typeof(Object), ".prefab");
            extensionDict.Add(typeof(GameObject), ".prefab");
            extensionDict.Add(typeof(TextAsset), ".txt");
            extensionDict.Add(typeof(Material), ".mat");
            extensionDict.Add(typeof(Shader), ".shader");
        }


        ///////////////////////////////////
        ///  ## 异步 加载 (线程) ##  //////
        ///////////////////////////////////
        public async ETTask LoadAsyncTask(string path, System.Action<IObject> callback)
        {
            await LoadAsyncTask(path, typeof(Object), (obj) =>
            {
                if (obj == null)
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("Res '{0}' is not found", path));
#endif
                }
                callback?.Invoke(obj);
            });
        }

        public async ETTask LoadAsyncTask<T>(string path, System.Action<IObject> callback) where T : Object
        {
            ETTask tcs = ETTask.Create(true);
            IObject result = null;
            await LoadAsyncTask(path, typeof(T), (obj) =>
            {
                if (obj == null)
                {
                    ZDebug.LogError(string.Format("Res '{0}' is not found", path));
                }

                result = obj;
                callback?.Invoke(result);
                tcs.SetResult();
            });

            await tcs;
            tcs = null;
        }

        public async ETTask LoadAsyncTask(string path, System.Type type, System.Action<IObject> callback)
        {
            path = PathFormat(path, type);

#if UNITY_EDITOR
            if (App.Env.DebugLevel == DebugLevels.Auto || App.Env.DebugLevel == DebugLevels.Develop)
            {
                string assetPath = App.Env.AssetPath.Substring(App.Env.AssetPath.IndexOf("Assets")) + App.Env.ResourcesBuildPath + Path.AltDirectorySeparatorChar + path;
                callback(new DefaultObjectWrapper(UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type)));

                return;
            }
#endif

            IObject hosted;
            if (ResourcesHosted != null)
            {
                hosted = ResourcesHosted.Get(path);
                if (hosted != null)
                {
                    callback(hosted);
                    return;
                }
            }
            CoroutineLock coroutineLock = await CoroutineLockComponent.Wait(CoroutineLockType.AssetBundle, LoadPathConvertHelper.LoadPathConvert(path));
            ETTask tcs = ETTask.Create(true);
            await App.AssetBundleLoader.TaskLoadAssetAsync(PathFormat(path, type), type,
            (obj) =>
            {
                hosted = ResourcesHosted != null ? ResourcesHosted.Hosted(path, obj) : MakeDefaultObjectInfo(obj);
                callback(hosted);
                tcs.SetResult();
                tcs = null;
            });
            await tcs;

            coroutineLock.Dispose();
        }


        ///////////////////////////////////
        ///  ##   Util Methods   ##  //////
        ///////////////////////////////////
        private IObject MakeDefaultObjectInfo(Object obj)
        {
            return new DefaultObjectWrapper(obj);
        }

        private string PathFormat(string path, System.Type type)
        {
            var extension = Path.GetExtension(path);
            if (extension != string.Empty)
            {
                return path;
            }
            if (extensionDict.ContainsKey(type))
            {
                return path + extensionDict[type];
            }
            return path;
        }
    }
}
