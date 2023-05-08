using UnityEngine;

namespace Goap.AI
{
    public interface ISense
    {
        void CollectConditions(GoapAIAgent aAgent, GoapAICondition aWorldState);
    }
}
