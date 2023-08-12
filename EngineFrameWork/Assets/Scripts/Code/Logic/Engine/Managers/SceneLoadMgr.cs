
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	SceneLoadMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 场景加载管理类。
                  用于加载进入的Game场景中，各类资源。
                  用于加载进入战斗场景中，各类资源。
*********************************************************************/
using Model;
using System;
using UnityEngine;
using FrameWork.Launch;
using Core.Interface.IO;
using System.Collections;
using UnityEngine.Networking;

namespace GameEngine
{
    public class SceneLoadMgr : MonoSingleton<SceneLoadMgr>
    {
        private byte[] _curMapData;
        private byte[] _curMapMaskData;
        private bool loadedCurMapData { get; set; }
        private bool loadedCurMapMaskData { get; set; }

        private Transform poolParent;
        public Transform PoolParent
        {
            get
            {
                if (poolParent == null)
                {
                    GameObject tar = new GameObject("Pools");

                    poolParent = tar.transform;
                    poolParent.SetParent(transform.parent);
                }

                return poolParent;
            }
        }

        private SceneModelMgr sceneModelMgr = null;
        private FightModelMgr fightModelMgr = null;

        public CharacterLoadMgr characterLoadMgr = null;

        #region Const String
        const string MapMgrPath = "map/common/MapMgr";
        const string CameraMgrPath = "map/common/CameraMgr";
        const string FightMapMgrPath = "map/common/FightMapMgr";
        const string FightCanvasPath = "ui/prefabs/fight/FightCanvas";

        // pools

        private const string UIPoolPath = "pools/original/UIPool";
        private const string MapPoolPath = "pools/original/MapPool";
        private const string PropPoolPath = "pools/original/PropPool";
        private const string EffectPoolPath = "pools/original/EffectPool";
        #endregion

        ~SceneLoadMgr()
        {
            OnDispose();
        }

        protected override void Init()
        {
            sceneModelMgr = GlobalData.instance.sceneModelMgr;
            fightModelMgr = GlobalData.instance.fightModelMgr;
            characterLoadMgr = new CharacterLoadMgr();
            base.Init();
        }

        public void OnDispose()
        {
            if (sceneModelMgr.SceneMap != null) { sceneModelMgr.SceneMap.Dispose(); }
            sceneModelMgr.SceneMap = null;

            if (characterLoadMgr != null) { characterLoadMgr.OnDispose(); }
            characterLoadMgr = null;

            _curMapData = null;
            _curMapMaskData = null;

            loadedCurMapData = false;
            loadedCurMapMaskData = false;
        }

        #region PreLoad Spawn Pool Assets / 加载缓存池相关资源

        public IEnumerator PreLoadPoolAssets()
        {
            yield return GameMgr.Ins.LoadGameAssets($"{GameConfig.PROP_POOL_NAME}PoolMgr", PropPoolPath, (tar) =>
            {
                tar.transform.SetParent(PoolParent);
                CDebug.LogProgress("## Pre Loaded Prop Pools ##");
            });

            yield return GameMgr.Ins.LoadGameAssets($"{GameConfig.EFFECT_POOL_NAME}PoolMgr", EffectPoolPath, (tar) =>
            {
                tar.transform.SetParent(PoolParent);
                CDebug.LogProgress("## Pre Loaded Effect Pools ##");
            });

            yield return GameMgr.Ins.LoadGameAssets($"{GameConfig.UI_POOL_NAME}PoolMgr", UIPoolPath, (tar) =>
            {
                tar.transform.SetParent(PoolParent);
                CDebug.LogProgress("## Pre Loaded UI Pools ##");
            });

            yield return GameMgr.Ins.LoadGameAssets($"{GameConfig.MAP_POOL_NAME}PoolMgr", MapPoolPath, (tar) =>
            {
                tar.transform.SetParent(PoolParent);
                CDebug.LogProgress("## Pre Loaded Map Cell Pools ##");
            });

            FightControl.Ins.Startup();
        }



        #endregion

        #region Load Fight Map Assets / 加载战斗相关资源

