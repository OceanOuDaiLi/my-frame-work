namespace Goap.AI
{
    using System.Collections.Generic;

    /// <summary>
    /// 构建计划
    /// </summary>
    public class GoapAIPlanner
    {
        #region Variables

        public const int MAX_ATOMS = 32;   // 最大条件数
        public const int MAX_ACTIONS = 32; // 最大行为数

        public delegate void PlanUpdatedDelegate(GoapAIPlan aNewPlan);
        public event PlanUpdatedDelegate EventPlanUpdated; // Called when plan is updated.

        public List<string> atoms = new List<string>();
        public List<GoapAIAction> actions = new List<GoapAIAction>();
        public List<GoapAICondition> goals = new List<GoapAICondition>();

#if UNITY_EDITOR
        public GoapAICondition DebugConditions { get; private set; }
#endif

        private string _scenarioName;
        private int _defActionIndex = -1;
        private int _defGoalIndex = -1;

        #endregion

        #region Public Methods

        public void LoadScenario(GoapAIScenario aScenario)
        {
            int atomIndex;
            _scenarioName = aScenario.name;

            // 0. Register all conditions.
            // ---------------------------
            for (int i = 0, n = aScenario.conditions.list.Length; i < n; i++)
            {
                GetAtomIndex(aScenario.conditions.list[i].name);
            }

            GoapAIAction action;
            GoapAIScenarioAction scenarioAction;
            for (int i = 0, n = aScenario.actions.Length; i < n; i++)
            {
                scenarioAction = aScenario.actions[i];
                action = GetAction(scenarioAction.name);
                action.state = scenarioAction.state;
                action.cost = scenarioAction.cost;


                //Read Pre Conditions.
                for (int j = 0, nj = scenarioAction.pre.Length; j < nj; j++)
                {
                    atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.pre[j].id));
                    action.pre.Set(atomIndex, scenarioAction.pre[j].value);
                }


                //Read Post Conditions.
                for (int j = 0, nj = scenarioAction.post.Length; j < nj; j++)
                {
                    atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.post[j].id));
                    action.post.Set(atomIndex, scenarioAction.post[j].value);
                }

                if (scenarioAction.isDefault)
                {
                    _defActionIndex = actions.Count - 1;
                }
            }

            // 1. Read Goal List
            // -----------------
            GoapAICondition goal;
            GoapAIScenarioGoal scenarioGoal;
            for (int i = 0, n = aScenario.goals.Length; i < n; i++)
            {
                scenarioGoal = aScenario.goals[i];
                goal = GetGoal(scenarioGoal.name);

                // Read Conditions.
                for (int j = 0, nj = scenarioGoal.conditions.Length; j < nj; j++)
                {
                    goal.Set(this, aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
                        scenarioGoal.conditions[j].value);
                }

                if (scenarioGoal.isDefault)
                {
                    _defGoalIndex = goals.Count - 1;
                }
            }
        }

        public bool Pre(string aActionName, string aAtomName, bool aValue)
        {
            var action = GetAction(aActionName);
            int atomId = GetAtomIndex(aAtomName);
            return (action == null || atomId == -1)
                ? false
                : action.pre.Set(atomId, aValue);
        }

        public bool Post(string aActionName, string aAtomName, bool aValue)
        {
            var action = GetAction(aActionName);
            int atomId = GetAtomIndex(aAtomName);
            return (action == null || atomId == -1)
                ? false
                : action.post.Set(atomId, aValue);
        }

        public bool SetCostOfAction(string aActionName, int aCost)
        {
            var action = GetAction(aActionName);
            if (action != null)
            {
                action.cost = aCost;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            atoms.Clear();
            actions.Clear();
        }

        /// <summary>
        /// Extracts Condition index by Condition name.
        /// </summary>
        /// <param name="aAtomName">Name of the Condition.</param>
        /// <returns>Returns index of the Condition in the list. If Condition not exists, then it will be added to the Conditions list.</returns>
        public int GetAtomIndex(string aAtomName)
        {
            int index = atoms.IndexOf(aAtomName);
            if (index == -1 && atoms.Count < MAX_ATOMS)
            {
                atoms.Add(aAtomName);
                index = atoms.Count - 1;
            }
            return index;
        }

        public GoapAICondition GetGoal(string aGoalName)
        {
            var goal = FindGoal(aGoalName);
            if (goal == null)
            {
                goal = new GoapAICondition { name = aGoalName };
                goals.Add(goal);
            }
            return goal;
        }

        public GoapAICondition FindGoal(string aGoalName)
        {
            return goals.Find(x => x.name.Equals(aGoalName));
        }

        public GoapAICondition GetDefaultGoal()
        {
            return (_defGoalIndex >= 0 && _defGoalIndex < goals.Count)
                ? goals[_defGoalIndex]
                : null;
        }

        public GoapAIAction GetAction(string aActionName)
        {
            var action = FindAction(aActionName);
            if (action == null && actions.Count < MAX_ACTIONS)
            {
                action = new GoapAIAction(aActionName);
                actions.Add(action);
            }
            return action;
        }

        public GoapAIAction FindAction(string aActionName)
        {
            return actions.Find(x => (x.name != null && x.name.Equals(aActionName)));
        }

   
        public GoapAIAction GetDefaultAction()
        {
            return (_defActionIndex >= 0 && _defActionIndex < actions.Count)
                ? actions[_defActionIndex]
                : null;
        }
        public void MakePlan(ref GoapAIPlan aPlan, GoapAICondition aCurrent, GoapAICondition aGoal)
        {
#if UNITY_EDITOR
            DebugConditions = aCurrent.Clone();
#endif
            var opened = new List<GoapAINode>();
            var closed = new List<GoapAINode>();

            opened.Add(new GoapAINode
            {
                world = aCurrent,
                parent = null,
                cost = 0,
                heuristic = aCurrent.Heuristic(aGoal),
                sum = aCurrent.Heuristic(aGoal),
                action = string.Empty
            });

            GoapAINode current = opened[0];
            while (opened.Count > 0)
            {
                // Find lowest rank.
                current = opened[0];
                for (int i = 1, n = opened.Count; i < n; i++)
                {
                    if (opened[i].sum < current.sum)
                    {
                        current = opened[i];
                    }
                }

                opened.Remove(current);

                if (current.world.Match(aGoal))
                {
                    //Found Plan.
                    ReconstructPlan(ref aPlan, closed, current);
                    aPlan.isSuccess = true;
                    if (EventPlanUpdated != null)
                    {
                        EventPlanUpdated(aPlan);
                    }

                    return;
                }

                closed.Add(current);

                //Get neighbors.
                List<GoapAIAction> neighbors = GetPossibleTransitions(current.world);
                GoapAICondition neighbor;
                int openedIndex = -1;
                int closedIndex = -1;
                int cost = -1;
                for (int i = 0, n = neighbors.Count; i < n; i++)
                {
                    cost = current.cost + neighbors[i].cost;

                    neighbor = current.world.Clone();
                    neighbor.Act(neighbors[i].post);

                    openedIndex = FindEqual(opened, neighbor);
                    closedIndex = FindEqual(closed, neighbor);

                    if (openedIndex > -1 && cost < opened[openedIndex].cost)
                    {
                        opened.RemoveAt(openedIndex);
                        openedIndex = -1;
                    }

                    if (closedIndex > -1 && cost < closed[closedIndex].cost)
                    {
                        closed.RemoveAt(closedIndex);
                        closedIndex = -1;
                    }

                    if (openedIndex == -1 && closedIndex == -1)
                    {
                        opened.Add(new GoapAINode
                        {
                            world = neighbor,
                            cost = cost,
                            heuristic = neighbor.Heuristic(aGoal),
                            sum = cost + neighbor.Heuristic(aGoal),
                            action = neighbors[i].name,
                            parent = current.world
                        });
                    }
                }
            }

            // Failed plan.
            ReconstructPlan(ref aPlan, closed, current);
            aPlan.isSuccess = false;

            if (EventPlanUpdated != null)
            {
                EventPlanUpdated(aPlan);
            }
        }

        #endregion

        #region Private Methods

        private List<GoapAIAction> GetPossibleTransitions(GoapAICondition aCurrent)
        {
            var possible = new List<GoapAIAction>();
            for (int i = 0, n = actions.Count; i < n; i++)
            {
                if (actions[i].pre.Match(aCurrent))
                {
                    possible.Add(actions[i]);
                }
            }
            return possible;
        }

        private int FindEqual(List<GoapAINode> aList, GoapAICondition aCondition)
        {
            for (int i = 0, n = aList.Count; i < n; i++)
            {
                if (aList[i].world.Equals(aCondition))
                {
                    return i;
                }
            }
            return -1;
        }

        private void ReconstructPlan(ref GoapAIPlan aPlan, List<GoapAINode> aClosed, GoapAINode aGoal)
        {
            aPlan.Reset();
            int index;
            GoapAINode current = aGoal;
            while (current != null && current.parent != null)
            {
                aPlan.Insert(current.action);
                index = FindEqual(aClosed, current.parent);
                current = (index == -1) ? aClosed[0] : aClosed[index];
            }
        }

        #endregion
    }
}
