/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	CharacterLoadMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 角色加载管理类。用于加载进入的Game场景中的角色资源
*********************************************************************/
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /// 创建测试 NPC 及 角色
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateSceneMapCharacters()
        {
            if (modelMgr == null)
            {
                modelMgr = GlobalData.instance.characterModelMgr;
            }

            Dictionary<int, UserCharacter> needCreate = modelMgr.GetAllCharacterData();
            loadCount = needCreate.Count;

            foreach (var character in needCreate)
            {
                yield return GameMgr.Ins.LoadGameAssets(character.Value.data[1].ToString(), character.Value.baseCharacter.prefahPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    data.Initialized(character.Value);
                    modelMgr.UpdateCharacterByInstanceId(character.Value.instanceId, data);

                    int resId = character.Value.baseCharacter.resId;
                    data.GameObjectSelf.name = character.Value.isNpc ? string.Format("{0}_Npc", resId) : string.Format("{0}_Player", resId);
                    data.TransformSelf.GetComponentInChildren<SpriteRenderer>().sortingOrder = GlobalData.instance.sceneModelMgr.SceneMap.m_nHeight - (int)data.TransformSelf.position.y;
                    data.TransformSelf.SetParent(GlobalData.instance.sceneModelMgr.MapMgrObj.transform, false);

                    loadCount--;
                });
            }
        }

        public IEnumerator CreateFightCharacters()
        {
            loadedFight = false;

            if (fightModelMgr == null)
            {
                fightModelMgr = GlobalData.instance.fightModelMgr;
            }


            Dictionary<int, UserCharacter> teamCharacters = fightModelMgr.GetFightTeamCharacters();
            Dictionary<int, UserCharacter> enemyCharacters = fightModelMgr.GetFightEnemyCharacters();
            var sceneModelMgr = GlobalData.instance.sceneModelMgr;

            loadCount = teamCharacters.Count;
            foreach (var character in teamCharacters)
            {
                yield return GameMgr.Ins.LoadGameAssets(character.Value.data[1].ToString(), character.Value.baseCharacter.prefahPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    data.Initialized(character.Value);


                    int resId = character.Value.baseCharacter.resId;
                    data.GameObjectSelf.name = resId.ToString();


                    var pos = data.Property.UserCharacter.pos;
                    UnityEngine.Transform parent = null;
                    switch (data.Property.UserCharacter.frontBack)
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

                    data.TransformSelf.SetParent(parent, false);

                    loadCount--;
                });
            }

            while (loadCount > 0) { yield return Yielders.EndOfFrame; }

            // load enemy. todo opt.
            loadCount = enemyCharacters.Count;
            foreach (var character in enemyCharacters)
            {
                yield return GameMgr.Ins.LoadGameAssets(character.Value.data[1].ToString(), character.Value.baseCharacter.prefahPath, (d) =>
                {
                    var data = d.GetComponent<Character>();
                    data.Initialized(character.Value);

                    int resId = character.Value.baseCharacter.resId;
                    data.GameObjectSelf.name = resId.ToString();
                    var pos = data.Property.UserCharacter.pos;

                    UnityEngine.Transform parent = null;
                    switch (data.Property.UserCharacter.frontBack)
                    {
                        case 0:
                            parent = sceneModelMgr.FightMap.GetEnemyFrontByPos(pos);
                            data.UpdateSortLayer(GameConfig.FightEnemyCharacter_Front_OrderInLayer);
                            break;
                        case 1:
                            parent = sceneModelMgr.FightMap.GetEnemyBackByPos(pos);
                            data.UpdateSortLayer(GameConfig.FightEnemyCharacter_Back_OrderInLayer);
                            break;
                        default:
                            break;
                    }

                    data.TransformSelf.SetParent(parent, false);

                    loadCount--;
                });
            }
            while (loadCount > 0) { yield return Yielders.EndOfFrame; }

            loadedFight = true;
        }

        public bool LoadedAllSceneCharacters()
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
