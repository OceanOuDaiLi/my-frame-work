namespace Goap.AI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class GoapAIEditorCommandWindow : EditorWindow
    {
        #region Private Static Editor Variables

        static int tab = 0;

        private static int x = 0;
        private static int y = 0;
        private static int width = 1920;
        private static int height = 1080;

        private const string windowTitle = "指令编辑面板";

        private static int saveIdx;
        private static Action<GoapAIScenario> action;

        private static List<CommandNode> _nodes;
        static GoapAIScenario scenario;
        static GoapAIEditorCommandWindow window;
        #endregion

        #region Private Static Command Variables

        private static float rectWidth = 200;
        private static float rectHeight = 100;
        private static float vertSub = rectHeight + 50;
        private static float horiSub = rectWidth + 50;
        private static float startX = 20;
        private static int row = 1;
        private static Vector2 startPos = new Vector2(20, 30);
        #endregion

        public static void OpenCommandEditorWindow(GoapAIScenario current, Action<GoapAIScenario> save)
        {
            scenario = current;

            window = GetWindow<GoapAIEditorCommandWindow>(false, windowTitle, true);
            x = (Screen.currentResolution.width - width) / 2;
            y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);
            window.maxSize = new Vector2(width, height);

            _nodes = new List<CommandNode>();

            for (int i = 0, n = scenario.actions.Length; i < n; i++)
            {
                _nodes.Add(CreateCommandNode(scenario.actions[i]));
            }

            window.Show();
        }

        #region Unity Calls

        private void OnGUI()
        {
            row = 1;
            startX = 20;
            horiSub = rectWidth + 50;
            vertSub = rectHeight + 50;
            startPos = new Vector2(20, 30);
            for (int i = 0; i < _nodes.Count; i++)
            {
                Draw(i);
            }
        }

        private void OnDisable()
        {
            //action?.Invoke(scenario);
        }

        #endregion

        private static CommandNode CreateCommandNode(GoapAIScenarioAction ac)
        {
            CommandNode tar = new CommandNode();
            tar.Name = ac.name;
            tar.Commands = ac.commands;
            tar.Style = CreateNodeStyle("node1.png");

            return tar;
        }

        #region GUI Style

        private static GUIStyle CreateNodeStyle(string aTextureName)
        {
            var style = new GUIStyle();
            style.normal.background = (EditorGUIUtility.isProSkin)
                ? (Texture2D)EditorGUIUtility.Load($"builtin skins/darkskin/images/{aTextureName}")
                : (Texture2D)EditorGUIUtility.Load($"builtin skins/lightskin/images/{aTextureName}");
            style.border = new RectOffset(12, 12, 12, 12);
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.padding = new RectOffset(12, 0, 10, 0);
            style.normal.textColor = new Color(0.639f, 0.65f, 0.678f);
            return style;
        }

        private static GUIStyle CreateStyle(string aTextureName)
        {
            var style = new GUIStyle(aTextureName);
            style.border = new RectOffset(12, 12, 12, 12);
            style.padding = new RectOffset(12, 0, 10, 0);
            return style;
        }

        #endregion

        private static void Draw(int idx)
        {
            Rect rect = new Rect();
            rect.width = rectWidth;
            rect.height = rectHeight;

            if (idx % 3 == 0 && idx != 0)
            {
                startPos.x = startX;
                startPos.y += vertSub;
                row++;
            }
            else
            {
                startPos.x += horiSub;
            }

            rect.position = startPos;

            Rect content = new Rect();
            content = new Rect(rect.position.x + 7, rect.position.y + 30.0f, rectWidth - 14.0f, rectHeight - 40.0f);

            GUI.Box(rect, "", _nodes[idx].Style);

            // Title
            GUI.Label(
                new Rect(rect.x + 12.0f, rect.y + 12.0f, rect.y + 12.0f, rect.width - 24.0f),
                _nodes[idx].Name,
                _nodes[idx].TitleStyle
            );

            content.x = rect.x + 7.0f;
            content.y = rect.y + 30.0f;
            content.width = rect.width - 14.0f;
            content.height = rect.height - 50.0f;
            GUI.Box(content, "", CreateStyle("IN BigTitle"));
        }

        public class CommandNode
        {
            public string Name { get; set; }
            public GoapCommandItem[] Commands { get; set; }
            public GUIStyle Style { get; set; }
            public GUIStyle TitleStyle = new GUIStyle();

            public CommandNode()
            {
                TitleStyle.fontStyle = FontStyle.Bold;
                if (EditorGUIUtility.isProSkin)
                {
                    TitleStyle.normal.textColor = Color.white;
                }
            }
        }
    }
}
