using System;
using FrameWork;
using DG.Tweening;
using UnityEngine;
using Core.Interface;
using Core.Interface.IO;
using System.Collections;
using strange.extensions.dispatcher.eventdispatcher.api;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	GameMgr.cs
	Author:		DaiLi.Ou
    
    Todo: 迁移游戏流程代码 & 加载相关代码
	Descriptions: Globally Unique Gameing Manager.
*********************************************************************/
public class GameMgr : MonoSingleton<GameMgr>
{
    [Space(3)]
    public DebugLevels debugLevel = DebugLevels.Auto;

    // MVC Global Event Dispatcher
    public IEventDispatcher CrossDispatcher;

    // Is GameMgr Inited.
    public bool Inited { get; private set; }

    private bool loadedCommonCanvas { get; set; }
    public bool LoadedCommonCanvas { get => loadedCommonCanvas; }

    // ui common canvas
    private const string CommonCanvasPath = "ui/prefabs/common/common_canvas";



    /////////////////////////////////////////////////
    /////////Unity Calls/////////////////////////////
    /////////////////////////////////////////////////
    protected override void Init()
    {

        App.Env.IsAssetCrypt = false;
        App.Env.SetDebugLevel(debugLevel);

        Application.lowMemory += LowMemoryCallBack;
        App.Instance = new Core.Application(this);

        DOTween.Init();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR
        App.Env.SetDebugLevel(DebugLevels.Auto);    //设置 DebugLevels.Product 模拟 Mobile 环境
#else
        App.Env.SetDebugLevel(DebugLevels.Product);
#endif
        base.Init();
    }

    void Start()
    {
        Inited = true;
    }

    void LowMemoryCallBack()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
        CDebug.LogError("### Low memory ### ");
    }

    //////////////////////////////////////////////////
    /////////Public Methods///////////////////////////
    //////////////////////////////////////////////////
    public IEnumerator LoadUICommonCanvas()
    {
        loadedCommonCanvas = false;
        yield return LoadGameAssets("CommonUICanvas", CommonCanvasPath, (tar) =>
        {
            DontDestroyOnLoad(tar);
            tar.transform.SetParent(transform.parent, false);
            loadedCommonCanvas = true;
        });
    }

    public IEnumerator LoadGameAssets(string name, string path, Action<GameObject> down)
    {
        yield return App.Res.LoadAsync<GameObject>(path, (obj) =>
        {
            GameObject prefabObj = obj.Get<GameObject>(this);
            if (prefabObj == null)
            {
                CDebug.LogError($"Load Asset Bundle Error {path}");
                return;
            }

            var ins = Instantiate(prefabObj, null);
            ins.name = name;

            down?.Invoke(ins);
        });
    }

    public IFile LoadINIFile(string fileName)
    {
        IFile file = null;

#if UNITY_EDITOR
        if (App.Env.DebugLevel == DebugLevels.Auto || App.Env.DebugLevel == DebugLevels.Develop)
        {
            file = App.AssetDisk.File(System.IO.Path.Combine(App.Env.DataPath + App.Env.ResourcesNoBuildPath, fileName), PathTypes.Absolute);
        }
        else
        {
#endif

            file = App.AssetDisk.File(System.IO.Path.Combine(App.Env.PlatformToName(), fileName));
#if UNITY_EDITOR
        }
#endif
        return file;
    }

}
