namespace Goap.AI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class GoapAIPropertyWindow : EditorWindow
    {
        #region Private Static Variables

        private static int x = 0;
        private static int y = 0;
        private static int width = 528;
        private static int height = 513;
        private const string windowTitle = "Property Editor";

        private static bool showShootWay = true;
        private static bool showTurnType = false;

        static GoapProperty curProperty;
        static GoapAIPropertyWindow window;

        private static Action<GoapProperty> down = null;
        #endregion

        public static void OpenGoapPropertyEditorWindow(Action<GoapProperty> action, GoapProperty cur)
        {
            down = action;
            curProperty = cur;


            window = GetWindow<GoapAIPropertyWindow>(false, windowTitle, true);
            x = (Screen.currentResolution.width - width) / 2;
            y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);
            window.maxSize = new Vector2(width, height);
            window.minSize = new Vector2(width, height);

            window.Show();
        }

        private void OnDisable()
        {
            //down?.Invoke();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(15.0f);
            DrawShootWay();

            GUILayout.Space(15.0f);
            DrawTurnType();
        }

        void DrawShootWay()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(width));
            {
                GUILayout.FlexibleSpace();
                showShootWay = GUILayout.Toggle(showShootWay, "     编辑出手方式", GUILayout.Width(width));
            }
            EditorGUILayout.EndHorizontal();

            if (showShootWay)
            {
                GUILayout.Space(8);
                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < curProperty.wsyModels.Length; i++)
                    {

                        curProperty.wsyModels[i].selected = GUILayout.Toggle(curProperty.wsyModels[i].selected
                            , GetWayType(curProperty.wsyModels[i].way)
                            , GUILayout.Width(width));

                        GUILayout.Space(2);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        void DrawTurnType()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.FlexibleSpace();
                showTurnType = GUILayout.Toggle(showTurnType, "     编辑回合", GUILayout.Width(width));
            }
            EditorGUILayout.EndHorizontal();

            if (showTurnType)
            {
                GUILayout.Space(8);
                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < curProperty.turnModels.Length; i++)
                    {

                        curProperty.turnModels[i].selected = GUILayout.Toggle(curProperty.turnModels[i].selected
                            , GetTurnType(curProperty.turnModels[i].turnType)
                            , GUILayout.Width(width));

                        GUILayout.Space(2);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        string GetWayType(ShootWay w)
        {
            string str = string.Empty;

            switch (w)
            {
                case ShootWay.PG_LAYUP:
                    str = "控球后卫(PG)上篮";
                    break;
                case ShootWay.PG_CIC:
                    str = "控球后卫(PG)中投";
                    break;
                case ShootWay.PG_TRISECTION:
                    str = "控球后卫(PG)三分";
                    break;
                case ShootWay.SG_LAYUP:
                    str = "得分后卫(SG)上篮";
                    break;
                case ShootWay.SG_CIC:
                    str = "得分后卫(SG)中投";
                    break;
                case ShootWay.SG_TRISECTION:
                    str = "得分后卫(SG)三分";
                    break;
                case ShootWay.SF_LAYUP:
                    str = "小前锋(SF)上篮";
                    break;
                case ShootWay.SF_CIC:
                    str = "小前锋(SF)中投";
                    break;
                case ShootWay.SF_TRISECTION:
                    str = "小前锋(SF)三分";
                    break;
                case ShootWay.PF_LAYUP:
                    str = "大前锋(PF)上篮";
                    break;
                case ShootWay.PF_CIC:
                    str = "大前锋(PF)中投";
                    break;
                case ShootWay.PF_TRISECTION:
                    str = "大前锋(PF)三分";
                    break;
                case ShootWay.C_LAYUP:
                    str = "中锋(C)上篮";
                    break;
                case ShootWay.C_CIC:
                    str = "中锋(C)中投";
                    break;
                case ShootWay.C_TRISECTION:
                    str = "中锋(C)三分";
                    break;
                default:
                    break;
            }

            return str;
        }

        string GetTurnType(TurnType t)
        {
            string str = string.Empty;

            switch (t)
            {
                case TurnType.SCORE:
                    str = "得分回合";
                    break;
                case TurnType.REBOUNDS:
                    str = "篮板回合合";
                    break;
                case TurnType.BLOCKS:
                    str = "盖帽回合";
                    break;
                default:
                    break;
            }

            return str;

        }
    }
}