using System;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class UIConfig
    {
        /// <summary>
        /// 所属文件夹名
        /// </summary>
        public string floaderName = string.Empty;
        /// <summary>
        /// 需要打开的ui预制体名
        /// </summary>
        public string prefabName = string.Empty;
        /// <summary>
        /// 只显示当前界面，其他界面全部隐藏
        /// </summary>
        public bool hideAllBefore = true;
        /// <summary>
        /// 是否全屏界面
        /// </summary>
        public bool fullScreen = true;

        public Transform transform = null;

        public UIConfig() { }

        public override string ToString()
        {
            return $"{floaderName}/{prefabName}, fullScreen: {fullScreen}, hideAllBefore: {hideAllBefore}" ;
        }
    }
}
