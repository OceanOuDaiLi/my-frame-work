using UnityEngine;
using System.Collections.Generic;

namespace Model
{
    public class FightModelMgr : BaseModelMgr
    {

        string curFightMapPath;
        Transform curFightMapTrans;

        Dictionary<int, UserCharacter> teamCharcters;
        Dictionary<int, UserCharacter> enemyCharcters;

        #region  Ctor
        public FightModelMgr() : base()
        {
            OnRegister();
        }

        private void OnRegister()
        {
            //dispatcher.AddListener("Proto.Login.RspRegisterAccount", UpdateCharacterData);
        }

        private void OnRemove()
        {
            //  (工具动态生成)
            //dispatcher.RemoveListener("Proto.Login.RspRegisterAccount", UpdateCharacterData);
        }
        #endregion

        private void AddTestTeamCharacters()
        {
            int _pos = 0;
            int len = 10;
            for (int i = 0; i < len; i++)
            {
                //0 down,4up
                _pos = i >= 5 ? len - i - 1 : i;
                teamCharcters[100 + i] = new UserCharacter
                {
                    instanceId = 100 + i,
                    data = new List<object>() { new Vector2(0, 0), "1001", 4, "龙傲天" },
                    isNpc = true,
                    frontBack = i < 5 ? 0 : 1,
                    pos = _pos,
                    baseCharacter = new BaseCharacter
                    {
                        resId = 1001,
                        prefahPath = "characters/1001/prefab/1001"
                    }
                };

            }
        }

        private void AddTestEnemyCharacters()
        {
            int _pos = 0;
            int len = 10;
            for (int i = 0; i < len; i++)
            {
                //0 down,4up
                _pos = i >= 5 ? len - i - 1 : i;
                enemyCharcters[100 + i] = new UserCharacter
                {
                    instanceId = 100 + i,
                    data = new List<object>() { new Vector2(0, 0), i % 3 == 0 ? "1002" : "1001", 0, "龙傲天" },
                    isNpc = true,
                    frontBack = i < 5 ? 0 : 1,
                    pos = _pos,
                    baseCharacter = new BaseCharacter
                    {
                        resId = i % 3 == 0 ? 1002 : 1001,
                        prefahPath = i % 3 == 0 ? "characters/1002/prefab/1002" : "characters/1001/prefab/1001"
                    }
                };
            }
        }


        public void SetDemoFightData()
        {
            // todo:
            // 1.server request,this fight.  websocket.send(xxx).
            // 2.on receive message, combine data and fight.

            // [1]. add test character data.
            teamCharcters = new Dictionary<int, UserCharacter>();
            enemyCharcters = new Dictionary<int, UserCharacter>();
            AddTestTeamCharacters();
            AddTestEnemyCharacters();

            // [2]. add test map data.
            curFightMapPath = "1005";
        }

        public string GetCurFightMapPath()
        {
            return curFightMapPath;
        }

        public void SetCurFightMap(Transform tar)
        {
            curFightMapTrans = tar;
        }

        public Transform GetCurFightMap()
        {
            return curFightMapTrans;
        }

        public Dictionary<int, UserCharacter> GetFightTeamCharacters()
        {
            return teamCharcters;
        }

        public Dictionary<int, UserCharacter> GetFightEnemyCharacters()
        {
            return enemyCharcters;
        }

        public void OnFightEnd()
        {

        }

        public override void OnDispose()
        {
            OnRemove();
            base.OnDispose();
        }
    }
}