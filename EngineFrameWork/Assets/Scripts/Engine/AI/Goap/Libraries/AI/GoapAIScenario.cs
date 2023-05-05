using System;
using UnityEngine;
using Goap.Utils;

namespace Goap.AI
{
    /// <summary>
    /// Scenario 是构建计划和完成目标所需的条件、行为和目标的配置文件
    /// </summary>
    [CreateAssetMenuAttribute(fileName = "Scenario", menuName = "Goap/AI Scenario", order = 1)]
    public class GoapAIScenario : ScriptableObject
    {
        [HideInInspector] public GoapAIScenarioGoal[] goals = new GoapAIScenarioGoal[0];
        [HideInInspector] public GoapAIWorldState[] worldStates = new GoapAIWorldState[0];
        [HideInInspector] public GoapAIScenarioAction[] actions = new GoapAIScenarioAction[0];
        [HideInInspector] public GoapScenarioCondition conditions = new GoapScenarioCondition();
        [HideInInspector] public GoapScenarioCommand commands = new GoapScenarioCommand();
        [HideInInspector] public GoapScenarioEvent events = new GoapScenarioEvent();
        [HideInInspector] public GoapProperty property = new GoapProperty();

        #region Public Methods

        public void AddCondition(string aName)
        {
            GoapArray.Add(ref conditions.list, new GoapAIScenarioConditionItem
            {
                id = conditions.list.Length,
                name = aName
            });
        }

        public void RemoveConditionAt(int aIndex)
        {
            if (aIndex >= 0 && aIndex < conditions.list.Length)
            {
                UpdateConditionIndexes(aIndex);
                UpdateActionIndexes(aIndex);
                UpdateGoalIndexes(aIndex);
                UpdateWorldStates(aIndex);

                GoapArray.RemoveAt(ref conditions.list, aIndex);
            }
        }

        public void AddCommand(string cName)
        {
            GoapArray.Add(ref commands.list, new GoapCommandItem
            {
                id = commands.list.Length,
                name = cName,
                excuteType = CommandExcuteType.ImmeDiately,
            });
        }

        public void RemoveCommandAt(int cIndex)
        {
            if (cIndex >= 0 && cIndex < commands.list.Length)
            {
                UpdateCommandIndexes(cIndex);

                GoapArray.RemoveAt(ref commands.list, cIndex);
            }
        }

        public void AddEvent(string eName)
        {
            GoapArray.Add(ref events.list, new GoapEventItem
            {
                eventName = eName,
            });
        }

        public void RemoveEventAt(int eIndex, string eName)
        {
            if (eIndex >= 0 && eIndex < events.list.Length)
            {
                UpdateEventIndexes(eIndex, eName);

                GoapArray.RemoveAt(ref events.list, eIndex);
            }
        }

        public void ChangeProperty(GoapProperty property)
        {
            this.property = property;
        }

        #endregion

        #region Private Methods

        private void UpdateConditionIndexes(int aDelIndex)
        {
            for (int i = conditions.list.Length - 1; i >= 0; i--)
            {
                if (conditions.list[i].id > aDelIndex)
                {
                    conditions.list[i].id -= 1;
                }
            }
        }

        private void UpdateActionIndexes(int aDelIndex)
        {
            for (int i = 0, n = actions.Length; i < n; i++)
            {
                var action = actions[i];

                // Update pre conditions for actions.
                for (int j = action.pre.Length - 1; j >= 0; j--)
                {
                    if (action.pre[j].id == aDelIndex)
                    {
                        GoapArray.RemoveAt(ref action.pre, j);
                    }
                    else
                    {
                        if (action.pre[j].id > aDelIndex)
                        {
                            action.pre[j].id -= 1;
                        }
                    }
                }

                // Update post conditions for actions.
                for (int j = action.post.Length - 1; j >= 0; j--)
                {
                    if (action.post[j].id == aDelIndex)
                    {
                        GoapArray.RemoveAt(ref action.post, j);
                    }
                    else
                    {
                        if (action.post[j].id > aDelIndex)
                        {
                            action.post[j].id -= 1;
                        }
                    }
                }
            }
        }

        private void UpdateGoalIndexes(int aDelIndex)
        {
            for (int i = 0, n = goals.Length; i < n; i++)
            {
                var goal = goals[i];

                // Update goal conditions.
                for (int j = goal.conditions.Length - 1; j >= 0; j--)
                {
                    if (goal.conditions[j].id == aDelIndex)
                    {
                        GoapArray.RemoveAt(ref goal.conditions, j);
                    }
                    else
                    {
                        if (goal.conditions[j].id > aDelIndex)
                        {
                            goal.conditions[j].id -= 1;
                        }
                    }
                }
            }
        }

