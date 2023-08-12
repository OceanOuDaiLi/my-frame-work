using GameEngine;
using UnityEngine;
using System.Collections.Generic;

namespace Model
{
    public class FightModelMgr : BaseModelMgr
    {

        string curFightMapPath;
        Transform curFightMapTrans;

        Dictionary<int, BaseCharacter> teamCharcters;           // int: instance id.
        Dictionary<int, BaseCharacter> enemyCharcters;

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

        #region Geter/Seter

        public string GetCurFightMapPath
        {
            get => curFightMapPath;
        }
        public Transform GetCurFightMap
        {
            get => curFightMapTrans;
        }

        #endregion

        private void AddTestFightTeamCharacters()
        {
            int _pos = 0;
            int len = 10;
            for (int i = 0; i < len; i++)
            {
                //0 down,4up
                _pos = i >= 5 ? len - i - 1 : i;

                int instanceID = 200 + i;

                teamCharcters[instanceID] = new FightCharacter();
                teamCharcters[instanceID].InstanceId = instanceID;
                CharacterProperty Property = new CharacterProperty
                {
                    ConfigProperty = new ConfigProperty
                    {
                        name = "龙傲天",
                        resId = i % 3 == 0 ? 1002 : 1001,
                        prefabPath = i % 3 == 0 ? "characters/1002/prefab/1002" : "characters/1001/prefab/1001"
                    },
                    MonoProperty = new MonoProperty
                    {
                        Pos = new Vector2(0, 0),
                        Dir = 4
                    },
                    TeamProperty = new TeamProperty
                    {
                        FrontOrBack = i < 5 ? 0 : 1,
                        FightTeamPos = _pos
                    }
                };

                teamCharcters[instanceID].Init(Property);
            }
        }

        private void AddTestFightEnemyCharacters()
        {
            int _pos = 0;
            int len = 10;
            for (int i = 0; i < len; i++)
            {
                //0 down,4up
                _pos = i >= 5 ? len - i - 1 : i;
                int instanceID = 1000 + i;
                enemyCharcters[instanceID] = new FightCharacter();
                enemyCharcters[instanceID].InstanceId = instanceID;
                CharacterProperty Property = new CharacterProperty()
                {
                    ConfigProperty = new ConfigProperty
                    {
                        name = "龙傲天",
                        resId = i % 3 == 0 ? 1002 : 1001,
                        prefabPath = i % 3 == 0 ? "characters/1002/prefab/1002" : "characters/1001/prefab/1001"
                    },
                    MonoProperty = new MonoProperty
                    {
                        Pos = new Vector2(0, 0),
                        Dir = 0
                    },
                    TeamProperty = new TeamProperty
                    {
                        FrontOrBack = i < 5 ? 0 : 1,
                        FightTeamPos = _pos
                    }
                };
                enemyCharcters[instanceID].Init(Property);
            }
        }

        public void SetDemoFightData()
        {
            // todo:
            // 1.server request,this fight.  websocket.send(xxx).
            // 2.on receive message, combine data and fight.

            // [1]. add test character data.
            teamCharcters = new Dictionary<int, BaseCharacter>();
            enemyCharcters = new Dictionary<int, BaseCharacter>();
            AddTestFightTeamCharacters();
            AddTestFightEnemyCharacters();

            // [2]. add test map data.
            curFightMapPath = "1005";
        }

        public void SetCurFightMap(Transform tar)
        {
            curFightMapTrans = tar;
        }

        public Dictionary<int, BaseCharacter> GetFightTeamCharacters()
        {
            return teamCharcters;
        }

        public Dictionary<int, BaseCharacter> GetFightEnemyCharacters()
        {
            return enemyCharcters;
        }

        public BaseCharacter GetFighCharacterByInstanceId(int instanceId)
        {
            BaseCharacter target = null;

            if (!teamCharcters.TryGetValue(instanceId, out target))
            {
                if (!enemyCharcters.TryGetValue(instanceId, out target))
                {
                    CDebug.LogError($"Can't find enemy and team character,which instanceId: {instanceId}");
                }
            }

            return target;
        }

        #region Get Characters Methids
        public BaseCharacter GetTeamCharacterByPos(int pos)
        {
            BaseCharacter tar = null;
            foreach (var item in teamCharcters)
            {
                if (item.Value.Property.TeamProperty.FightTeamPos.Equals(pos))
                {
                    tar = item.Value;
                    break;
                }
            }

            return tar;
        }

        public BaseCharacter GetEnemyCharacterByPos(int pos, int frontOrBack)
        {
            BaseCharacter tar = null;
            foreach (var item in enemyCharcters)
            {
                var teamProperty = item.Value.Property.TeamProperty;
                if (teamProperty.FightTeamPos.Equals(pos) && teamProperty.FrontOrBack == frontOrBack)
                {
                    tar = item.Value;
                    break;
                }
            }

            return tar;
        }


        #endregion

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