
using System;
using UnityEngine;
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/07
	Filename: 	TeamProperty.cs
	Author:		DaiLi.Ou
	Descriptions: 角色队伍属性
*********************************************************************/
namespace GameEngine
{
    [Serializable]
    public class TeamProperty : ISerializationCallbackReceiver, IDisposable
    {

        // Map public variables
        public bool IsLeader { get; set; }              // is team leader,on map scene.
        public bool MapTeamPos { get; set; }            // team pos,on mao scene.
                                                        // other propertys ...

        // Fight public variables
        public int FrontOrBack { get; set; }            // 0: 前排 1: 后排
        public int FightTeamPos { get; set; }           // 从左到右位置

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
