using UnityEditor;
using UnityEngine;

/// <summary>
/// Author£∫Daili.OU
/// Created Time£∫2022/05/06
/// Descriptions£∫
namespace AI.Tools
{
    public class UVBrush : BaseBrush
    {
        // Private Variables
        Rect rect;
        static UVBrush window;

        #region Initialize Windows

        public static UVBrush ShowWindow()
        {
            window = (UVBrush)EditorWindow.GetWindow(typeof(UVBrush), false, "UV Brush Window");
            window.autoRepaintOnSceneChange = true;

            Vector2 rect = new Vector2(1247, 914);
            window.position = new Rect(200, 50, rect.x, rect.y);
            window.minSize = rect;
            window.maxSize = rect;

            return window;
        }

        public static void OpenBrushConfig(string aName)
        {
            ShowWindow().SelectPresetHandler(aName);
        }

        #endregion

        #region Unity Calls

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void OnGUI()
        {
            if (_isLoadedBrushCfg)
            {
                SetCurrentBrushCfg(_current);
                _isLoadedBrushCfg = false;
            }

            GUILayout.BeginHorizontal();
            {
                DrawLeftPanel();

                DrawMiddlePanle();

                DrawRightPanle();
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Draw GUI

        private void DrawLeftPanel()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(_leftPaneWidth));
                {
                    GUILayout.BeginVertical(GUILayout.Width(_leftPaneWidth));
                    {
                        DrawPreset();

                        if (!_loadedModelObject)
                        {

                        }
                        else 
                        {
                            
                        }

                        //DrawAutoSetModelFigure();

                        //GUILayout.Space(5);

                        //if (_loadedModelObject)
                        //{
                        //    DrawRenderType();

                        //    GUILayout.Space(5);

                        //    DrawColorEditor();

                        //    GUILayout.Space(5);
                        //}

                        //if (!_isVertexColorPainting)
                        //{
                        //    DrawLoad();

                        //    DrawObject();
                        //}

                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GuiDrawer.DrawLine(
                    new Vector2(_leftPaneWidth + 1, 0.0f),
                    new Vector2(_leftPaneWidth + 1, position.height),
                    Color.white,
                    2.0f
                );
            }
            GUILayout.EndHorizontal();
        }

        private void DrawMiddlePanle()
        {
            GUILayout.BeginVertical();
            {
                //DrawPreviewMesh2D();

                GUILayout.BeginHorizontal(GUILayout.Width(_leftPaneWidth));
                {
                    GuiDrawer.DrawLine(
                        new Vector2(_middlePaneWidth + 1, 0.0f),
                        new Vector2(_middlePaneWidth + 1, position.height),
                        Color.white,
                        2.0f
                    );
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawRightPanle()
        {
            GUILayout.BeginVertical(GUILayout.Width(_middlePaneWidth));
            {
                //DrawPreviewMesh3D();

                GUILayout.BeginHorizontal(GUILayout.Width(_middlePaneWidth));
                {

                    GuiDrawer.DrawLine(
                        new Vector2(_middlePaneWidth + 520, 0.0f),
                        new Vector2(_middlePaneWidth + 520, position.height),
                        Color.white,
                        2.0f
                    );
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawPreviewMesh3D()
        {
            rect = new Rect(_middlePaneWidth + 10, 10, 500, position.height - 21);
            if (!CheckObjectLoaded(rect, 108))
            {
                return;
            }

            UVBrushMeshEditor.DrawRightPreviewWindow(skRender.sharedMesh, rect);
        }

        private void DrawPreviewMesh2D()
        {
            rect = new Rect(_leftPaneWidth + 10, 10, 500, position.height - 21);
            if (!CheckObjectLoaded(rect))
            {
                return;
            }

            UVBrushMeshEditor.DrawMiddlePreviewWindow(skRender.gameObject, skRender.sharedMesh, skRender.gameObject, rect, window);
        }

        private bool CheckObjectLoaded(Rect rect, float sub = 0)
        {
            //if (!_loadedModelObject || _curObj == null || skRender == null)
            //{
            //    GUI.Box(rect, "");

            //    GUILayout.Space(position.height / 2);

            //    EditorGUILayout.BeginHorizontal(EditorStyles.label);

            //    GUILayout.Space(195 + sub);
            //    EditorGUILayout.LabelField("Œﬁ‘§¿¿ƒ⁄»›", _titleStyle);

            //    EditorGUILayout.EndHorizontal(); 

            //    return false;
            //}

            return true;
        }

        #endregion
    }
}