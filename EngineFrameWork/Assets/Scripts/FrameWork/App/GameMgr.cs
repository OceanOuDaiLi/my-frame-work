using System;
using System.IO;
using FrameWork;
using UnityEngine;
using Core.Interface;
using Core.Interface.IO;
using strange.extensions.dispatcher.eventdispatcher.api;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	GameMgr.cs
	Author:		DaiLi.Ou

	Descriptions: Globally Unique Gameing Manager.
*********************************************************************/
public class GameMgr : MonoSingleton<GameMgr>
{
    [Space(3)]
    public DebugLevels debugLevel = DebugLevels.Auto;

    //MVC Global Event Dispatcher
    public IEventDispatcher CrossDispatcher;

    public bool Inited { get; private set; }

    #region Unity Calls

    void Awake()
    {
        App.Env.IsAssetCrypt = false;
        App.Env.SetDebugLevel(debugLevel);

        Application.lowMemory += LowMemoryCallBack;
        App.Instance = new Core.Application(this);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR
        App.Env.SetDebugLevel(DebugLevels.Auto);
#else
        App.Env.SetDebugLevel(DebugLevels.Product);
#endif
    }

    void Start()
    {
        Inited = true;
        CDebug.Log("GameMgr Start");
    }

    void LowMemoryCallBack()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
        CDebug.LogError("### Low memory ### ");
    }

    #endregion

    public IFile LoadINIFile(string fileName)
    {
        IFile file = null;
        
#if UNITY_EDITOR
        if (App.Env.DebugLevel == DebugLevels.Auto || App.Env.DebugLevel == DebugLevels.Develop)
        {
            file = App.AssetDisk.File(Path.Combine(App.Env.DataPath + App.Env.ResourcesNoBuildPath, fileName), PathTypes.Absolute);
        }
        else
        {
#endif
            file = App.AssetDisk.File(System.IO.Path.Combine(App.Env.PlatformToName(), fileName));
            //if (Ins.isAssetCrypt)
            //{
            //    file = App.AssetCryptDisk.File(App.Env.PlatformToName() + Path.AltDirectorySeparatorChar + fileName);
            //}
            //else
            //{
            //    file = App.AssetDisk.File(App.Env.PlatformToName() + Path.AltDirectorySeparatorChar + fileName);
            //}
#if UNITY_EDITOR
        }
#endif
        return file;
    }
}
