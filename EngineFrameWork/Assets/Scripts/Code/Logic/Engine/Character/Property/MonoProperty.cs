using System;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/07
	Filename: 	MonoProperty.cs
	Author:		DaiLi.Ou
	Descriptions: 角色MonoBehaviour组件属性
*********************************************************************/

namespace GameEngine
{
    [Serializable]
    public class MonoProperty : ISerializationCallbackReceiver, IDisposable
    {
        // Public Properties
        public int Dir { get; set; }                                  // 角色朝向
        public Vector2 Pos { get; set; }                              // 角色位置

        public float Speed { get; set; }                              // 移动速度
        public int SortOrder { get; set; }                            // 排序层级
        public string MonoName { get; set; }                          // 角色Mono名称
        public string ActionName { get; set; }                        // 角色动作名称

        // Public Properties for fight
        public Vector3 FightCreatePos { get; set; }                   // 初始创建世界坐标位置

        // Public Variables with default value.
        public HeadFlagCfg HeadFlag = HeadFlagCfg.DEFAULT_NONE;       // 头顶标记


        // other propertys ...
        public void Dispose()
        {
            // 引用类型 置空
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }

    }
}
