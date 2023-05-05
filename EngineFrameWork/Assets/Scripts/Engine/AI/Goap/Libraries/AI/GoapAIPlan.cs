namespace Goap.AI
{
    using System.Collections.Generic;

    /// <summary>
    /// 将行动清单作为计划实施。
    /// </summary>
    public class GoapAIPlan
    {
        #region Variables

        public bool isSuccess;                   // True if plan is successed.

        private readonly List<string> _actions;

        #endregion

        #region Getters / Setters

        public string this[int aIndex]
        {
            get
            {
                if (aIndex >= 0 && aIndex < _actions.Count)
                {
                    return _actions[aIndex];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int Count { get { return _actions.Count; } }

        #endregion

        #region Public Methods

        public GoapAIPlan()
        {
            _actions = new List<string>();
        }

        /// <summary>
        /// Removes all actions from the plan.
        /// </summary>
        public void Reset()
        {
            _actions.Clear();
        }

        /// <summary>
        /// Adds new action to the plan.
        /// </summary>
        /// <param name="aValue">Action name.</param>
        public void Insert(string aValue)
        {
            _actions.Insert(0, aValue);
        }

        #endregion
    }
}
