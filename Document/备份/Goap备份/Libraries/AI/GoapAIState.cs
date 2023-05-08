using UnityEngine;
using System.Collections.Generic;

namespace Goap.AI
{
    /// <summary>
    /// AIAgent 状态的默认实现。
    /// </summary>
    public abstract class GoapAIState : MonoBehaviour
    {
        #region Private Variables
        private bool _isFinished = false;
        private bool _allowInterrupting = true;
        private List<string> _interruptions = new List<string>();
        #endregion

        #region Getters / Setters

        /// <summary>
        /// 中断条件列表。
        /// </summary>
        public List<string> Interruptions
        {
            get
            {
                return _interruptions;
            }
        }

        /// <summary>
        /// 是否允许强制中断。
        /// </summary>
        public virtual bool AllowInterrupting
        {
            get
            {
                return _allowInterrupting;
            }
            set
            {
                _allowInterrupting = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calling when game object and state is created.
        /// </summary>
        public virtual void Create(GameObject aGameObject, ISense sense)
        {
            // ...
        }

        /// <summary>
        /// Calling before destroying the game object.
        /// </summary>
        /// <param name="aGameObject">Reference to the owner game object.</param>
        public virtual void Destroy(GameObject aGameObject)
        {
            // ...
        }

        /// <summary>
        /// Calling before entering to state.
        /// </summary>
        public virtual void Enter()
        {
            // ...
        }

        /// <summary>
        /// Calling every frame when state is active.
        /// </summary>
        /// <param name="aDeltaTime">Delta time.</param>
        /// <param name="aTimeScale">Time scale.</param>
        public virtual void Execute(float aDeltaTime, float aTimeScale)
        {
            // ...
        }

        /// <summary>
        /// Calling before leaving from the state.
        /// </summary>
        public virtual void Exit()
        {
            // ...
        }

        /// <summary>
        /// Call this function when state is finished.
        /// </summary>
        public void Finish()
        {
            _isFinished = true;
        }

        public void Reset()
        {
            _isFinished = false;
        }

        public virtual bool IsFinished(GoapAIAgent aAgent, GoapAICondition aWorldState)
        {
            return (_isFinished || OverlapInterrupts(aAgent, aWorldState));
        }

        /// <summary>
        /// Checking if current world state is equal interruption settings.
        /// </summary>
        /// <param name="aAgent">Ref to the GoapAIAgent.</param>
        /// <param name="aWorldState">Current world state.</param>
        /// <returns></returns>
        public bool OverlapInterrupts(GoapAIAgent aAgent, GoapAICondition aWorldState)
        {
            int index = -1;
            for (int i = 0, n = _interruptions.Count; i < n; i++)
            {
                index = aAgent.planner.GetAtomIndex(_interruptions[i]);
                if (aWorldState.GetValue(index))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
