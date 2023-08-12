using Model;
using System;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/07
	Filename: 	CharacterProperty.cs
	Author:		DaiLi.Ou
	Descriptions: 角色属性基类
*********************************************************************/

namespace GameEngine
{
    public enum CharacterType
    {
        MAP_NPC = 0,                     // npc
        MAP_HERO = 1,                    // 当前玩家
        MAP_PLAYER = 2,                  // 其他c端玩家

        FIGHT_HERO = 3,                  // 战斗玩家
        // others
    }

    [Serializable]
    public class CharacterProperty : ISerializationCallbackReceiver, IDisposable
    {

        public CharacterType CharacterType { get; set; }            // 角色类型

        public MonoProperty MonoProperty { get; set; }              // 角色组件属性
        public TeamProperty TeamProperty { get; set; }              // 角色队伍属性
        public SkillProperty SkillProperty { get; set; }            // 角色技能属性
        public ConfigProperty ConfigProperty { get; set; }          // 角色配置属性

        /////////// Public Geter/Seter ///////////
        public bool IsNpc
        {
            get => CharacterType.Equals(CharacterType.MAP_NPC);
        }

        public bool IsMapHero
        {
            get => CharacterType.Equals(CharacterType.MAP_HERO);
        }

        public bool IsFightHero
        {
            get => CharacterType.Equals(CharacterType.FIGHT_HERO);
        }

        /////////// Public Methods ///////////
        public void Dispose()
        {
            if (MonoProperty != null)
            {
                MonoProperty.Dispose();
            }
            if (SkillProperty != null)
            {
                SkillProperty.Dispose();
            }
            if (ConfigProperty != null)
            {
                ConfigProperty.Dispose();
            }
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }

    }
}
