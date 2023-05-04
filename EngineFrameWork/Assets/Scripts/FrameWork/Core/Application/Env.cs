using FrameWork;
using System;
using System.IO;
using UnityEngine;
using Core.Interface;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	Env.cs
	Author:		DaiLi.Ou

	Descriptions: Game Env.
*********************************************************************/
namespace Core
{
    public sealed class Env : IEnv
    {
        /// <summary>
        /// 调试等级
        /// </summary>
        public DebugLevels DebugLevel { get; private set; }

        /// <summary>
        /// 是否加密asset bundle
        /// </summary>
        public bool IsAssetCrypt { get; set; }

        /// <summary>
        /// 资源发布路径
        /// </summary>
        private string releasePath;

        /// <summary>
        /// 编译完成后发布AssetBundle的路径
        /// </summary>
        public string ReleasePath
        {
            get { return releasePath; }
        }

        /// <summary>
        /// 需要编译成AssetBundle的资源包路径
        /// </summary>
        private string resourcesBuildPath;

        /// <summary>
        /// 需要编译成AssetBundle的资源包路径
        /// </summary>
        public string ResourcesBuildPath
        {
            get { return resourcesBuildPath; }
        }

        /// <summary>
        /// 资源无需编译成资源包的路径
        /// </summary>
        private string resourcesNoBuildPath;

        /// <summary>
        /// 资源无需编译成资源包的路径
        /// </summary>
        public string ResourcesNoBuildPath
        {
            get { return resourcesNoBuildPath; }
        }

        /// <summary>
        /// 只可读不可写的文件存放路径(不能做热更新)
        /// </summary>
        public string StreamingAssetsPath
        {
            get { return UnityEngine.Application.streamingAssetsPath; }
        }

        /// <summary>
        /// 工程根目录
        /// </summary>
        public string DataPath
        {
            get { return UnityEngine.Application.dataPath; }
        }

