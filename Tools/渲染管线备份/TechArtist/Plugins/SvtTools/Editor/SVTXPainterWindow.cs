using AI.Tools;
using UnityEngine;
using UnityEditor;
using SVTXPainter;

//引用 ： https://github.com/alpacasking/SimpleVertexPainter
namespace SVTXPainterEditor
{
    public enum SelectType
    {
        HEAD = 0,
        COLTH = 1,
        LIMBS = 2,
        SHOES = 3,
    }

    public class SVTXPainterWindow : EditorWindow
    {
        #region Variables
        private GUIStyle titleStyle;
        // Helper
        private bool isRecord = false;
        private bool isPainting = false;
        private bool allowSelect = false;
        private bool allowPainting = false;
        private bool changingBrushValue = false;

        public bool[] _flodOut = new bool[4] { false, false, true, false };

        // Physics
        private RaycastHit curHit;
        private Vector2 mousePos = Vector2.zero;
        private Vector2 lastMousePos = Vector2.zero;

        // GUI
        private float brushSize = .1f;
        private float brushOpacity = .3f;
        private float brushFalloff = 0.1f;
        public const float MaxBrushSize = 0.1f;
        private const float MinBrushSize = 0.01f;
        private Color brushColor = new Color(.99f, .99f, .99f, 1);

        // Data
        private Mesh curMesh;
        private SVTXObject m_target;
        private GameObject m_active;

        private static UvBrushConfig customCfg;
        private static BaseBrush baseBrush;
        private static Mesh[] meshes = new Mesh[4];
        private static SVTXObject[] svtxObjs = new SVTXObject[4];

        public static SVTXPainterWindow PainterWindow;
        private SelectType selectType = SelectType.LIMBS;

        #endregion

        #region Unity Calls / Init
        public static void LauchVertexPainter(BaseBrush _baseBrush)
        {
            baseBrush = _baseBrush;
            meshes[0] = _baseBrush.headRender.sharedMesh;
            meshes[1] = _baseBrush.clothRender.sharedMesh;
            meshes[2] = _baseBrush.limbsRender.sharedMesh;
            meshes[3] = _baseBrush.shoesRender.sharedMesh;

            svtxObjs[0] = _baseBrush.headRender.gameObject.GetComponent<SVTXObject>();
            svtxObjs[1] = _baseBrush.clothRender.gameObject.GetComponent<SVTXObject>();
            svtxObjs[2] = _baseBrush.limbsRender.gameObject.GetComponent<SVTXObject>();
            svtxObjs[3] = _baseBrush.shoesRender.gameObject.GetComponent<SVTXObject>();

            PainterWindow = EditorWindow.GetWindow<SVTXPainterWindow>();
            PainterWindow.minSize = new Vector2(492, 368);
            PainterWindow.maxSize = new Vector2(492, 368);
            PainterWindow.titleContent = new GUIContent("Vertex Painter");

            PainterWindow.Show();
            PainterWindow.SetSelectionObj();
            PainterWindow.GenerateStyles();

        }

        private void OnEnable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            SceneView.duringSceneGui += this.OnSceneGUI;
            if (titleStyle == null)
            {
                GenerateStyles();
            }
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            baseBrush.OnClosePaniterWindow();
        }

        private void SetSelectionObj()
        {
            curMesh = meshes[(int)selectType];
            m_target = svtxObjs[(int)selectType];
            m_active = svtxObjs[(int)selectType].gameObject;

            allowSelect = (m_target == null);

            Repaint();
        }

        private void OnSelectionChange()
        {
            SetSelectionObj();
        }

        #endregion

        #region GUI Methods