        private void UpdateWorldStates(int aDelIndex)
        {
            for (int i = 0, n = worldStates.Length; i < n; i++)
            {
                var state = worldStates[i];

                // Update world state conditions.
                for (int j = state.list.Length - 1; j >= 0; j--)
                {
                    if (state.list[j].id == aDelIndex)
                    {
                        GoapArray.RemoveAt(ref state.list, j);
                    }
                    else
                    {
                        if (state.list[j].id > aDelIndex)
                        {
                            state.list[j].id -= 1;
                        }
                    }
                }
            }
        }

        private void UpdateCommandIndexes(int aDelIndex)
        {
            for (int i = commands.list.Length - 1; i >= 0; i--)
            {
                if (commands.list[i].id > aDelIndex)
                {
                    commands.list[i].id -= 1;
                }
            }

            for (int i = 0, n = actions.Length; i < n; i++)
            {
                var action = actions[i];

                // Update pre conditions for actions.
                for (int j = action.commands.Length - 1; j >= 0; j--)
                {
                    if (action.commands[j].id == aDelIndex)
                    {
                        GoapArray.RemoveAt(ref action.commands, j);
                    }
                    else
                    {
                        if (action.commands[j].id > aDelIndex)
                        {
                            action.commands[j].id -= 1;
                        }
                    }
                }
            }
        }

