namespace Goap.AI
{
    using UnityEngine;

    public class GoapAIAction
    {
        #region Public Variables

        public int cost;            // Cost of the action.
        public string name;         // Name of the action.
        public GameObject state;    // Reference to the GoapAIState.
        public GoapAICondition pre;  // Previous world state.
        public GoapAICondition post; // Current world state.

        #endregion

        #region Public Methods

        public GoapAIAction(string aName, int aCost = 1)
        {
            cost = aCost;
            name = aName;
            state = null;
            pre = new GoapAICondition();
            post = new GoapAICondition();
        }

        #endregion
    }
}
