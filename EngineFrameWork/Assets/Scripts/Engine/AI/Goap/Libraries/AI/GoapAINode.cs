namespace Goap.AI
{
    /// <summary>
    /// 状态节点
    /// </summary>
    public class GoapAINode
    {
        public GoapAICondition parent;              // Parent world state from which we came.
        public GoapAICondition world;               // Current world state.
        public string action;                       // Action that led to this condition.
        public int heuristic;
        public int cost;                            // Cost of the node.
        public int sum;                             // Summ heruistic and cost.
    }
}
