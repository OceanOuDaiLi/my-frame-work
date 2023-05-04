using FrameWork;
using System;
using System.IO;
using UnityEngine;
using Core.Interface;
using Core.Interface.IO;
using Core.Interface.Resources;
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

    [Space(3)]
    [Header("Resources Crypt Toggle")]
    public bool isAssetCrypt = false;

    //MVC Global Event Dispatcher
    public IEventDispatcher CrossDispatcher;

    public bool Inited { get; private set; }

    #region Unity Calls

    void Awake()
    {
        Application.lowMemory += LowMemoryCallBack;
        App.Instance = new Core.Application(this);

        App.Env.SetDebugLevel(debugLevel);
        App.Env.IsAssetCrypt = isAssetCrypt;

#if UNITY_EDITOR
        App.Env.SetDebugLevel(DebugLevels.Auto);
#else
        App.Env.SetDebugLevel(DebugLevels.Product);
#endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        Inited = true;
    }

    //void Update()
    //{
    //    try
    //    {
    //        actionUpdate?.Invoke();
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application Update exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    //void FixedUpdate()
    //{
    //    try
    //    {
    //        actionFixedUpdate?.Invoke();
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application FixedUpdate exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    //void LateUpdate()
    //{
    //    try
    //    {
    //        actionLateUpdate?.Invoke();
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application LateUpdate exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    //void OnApplicationQuit()
    //{
    //    try
    //    {
    //        actionAppQuit?.Invoke();
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application OnApplicationQuit exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    //void OnApplicationPause(bool pauseStatus)
    //{
    //    try
    //    {
    //        actionAppPause?.Invoke(pauseStatus);
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application OnApplicationPause exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    try
    //    {
    //        actionAppFocus?.Invoke(hasFocus);
    //    }
    //    catch (Exception e)
    //    {
    //        ZDebug.LogError("Application OnApplicationFocus exception :" + e.Message + " " + e.StackTrace);
    //    }
    //}

    void LowMemoryCallBack()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
        ZDebug.LogError("### Low memory ### ");
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
            if (Ins.isAssetCrypt)
            {
                file = App.AssetCryptDisk.File(App.Env.PlatformToName() + Path.AltDirectorySeparatorChar + fileName);
            }
            else
            {
                file = App.AssetDisk.File(App.Env.PlatformToName() + Path.AltDirectorySeparatorChar + fileName);
            }
#if UNITY_EDITOR
        }
#endif
        return file;
    }
}
