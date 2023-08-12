using Model;
using FrameWork;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	FightControl.cs
	Author:		DaiLi.Ou
	Descriptions: 战斗控制抽象类。
*********************************************************************/
namespace GameEngine
{
    public enum FightState
    {
        Initialize,         //初始化
        Start,              //游戏开始
        RoundStart,         //回合开始，进入倒计时。
        Win,                //战斗胜利
        Lost,               //战斗失败
        End                 //战斗结束
    }

    public class FightDirector
    {
        // todo:
        // 1. 并行执行支持。
        //      不同角色BehaviourDic，执行索引一致。执行不同行为，执行有先后。
        // 2. 回合Director可视化 & 回放支持。
        //      Editor策划自行编辑当前回合所有单位的执行行为，并运行。  NotNecessary.
        //      Runtime时，实时预览当前回合所有行为的FSM树状图。
        //      时间线可视化 + FSM 树状图可视化 + 行为节点轨道可视化。
        //      回放支持：指令记录

        public int ActionIdx = -1;                                         // 出手顺序
        public int RoundIdx { get; set; }                                  // 回合信息.
        public int InstanceId { get; set; }                                // 执行行为角色实例ID.
        public Queue<BaseStateInfo> BehaviourQueue { get; set; }           // 当前回合执行的行为.  behaviour: 对应FSM的行为及行为执行优先级。

    }

    public class FightControl : MonoSingleton<FightControl>
    {
        private FightState state;
        public FightState State
        {
            get => state;
            set
            {
                state = value;
                CDebug.FightLog($"Fihgt State Change to {state}");
                App.Instance.Trigger(FightEvent.FIGHT_EVENT_GAME_FIGHT_STATE_CHANGE, state);
            }
        }

        private FightModelMgr modelMgr;
        private int curActionIdx = 0;
        public List<FightDirector> CurRoundDirectors = new List<FightDirector>();

        /// <summary>
        ///   组装当前的剧本信息。
        ///   Editor基于此组装信息。开发可视化面板。
        ///   回放记录此组装信息，进行回放功能读取和播放。
        /// </summary>
        public void PackRoundDirectors()
        {
            CurRoundDirectors.Sort((a, b) => { return a.ActionIdx > b.ActionIdx ? 1 : 0; });

            ExcuteRoundDirectors();
        }

        public void ExcuteRoundDirectors()
        {
            //App.Instance.Trigger(FightEvent.EXCUTE_FIGHT_STATE_BEHAVIOUR, ExcuteDirectors.Dequeue());

            var curActions = CurRoundDirectors.FindAll((x) => { return x.ActionIdx.Equals(curActionIdx); });
            if (curActions.Count < 1)
            {
                // round end.
                CDebug.FightLog("###   round end.   ###");
                return;
            }

            foreach (var director in curActions)
            {
                if (director.ActionIdx != -1)
                {
                    var chara = (FightCharacter)modelMgr.GetFighCharacterByInstanceId(director.InstanceId);
                    chara.ExcuteBehaviour(director);
                }
            }

            curActionIdx++;
        }

        /// <summary>
        ///  Demos数据
        ///  模拟服务器
        ///  组装一回合简单的对位或随机普通攻击FightDirector。
        /// </summary>
        public void DemoSimulateServerNormalAttack()
        {
            CurRoundDirectors = new List<FightDirector>();
            if (modelMgr == null) { modelMgr = GlobalData.instance.fightModelMgr; }

            Dictionary<int, BaseCharacter> teamCharacters = modelMgr.GetFightTeamCharacters();
            Dictionary<int, BaseCharacter> enemyCharacters = modelMgr.GetFightEnemyCharacters();


            // Simulate server users' team directors.
            int roundIdx = 1;
            int actionIdx = 0;
            foreach (var tar in teamCharacters)
            {
                var baseChara = tar.Value;
                FightDirector tmp = new FightDirector();
                tmp.RoundIdx = roundIdx;
                tmp.InstanceId = tar.Key;
                tmp.ActionIdx = actionIdx;

                var stateInfo = new BaseStateInfo();
                // 设置对位同排同位置的为敌人
                int pos = baseChara.Property.TeamProperty.FightTeamPos;
                int frontBack = baseChara.Property.TeamProperty.FrontOrBack;
                stateInfo.TargetInstanceId = modelMgr.GetEnemyCharacterByPos(pos, frontBack).InstanceId;
                // 设置当前行为优先级
                stateInfo.StateType = StateType.ATTACK;

                tmp.BehaviourQueue = new Queue<BaseStateInfo>();                             // 测试数据只执行一次行为。可执行多次。如：连击，或 普攻触发技能。
                tmp.BehaviourQueue.Enqueue(stateInfo);

                CurRoundDirectors.Add(tmp);
                actionIdx++;
            }

            actionIdx = 0;
            foreach (var tar in enemyCharacters)
            {
                var baseChara = tar.Value;
                int pos = baseChara.Property.TeamProperty.FightTeamPos;

                FightDirector tmp = new FightDirector();
                tmp.RoundIdx = roundIdx;
                tmp.InstanceId = tar.Key;
                tmp.ActionIdx = actionIdx;

                var stateInfo = new BaseStateInfo();
                stateInfo.StateType = StateType.DEFEND;

                tmp.BehaviourQueue = new Queue<BaseStateInfo>();
                tmp.BehaviourQueue.Enqueue(stateInfo);

                CurRoundDirectors.Add(tmp);

                actionIdx++;
            }

            PackRoundDirectors();
        }

        /// <summary>
        ///  Demo数据
        ///  模拟服务器
        ///  组装一回合简单的技能攻击FightDirector。
        /// </summary>
        public void DemoSimulateServerSkill()
        {

        }
    }
}