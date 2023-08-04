
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	FightMap.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/
namespace GameEngine
{
    public class FightMap : MonoBehaviour
    {

        [SerializeField] Transform[] TeamBack;
        [SerializeField] Transform[] TeamFront;

        [SerializeField] Transform[] EnemyBack;
        [SerializeField] Transform[] EnemyFront;

        public Transform GetTeamBackByPos(int pos)
        {
            return TeamBack[pos];
        }

        public Transform GetTeamFrontByPos(int pos)
        {
            return TeamFront[pos];
        }

        public Transform GetEnemyBackByPos(int pos)
        {
            return EnemyBack[pos];
        }

        public Transform GetEnemyFrontByPos(int pos)
        {
            return EnemyFront[pos];
        }
    }
}