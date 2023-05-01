using System;
using UnityEditor;
using UnityEngine;

namespace AI.Tools
{
    public static class GuiDrawer
    {

        public static void DrawColoredGUIBox(Rect aBoxRect, Color aBoxColor)
        {
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = aBoxColor;
            GUI.Box(aBoxRect, GUIContent.none, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = Texture2D.whiteTexture
                }
            });

            GUI.backgroundColor = backgroundColor;
        }

        public static void DrawColoredGUIBoxWithBorder(Rect aBoxRect, Color aBoxColor, float aBorderThickness, Color aBorderColor)
        {
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = aBoxColor;
            GUI.Box(aBoxRect, GUIContent.none, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = Texture2D.whiteTexture
                }
            });

            GUI.backgroundColor = backgroundColor;

            Vector2 topLeft = aBoxRect.position;
            Vector2 topRight = new Vector2(aBoxRect.position.x + aBoxRect.width, aBoxRect.position.y);
            Vector2 bottomLeft = new Vector2(aBoxRect.position.x, aBoxRect.position.y + aBoxRect.height);
            Vector2 bottomRight = new Vector2(aBoxRect.position.x + aBoxRect.width, aBoxRect.position.y + aBoxRect.height);

            DrawLine(topLeft, topRight, aBorderColor, aBorderThickness);
            DrawLine(topRight, bottomRight, aBorderColor, aBorderThickness);
            DrawLine(bottomRight, bottomLeft, aBorderColor, aBorderThickness);
            DrawLine(bottomLeft, topLeft, aBorderColor, aBorderThickness);
        }

        public static string AddHorizontalSeperationLine()
        {
            return EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }

        public static void AddVerticalSeperatorLine()
        {
            GUI.color = Color.gray;
            EditorGUILayout.LabelField("|", EditorStyles.boldLabel, GUILayout.MaxWidth(10f), GUILayout.MaxHeight(4f));
            GUI.color = Color.white;
        }

        public static void DrawLine(Vector2 aStartPos, Vector2 aEndPos, Color aColour, float aThickness)
        {
            Handles.color = aColour;
            Handles.DrawAAPolyLine(aThickness, new Vector3[]
            {
                aStartPos,
                aEndPos
            });

            Handles.color = Color.white;
        }

        public static void DrawBackgroundGrid(Rect aScrollViewRect, Vector2 aScrollPos, float aGridSquareWidth,
            Color aLineColour, float aLineThickness)
        {
            DrawBackgroundGrid(aScrollViewRect, aScrollPos, aGridSquareWidth, aLineColour, aLineThickness / 2.5f, 1, 0.1f);
        }

        public static void DrawBackgroundGrid(Rect aScrollViewRect, Vector2 aScrollPos, float aGridSquareWidth,
            Color aLineColour, float aLineThickness, int aThickerLineInterval, float aOpacity)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var colour = new Color(aLineColour.r, aLineColour.g, aLineColour.b, aOpacity);

                Vector2 offset = new Vector2(
                    Mathf.Abs(aScrollPos.x % aGridSquareWidth - aGridSquareWidth),
                    Mathf.Abs(aScrollPos.y % aGridSquareWidth - aGridSquareWidth)
                );

                int numXLines = Mathf.CeilToInt((aScrollViewRect.width + (aGridSquareWidth - offset.x)) / aGridSquareWidth);
                int numYLines = Mathf.CeilToInt((aScrollViewRect.height + (aGridSquareWidth - offset.y)) / aGridSquareWidth);

                for (int x = 0; x < numXLines; x++)
                {
                    float lineThicknessToUse = (x % aThickerLineInterval == 0)
                        ? aLineThickness * 2.5f
                        : aLineThickness;

                    DrawLine(
                        new Vector2(offset.x + (x * aGridSquareWidth) + aScrollViewRect.x, 0f) + aScrollPos,
                        new Vector2(offset.x + (x * aGridSquareWidth) + aScrollViewRect.x, aScrollViewRect.height) + aScrollPos,
                        colour,
                        lineThicknessToUse
                    );
                }

                for (int y = 0; y < numYLines; y++)
                {
                    float lineThicknessToUse = (y % aThickerLineInterval == 0)
                        ? aLineThickness * 2.5f
                        : aLineThickness;

                    DrawLine(
                        new Vector2(aScrollViewRect.x, offset.y + (y * aGridSquareWidth) + aScrollViewRect.y) + aScrollPos,
                        new Vector2(aScrollViewRect.x + aScrollViewRect.width, offset.y + (y * aGridSquareWidth) + aScrollViewRect.y) + aScrollPos,
                        colour,
                        lineThicknessToUse
                    );
                }
            }
        }

        public static void DrawNodeCurve(Rect aStart, Rect aEnd)
        {
            DrawNodeCurve(aStart, aEnd, Color.black, 0.5f);
        }

        public static void DrawNodeCurve(Rect aStart, Rect aEnd, Color aColor, float aCurveStrength)
        {
            Vector3 startPos = new Vector3(aStart.x + aStart.width * 0.5f, aStart.y + aStart.height * 0.5f, 0.0f);
            Vector3 endPos = new Vector3(aEnd.x + aEnd.width * 0.5f, aEnd.y + aEnd.height * 0.5f, 0.0f);
            Vector3 startTan = startPos + Vector3.right * (aCurveStrength * 100.0f);
            Vector3 endTan = endPos + Vector3.left * (aCurveStrength * 100.0f);
            Color shadowCol = new Color(0.0f, 0.0f, 0.0f, 0.1f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5.0f);
            }

            Handles.color = aColor;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, aColor, null, 2.0f);
            Handles.color = Color.white;
        }

        public static void DrawNodeLine(Rect start, Rect end, Color color)
        {
            Vector3 startPos = new Vector3(start.x + start.width * 0.5f, start.y + start.height * 0.5f, 0.0f);
            Vector3 endPos = new Vector3(end.x + end.width * 0.5f, end.y + end.height * 0.5f, 0.0f);

            Handles.color = color;
            Handles.DrawLine(startPos, endPos);
            Handles.color = Color.white;
        }
    }

    public static class GUIDrawerStyles
    {
        public static Color backgroundColor = new Color32(71, 71, 71, 255);
        public static Color gridColor = new Color32(100, 100, 100, 255);
        public static Color wireframeColor = new Color32(255, 255, 255, 255);
        public static Color wireframeColor2 = new Color32(93, 118, 154, 255);

        public static GUIStyle logoFont;

        public static GUIStyle hudFont;

        public static GUIStyle tooltip;

        public static string[] uvSetNames = Enum.GetNames(typeof(UVSet));
        public static string[] colorChannelsNames = Enum.GetNames(typeof(ColorChannels));
        public static string[] previewTextureSourceNames = Enum.GetNames(typeof(PreviewTextureSource));

        public static GUIStyle buttonLeft;
        public static GUIStyle buttonMid;
        public static GUIStyle buttonRight;

        public const int kSubMeshButtonWitdh = 30;
        public static string[] subMeshLabels = new string[32];

        public static Color foldoutTintColor;

        static GUIDrawerStyles()
        {
            logoFont = new GUIStyle(EditorStyles.label);
            logoFont.alignment = TextAnchor.MiddleCenter;
            logoFont.fontSize = 20;

            hudFont = new GUIStyle(EditorStyles.boldLabel);
            hudFont.alignment = TextAnchor.LowerCenter;
            hudFont.normal.textColor = Color.white;

            buttonLeft = GUI.skin.GetStyle("buttonLeft");
            buttonMid = GUI.skin.GetStyle("buttonMid");
            buttonRight = GUI.skin.GetStyle("buttonRight");

            tooltip = GUI.skin.GetStyle("tooltip");

            for (int i = 0; i < 32; i++)
                subMeshLabels[i] = "#" + i.ToString();

            foldoutTintColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.05f) : new Color(0f, 0f, 0f, 0.05f);
        }
    }

    #region UV Brush Mesh Enum

    enum UVSet
    {
        UV1 = 0, UV2 = 1, UV3 = 2, UV4 = 3
    }

    enum PreviewTextureSource
    {
        None,
        Material,
        Custom,
    }

    enum ColorChannels
    {
        R, G, B, A, All,
    }

    #endregion
}