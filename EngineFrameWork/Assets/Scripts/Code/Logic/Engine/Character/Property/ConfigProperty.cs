using System;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/07
	Filename: 	ConfigProperty.cs
	Author:		DaiLi.Ou
	Descriptions: 角色配置表属性
*********************************************************************/
namespace GameEngine
{
    [Serializable]
    public class ConfigProperty : ISerializationCallbackReceiver, IDisposable
    {
        public int resId { get; set; }                      // 资源id
        public string name { get; set; }                    // 角色名称
        public string prefabPath { get; set; }              // 资源加载路径

        public void Dispose()
        {
            // 引用类型 置空
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }
    }
}