        private void OnGUI()
        {
            //Header
            GUILayout.BeginHorizontal();
            GUILayout.Box("Vertex Painter", titleStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            //Body
            GUILayout.BeginVertical(GUI.skin.box);
            {
                // 1.Draw Editor Toggle
                allowPainting = GUILayout.Toggle(allowPainting, "编辑模式");

                GUILayout.Space(20);

                if (allowPainting)
                {
                    PainterWindow.minSize = new Vector2(492, 440);
                    PainterWindow.maxSize = new Vector2(492, 440);
                    Tools.current = Tool.None;

                    GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(100));
                    {
                        // head
                        GUIStyle styleHead = new GUIStyle(EditorStyles.toolbarButton);
                        styleHead.alignment = TextAnchor.MiddleCenter;

                        styleHead.normal.textColor = _flodOut[0] ? Color.green : Color.white;
                        styleHead.hover.textColor = _flodOut[0] ? Color.green : Color.white;
                        styleHead.fontSize = 12;
                        if (GUILayout.Button("头部", styleHead))
                        {
                            SetFlodOut(0);
                            selectType = SelectType.HEAD;
                        }

                        // cloth
                        GUIStyle styleCloth = new GUIStyle(EditorStyles.toolbarButton);
                        styleCloth.alignment = TextAnchor.MiddleCenter;

                        styleCloth.hover.textColor = _flodOut[1] ? Color.green : Color.white;
                        styleCloth.normal.textColor = _flodOut[1] ? Color.green : Color.white;
                        styleCloth.fontSize = 12;
                        if (GUILayout.Button("身体", styleCloth))
                        {
                            SetFlodOut(1);
                            selectType = SelectType.COLTH;
                        }

                        // limbs
                        GUIStyle styleLimbs = new GUIStyle(EditorStyles.toolbarButton);
                        styleLimbs.alignment = TextAnchor.MiddleCenter;

                        styleLimbs.hover.textColor = _flodOut[2] ? Color.green : Color.white;
                        styleLimbs.normal.textColor = _flodOut[2] ? Color.green : Color.white;
                        styleLimbs.fontSize = 12;
                        if (GUILayout.Button("四肢", styleLimbs))
                        {
                            SetFlodOut(2);
                            selectType = SelectType.LIMBS;
                        }

                        // shoes
                        GUIStyle styleShoes = new GUIStyle(EditorStyles.toolbarButton);
                        styleShoes.alignment = TextAnchor.MiddleCenter;

                        styleShoes.hover.textColor = _flodOut[3] ? Color.green : Color.white;
                        styleShoes.normal.textColor = _flodOut[3] ? Color.green : Color.white;
                        styleShoes.fontSize = 12;
                        if (GUILayout.Button("鞋子", styleShoes))
                        {
                            SetFlodOut(3);
                            selectType = SelectType.SHOES;
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);
                }
                else
                {
                    PainterWindow.minSize = new Vector2(492, 400);
                    PainterWindow.maxSize = new Vector2(492, 400);
                }

                // 2.Draw Select type
                GUILayout.BeginHorizontal();
                {
                    brushColor = EditorGUILayout.ColorField("笔刷颜色:", brushColor);
                    if (GUILayout.Button("纯色填充Mesh"))
                    {
                        FillVertexColor();
                    }
                }
                GUILayout.EndHorizontal();

                brushSize = EditorGUILayout.Slider("笔刷大小:", brushSize, MinBrushSize, MaxBrushSize);
                brushOpacity = EditorGUILayout.Slider("笔刷透明度:", brushOpacity, 0, 1);
                brushFalloff = EditorGUILayout.Slider("笔刷衰减度:", brushFalloff, MinBrushSize, brushSize);

                GUILayout.Space(5);

                // 3.Draw TargetConfig

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(65));
                {
                    if (GUILayout.Button("加载指定配置", baseBrush._badgeStyle))
                    {
                        if (customCfg != null) { }
                            //baseBrush._current.SetToConfigColor(baseBrush, customCfg);
                    }
                }
                EditorGUILayout.EndHorizontal();
                customCfg = (UvBrushConfig)EditorGUILayout.ObjectField(customCfg, typeof(UvBrushConfig), false);


                // 3.Draw Tips
                GUILayout.Space(20);
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 12;
                GUILayout.Label("Tips" + "\n" +
                    "Z 键 :开启/关闭 编辑模式" +
                    "\n" +
                    "鼠标左键: 喷涂"
                    , style);

                // 4.Draw Button Save
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(PainterWindow.position.width));
                {
                    if (GUILayout.Button("保存", baseBrush._badgeStyle))
                    {
                        baseBrush._current.SaveChangeColor(baseBrush);
                        baseBrush.OnClosePaniterWindow();
                        baseBrush.ConfirmAlert();
                        PainterWindow.Close();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // 5.Draw Button Revert
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(PainterWindow.position.width));
                {
                    if (GUILayout.Button("还原配置", baseBrush._badgeStyle))
                    {
                        //baseBrush._current.SetToConfigColor(baseBrush);
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(PainterWindow.position.width));
                {
                    if (GUILayout.Button("还原模型", baseBrush._badgeStyle))
                    {
                        //baseBrush._current.RevertToDefaultColor(baseBrush);
                    }
                }
                EditorGUILayout.EndHorizontal();

                Repaint();
            }
            GUILayout.EndVertical();
        }

        void SetFlodOut(int index)
        {
            for (int i = 0; i < _flodOut.Length; i++)
            {
                _flodOut[i] = index == i;
            }
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (allowPainting)
            {
                bool isHit = false;
                if (!allowSelect)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                }
                Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);
                if (m_target != null && curMesh != null)
                {
                    Matrix4x4 mtx = m_target.transform.localToWorldMatrix;
                    RaycastHit tempHit;
                    isHit = RXLookingGlass.IntersectRayMesh(worldRay, curMesh, mtx, out tempHit);
                    if (isHit)
                    {
                        if (!changingBrushValue)
                        {
                            curHit = tempHit;
                        }
                        if (isPainting && m_target.isActiveAndEnabled && !changingBrushValue)
                        {
                            PaintVertexColor();
                        }
                    }
                }

                if (isHit || changingBrushValue)
                {

                    Handles.color = new Color(brushColor.r, brushColor.g, brushColor.b, brushOpacity); ;
                    Handles.DrawSolidDisc(curHit.point, curHit.normal, brushSize);
                    Handles.color = new Color(1 - brushColor.r, 1 - brushColor.g, 1 - brushColor.b, 1);
                    Handles.DrawWireDisc(curHit.point, curHit.normal, brushSize);
                    Handles.DrawWireDisc(curHit.point, curHit.normal, brushFalloff);
                }
            }

