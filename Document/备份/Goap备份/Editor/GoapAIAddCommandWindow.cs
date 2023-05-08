namespace Goap.AI
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class GoapAIAddCommandWindow : EditorWindow
    {
        #region Private Static Editor Variables

        static int tab = 0;

        private static int x = 0;
        private static int y = 0;
        private static int width = 880;
        private static int height = 520;

        private const string windowTitle = "指令增加面板";
        private const string callTitle = "指令调用时序设置";
        private static string callDesrc = "Create:  创建时执行 Enter: 进入节点时执行  Update: 节点每帧执行  Exit: 节点结束时执行";

        private const string callTypeTitle = "指令执行设置";
        private static string callTypeDesrc = "ImmeDiately:  立即调用 Delay:  延迟调用  Customer:  自定义条件执行";

        private const string callDesrcTitle = "指令描述(必须填写)";


        private static int saveIdx;
        private static Action<GoapCommandItem, int> action;

        static GoapAIAddCommandWindow window;
        static GoapCommandItem commandItem;
        static GoapAIScenario scenario;
        #endregion

        #region Private Static Command Variables


        #endregion

        public static void OpenCommandEditorWindow(GoapAIScenario current, GoapCommandItem target, int idx, Action<GoapCommandItem, int> save)
        {
            commandItem = target;
            action = save;
            saveIdx = idx;
            scenario = current;

            window = GetWindow<GoapAIAddCommandWindow>(false, windowTitle, true);
            x = (Screen.currentResolution.width - width) / 2;
            y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);
            window.maxSize = new Vector2(width, height);

            window.Show();
        }

        private void OnDisable()
        {
            action?.Invoke(commandItem, saveIdx);
        }

        private void OnGUI()
        {
            tab = GUILayout.Toolbar(tab, new string[] { "默认", "移动", "自定义" });

            GUIStyle style = new GUIStyle { richText = true };
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                string str = "";
                switch (tab)
                {
                    case 0: str = "默认"; break;
                    case 1: str = "初始跑位"; break;
                    case 2: str = "自定义"; break;
                    default: break;
                }

                EditorGUILayout.LabelField(string.Format("<color=green>使用指令类型:</color> <color=red>{0}</color>  ", str), style, GUILayout.MaxWidth(width));
            }
            GUILayout.EndHorizontal();

            switch (tab)
            {
                case 0: ShowDefault(); break;
                case 1: ; break;
                case 2: ; break;
                case 3: ; break;
                default: break;
            }
        
        }

        #region Default Command

        void ShowDefault()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(15.0f);
            DrawCommandCallTimeSetting();

            GUILayout.Space(30.0f);
            DrawCommandCallTypeSetting();

            GUILayout.Space(30.0f);
            DrawCommandDescription();

            GUILayout.EndVertical();
        }

        void DrawCommandCallTimeSetting()
        {
            GUIStyle style = new GUIStyle { richText = true };
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", callTitle), style, GUILayout.MaxWidth(width));

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                //                string color = "black";
                //#if UNITY_2019_OR_NEWER
                //                color = "white"
                //#endif
                callDesrc = EditorGUILayout.TextArea(string.Format("<color={0}>{1}</color>", "blue", callDesrc), style, GUILayout.MaxWidth(width));
                switch (commandItem.commandLife)
                {
                    case CommandLife.Create:
                        callDesrc = " 创建Action时执行";
                        break;
                    case CommandLife.Enter:
                        callDesrc = " 进入Action时执行";
                        break;
                    case CommandLife.Update:
                        callDesrc = " Action每帧执行";
                        break;
                    case CommandLife.Exit:
                        callDesrc = " Action结束时执行";
                        break;
                    default:
                        break;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Space(2.0f);

                commandItem.commandLife = (CommandLife)EditorGUILayout.EnumPopup(commandItem.commandLife, GUILayout.MaxWidth(65.0f));
            }
            GUILayout.EndHorizontal();
        }

        void DrawCommandCallTypeSetting()
        {
            GUIStyle style = new GUIStyle { richText = true };
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", callTypeTitle), style, GUILayout.MaxWidth(width));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                //                string color = "black";
                //#if UNITY_2019_OR_NEWER
                //                color = "white"
                //#endif
                callTypeDesrc = EditorGUILayout.TextArea(string.Format("<color={0}>{1}</color>", "blue", callTypeDesrc), style, GUILayout.MaxWidth(width));

                GUILayout.FlexibleSpace();

                commandItem.excuteType = (CommandExcuteType)EditorGUILayout.EnumPopup(commandItem.excuteType, GUILayout.MaxWidth(120f));

                switch (commandItem.excuteType)
                {
                    case CommandExcuteType.ImmeDiately:
                        callTypeDesrc = " 立即调用";
                        break;
                    case CommandExcuteType.Delay:
                        callTypeDesrc = " 延迟调用";
                        break;
                    case CommandExcuteType.Customer:
                        callTypeDesrc = " 自定义条件调用";
                        break;
                    default:
                        break;
                }

                if (commandItem.excuteType.Equals(CommandExcuteType.Delay))
                {
                    GUILayout.FlexibleSpace();
                    commandItem.delayStartTm = EditorGUILayout.Slider("延迟执行时长 (单位/秒)", commandItem.delayStartTm, 0, 60, GUILayout.MaxWidth(200));
                }
            }
            GUILayout.EndHorizontal();
        }

        void DrawCommandDescription()
        {
            GUIStyle style = new GUIStyle { richText = true };
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", callDesrcTitle), style, GUILayout.MaxWidth(width));

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            {
                commandItem.commandDescr = EditorGUILayout.TextArea(commandItem.commandDescr, GUILayout.Width(width - 4), GUILayout.Height(100));
            }
            GUILayout.EndVertical();
        }
        #endregion

        #region Move Command
        void ShowMoveCmd() 
        {
            //scenario.property
        }
        #endregion

        #region Combine Command

        #endregion

    }
}