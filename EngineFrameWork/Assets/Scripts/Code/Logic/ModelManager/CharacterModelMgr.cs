
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
        Dictionary<int, UserCharacter> allUserCharacters = null;

        public CharacterModelMgr() : base()
        {
            OnRegister();

            if (allUserCharacters != null) { allUserCharacters.Clear(); }
            allUserCharacters = new Dictionary<int, UserCharacter>();

            // tips: This is test code for add test data..
            allUserCharacters[101] = new UserCharacter
            {
                instanceId = 101,
                data = new List<object>() { new Vector2(3152, 700), "1001", 0, "龙傲天" },
                isNpc = false,
                baseCharacter = new BaseCharacter
                {
                    resId = 1001,
                    prefahPath = "characters/1001/prefab/1001"
                }
            };
            allUserCharacters[50001] = new UserCharacter
            {
                instanceId = 50001,
                data = new List<object> { new Vector2(2896, 976), "1001", 0, "武器商人" },
                isNpc = true,
                baseCharacter = new BaseCharacter
                {
                    resId = 1002,
                    prefahPath = "characters/1002/prefab/1002"
                }
            };
            allUserCharacters[50002] = new UserCharacter
            {
                instanceId = 50002,
                data = new List<object> { new Vector2(3152, 208), "1001", 0, "云游道长" },
                isNpc = true,
                baseCharacter = new BaseCharacter
                {
                    resId = 1002,
                    prefahPath = "characters/1002/prefab/1002"
                }
            };
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

        public Dictionary<int, UserCharacter> GetAllCharacterData()
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

        public Character GetPlayerCharacter()
        {
            Character character = null;
            foreach (var item in allUserCharacters)
            {
                if (item.Value.isNpc == false)
                {
                    character = item.Value.Character;
                    break;
                }
            }

            return character;
        }

        public UserCharacter GetUserCharacterByInstanceId(int id)
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
    }
}