        public IEnumerator StartLoadFightMapAssets()
        {
            // 1.load fightMap parent Node
            if (sceneModelMgr.FightMap == null)
            {
                yield return LoadFightMapMgr();
            }
            while (sceneModelMgr.FightMap == null) { yield return Yielders.EndOfFrame; }

            // 2.load fight map node
            if (fightModelMgr.GetCurFightMap == null)
            {
                yield return LoadFightMapInstance();
            }
            while (fightModelMgr.GetCurFightMap == null) { yield return Yielders.EndOfFrame; }

            // 3.load fight canvas
            if (sceneModelMgr.FightCanvas == null)
            {
                yield return LoadFightCanvas();
            }
            while (sceneModelMgr.FightCanvas == null) { yield return Yielders.EndOfFrame; }

            yield return characterLoadMgr.CreateFightCharacters();
            while (!characterLoadMgr.LoadedFightCharacters()) { yield return Yielders.EndOfFrame; }
            CDebug.LogProgress("## [4] FightCharacters Loaded ##");

            CameraMgr.Instance.StartFightBindCamera();
        }

        private IEnumerator LoadFightMapMgr()
        {
            yield return GameMgr.Ins.LoadGameAssets("FightMapMgr", FightMapMgrPath, (tar) =>
            {
                sceneModelMgr.FightMap = tar.GetComponent<FightMap>();
                CDebug.LogProgress("## [1] FightMapMgr Loaded ##");
            });
        }

        private IEnumerator LoadFightMapInstance()
        {
            string loadPath = fightModelMgr.GetCurFightMapPath;
            yield return GameMgr.Ins.LoadGameAssets($"{loadPath}_fight_map", $"map/fight/{loadPath}/{loadPath}_fight", (tar) =>
            {
                var tarTrans = tar.transform;
                tarTrans.SetParent(sceneModelMgr.FightMap.transform, false);
                fightModelMgr.SetCurFightMap(tarTrans);
                CDebug.LogProgress("## [2] FightMap Instance Loaded ##");
            });
        }

        private IEnumerator LoadFightCanvas()
        {
            yield return GameMgr.Ins.LoadGameAssets("FightCanvas", FightCanvasPath, (tar) =>
            {
                DontDestroyOnLoad(tar);
                tar.transform.SetParent(GameMgr.Ins.transform.parent, false);
                sceneModelMgr.FightCanvas = tar;
                CDebug.LogProgress("## [3] FightCanvas Loaded ##");
            });
        }

        #endregion

        #region Load Game Map Assets / 加载Game地图相关资源

        public IEnumerator StartLoadGameMapAssets(Action down, Action<float> showProgress)
        {
            // 1. Load MapBlockData.(.cel file)
            string blockName = "1005";
            yield return LoadMapCelData($"mapCfg/{blockName}/{blockName}.cel");
            while (!loadedCurMapData) { yield return Yielders.EndOfFrame; }
            showProgress((float)(1 / 7));

            // 2. Load MapMaskData.(.mkd file)
            yield return LoadMapMaskData($"mapCfg/{blockName}/{blockName}.mkd");
            while (!loadedCurMapMaskData) { yield return Yielders.EndOfFrame; }
            showProgress((float)(2 / 7));

            // 3. Init MapBlockData.
            InitMapConfig(blockName);

            // 4. Load MapMgr
            yield return SyncLoadMapMgr();
            while (sceneModelMgr.MapMgrObj == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(4 / 7));
            // 5. Load CameraMgr
            yield return SyncLoadCameraMgr();
            while (sceneModelMgr.CameraMgrObj == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(5 / 7));
            // 6. Load MapInstance.
            yield return SyncLoadMapInstance();
            while (sceneModelMgr.MapInstance == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(6 / 7));

            // 7. Load User Characters.
            yield return characterLoadMgr.CreateSceneMapCharacters();
            while (!characterLoadMgr.LoadedSceneCharacters()) { yield return Yielders.EndOfFrame; }
            showProgress(1);

            down?.Invoke();
        }

        /// <summary>
        /// 加载地图配置
        /// </summary>
        private void InitMapConfig(string blockName)
        {
            sceneModelMgr.SceneMap.LoadMapBlock(_curMapData, _curMapMaskData, blockName);
            CDebug.LogProgress("## [3] InitMapConfig Success ##");
        }