            ProcessInputs();

            sceneView.Repaint();

        }

        private void OnInspectorUpdate()
        {
            SetSelectionObj();
        }

        #endregion

        #region TempPainter Method

        void PaintVertexColor()
        {
            if (m_target && m_active)
            {
                curMesh = SVTXPainterUtils.GetMesh(m_active);
                if (curMesh)
                {
                    if (isRecord)
                    {
                        m_target.PushUndo();
                        isRecord = false;
                    }
                    Vector3[] verts = curMesh.vertices;
                    Color[] colors = new Color[0];
                    if (curMesh.colors.Length > 0)
                    {
                        colors = curMesh.colors;
                    }
                    else
                    {
                        colors = new Color[verts.Length];
                    }
                    for (int i = 0; i < verts.Length; i++)
                    {
                        Vector3 vertPos = m_target.transform.TransformPoint(verts[i]);
                        float mag = (vertPos - curHit.point).magnitude;
                        if (mag > brushSize)
                        {
                            continue;
                        }
                        float falloff = SVTXPainterUtils.LinearFalloff(mag, brushSize);
                        falloff = Mathf.Pow(falloff, Mathf.Clamp01(1 - brushFalloff / brushSize)) * brushOpacity;

                        colors[i] = SVTXPainterUtils.VTXColorLerp(colors[i], brushColor, falloff);
                    }
                    curMesh.colors = colors;
                }
                else
                {
                    SetSelectionObj();
                    Debug.LogWarning("Nothing to paint!");
                }

            }
            else
            {
                SetSelectionObj();
                Debug.LogWarning("Nothing to paint!");
            }
        }

        void FillVertexColor()
        {
            if (curMesh)
            {
                Vector3[] verts = curMesh.vertices;
                Color[] colors = new Color[0];
                if (curMesh.colors.Length > 0)
                {
                    colors = curMesh.colors;
                }
                else
                {
                    colors = new Color[verts.Length];
                }
                for (int i = 0; i < verts.Length; i++)
                {
                    colors[i] = brushColor;

                }
                curMesh.colors = colors;
            }
            else
            {
                Debug.LogWarning("Nothing to fill!");
            }
        }

        #endregion

        #region Utility Methods

        void ProcessInputs()
        {
            if (m_target == null)
            {
                return;
            }
            Event e = Event.current;
            mousePos = e.mousePosition;
            if (e.type == EventType.KeyDown)
            {
                if (e.isKey)
                {
                    if (e.keyCode == KeyCode.Z)
                    {
                        allowPainting = !allowPainting;
                        if (allowPainting)
                        {
                            Tools.current = Tool.None;
                        }
                    }
                }
            }
            if (e.type == EventType.MouseUp)
            {
                changingBrushValue = false;
                isPainting = false;

            }
            if (lastMousePos == mousePos)
            {
                isPainting = false;
            }
            if (allowPainting)
            {
                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && !e.control && e.button == 0 && !e.shift && !e.alt)
                {
                    isPainting = true;
                    if (e.type == EventType.MouseDown)
                    {
                        isRecord = true;
                    }
                }
            }
            lastMousePos = mousePos;
        }

        void GenerateStyles()
        {
            titleStyle = new GUIStyle();
            titleStyle.normal.textColor = Color.white;
            titleStyle.border = new RectOffset(3, 3, 3, 3);
            titleStyle.margin = new RectOffset(2, 2, 2, 2);

            titleStyle.fontSize = 25;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
        }

        #endregion
    }

}