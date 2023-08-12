using System;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/07
	Filename: 	SkillProperty.cs
	Author:		DaiLi.Ou
	Descriptions: 角色技能属性
*********************************************************************/
namespace GameEngine
{
    [Serializable]
    public class SkillProperty : ISerializationCallbackReceiver, IDisposable
    {
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