        private void UpdateEventIndexes(int aDelIndex, string eName)
        {

            for (int i = 0, n = actions.Length; i < n; i++)
            {
                var action = actions[i];

                // Update pre conditions for actions.
                for (int j = action.events.Length - 1; j >= 0; j--)
                {
                    if (action.events[j].eventName == eName)
                    {
                        GoapArray.RemoveAt(ref action.events, j);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Static Methods
        public static void TestCommon() { }

        public static string GetName<T>(int idx, T list)
        {
            Type t = list.GetType();

            int index = -1;
            int listLengh = 0;
            string name = string.Empty;
            switch (t.Name)
            {
                case "GoapAIScenarioConditionItem[]":
                    var conditionList = list as GoapAIScenarioConditionItem[];
                    index = Array.FindIndex(conditionList, x => x.id == idx);
                    listLengh = conditionList.Length;
                    name = conditionList[index].name;
                    break;

                case "GoapCommandItem[]":
                    var commandList = list as GoapCommandItem[];
                    index = Array.FindIndex(commandList, x => x.id == idx);
                    listLengh = commandList.Length;
                    name = commandList[index].name;
                    break;

                default:
                    break;
            }

            return (index >= 0 && index < listLengh) ? name : null;
        }


        public static int GetIndex<T>(string name, T list)
        {
            Type t = list.GetType();

            int index = -1;
            int listLengh = 0;
            int id = -1;
            switch (t.Name)
            {
                case "GoapAIScenarioConditionItem[]":
                    var conditionList = list as GoapAIScenarioConditionItem[];
                    index = Array.FindIndex(conditionList, x => x.name.Equals(name));
                    listLengh = conditionList.Length;
                    id = conditionList[index].id;
                    break;

                case "GoapCommandItem[]":
                    var commandList = list as GoapCommandItem[];
                    index = Array.FindIndex(commandList, x => x.name.Equals(name));
                    listLengh = commandList.Length;
                    id = commandList[index].id;
                    break;

                default:
                    break;
            }

            return (index >= 0 && index < listLengh) ? id : -1;
        }


        #endregion
    }

    /// <summary>
    /// Editor下Goap驱动决策时，当前状态。
    /// </summary>
    [Serializable]
    public class GoapAIWorldState
    {
        public Vector2 position;
        public bool isAutoUpdate;
        public GoapAIScenarioItem[] list;

        public GoapAIWorldState()
        {
            list = new GoapAIScenarioItem[0];
        }
    }

    /// <summary>
    /// 配置目标
    /// </summary>
    [Serializable]
    public class GoapAIScenarioGoal
    {
        public string name;                        // Name of the goal.
        public bool isDefault;                     // This goal will be active by default.
        public Vector2 position;                   // Position of the node in the editor workplace.
        public GoapAIScenarioItem[] conditions;     // World state what we want to reach.

        public GoapAIScenarioGoal()
        {
            name = "<Unnamed>";
            position = Vector2.zero;
            conditions = new GoapAIScenarioItem[0];
        }
    }

    /// <summary>
    /// Goap配置行为
    /// </summary>
    [Serializable]
    public class GoapAIScenarioAction
    {
        public string name;                        // Name of the action.
        public string desc;                        // Description of the action.
        public float rectW;                        // Slider to change rect width.
        public bool isDefault;                     // This action will be as default state.
        public GameObject state;                   // Reference to the prefab with custom GoapAIState script.
        public int cost;                           // Cost of the action.
        public Vector2 position;                   // Position of the node in the editor workplace.
        public GoapAIScenarioItem[] pre;           // Conditions before this action will be called.
        public GoapAIScenarioItem[] post;          // Conditions after this action will be called.
        public GoapEventItem[] events;             // Event list of the action.
        public GoapCommandItem[] commands;         // Command list of the action.

        public GoapAIScenarioAction()
        {
            name = "<Unnamed>";
            desc = "<Description>";
            rectW = 200.0f;
            isDefault = false;
            state = null;
            cost = 0;
            position = Vector2.zero;
            pre = new GoapAIScenarioItem[0];
            post = new GoapAIScenarioItem[0];
            commands = new GoapCommandItem[0];
        }
    }

    /// <summary>
    ///Editor下配置面板Item
    /// </summary>
    [Serializable]
    public struct GoapAIScenarioItem
    {
        public int id;
        public bool value;
        public float weight;
        public ConditionLevel conditionLevel;
    }

    public enum ConditionLevel
    {
        MUST = 0,                               //Condition which depend's on boolean.
        OPTIONAL = 1,                           //Condition which depend's on weight.
    }

    /// <summary>
    /// 描述配置条件。
    /// </summary>
    [Serializable]
    public class GoapScenarioCondition
    {
        public GoapAIScenarioConditionItem[] list = new GoapAIScenarioConditionItem[0];

        public GoapScenarioCondition Clone()
        {
            var clone = new GoapScenarioCondition();
            clone.list = new GoapAIScenarioConditionItem[list.Length];
            for (int i = 0, n = list.Length; i < n; i++)
            {
                clone.list[i] = list[i];
            }
            return clone;
        }

        public string GetName(int aIndex)
        {
            return GoapAIScenario.GetName(aIndex, list);
        }

        public int GetIndex(string aConditionName)
        {
            return GoapAIScenario.GetIndex(aConditionName, list);
        }

        public string this[int aIndex]
        {
            get
            {
                if (aIndex >= 0 && aIndex < list.Length)
                {
                    return list[aIndex].name;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get { return list.Length; }
        }
    }

    /// <summary>
    /// Editor下配置面板条件Item
    /// </summary>
    [Serializable]
    public struct GoapAIScenarioConditionItem
    {
        public int id;
        public string name;
    }

    /// <summary>
    /// 指令Item.
    /// </summary>
    [Serializable]
    public struct GoapCommandItem
    {
        public int id;
        public string name;
        public string commandDescr;
        public float delayStartTm;
        public float delayEndTm;
        public CommandExcuteType excuteType;
        public CommandLife commandLife;

        public CommandType commandType;
    }

    public enum CommandLife
    {
        Create = 0,
        Enter = 1,
        Update = 2,
        Exit = 3
    }

    [Serializable]
    public class MoceCmdModel 
    {
        public GoapProperty conditions;
    }


    public enum CommandType 
    {
        Default = 0,    //默认指令
        Move = 1,       //移动指令
        Combine = 2,    //组合指令
    }

    public enum CommandExcuteType
    {
        ImmeDiately = 0,           //立即执行
        Delay = 1,                 //延迟执行
        Customer = 2               //自定义条件执行
    }

    /// <summary>
    /// 描述配置指令
    /// </summary>
    [Serializable]
    public class GoapScenarioCommand
    {
        public GoapCommandItem[] list = new GoapCommandItem[0];

        public GoapScenarioCommand Clone()
        {
            var clone = new GoapScenarioCommand();
            clone.list = new GoapCommandItem[list.Length];
            for (int i = 0, n = list.Length; i < n; i++)
            {
                clone.list[i] = list[i];
            }
            return clone;
        }

        public string GetName(int aIndex)
        {
            return GoapAIScenario.GetName(aIndex, list);
        }

        public int GetIndex(string aConditionName)
        {
            return GoapAIScenario.GetIndex(aConditionName, list);
        }

        public string this[int aIndex]
        {
            get
            {
                if (aIndex >= 0 && aIndex < list.Length)
                {
                    return list[aIndex].name;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get { return list.Length; }
        }
    }

    /// <summary>
    /// 事件Item.
    /// </summary>
    [Serializable]
    public struct GoapEventItem
    {
        public string eventName;
        public object[] param;
        public GoapEventType goapEventType;
        public CommandLife excuteLife;
    }

    public enum GoapEventType
    {
        None = 0,
        Register = 1,
        Dispatch = 2
    }

    /// <summary>
    /// 描述配置事件
    /// </summary>
    [Serializable]
    public class GoapScenarioEvent
    {
        public GoapEventItem[] list = new GoapEventItem[0];

        public GoapScenarioEvent Clone()
        {
            var clone = new GoapScenarioEvent();
            clone.list = new GoapEventItem[list.Length];
            for (int i = 0, n = list.Length; i < n; i++)
            {
                clone.list[i] = list[i];
            }
            return clone;
        }

        public string GetName(int aIndex)
        {
            return GoapAIScenario.GetName(aIndex, list);
        }

        public int GetIndex(string aConditionName)
        {
            return GoapAIScenario.GetIndex(aConditionName, list);
        }

        public string this[int aIndex]
        {
            get
            {
                if (aIndex >= 0 && aIndex < list.Length)
                {
                    return list[aIndex].eventName;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get { return list.Length; }
        }
    }

    /// <summary>
    /// For NBA Project
    /// 回合类型
    /// </summary>
    public enum TurnType
    {
        SCORE = 0,      //得分回合
        REBOUNDS = 1,   //篮板回合
        BLOCKS = 2,     //盖帽回合
    }

    /// <summary>
    /// For NBA Project
    /// 出手方式
    /// </summary>
    public enum ShootWay
    {
        /// <summary>
        /// 控球后卫(PG)上篮
        /// </summary>
        PG_LAYUP = 0,

        /// <summary>
        /// 控球后卫(PG)中投
        /// </summary>
        PG_CIC = 1,

        /// <summary>
        /// 控球后卫(PG)三分
        /// </summary>
        PG_TRISECTION = 2,

        /// <summary>
        /// 得分后卫(SG)上篮
        /// </summary>
        SG_LAYUP = 3,

        /// <summary>
        /// 得分后卫(SG)中投
        /// </summary>
        SG_CIC = 4,

        /// <summary>
        /// 得分后卫(SG)三分
        /// </summary>
        SG_TRISECTION = 5,

        /// <summary>
        /// 小前锋(SF)上篮
        /// </summary>
        SF_LAYUP = 6,

        /// <summary>
        /// 小前锋(SF)中投
        /// </summary>
        SF_CIC = 7,

        /// <summary>
        /// 小前锋(SF)三分
        /// </summary>
        SF_TRISECTION = 8,

        /// <summary>
        /// 大前锋(PF)上篮
        /// </summary>
        PF_LAYUP = 9,

        /// <summary>
        /// 大前锋(PF)中投
        /// </summary>
        PF_CIC = 10,

        /// <summary>
        /// 大前锋(PF)三分
        /// </summary>
        PF_TRISECTION = 11,

        /// <summary>
        /// 中锋(C)上篮
        /// </summary>
        C_LAYUP = 12,

        /// <summary>
        /// 中锋(C)中投
        /// </summary>
        C_CIC = 13,

        /// <summary>
        /// 中锋(C)三分
        /// </summary>
        C_TRISECTION = 14,
    }

    [Serializable]
    public class ShootWayModel
    {
        public ShootWay way;
        public bool selected;

        public ShootWayModel(ShootWay way, bool selected)
        {
            this.way = way;
            this.selected = selected;
        }
    }

    [Serializable]
    public class TurnTypeModel
    {
        public TurnType turnType;
        public bool selected;

        public TurnTypeModel(TurnType turnType, bool selected)
        {
            this.turnType = turnType;
            this.selected = selected;
        }
    }

    /// <summary>
    /// 描述配置属性
    /// </summary>
    [Serializable]
    public class GoapProperty
    {
        /// <summary>
        /// 出手方式
        /// </summary>
        public ShootWayModel[] wsyModels = new ShootWayModel[0];

        /// <summary>
        /// 回合类型
        /// </summary>
        public TurnTypeModel[] turnModels = new TurnTypeModel[0];

        public GoapProperty()
        {
            if (wsyModels.Length <= 1)
            {
                ShootWay[] shootWayAry = Enum.GetValues(typeof(ShootWay)) as ShootWay[];

                wsyModels = new ShootWayModel[shootWayAry.Length];
                for (int i = 0; i < shootWayAry.Length; i++)
                {
                    wsyModels[i] = new ShootWayModel(shootWayAry[i], true);
                }
            }

            if (turnModels.Length < 1)
            {
                TurnType[] turnTypeAry = Enum.GetValues(typeof(TurnType)) as TurnType[];
                turnModels = new TurnTypeModel[turnTypeAry.Length];
                for (int i = 0; i < turnTypeAry.Length; i++)
                {
                    turnModels[i] = new TurnTypeModel(turnTypeAry[i], true);
                }
            }
        }

    }
}
