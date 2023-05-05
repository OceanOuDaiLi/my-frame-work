using UnityEngine;
using System.Collections.Generic;

namespace Goap.AI
{
    public enum TemplateSense { }

    public class TacticsAISense : MonoBehaviour, ISense
    {
        #region Private/Public Variables.
        private Dictionary<TemplateSense, bool> stateDic;
        #endregion

        #region Private method for get state.
        //JudgeFunc
        #endregion

        #region Unity Calls

        void Awake()
        {
            InitStateDic();

        }

        void OnDestroy() 
        {
        }

        #endregion

        void InitStateDic()
        {
            stateDic = new Dictionary<TemplateSense, bool>();
            //AddDic
        }

        public void UpdateStateDic(TemplateSense state, bool value)
        {
            if (!stateDic.ContainsKey(state))
            {
                ZDebug.LogError("Updateing Error Key.");
                return;
            }

            stateDic[state] = value;
        }

        public void CollectConditions(GoapAIAgent aAgent, GoapAICondition aWorldState)
        {
            aWorldState.BeginUpdate(aAgent.planner);
            {
                //SetState
            }
            aWorldState.EndUpdate();
        }
    }
}