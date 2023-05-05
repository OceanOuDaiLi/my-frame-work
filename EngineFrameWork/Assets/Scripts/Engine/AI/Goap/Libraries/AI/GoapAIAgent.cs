namespace Goap.AI
{
    using System.Collections.Generic;
    using UnityEngine;
    using Goap.Utils;
    using System.Collections;

    /// <summary>
    ///Goap AI 基础实现类
    /// </summary>
    [AddComponentMenu("Goap/AIAgent")]
    public class GoapAIAgent : MonoBehaviour
    {
        #region Public Variables

        [Tooltip("false: Mono Update. true: Custome update")]
        public bool manualUpdate;

        [Tooltip("Delay between updating the world state and plan builds.")]
        public float thinkInterval = 0.1f;

        [Tooltip("Reference to the AI scenario.")]
        public GoapAIScenario scenario;

        public GoapAIPlanner planner = new GoapAIPlanner();
        public GoapAICondition worldState = new GoapAICondition();
        public GoapAIPlan currentPlan = new GoapAIPlan();
        #endregion

        #region Private Variables

        private List<GoapAIState> _states;
        private GoapAIState _currentState;
        private GoapAICondition _currentGoal;
        private ISense[] _sensors;

        private float _thinkInterval;

        private bool Inited = false;
        #endregion

        #region Getters / Setters

        /// <summary>
        /// Returns active goal.
        /// </summary>
        public GoapAICondition Goal { get { return _currentGoal; } }

        /// <summary>
        /// Returns current state.
        /// </summary>
        public GoapAIState State { get { return _currentState; } }

        #endregion

        #region Unity Calls

        private void Awake()
        {
            _thinkInterval = GoapMath.RandomRangeFloat(0.0f, thinkInterval);

            _sensors = GetComponents<ISense>();
            if (_sensors.Length == 0)
            {
                AILog.Warning($"AIAgent `{gameObject.name}` have no sensors for collection conditions of the world state.");
            }
        }

        public void InitAgent(ISense sense)
        {
            AILog.Assert(scenario == null, $"Have no specified AI Scenario for `{name}` object.");
            planner.LoadScenario(scenario);

            _states = new List<GoapAIState>();
            for (int i = 0, n = scenario.actions.Length; i < n; i++)
            {
                AILog.Assert(
                    scenario.actions[i].state == null,
                    $"AI Scenario of `{name}` object have no state prefab for `{scenario.actions[i].name}` action."
                );

                if (ContainsState(scenario.actions[i].state.name))
                {
                    continue;
                }

                var go = GameObject.Instantiate(scenario.actions[i].state);
                go.transform.SetParent(gameObject.transform, false);
                go.name = scenario.actions[i].state.name;

                var state = go.GetComponent<GoapAIState>();

                AILog.Assert(
                    state == null,
                    $"GameObject `{go.name}` have no `GoapAIState` script`!"
                );

                state.Create(gameObject, sense);
                state.gameObject.SetActive(false);
                _states.Add(state);
            }

            Inited = true;
        }

        IEnumerator Start()
        {
            if (!Inited) { yield return null; }
            SetDefaultState();
            SetDefaultGoal();
            Think();
        }

        private void Update()
        {
            if (!Inited) { return; }

            if (!manualUpdate)
            {
                Execute(Time.deltaTime, Time.timeScale);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 更新AI决策
        /// </summary>
        /// <param name="aDeltaTime">Delta Time.</param>
        /// <param name="aTimeScale">Time Scale.</param>
        public void Execute(float aDeltaTime, float aTimeScale)
        {
            _currentState.Execute(aDeltaTime, aTimeScale);

            _thinkInterval -= aDeltaTime;
            if (_thinkInterval < 0.0f)
            {
                _thinkInterval = thinkInterval;
                Think();
            }
        }

        /// <summary>
        /// 更新goap中行为或条件状态时，调用
        /// </summary>
        public void Think()
        {
            // Update world state.
            for (int i = 0, n = _sensors.Length; i < n; i++)
            {
                _sensors[i].CollectConditions(this, worldState);
            }

            // Check the current action.
            if (_currentState != null)
            {
                if (_currentState.IsFinished(this, worldState))
                {
                    SetState(SelectNewState(worldState), true);
                }
                else if (_currentState.AllowInterrupting)
                {
                    SetState(SelectNewState(worldState));
                }
            }
            else
            {
                SetDefaultState();
            }
        }

        /// <summary>
        /// 设置一个AI 目标。
        /// </summary>
        /// <param name="aGoalName">Name of the goal that exists in the AI Scenario.</param>
        public void SetGoal(string aGoalName)
        {
            _currentGoal = planner.FindGoal(aGoalName);
            AILog.Assert(_currentGoal == null, $"Can't find goal `{aGoalName}` for `{gameObject.name}` AI Agent.");
        }

        /// <summary>
        /// 基于配置设置默认目标
        /// </summary>
        public void SetDefaultGoal()
        {
            _currentGoal = planner.GetDefaultGoal();
            AILog.Assert(_currentGoal == null, $"Can't find default goal for `{gameObject.name}` AI Agent.");
        }

        /// <summary>
        /// 基于配置设置默认状态
        /// </summary>
        public void SetDefaultState()
        {
            var defAction = planner.GetDefaultAction();
            AILog.Assert(defAction == null, $"Can't find default action for `{gameObject.name}` AI Agent.");
            SetState(defAction.state.name, true);
        }

        #endregion

        #region Private Methods

        private bool ContainsState(string aName)
        {
            var index = _states.FindIndex(x => x.name.Equals(aName));
            return (index >= 0 && index < _states.Count);
        }

        private string SelectNewState(GoapAICondition aWorldState)
        {
            var defAction = planner.GetDefaultAction();
            var stateName = (defAction != null) ? defAction.state.name : string.Empty;
            planner.MakePlan(ref currentPlan, aWorldState, _currentGoal);
            if (currentPlan.isSuccess || currentPlan.Count > 0)
            {
                var action = planner.FindAction(currentPlan[0]);
                if (action != null && action.state != null)
                {
                    stateName = action.state.name;
                }
            }

            return stateName;
        }

        private void SetState(string aStateName, bool aForce = false)
        {
            // 离开当前状态
            if (_currentState != null)
            {
                if (!aForce && string.Equals(_currentState.name, aStateName))
                {
                    // 已进入条件。跳过
                    return;
                }

                _currentState.Exit();
                _currentState.gameObject.SetActive(false);
                _currentState = null;
            }

            //设置新状态
            int index = _states.FindIndex(x => x.name.Equals(aStateName));
            if (index >= 0 && index < _states.Count)
            {
                _currentState = _states[index];
                _currentState.gameObject.SetActive(true);
                _currentState.Reset();
                _currentState.Enter();
            }
            else
            {
                AILog.Warning($"Can't find state `{aStateName}` for `{gameObject.name}` AI Agent.");
            }
        }

        #endregion
    }
}