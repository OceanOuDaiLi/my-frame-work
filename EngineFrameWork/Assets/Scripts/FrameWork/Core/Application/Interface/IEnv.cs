﻿
using UnityEngine;

namespace Core.Interface
{
    /// <summary>
    /// 环境
    /// </summary>
    public interface IEnv
    {
        /// <summary>
        /// 调试等级
        /// </summary>
        DebugLevels DebugLevel { get; }

        /// <summary>
        /// 系统资源路径
        /// <para>不同的调试等级下对应不同的资源路径</para>
        /// <para><c>DebugLevels.Prod</c> : 生产环境下将会为<c>Application.persistentDataPath</c>读写目录</para>
        /// <para><c>DebugLevels.Staging</c> : 仿真环境下将会为<c>StreamingAssets</c>文件夹</para>
        /// <para><c>DebugLevels.Dev</c> : 开发者环境下将会为<c>Application.dataPath</c>数据路径</para>
        /// <para>调试等级无论如何设置，脱离编辑器将自动使用<c>Application.persistentDataPath</c>读写目录</para>
        /// </summary>
        string AssetPath { get; }

        /// <summary>
        /// 设定资源路径，开发者设定的资源路径会覆盖默认的资源路径策略
        /// </summary>
        /// <param name="path">路径</param>
        void SetAssetPath(string path);

        /// <summary>
        /// 当前运行的平台(和编辑器所在平台有关)
        /// </summary>
        RuntimePlatform Platform { get; }

        /// <summary>
        /// 当前所选择的编译平台
        /// </summary>
        RuntimePlatform SwitchPlatform { get; }

        /// <summary>
        /// 将平台转为名字
        /// </summary>
        /// <param name="platform">平台名</param>
        /// <returns>转换后的名字</returns>
        string PlatformToName(RuntimePlatform? platform = null);
    }
}