        /// <summary>
        /// 加载MapMgr预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadMapMgr()
        {
            yield return GameMgr.Ins.LoadGameAssets("MapMgr", MapMgrPath, (tar) =>
            {
                sceneModelMgr.MapMgrObj = tar;
                CDebug.LogProgress("## [4] MapMgr Loaded ##");
            });
        }

        /// <summary>
        /// 加载CameraMgr预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadCameraMgr()
        {
            yield return GameMgr.Ins.LoadGameAssets("CameraMgr", CameraMgrPath, (tar) =>
            {
                sceneModelMgr.CameraMgrObj = tar;
                CDebug.LogProgress("## [5] CameraMgr Loaded ##");
            });
        }

        /// <summary>
        /// 加载地图预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadMapInstance()
        {
            string path = $"map/base/1005/1005";
            yield return GameMgr.Ins.LoadGameAssets("1_map_bg", path, (tar) =>
            {
                sceneModelMgr.MapInstance = tar.transform;
                sceneModelMgr.MapInstance.SetParent(sceneModelMgr.MapMgrObj.transform, false);
                CDebug.LogProgress("## [6] MapInstance Loaded ##");
            });
        }

        #endregion

        #region Load MapBlock Config / 加载Game地图相关配置

        IDirectory streamDir = null;
        string realPath = string.Empty;
        private IEnumerator LoadMapCelData(string fileName)
        {
            loadedCurMapData = false;
            IFile mapFile = GameMgr.Ins.LoadINIFile(fileName);
            // Load Cel File from PersistentDataPath
            if (mapFile.Exists)
            {
                _curMapData = mapFile.Read();
                loadedCurMapData = true;
                CDebug.LogProgress("## [1] Map Cel Loaded From PersistentDataPath ##");
                yield break;
            }

            // Load Cel File from StreamingAssetsPath
            if (streamDir == null)
            {
                streamDir = IOHelper.StreamingDisk.Directory(IOHelper.PlatformToName());
            }

            realPath = FormatStreamingFilePath(streamDir.File(fileName).FullName);
            yield return UnityWebRequestGet(realPath, (result) =>
            {
                _curMapData = result;
                loadedCurMapData = true;
                CDebug.LogProgress("## [1] Map Cel Loaded From StreamingAssetsPath ##");
            });
        }

        private IEnumerator LoadMapMaskData(string fileName)
        {
            loadedCurMapMaskData = false;

            IFile mapFile = GameMgr.Ins.LoadINIFile(fileName);
            // Load Cel File from PersistentDataPath
            if (mapFile.Exists)
            {
                _curMapMaskData = mapFile.Read();
                loadedCurMapMaskData = true;
                CDebug.LogProgress("## [2] Map Mask Loaded From PersistentDataPath ##");
                yield break;
            }

            // Load Cel File from StreamingAssetsPath
            if (streamDir == null)
            {
                streamDir = IOHelper.StreamingDisk.Directory(IOHelper.PlatformToName());
            }

            realPath = FormatStreamingFilePath(streamDir.File(fileName).FullName);
            yield return UnityWebRequestGet(realPath, (result) =>
            {
                _curMapMaskData = result;
                loadedCurMapMaskData = true;
                CDebug.LogProgress("## [2] Map Mask Loaded From StreamingAssetsPath ##");
            });
        }

        private IEnumerator UnityWebRequestGet(string url, Action<byte[]> response, string errorTips = "", Action failed = null)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation webRequestAsync = webRequest.SendWebRequest();

                while (!webRequestAsync.isDone)
                {
                    yield return Yielders.EndOfFrame;
                }

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                    if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    OnNetError(string.Format($"本地配置文件获取失败 \n  URL: {url} "));
                }
                response?.Invoke(webRequest.downloadHandler.data);
                webRequest.downloadHandler.Dispose();
            }
        }

        private void OnNetError(string info)
        {
            CDebug.LogError(info);
        }

        private string FormatStreamingFilePath(string path)
        {
            if (UnityEngine.Application.isMobilePlatform || UnityEngine.Application.isConsolePlatform)
            {
                if (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return string.Format("file://{0}", path);
                }
                else if (UnityEngine.Application.platform == RuntimePlatform.Android)
                {
                    return path;
                }
                else
                {
                    return path;
                }
            }
            else
            {
                return string.Format("file:///{0}", path);
            }
        }

        #endregion
    }
}