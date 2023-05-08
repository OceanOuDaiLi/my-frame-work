namespace Goap.AI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class GoapAIEventWindow : EditorWindow
    {
        #region Private Static Variables

        private static int x = 0;
        private static int y = 0;
        private static int width = 512;
        private static int height = 512;
        private const string windowTitle = "事件编辑面板";

        private static int sub;
        private static int row;
        private static int totalEventCount;
        private static bool _isDeleteDispatchEvt;
        private static bool _isDeleteRegisterEvt;

        private static GoapAIEventWindow window;
        private static GoapEventItem[] eventArray;
        private static List<GoapEventItem> dispatchEvents;
        private static List<GoapEventItem> registerEvents;

        private static Action<GoapEventItem[]> action;

        #endregion

        public static void OpenEventEditorWindow(GoapAIScenario target, GoapEventItem[] actionEvents, Action<GoapEventItem[]> save)
        {
            eventArray = target.events.list;
            dispatchEvents = new List<GoapEventItem>();
            registerEvents = new List<GoapEventItem>();
            if (actionEvents != null)
            {
                for (int i = 0; i < actionEvents.Length; i++)
                {
                    switch (actionEvents[i].goapEventType)
                    {
                        case GoapEventType.Register:
                            registerEvents.Add(actionEvents[i]);
                            break;
                        case GoapEventType.Dispatch:
                            dispatchEvents.Add(actionEvents[i]);
                            break;
                        default:
                            break;
                    }
                }
            }

            action = save;

            window = GetWindow<GoapAIEventWindow>(false, string.Format("{0}: {1}", windowTitle, target.name), true);
            x = (Screen.currentResolution.width - width) / 2;
            y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);
            window.maxSize = new Vector2(width, height);

            window.Show();
        }

        #region Unity Calls
        private void OnDisable()
        {
            if (dispatchEvents == null || registerEvents == null) { return; }

            GoapEventItem[] newArray = new GoapEventItem []{ };
            if (eventArray.Length < 1) 
            {
                action?.Invoke(newArray);
                return;
            }

            newArray = new GoapEventItem[dispatchEvents.Count + registerEvents.Count];
            dispatchEvents.AddRange(registerEvents);
            for (int i = 0; i < dispatchEvents.Count; i++)
            {
                newArray[i] = dispatchEvents[i];
            }

            action?.Invoke(newArray);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            StartDrawChachedEventArea();

            StartDrawDispatchEventArea();

            StartDrawRegisterEventArea();
            GUILayout.EndVertical();
        }
        #endregion

        #region Function for draw 'cached' rvent list area.
        private void StartDrawChachedEventArea()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawChachedEventTitle();

                DrawCachedBoxView();
            }
            GUILayout.EndVertical();
        }

        private void DrawChachedEventTitle()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", "已配置事件列表"), new GUIStyle { richText = true }, GUILayout.MaxWidth(width));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCachedBoxView()
        {
            GUILayout.Space(5);

            if (ShowErrorTips()) { return; }

            if (eventArray.Length < 1)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUILayout.LabelField("未配置事件", EditorStyles.centeredGreyMiniLabel);
                GUILayout.EndHorizontal();
                return;
            }

            DrawCachedEventBtnsVertical();
        }

        private bool ShowErrorTips()
        {
            if (eventArray == null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                {
                    EditorGUILayout.LabelField(string.Format("<color=red>{0}</color>  ", "勿在修改代码时，配置事件窗口！ 请关闭，重开。"),
                        new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(width));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }

            return eventArray == null;
        }

        private void DrawCachedEventBtnsVertical()
        {
            totalEventCount = eventArray.Length;
            row = (int)(totalEventCount / 3) + 1;

            for (int i = 0; i < row; i++)
            {
                GUILayout.BeginVertical(EditorStyles.textArea);
                {
                    DrawCachedRowBtnsHorizontal(i == 0 ? 0 : i * 3);
                }
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
        }

        private void DrawCachedRowBtnsHorizontal(int startIdx)
        {
            GUILayout.BeginHorizontal();
            {
                for (int j = startIdx; j < startIdx + 3; j++)
                {
                    if (j >= eventArray.Length) { break; }
                    if (GUILayout.Button(eventArray[j].eventName, GUILayout.Width(130)))
                    {
                    }
                    GUILayout.Space(40);
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Function for drawing 'trigger' event list area.
        private void StartDrawDispatchEventArea()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawTriggerEventTitle();

                DrawTriggerEventBoxView();
            }
            GUILayout.EndVertical();
        }

        private void DrawTriggerEventTitle()
        {
            var c = GUI.color;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", "添加发送事件列表"), new GUIStyle { richText = true }, GUILayout.MaxWidth(width));
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(2.0f);
                if (GUILayout.Button("", "OL Plus"))
                {
                    var menu = new GenericMenu();
                    for (int i = 0; i < eventArray.Length; i++)
                    {
                        object[] param = new object[] { eventArray[i].eventName, GoapEventType.Dispatch };
                        menu.AddItem(
                            new GUIContent
                            (
                              eventArray[i].eventName),
                              false,
                              OnAddEvent,
                              param
                            );
                    }
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndVertical();

                //OL Minus
                EditorGUILayout.BeginVertical();
                GUILayout.Space(2.0f);

                GUI.color = _isDeleteDispatchEvt ? Color.red : Color.white;
                if (GUILayout.Button("", "OL Minus"))
                {
                    _isDeleteDispatchEvt = !_isDeleteDispatchEvt;
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        private void DrawTriggerEventBoxView()
        {
            GUILayout.Space(5);

            if (ShowErrorTips()) { return; }

            DrawDispatchRawBtnsVertical();
        }

        private void DrawDispatchRawBtnsVertical()
        {
            sub = _isDeleteDispatchEvt ? 2 : 3;
            totalEventCount = dispatchEvents.Count;
            row = (int)(totalEventCount / sub) + 1;
            for (int i = 0; i < row; i++)
            {
                GUILayout.BeginVertical(EditorStyles.textArea);
                {
                    DrawDispatchRowBtnsHorizontal(i == 0 ? 0 : i * sub);
                }
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
        }

        private void DrawDispatchRowBtnsHorizontal(int startIdx)
        {
            var c = GUI.color;
            GUILayout.BeginHorizontal();
            {
                for (int j = startIdx; j < startIdx + sub; j++)
                {
                    if (j >= dispatchEvents.Count) { break; }
                    if (GUILayout.Button(dispatchEvents[j].eventName, GUILayout.Width(130)))
                    {

                    }
                    if (_isDeleteDispatchEvt)
                    {
                        GUI.color = GoapAIEditorStyle.Red;
                        if (GUILayout.Button("Delete?", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
                        {
                            dispatchEvents.RemoveAt(j);
                        }
                        GUI.color = c;
                    }
                    GUILayout.Space(40);
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Function for drawing 'register' event list area.
        private void StartDrawRegisterEventArea()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawRegisterEventTitle();

                DrawRegisterEventBoxView();
            }
            GUILayout.EndVertical();
        }

        private void DrawRegisterEventTitle()
        {
            var c = GUI.color;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField(string.Format("<color=cyan>{0}</color>  ", "添加监听事件列表"), new GUIStyle { richText = true }, GUILayout.MaxWidth(width));
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(2.0f);
                if (GUILayout.Button("", "OL Plus"))
                {
                    var menu = new GenericMenu();
                    for (int i = 0; i < eventArray.Length; i++)
                    {
                        object[] param = new object[] { eventArray[i].eventName, GoapEventType.Register };
                        menu.AddItem(
                            new GUIContent
                            (
                              eventArray[i].eventName),
                              false,
                              OnAddEvent,
                              param
                            );
                    }
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndVertical();

                //OL Minus
                EditorGUILayout.BeginVertical();
                GUILayout.Space(2.0f);

                GUI.color = _isDeleteRegisterEvt ? Color.red : Color.white;
                if (GUILayout.Button("", "OL Minus"))
                {
                    _isDeleteRegisterEvt = !_isDeleteRegisterEvt;
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        private void DrawRegisterEventBoxView()
        {
            GUILayout.Space(5);

            if (ShowErrorTips()) { return; }

            DrawRegisterRawBtnsVertical();
        }

        private void DrawRegisterRawBtnsVertical()
        {
            sub = _isDeleteRegisterEvt ? 2 : 3;
            totalEventCount = registerEvents.Count;
            row = (int)(totalEventCount / sub) + 1;
            for (int i = 0; i < row; i++)
            {
                GUILayout.BeginVertical(EditorStyles.textArea);
                {
                    DrawRegisterRawBtnsHorizontal(i == 0 ? 0 : i * sub);
                }
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
        }

        private void DrawRegisterRawBtnsHorizontal(int startIdx)
        {
            var c = GUI.color;
            GUILayout.BeginHorizontal();
            {
                for (int j = startIdx; j < startIdx + sub; j++)
                {
                    if (j >= registerEvents.Count) { break; }
                    if (GUILayout.Button(registerEvents[j].eventName, GUILayout.Width(130)))
                    {

                    }
                    if (_isDeleteRegisterEvt)
                    {
                        GUI.color = GoapAIEditorStyle.Red;
                        if (GUILayout.Button("Delete?", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
                        {
                            registerEvents.RemoveAt(j);
                        }
                        GUI.color = c;
                    }
                    GUILayout.Space(40);
                }
            }
            GUILayout.EndHorizontal();
        }

        #endregion;

        private void OnAddEvent(object aValue)
        {
            object[] param = aValue as object[];

            string _name = param[0].ToString();
            GoapEventType t = (GoapEventType)param[1];

            var item = new GoapEventItem
            {
                eventName = _name,
                goapEventType = t
            };

            switch (t)
            {
                case GoapEventType.None:
                    break;
                case GoapEventType.Register:
                    if (!JudgeChachedEvent(item, registerEvents)) registerEvents.Add(item);
                    break;
                case GoapEventType.Dispatch:
                    if (!JudgeChachedEvent(item, dispatchEvents)) dispatchEvents.Add(item);
                    break;
                default:
                    break;
            }
        }

        private bool JudgeChachedEvent(GoapEventItem tar, List<GoapEventItem> owner)
        {
            foreach (var item in owner)
            {
                if (item.eventName.Equals(tar.eventName))
                {
                    return true;
                }
            }

            return false;

        }
    }
}
