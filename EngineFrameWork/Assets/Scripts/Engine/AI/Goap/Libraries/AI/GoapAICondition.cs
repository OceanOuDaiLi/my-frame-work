namespace Goap.AI
{
    /// <summary>
    /// 条件节点
    /// </summary>
    public class GoapAICondition
    {
        #region Variables

        public string name;   // Name of the Conditions list.
        public bool[] values; // Values of the conditions.
        public bool[] mask;   // Mask of the conditions.

        #endregion

        #region Private Variables

        private GoapAIPlanner _planner;

        #endregion

        #region Public Methods

        public GoapAICondition()
        {
            values = new bool[GoapAIPlanner.MAX_ATOMS];
            mask = new bool[GoapAIPlanner.MAX_ATOMS];
        }
     
        public void Clear()
        {
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                values[i] = false;
                mask[i] = false;
            }
        }

        public void BeginUpdate(GoapAIPlanner aPlanner)
        {
            _planner = aPlanner;
        }

        public void EndUpdate()
        {
            _planner = null;
        }

        public int GetValue(string aAtomName)
        {
            return GetValue(_planner, aAtomName);
        }

        public int GetValue(GoapAIPlanner aPlanner, string aAtomName)
        {
            return aPlanner.GetAtomIndex(aAtomName);
        }

        public bool Has(string aAtomName)
        {
            return Has(_planner, aAtomName);
        }

        public bool Has(GoapAIPlanner aPlanner, string aAtomName)
        {
            int index = aPlanner.GetAtomIndex(aAtomName);
            return (index >= 0 && index < values.Length)
                ? values[index]
                : false;
        }

        /// <summary>
        ///设置条件的状态
        /// BeginUpdate() 后调用.
        /// </summary>
        /// <param name="aAtomName">Name of Condition.</param>
        /// <param name="aValue">Value of Condition.</param>
        /// <returns>返回 True 则可以设置状态.</returns>
        public bool Set(string aAtomName, bool aValue)
        {
            return Set(_planner.GetAtomIndex(aAtomName), aValue);
        }

        public bool Set(GoapAIPlanner aPlanner, string aAtomName, bool aValue)
        {
            return Set(aPlanner.GetAtomIndex(aAtomName), aValue);
        }

        //public bool Set<T>(T aAtomEnum, bool aValue) where T : System.Enum
        //{
        //    return Set((int)(object)aAtomEnum, aValue);
        //}

        public bool Set(int aIndex, bool aValue)
        {
            if (aIndex >= 0 && aIndex < GoapAIPlanner.MAX_ATOMS)
            {
                values[aIndex] = aValue;
                mask[aIndex] = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启发式搜索F = G + H，从指定条件转换到当前条件。
        /// specified conditions to the current one.
        /// </summary>
        /// <param name="aOther">From state.</param>
        /// <returns>Value of heuristics.</returns>
        public int Heuristic(GoapAICondition aOther)
        {
            int dist = 0;
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                if (aOther.mask[i] && values[i] != aOther.values[i])
                {
                    dist++;
                }
            }
            return dist;
        }

        public bool Match(GoapAICondition aOther)
        {
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                if ((mask[i] && aOther.mask[i]) && (values[i] != aOther.values[i]))
                {
                    return false;
                }
            }
            return true;
        }
     
        public bool GetMask(int aIndex)
        {
            return (aIndex >= 0 && aIndex < GoapAIPlanner.MAX_ATOMS)
                ? mask[aIndex]
                : false;
        }

        public bool GetValue(int aIndex)
        {
            return (aIndex >= 0 && aIndex < GoapAIPlanner.MAX_ATOMS)
                ? values[aIndex]
                : false;
        }

        public GoapAICondition Clone()
        {
            var clone = new GoapAICondition();
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                clone.values[i] = values[i];
                clone.mask[i] = mask[i];
            }
            return clone;
        }

        public void Act(GoapAICondition aPost)
        {
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                mask[i] = mask[i] || aPost.mask[i];
                if (aPost.mask[i])
                {
                    values[i] = aPost.values[i];
                }
            }
        }

        public bool Equals(GoapAICondition aCondition)
        {
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                if (values[i] != aCondition.values[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool[] Description()
        {
            var result = new bool[GoapAIPlanner.MAX_ATOMS];
            for (int i = 0; i < GoapAIPlanner.MAX_ATOMS; i++)
            {
                result[i] = mask[i] && values[i];
            }
            return result;
        }

        #endregion
    }
}
