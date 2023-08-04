
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	SceneLoadMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 场景加载管理类。用于加载进入的Game场景中，各类资源
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

        private SceneModelMgr modelMgr = null;

        public CharacterLoadMgr characterLoadMgr = null;

        ~SceneLoadMgr()
        {
            OnDispose();
        }

        protected override void Init()
        {
            modelMgr = GlobalData.instance.sceneModelMgr;
            characterLoadMgr = new CharacterLoadMgr();
            base.Init();
        }

        public void OnDispose()
        {
            if (modelMgr.SceneMap != null) { modelMgr.SceneMap.Dispose(); }
            modelMgr.SceneMap = null;

            if (characterLoadMgr != null) { characterLoadMgr.OnDispose(); }
            characterLoadMgr = null;

            _curMapData = null;
            _curMapMaskData = null;

            loadedCurMapData = false;
            loadedCurMapMaskData = false;
        }

        public IEnumerator StartLoadSceneAssets(Action down, Action<float> showProgress)
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
            while (modelMgr.MapMgrObj == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(4 / 7));
            // 5. Load CameraMgr
            yield return SyncLoadCameraMgr();
            while (modelMgr.CameraMgrObj == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(5 / 7));
            // 6. Load MapInstance.
            yield return SyncLoadMapInstance();
            while (modelMgr.MapInstance == null) { yield return Yielders.EndOfFrame; }
            showProgress((float)(6 / 7));

            // 7. Load User Characters.
            yield return characterLoadMgr.CreateSceneMapCharacters();
            while (!characterLoadMgr.LoadedAllSceneCharacters()) { yield return Yielders.EndOfFrame; }
            showProgress(1);

            down?.Invoke();
        }

        #region Load Fight Map Assets

        public IEnumerator StartLoadFightMapAssets()
        {
            // load fightMap parent Node
            string mapPath = $"map/common/FightMapMgr";
            if (modelMgr.FightMap == null)
            {
                yield return GameMgr.Ins.LoadGameAssets("FightMapMgr", mapPath, (tar) =>
                {
                    modelMgr.FightMap = tar.GetComponent<FightMap>();
                    CDebug.LogProgress("## [1] FightMapMgr Loaded ##");
                });
            }
            while (modelMgr.FightMap == null) { yield return Yielders.EndOfFrame; }

            // load fight map node
            var fightModelMgr = GlobalData.instance.fightModelMgr;
            var fightMapTrans = fightModelMgr.GetCurFightMap();
            if (fightMapTrans == null)
            {
                string loadPath = fightModelMgr.GetCurFightMapPath();
                yield return GameMgr.Ins.LoadGameAssets($"{loadPath}_fight_map", $"fight_map/{loadPath}/{loadPath}_fight", (tar) =>
                {
                    //modelMgr.FightMap = tar.GetComponent<FightMap>();
                    tar.transform.SetParent(modelMgr.FightMap.transform, false);
                    fightModelMgr.SetCurFightMap(tar.transform);
                    CDebug.LogProgress("## [2] FightMap Instance Loaded ##");
                    // CameraMgr.Instance.StartFigtBindCamera(tar);
                });
            }


            yield return characterLoadMgr.CreateFightCharacters();
            while (!characterLoadMgr.LoadedFightCharacters()) { yield return Yielders.EndOfFrame; }


            CameraMgr.Instance.StartFigtBindCamera();
            CDebug.LogProgress("## [2] FightCharacters Loaded ##");

        }

        #endregion

        #region Load Game Assets Methods

        /// <summary>
        /// 加载地图配置
        /// </summary>
        private void InitMapConfig(string blockName)
        {
            modelMgr.SceneMap.LoadMapBlock(_curMapData, _curMapMaskData, blockName);
            CDebug.LogProgress("## [3] InitMapConfig Success ##");
        }

        /// <summary>
        /// 加载MapMgr预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadMapMgr()
        {
            string mapPath = $"map/common/MapMgr";
            yield return GameMgr.Ins.LoadGameAssets("MapMgr", mapPath, (tar) =>
            {
                modelMgr.MapMgrObj = tar;
                CDebug.LogProgress("## [4] MapMgr Loaded ##");
            });
        }

        /// <summary>
        /// 加载CameraMgr预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadCameraMgr()
        {
            string cameraPath = $"map/common/CameraMgr";
            yield return GameMgr.Ins.LoadGameAssets("CameraMgr", cameraPath, (tar) =>
            {
                modelMgr.CameraMgrObj = tar;
                CDebug.LogProgress("## [5] CameraMgr Loaded ##");
            });
        }

        /// <summary>
        /// 加载地图预制体
        /// </summary>
        /// <returns></returns>
        private IEnumerator SyncLoadMapInstance()
        {
            //string path = $"map/1_test/{name}";
            string path = $"map/1005/1005";
            yield return GameMgr.Ins.LoadGameAssets("1_map_bg", path, (tar) =>
            {
                modelMgr.MapInstance = tar.transform;
                modelMgr.MapInstance.SetParent(modelMgr.MapMgrObj.transform, false);
                CDebug.LogProgress("## [6] MapInstance Loaded ##");
            });
        }


        #endregion

        #region Load MapBlock Config

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