        /// <summary>
        /// 可以更删改的文件路径
        /// </summary>
        public string PersistentDataPath
        {
            get
            {
                return UnityEngine.Application.persistentDataPath;
            }
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        private string assetPath;

        /// <summary>
        /// 系统资源路径
        /// <para>不同的调试等级下对应不同的资源路径</para>
        /// <para><c>DebugLevels.Prod</c> : 生产环境下将会为<c>Application.persistentDataPath</c>读写目录</para>
        /// <para><c>DebugLevels.Staging</c> : 仿真环境下将会为<c>StreamingAssets</c>文件夹</para>
        /// <para><c>DebugLevels.Dev</c> : 开发者环境下将会为<c>Application.dataPath</c>数据路径</para>
        /// <para>调试等级无论如何设置，脱离编辑器将自动使用<c>Application.persistentDataPath</c>读写目录</para>
        /// <para>如果开发者有手动设置资源路径，将使用开发者设置的路径</para>
        /// </summary>
        public string AssetPath
        {
            get
            {
                if (!string.IsNullOrEmpty(assetPath))
                {
                    return assetPath;
                }

#if UNITY_EDITOR
                switch (DebugLevel)
                {
                    case DebugLevels.Staging:
                        return UnityEngine.Application.dataPath + Path.AltDirectorySeparatorChar + "StreamingAssets" + Path.AltDirectorySeparatorChar + App.Env.PlatformToName(App.Env.SwitchPlatform);
                    case DebugLevels.Auto:
                    case DebugLevels.Develop:
                        return UnityEngine.Application.dataPath;
                    case DebugLevels.Product:
                        return UnityEngine.Application.persistentDataPath;
                }
#endif
                if (App.Env.Platform == RuntimePlatform.Android || App.Env.Platform == RuntimePlatform.IPhonePlayer)
                {
                    return UnityEngine.Application.persistentDataPath;
                }
                else if (App.Env.Platform == RuntimePlatform.WindowsPlayer)
                {
                    return UnityEngine.Application.streamingAssetsPath;
                }
                else
                {
                    return UnityEngine.Application.persistentDataPath;
                }
            }
        }

        /// <summary>
        /// 当前运行的平台(和编辑器所在平台有关)
        /// </summary>
        public RuntimePlatform Platform
        {
            get
            {
                return UnityEngine.Application.platform;
            }
        }

        /// <summary>
        /// 当前所选择的编译平台
        /// </summary>
        public RuntimePlatform SwitchPlatform
        {
            get
            {
#if UNITY_ANDROID
                return RuntimePlatform.Android;
#endif

#if UNITY_IOS
                return RuntimePlatform.IPhonePlayer;
#endif

#if UNITY_STANDALONE_WIN
                return RuntimePlatform.WindowsPlayer;
#endif

#if UNITY_STANDALONE_OSX
				 return RuntimePlatform.OSXPlayer;
#endif
                throw new Exception("Undefined Switch Platform");
            }
        }

        /// <summary>
        /// 将平台转为名字
        /// </summary>
        /// <param name="platform">平台名</param>
        /// <returns>转换后的名字</returns>
        public string PlatformToName(RuntimePlatform? platform = null)
        {
            if (platform == null)
            {
                platform = Platform;
            }
            switch (platform)
            {
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Win";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    throw new ArgumentException("Undefined Platform");
            }
        }

        /// <summary>
        /// 构造一个环境
        /// </summary>
        public Env()
        {
            SetDebugLevel(DebugLevels.Auto);
        }

        #region Config

#if UNITY_EDITOR
        /// <summary>
        /// 设定发布路径
        /// 设定资源路径，开发者设定的资源路径会覆盖默认的资源路径策略
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public void SetReleasePath(string path)
        {
            releasePath = path;
            if (string.IsNullOrEmpty(releasePath))
            {
                releasePath = Path.AltDirectorySeparatorChar + "StreamingAssets";
            }
            else
            {
                releasePath = Path.AltDirectorySeparatorChar + releasePath.Trim(Path.AltDirectorySeparatorChar);
            }
        }

        /// <summary>
        /// 设定资源编译路径
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public void SetResourcesBuildPath(string path)
        {
            resourcesBuildPath = path;
            if (string.IsNullOrEmpty(resourcesBuildPath))
            {
                resourcesBuildPath = Path.AltDirectorySeparatorChar + "Assets/AssetBundle";
            }
            else
            {
                resourcesBuildPath = Path.AltDirectorySeparatorChar + resourcesBuildPath.Trim(Path.AltDirectorySeparatorChar);
            }
        }

        /// <summary>
        /// 设定资源非编译路径
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public void SetResourcesNoBuildPath(string path)
        {
            resourcesNoBuildPath = path;
            if (string.IsNullOrEmpty(resourcesNoBuildPath))
            {
                resourcesNoBuildPath = Path.AltDirectorySeparatorChar + "ABAssets/NotAssetBundle";
            }
            else
            {
                resourcesNoBuildPath = Path.AltDirectorySeparatorChar + resourcesNoBuildPath.Trim(Path.AltDirectorySeparatorChar);
            }
        }
#endif

        /// <summary>
        /// 设定资源路径，开发者设定的资源路径会覆盖默认的资源路径策略
        /// </summary>
        /// <param name="path">路径</param>
        public void SetAssetPath(string path)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                assetPath = PersistentDataPath + Path.AltDirectorySeparatorChar + "Assets";
            }
            else
            {
                assetPath = PersistentDataPath + Path.AltDirectorySeparatorChar + assetPath.Trim(Path.AltDirectorySeparatorChar);
            }
        }

        /// <summary>
        /// 设定调试等级
        /// </summary>
        /// <param name="level">调试等级</param>
        public void SetDebugLevel(DebugLevels level)
        {
            DebugLevel = level;
        }
        #endregion
    }
}