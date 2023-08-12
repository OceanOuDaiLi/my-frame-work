/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	CharacterLoadMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 角色加载管理类。用于加载进入的Game场景中的角色资源
*********************************************************************/
using Model;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameEngine
{
    public class CharacterLoadMgr
    {
        // test code.
        int loadCount = 0;
        bool loadedFight = false;

        private CharacterModelMgr modelMgr = null;
        private FightModelMgr fightModelMgr = null;

        /// <summary>
        /// Game场景
        /// 创建NPC及角色
        /// 使用测试数据
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateSceneMapCharacters()
        {
#warning need opt code. using character pool. & IEnumerator foreach => property tocorrect.

            if (modelMgr == null)
            {
                modelMgr = GlobalData.instance.characterModelMgr;
            }

            Dictionary<int, BaseCharacter> needCreate = modelMgr.GetAllCharacterData();
            loadCount = needCreate.Count;

            foreach (var character in needCreate)
            {
                var insCharacter = character.Value;
                int resId = insCharacter.GetResID();
                string szPath = insCharacter.GetResPath();
                yield return GameMgr.Ins.LoadGameAssets(resId.ToString(), szPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    modelMgr.UpdateCharacterByInstanceId(insCharacter.InstanceId, data);

                    insCharacter.SetCharacter(data);
                    insCharacter.SetParent(GlobalData.instance.sceneModelMgr.MapMgrObj.transform, false);

                    insCharacter.OnCreateMono(data);
                    loadCount--;
                });
            }
        }

        /// <summary>
        /// 战斗
        /// 创建战斗角色
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateFightCharacters()
        {
            loadedFight = false;

            if (fightModelMgr == null)
            {
                fightModelMgr = GlobalData.instance.fightModelMgr;
            }


            Dictionary<int, BaseCharacter> teamCharacters = fightModelMgr.GetFightTeamCharacters();
            Dictionary<int, BaseCharacter> enemyCharacters = fightModelMgr.GetFightEnemyCharacters();
            var sceneModelMgr = GlobalData.instance.sceneModelMgr;

            loadCount = teamCharacters.Count;
            foreach (var character in teamCharacters)
            {
                var property = character.Value.Property;
                var teamProperty = property.TeamProperty;
                var cfgProperty = property.ConfigProperty;
                yield return GameMgr.Ins.LoadGameAssets(cfgProperty.resId.ToString(), cfgProperty.prefabPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    data.RefreshData(property);


                    int resId = cfgProperty.resId;
                    data.GameObjectSelf.name = resId.ToString();

                    var pos = teamProperty.FightTeamPos;
                    UnityEngine.Transform parent = null;
                    switch (teamProperty.FrontOrBack)
                    {
                        case 0:
                            parent = sceneModelMgr.FightMap.GetTeamFrontByPos(pos);
                            data.UpdateSortLayer(GameConfig.FightTeamCharacter_Front_OrderInLayer);
                            break;
                        case 1:
                            parent = sceneModelMgr.FightMap.GetTeamBackByPos(pos);
                            data.UpdateSortLayer(GameConfig.FightTeamCharacter_Back_OrderInLayer);
                            break;
                        default:
                            break;
                    }

                    // need to cleaning code . add by daili.ou
                    data.TransformSelf.SetParent(parent, false);
                    character.Value.OnCreateMono(data);
                    data.SetMonoName(character.Key + "_TeamPlayer");
                    data.Property.MonoProperty.FightCreatePos = data.TransformSelf.position;
                    loadCount--;
                });
            }

            while (loadCount > 0) { yield return Yielders.EndOfFrame; }

            // load enemy. todo opt.
            loadCount = enemyCharacters.Count;
            foreach (var character in enemyCharacters)
            {
                var property = character.Value.Property;
                var monoProperty = property.MonoProperty;
                var teamProperty = property.TeamProperty;
                var cfgProperty = property.ConfigProperty;
                yield return GameMgr.Ins.LoadGameAssets(cfgProperty.resId.ToString(), cfgProperty.prefabPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    data.RefreshData(property);

                    int resId = cfgProperty.resId;
                    data.GameObjectSelf.name = resId.ToString();

                    UnityEngine.Transform parent = null;
                    switch (teamProperty.FrontOrBack)
                    {
                        case 0:
                            parent = sceneModelMgr.FightMap.GetEnemyFrontByPos(teamProperty.FightTeamPos);
                            data.UpdateSortLayer(GameConfig.FightEnemyCharacter_Front_OrderInLayer);
                            break;
                        case 1:
                            parent = sceneModelMgr.FightMap.GetEnemyBackByPos(teamProperty.FightTeamPos);
                            data.UpdateSortLayer(GameConfig.FightEnemyCharacter_Back_OrderInLayer);
                            break;
                        default:
                            break;
                    }

                    // need to cleaning code . add by daili.ou
                    data.TransformSelf.SetParent(parent, false);
                    character.Value.OnCreateMono(data);
                    data.SetMonoName(character.Key + "_EnemyPlayer");
                    data.Property.MonoProperty.FightCreatePos = data.TransformSelf.position;
                    loadCount--;
                });
            }
            while (loadCount > 0) { yield return Yielders.EndOfFrame; }

            loadedFight = true;
        }

        public bool LoadedSceneCharacters()
        {
            bool loaded = loadCount == 0;
            return loaded;
        }

        public bool LoadedFightCharacters()
        {
            return loadedFight;
        }

        public void OnDispose()
        {
            modelMgr = null;
        }
    }
}
