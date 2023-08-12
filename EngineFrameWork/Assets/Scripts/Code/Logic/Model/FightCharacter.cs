using GameEngine;
using Core.Interface.Event;
using FrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class FightCharacter : BaseCharacter
    {
        IEventHandler eventHandlerExcuteBehaviour = null;
        Queue<BaseStateInfo> actionQueue = new Queue<BaseStateInfo>();

        public override void OnInit()
        {
            base.OnInit();
            Property.CharacterType = CharacterType.FIGHT_HERO;
            // eventHandlerExcuteBehaviour = App.Instance.On(FightEvent.EXCUTE_FIGHT_STATE_BEHAVIOUR, ExcuteBehaviour);
        }

        public override void OnCreateMono(Character ins)
        {
            base.OnCreateMono(ins);

            Character.Property.MonoProperty.Pos = Vector2.zero;
            Character.TransformSelf.localPosition = Vector3.zero;
        }

        public override void OnDispose()
        {
            // 引用类型置空
            base.OnDispose();

            //if (eventHandlerExcuteBehaviour != null) { eventHandlerExcuteBehaviour.Cancel(); }
            //eventHandlerExcuteBehaviour = null;
        }

        //public void ExcuteBehaviour(object sender, EventArgs e)
        public void ExcuteBehaviour(FightDirector fightDirector)
        {

            var action = fightDirector.BehaviourQueue.Dequeue();


            // CDebug.LogProgress($" InstanceId: {fightDirector.InstanceId}  ActionIdx:{action.StateType.ToString()} ");


            switch (action.StateType)
            {
                case StateType.ATTACK:
                    var enemyFigth = GlobalData.instance.fightModelMgr.GetFighCharacterByInstanceId(action.TargetInstanceId);
                    Character.Enemy = enemyFigth.Character;
                    CDebug.FightLog($" ### {InstanceId} 进入追击 目标： {enemyFigth.InstanceId} ###");
                    Character.SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_CHASEING);
                    break;
                case StateType.SKILL:
                    break;
                case StateType.DEFEND:
                    Character.OnDefend();
                    CDebug.FightLog($" ### {InstanceId} 进入防守 ###");
                    break;
                case StateType.BEATTACK:
                    break;
                case StateType.DIE:
                    break;
                default:
                    break;
            }

        }

        private void ExcuteAction(BaseStateInfo action)
        {

        }
    }
}
