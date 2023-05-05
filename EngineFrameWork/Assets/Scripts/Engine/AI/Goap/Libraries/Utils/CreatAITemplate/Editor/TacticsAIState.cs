using FrameWork;
using UnityEngine;
namespace Goap.AI
{
    public class TacticsAIState : GoapAIState
    {
        #region Private Variables
        #endregion

        #region Register Event & DispatchEvent
        public void RemoveEvent() 
        {
            //RemoveEvent
        }

        public void RegisterEvent() 
        {
            //SetRegister
        }

        //RegisterImplement
        //SetDispatch
        #endregion

        #region Commands
        //SetCommands
        #endregion


        /// <summary>
        /// Calling when game object and state is created.
        /// </summary>
        /// <param name="aGameObject">Reference to the owner game object.</param>
        /// <param name="sense"></param>
        public override void Create(GameObject aGameObject, ISense sense)
        {
            RegisterEvent();
        }


        /// <summary>
        /// State Machine Enter. 
        /// Reset Finished State.
        /// </summary>
        public override void Enter()
        {
            Reset();
        }

        /// <summary>
        /// Driving by Mono Update.
        /// </summary>
        /// <param name="aDeltaTime"></param>
        /// <param name="aTimeScale"></param>
        public override void Execute(float aDeltaTime, float aTimeScale)
        {
           
        }

        public override void Exit()
        {
           
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }
    }
}