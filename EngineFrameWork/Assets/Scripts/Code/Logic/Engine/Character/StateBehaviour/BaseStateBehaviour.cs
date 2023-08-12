/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/10
	Filename: 	BaseStateBehaviour.cs
	Author:		DaiLi.Ou
	Descriptions: Character FSM Base Script.
*********************************************************************/

using UnityEngine;
namespace GameEngine
{
    public enum StateType
    {
        ATTACK = 0,             // 普通攻击
        SKILL = 1,              // 技能攻击
        DEFEND = 2,             // 防守
        BEATTACK = 3,           // 受击
        DIE = 4,                // 死亡
    }

    public class BaseStateInfo
    {
        public int TargetInstanceId { get; set; }

        public StateType StateType { get; set; }        // 同一行为执行索引下的，表现执行优先级。 如A 攻击B. C 保护B.
                                                        // 优先级从->低： 0 -> 999
    }

    public class BaseStateBehaviour : StateMachineBehaviour
    {
        protected Character owner = null;
        protected Transform transform = null;

        protected void Initialize(Animator animator)
        {
            if (owner == null)
            {
                owner = animator.GetComponentInParent<Character>();
                if (owner == null)
                {
                    CDebug.LogError("Can't find Character component on prefab.");
                }
                transform = owner.TransformSelf;
                OnInitialize();
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Initialize(animator);

            OnLocalStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Initialize(animator);

            OnLocalStateExit(animator, stateInfo, layerIndex);
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            Initialize(animator);

            OnLocalStateMachineEnter(animator, stateMachinePathHash);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            Initialize(animator);

            OnLocalStateMachineExit(animator, stateMachinePathHash);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Initialize(animator);

            OnLocalStateMove(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Initialize(animator);

            OnLocalStateUpdate(animator, stateInfo, layerIndex);
        }

        public virtual void OnInitialize() { }

        #region Sync State Callback

        public virtual void OnLocalStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnLocalStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnLocalStateMachineEnter(Animator animator, int stateMachinePathHash) { }
        public virtual void OnLocalStateMachineExit(Animator animator, int stateMachinePathHash) { }
        public virtual void OnLocalStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnLocalStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        #endregion
    }
}