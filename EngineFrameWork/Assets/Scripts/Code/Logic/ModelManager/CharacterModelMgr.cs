
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	CharacterMgr.cs
	Author:		shenma
	Descriptions: 
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
using GameEngine;

namespace Model
{
    public class CharacterModelMgr : BaseModelMgr
    {
        /// <summary>
        /// int: 示例id
        /// 测试模拟服务器数据 服务器ID 位置  资源ID 朝向 名字 是否是主角等。
        /// </summary>
        Dictionary<int, BaseCharacter> allUserCharacters = null;

        public Character ChooseTarget { get; set; }

        public CharacterModelMgr() : base()
        {
            OnRegister();

            if (allUserCharacters != null) { allUserCharacters.Clear(); }
            allUserCharacters = new Dictionary<int, BaseCharacter>();

            CharacterProperty property;
            // tips: This is test code for add test data..
            allUserCharacters[101] = new HeroCharacter();
            allUserCharacters[101].InstanceId = 101;
            property = new CharacterProperty()
            {
                ConfigProperty = new ConfigProperty
                {
                    name = "龙傲天",
                    resId = 1001,
                    prefabPath = "characters/1001/prefab/1001"
                },
                MonoProperty = new MonoProperty
                {
                    Pos = new Vector2(3152, 700),
                    Dir = 0
                }
            };
            allUserCharacters[101].Init(property);

            allUserCharacters[50001] = new NpcCharacter();
            allUserCharacters[50001].InstanceId = 50001;
            property = new CharacterProperty()
            {
                CharacterType = CharacterType.MAP_NPC,
                ConfigProperty = new ConfigProperty
                {
                    name = "武器商人",
                    resId = 1002,
                    prefabPath = "characters/1002/prefab/1002"

                },
                MonoProperty = new MonoProperty
                {
                    Pos = new Vector2(2896, 976),
                    Dir = 0,
                    HeadFlag = HeadFlagCfg.TASK_FLAG_AVALIABLE,
                }
            };
            allUserCharacters[50001].Init(property);

            allUserCharacters[50002] = new NpcCharacter();
            allUserCharacters[50002].InstanceId = 50002;
            property = new CharacterProperty()
            {
                CharacterType = CharacterType.MAP_NPC,
                ConfigProperty = new ConfigProperty
                {
                    name = "云游道长",
                    resId = 1002,
                    prefabPath = "characters/1002/prefab/1002"
                },
                MonoProperty = new MonoProperty
                {
                    Pos = new Vector2(3152, 208),
                    Dir = 0,
                    HeadFlag = HeadFlagCfg.TASK_FLAG_COMPLETE,
                }
            };
            allUserCharacters[50002].Init(property);
        }

        #region Auto Event Binding.
        private void OnRegister()
        {
            //  (工具动态生成)
            //dispatcher.AddListener("Proto.Login.RspRegisterAccount", UpdateCharacterData);
        }

        private void OnRemove()
        {
            //  (工具动态生成)
            //dispatcher.RemoveListener("Proto.Login.RspRegisterAccount", UpdateCharacterData);

        }
        #endregion

        #region Methods for create characters.

        public Dictionary<int, BaseCharacter> GetAllCharacterData()
        {
            return allUserCharacters;
        }

        public void UpdateCharacterByInstanceId(int id, Character character)
        {
            if (!allUserCharacters.ContainsKey(id)) { CDebug.LogError($"Update Character failed.Can't find instanceId {id}"); };

            allUserCharacters[id].Character = character;
        }

        #endregion

        #region Methods for get/set characters.

        public HeroCharacter GetMapPlayerCharacter()
        {
            HeroCharacter character = null;
            foreach (var item in allUserCharacters)
            {
                if (item.Value.Property.IsMapHero)
                {
                    character = item.Value as HeroCharacter;
                    break;
                }
            }

            return character;
        }

        public BaseCharacter GetMapCharacterByInstanceId(int id)
        {
            if (!allUserCharacters.ContainsKey(id))
            {
                CDebug.LogError($"Update Character failed.Can't find instanceId {id}");
                return null;
            };

            return allUserCharacters[id];
        }

        #endregion

        public override void OnDispose()
        {
            OnRemove();
            base.OnDispose();

            if (allUserCharacters != null) { allUserCharacters.Clear(); }
            allUserCharacters = null;

            // todo: unload CreatedCharacters
        }

        public void SetChooseCharacter(Character ins)
        {
            if (ChooseTarget != null)
                ChooseTarget.SetChooseFlag(false);
            ChooseTarget = ins;

            if (ChooseTarget != null)
                ChooseTarget.SetChooseFlag(true);
        }
    }
}