using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author：Daili.OU
/// Created Time：2022/05/14
/// Descriptions：
namespace AI.Tools
{
    public class UVBrushMeshEditor : Editor
    {
        class BrushMeshInfo
        {
            public int vertexCount = 0;
            public int subMeshCount = 0;
            public int triangleCount = 0;

            public bool hasColors = false;
            public bool[] hasUV = { false, false, false, false };

            public Mesh mesh = null;
            public Color32[] color = null;
            public List<Vector2> uv = null;
            public UnityEngine.Object obj = null;
            public GameObject gameObject = null;
        }

        #region Private Variables -> Preview 3D

        public static bool ShowWireFrame = false;

        static float m_PreviewWindowScale3D = 0.53f;
        static readonly float m_PreviewDistance = 10.0f;

        static Rect m_ViewportRect3D = new Rect();
        static Vector2 m_Drag3D = new Vector2(0, -0);
        static Vector2 m_ScroolPosition3D = Vector2.zero;

        static Material m_ColorMat;
        static readonly string VertexColorShader = "FTX/UvBrush//MeshColor";
        static Material colorMat
        {
            get
            {
                if (m_ColorMat == null)
                    m_ColorMat = new Material(Shader.Find(VertexColorShader));
                return m_ColorMat;
            }
        }

        static Material m_WireMat;
        static readonly string WireShaderPath = "FTX/UvBrush/MeshWire";
        static Material wireMat
        {
            get
            {
                if (m_WireMat == null)
                    m_WireMat = new Material(Shader.Find(WireShaderPath));
                return m_WireMat;
            }
        }

        static PreviewRenderUtility m_PreviewGUI;
        public static PreviewRenderUtility PreviewGUI
        {
            get
            {
                if (m_PreviewGUI == null)
                {
                    m_PreviewGUI = new PreviewRenderUtility();
                    m_PreviewGUI.camera.farClipPlane = 500;
                    m_PreviewGUI.camera.clearFlags = CameraClearFlags.SolidColor;
                    m_PreviewGUI.camera.transform.position = new Vector3(0, 0, -m_PreviewDistance);
                }
                return m_PreviewGUI;
            }
            set
            {
                m_PreviewGUI = value;
            }
        }

        #endregion

        #region Private Variables -> Preview 2D

        // Mouse Helper
        static bool m_UVHit = false;
        static float m_UVHitPointSize = 7f;
        static Vector2 m_UVHitPosition = new Vector2();
        static Vector2 m_ScroolPosition2D = Vector2.zero;
        static string m_UVHitTooltipText = string.Empty;

        // GUI 
        static UVSet m_PreviewUVSet = UVSet.UV1;
        static float m_PreviewWindowScale2D = 1.8f;
        static Vector2 m_PreviewWindowPosition = new Vector2(-0.5f, -0.5f);
        static int s_UVPreviewWindowHash = "FTX.UVBrush.PreviewWindow".GetHashCode();

        //Data
        static UVBrush window;
        static Material m_uvPreviewMaterial = null;
        static Material m_SimpleMaterial = null;
        static BrushMeshInfo meshInfo = null;

        static Rect m_ViewportRect2D = new Rect();
        static GUIContent s_TempContent = new GUIContent();

        #endregion

        #region Private Methods Set Gui Infomation -> Preview 2D

        static bool LoadMaterials()
        {
            Shader uvPreviewShader = Shader.Find("FTX/UvBrush/MeshPreview");
            Shader simpleShader = Shader.Find("FTX/UvBrush/TextureBlend");

            if (uvPreviewShader == null || simpleShader == null)
            {
                return false;
            }

            m_uvPreviewMaterial = new Material(uvPreviewShader);
            m_uvPreviewMaterial.hideFlags = HideFlags.HideAndDontSave;

            m_SimpleMaterial = new Material(simpleShader);
            m_SimpleMaterial.hideFlags = HideFlags.HideAndDontSave;
            return true;
        }

        static void SetMaterialKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        static void SetMeshInfo(UnityEngine.Object obj, Mesh mesh, GameObject gameObject)
        {
            meshInfo = new BrushMeshInfo();
            meshInfo.obj = obj;
            meshInfo.gameObject = gameObject;
            meshInfo.mesh = mesh;

            //set triangle
            int[] triangles = mesh.triangles;
            if (triangles != null)
                meshInfo.triangleCount = triangles.Length / 3;

            //set vertex & subMesh
            meshInfo.vertexCount = meshInfo.mesh.vertexCount;
            meshInfo.subMeshCount = Mathf.Min(mesh.subMeshCount, 32);

            //set uvs
            meshInfo.uv = new List<Vector2>();
            meshInfo.hasUV[0] = meshInfo.mesh.uv != null && meshInfo.mesh.uv.Length > 0;
            meshInfo.hasUV[1] = meshInfo.mesh.uv2 != null && meshInfo.mesh.uv2.Length > 0;
            meshInfo.hasUV[2] = meshInfo.mesh.uv3 != null && meshInfo.mesh.uv3.Length > 0;
            meshInfo.hasUV[3] = meshInfo.mesh.uv4 != null && meshInfo.mesh.uv4.Length > 0;

            //set vertex colors
            Color32[] colors;
            meshInfo.hasColors = (colors = meshInfo.mesh.colors32) != null && colors.Length > 0;
            if (meshInfo.hasColors)
            {
                meshInfo.color = colors;
            }
        }

        #endregion

        #region Private Methods Draw GUI -> Preview 2D

        static void DrawPreviewWindow2D(Rect rect)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(s_UVPreviewWindowHash, FocusType.Passive, rect);
            switch (e.GetTypeForControl(controlID))
            {
                case EventType.ScrollWheel:
                    OnScrollWheel(e, rect);
                    break;
                case EventType.MouseDown:
                    OnMouseDown(e, rect, controlID);
                    break;
                case EventType.MouseUp:
                    MouseUp(e, controlID);
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag(e, rect, controlID);
                    break;
                case EventType.MouseMove:
                    MouseMove(e, rect);
                    break;
                case EventType.Repaint:
                    {

                        GUI.BeginGroup(rect);

                        m_ViewportRect2D = rect;

                        m_ViewportRect2D.position = m_ViewportRect2D.position - m_ScroolPosition2D;// apply scroll

                        // 1.clamp rect 
                        if (m_ViewportRect2D.position.x < 0f)
                        {
                            m_ViewportRect2D.width += m_ViewportRect2D.position.x; // -= abs(x)
                            m_ViewportRect2D.position = new Vector2(0f, m_ViewportRect2D.position.y);

                            if (m_ViewportRect2D.width <= 0f)
                                break;
                        }
                        if (m_ViewportRect2D.position.y < 0f)
                        {
                            m_ViewportRect2D.height += m_ViewportRect2D.position.y; // -= abs(y)
                            m_ViewportRect2D.position = new Vector2(m_ViewportRect2D.position.x, 0f);

                            if (m_ViewportRect2D.height <= 0f)
                                break;
                        }


                        Rect screenViewportRect = m_ViewportRect2D;
                        screenViewportRect.y = window.position.height - screenViewportRect.y - screenViewportRect.height;
                        screenViewportRect.position += new Vector2(1, 3); // hack

                        GL.Viewport(EditorGUIUtility.PointsToPixels(screenViewportRect));
                        GL.PushMatrix();

                        // 2.clear bg
                        {
                            GL.LoadIdentity();
                            GL.LoadProjectionMatrix(Matrix4x4.Ortho(0f, 1f, 0f, 1f, -1f, 1f));

                            SetMaterialKeyword(m_SimpleMaterial, "_COLOR_MASK", false);
                            SetMaterialKeyword(m_SimpleMaterial, "_NORMALMAP", false);
                            m_SimpleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            m_SimpleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            m_SimpleMaterial.SetTexture("_MainTex", null);
                            m_SimpleMaterial.SetColor("_Color", Color.white);
                            m_SimpleMaterial.SetPass(0);

                            GL.Begin(GL.TRIANGLE_STRIP);
                            GL.Color(GUIDrawerStyles.backgroundColor);
                            GL.Vertex3(1, 0, 0);
                            GL.Vertex3(0, 0, 0);
                            GL.Vertex3(1, 1, 0);
                            GL.Vertex3(0, 1, 0);
                            GL.End();
                        }


                        GL.LoadIdentity();
                        float aspect = m_ViewportRect2D.height / m_ViewportRect2D.width;
                        Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-1f, 1f, -1f * aspect, 1f * aspect, -1f, 1f);
                        GL.LoadProjectionMatrix(projectionMatrix);

                        Matrix4x4 viewMatrix = Matrix4x4.Scale(new Vector3(m_PreviewWindowScale2D, m_PreviewWindowScale2D, m_PreviewWindowScale2D))
                            * Matrix4x4.Translate(new Vector3(m_PreviewWindowPosition.x, m_PreviewWindowPosition.y, 0));
                        GL.MultMatrix(viewMatrix);



                        // 3.grid
                        {
                            GL.wireframe = false;

                            SetMaterialKeyword(m_SimpleMaterial, "_COLOR_MASK", false);
                            SetMaterialKeyword(m_SimpleMaterial, "_NORMALMAP", false);
                            m_SimpleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            m_SimpleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            m_SimpleMaterial.SetTexture("_MainTex", null);
                            m_SimpleMaterial.SetColor("_Color", Color.white);
                            m_SimpleMaterial.SetPass(0);


                            GL.Begin(GL.LINES);

                            float x = -1.0f;
                            GL.Color(GUIDrawerStyles.gridColor);
                            for (int i = 0; i <= 20; i++, x += 0.1f)
                            {
                                GL.Vertex3(x, 1, 0);
                                GL.Vertex3(x, -1, 0);
                            }


                            float y = -1.0f;
                            GL.Color(GUIDrawerStyles.gridColor);
                            for (int i = 0; i <= 20; i++, y += 0.1f)
                            {
                                GL.Vertex3(1, y, 0);
                                GL.Vertex3(-1, y, 0);
                            }

                            GL.Color(Color.gray);
                            GL.Vertex3(1, 0, 0);
                            GL.Vertex3(-1, 0, 0);
                            GL.Vertex3(0, 1, 0);
                            GL.Vertex3(0, -1, 0);

                            GL.Color(Color.red);
                            GL.Vertex3(0.3f, 0, 0);
                            GL.Vertex3(0, 0, 0);


                            GL.Color(Color.green);
                            GL.Vertex3(0, 0.3f, 0);
                            GL.Vertex3(0, 0, 0);

                            GL.End();
                        }


                        // 4.uvs. Tips: will always show uv1.
                        {
                            SetMaterialKeyword(m_uvPreviewMaterial, "_UV1", false);
                            SetMaterialKeyword(m_uvPreviewMaterial, "_UV2", false);
                            SetMaterialKeyword(m_uvPreviewMaterial, "_UV3", false);

                            switch (m_PreviewUVSet)
                            {
                                case UVSet.UV2:
                                    SetMaterialKeyword(m_uvPreviewMaterial, "_UV1", true);
                                    break;
                                case UVSet.UV3:
                                    SetMaterialKeyword(m_uvPreviewMaterial, "_UV2", true);
                                    break;
                                case UVSet.UV4:
                                    SetMaterialKeyword(m_uvPreviewMaterial, "_UV3", true);
                                    break;
                            }


                            GL.wireframe = true;

                            SetMaterialKeyword(m_uvPreviewMaterial, "_VERTEX_COLORS", meshInfo.hasColors);
                            m_uvPreviewMaterial.SetColor("_Color", GUIDrawerStyles.wireframeColor2);
                            m_uvPreviewMaterial.SetPass(0);
                            for (int j = 0; j < meshInfo.subMeshCount && j < 32; j++)
                            {
                                Graphics.DrawMeshNow(meshInfo.mesh, viewMatrix, j);
                            }
                        }


                        if (m_UVHit)
                        {
                            GL.wireframe = false;

                            SetMaterialKeyword(m_SimpleMaterial, "_COLOR_MASK", false);
                            SetMaterialKeyword(m_SimpleMaterial, "_NORMALMAP", false);
                            m_SimpleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            m_SimpleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            m_SimpleMaterial.SetTexture("_MainTex", null);
                            m_SimpleMaterial.SetColor("_Color", Color.white);
                            m_SimpleMaterial.SetPass(0);

                            float pointSize = (1f / Mathf.Max(m_ViewportRect2D.width, m_ViewportRect2D.height)) * m_UVHitPointSize * 3.00f;

                            GL.PushMatrix();
                            //GL.LoadProjectionMatrix(projectionMatrix); // 编辑器报错了 'Matrix stack full depth reached' 时，注释此行。
                            GL.MultMatrix(viewMatrix * Matrix4x4.Translate(m_UVHitPosition) * Matrix4x4.Scale(Vector3.one * pointSize * (1f / m_PreviewWindowScale2D)));

                            GL.Begin(GL.TRIANGLE_STRIP);
                            GL.Color(Color.red);
                            GL.Vertex3(-.5f, -.5f, 0);
                            GL.Vertex3(.5f, -.5f, 0);
                            GL.Vertex3(-.5f, .5f, 0);
                            GL.Vertex3(.5f, .5f, 0);
                            GL.End();

                            GL.PopMatrix();
                        }

                        GL.PopMatrix();
                        GL.wireframe = false;
                        GUI.EndGroup();

                        // ShowGrid
                        {
                            GUI.BeginGroup(rect);
                            Matrix4x4 MVPMatrix = (projectionMatrix * viewMatrix);
                            DrawLabel(new Vector3(0, 0, 0), rect, MVPMatrix, "0.0", EditorStyles.whiteMiniLabel, TextAnchor.MiddleLeft);
                            DrawLabel(new Vector3(0, 1, 0), rect, MVPMatrix, "1.0", EditorStyles.whiteMiniLabel, TextAnchor.MiddleLeft);
                            DrawLabel(new Vector3(0, -1, 0), rect, MVPMatrix, "-1.0", EditorStyles.whiteMiniLabel, TextAnchor.UpperLeft);
                            DrawLabel(new Vector3(1, 0, 0), rect, MVPMatrix, "1.0", EditorStyles.whiteMiniLabel, TextAnchor.MiddleLeft);
                            DrawLabel(new Vector3(-1, 0, 0), rect, MVPMatrix, "-1.0", EditorStyles.whiteMiniLabel, TextAnchor.MiddleRight);
                            GUI.EndGroup();
                        }

                    }
                    break;
                default:
                    break;
            }
        }

        static void MouseMove(Event e, Rect rect)
        {
            if (rect.Contains(e.mousePosition))
            {
                //Matrix change : mouse pos -> vertex pos
                float _aspect = m_ViewportRect2D.height / m_ViewportRect2D.width;
                Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-1f, 1f, -1f * _aspect, 1f * _aspect, -1f, 1f);

                Matrix4x4 viewMatrix = Matrix4x4.Scale(new Vector3(m_PreviewWindowScale2D, m_PreviewWindowScale2D, m_PreviewWindowScale2D))
                    * Matrix4x4.Translate(new Vector3(m_PreviewWindowPosition.x, m_PreviewWindowPosition.y, 0));

                Vector2 mousePos = e.mousePosition - m_ViewportRect2D.position;
                Matrix4x4 W2SMatrix = projectionMatrix * viewMatrix;
                float nearestDistance = float.PositiveInfinity;
                float pointSizeSq = m_UVHitPointSize * m_UVHitPointSize;

                m_UVHit = false;

                //get uvs
                Mesh mesh = meshInfo.mesh;
                int[] indices = mesh.triangles;
                mesh.GetUVs((int)m_PreviewUVSet, meshInfo.uv);

                int subMeshCount = mesh.subMeshCount;

                for (int j = 0; j < subMeshCount; j++)
                {
                    uint indexStart = mesh.GetIndexStart(j);
                    uint indexCount = mesh.GetIndexCount(j);

                    for (uint t = indexStart; t < indexCount; t++)
                    {
                        Vector2 uv = W2SMatrix.MultiplyPoint(meshInfo.uv[indices[t]]);

                        uv = (uv + Vector2.one) * 0.5f;
                        uv.x = uv.x * m_ViewportRect2D.width;
                        uv.y = uv.y * m_ViewportRect2D.height;
                        uv.y = m_ViewportRect2D.height - uv.y;

                        float sqDistance = (mousePos - uv).sqrMagnitude;

                        if (sqDistance < pointSizeSq && sqDistance < nearestDistance)
                        {
                            nearestDistance = sqDistance;

                            m_UVHit = true;
                            m_UVHitTooltipText = "UV: #" + indices[t] + " : " + meshInfo.uv[indices[t]].ToString("F6") + " Color : " + meshInfo.color[indices[t]].ToString();

                            //ToDo: uv 预览图上直接修改顶点色.
                            //美术有要求的情况下,可以再这里拓展.  by daili.ou 2022/05/16

                            m_UVHitPosition = meshInfo.uv[indices[t]];
                            break;
                        }
                    }
                }

                HandleUtility.Repaint();
                e.Use();
            }
        }

        static void MouseUp(Event e, int controlID)
        {
            if (GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;
                e.Use();
            }
        }

        static void OnMouseDown(Event e, Rect rect, int controlID)
        {
            if (rect.Contains(e.mousePosition))
            {
                if (e.button == 0 || e.button == 1 || e.button == 2)
                {
                    GUI.changed = true;

                    GUIUtility.keyboardControl = controlID;
                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
            }
        }

        static void OnMouseDrag(Event e, Rect rect, int controlID)
        {
            if (GUIUtility.hotControl == controlID)
            {
                GUI.changed = true;

                if (e.button == 0 || e.button == 2)
                    m_PreviewWindowPosition += new Vector2(e.delta.x, -e.delta.y) * (2.0f / rect.width) / m_PreviewWindowScale2D;

                if (e.button == 1)
                {
                    float aspect = Mathf.Min(m_ViewportRect2D.width, m_ViewportRect2D.height) / Mathf.Max(m_ViewportRect2D.width, m_ViewportRect2D.height, 1f);
                    float scale = e.delta.magnitude / aspect * Mathf.Sign(Vector2.Dot(e.delta, new Vector2(1.0f, 0.0f))) * (2.0f / rect.width) * (m_PreviewWindowScale2D) * 0.5f;
                    m_PreviewWindowScale2D += scale;
                    m_PreviewWindowScale2D = Mathf.Max(m_PreviewWindowScale2D, 0.01f);
                }

                e.Use();
            }
        }

        static void OnScrollWheel(Event e, Rect rect)
        {
            if (rect.Contains(e.mousePosition))
            {
                GUI.changed = true;

                float aspect = Mathf.Min(m_ViewportRect2D.width, m_ViewportRect2D.height) / Mathf.Max(m_ViewportRect2D.width, m_ViewportRect2D.height, 1f);

                m_PreviewWindowScale2D += e.delta.magnitude / aspect * Mathf.Sign(Vector2.Dot(e.delta, new Vector2(1.0f, -0.1f).normalized)) * (2.0f / rect.width) * (m_PreviewWindowScale2D) * 5.5f;
                m_PreviewWindowScale2D = Mathf.Max(m_PreviewWindowScale2D, 0.01f);

                e.Use();
            }
        }

        static void DrawLabel(Vector3 worldPoint, Rect viewport, Matrix4x4 MVPMatrix, string text, GUIStyle style, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            Vector2 guiPoint = MVPMatrix.MultiplyPoint(worldPoint);

            guiPoint = new Vector2(guiPoint.x * 0.5f + 0.5f, 0.5f - guiPoint.y * 0.5f);
            guiPoint = new Vector2(guiPoint.x * viewport.width, guiPoint.y * viewport.height);

            s_TempContent.text = text;
            Vector2 size = style.CalcSize(s_TempContent);

            Rect labelRect = new Rect(guiPoint.x, guiPoint.y, size.x, size.y);

            labelRect = AlignTextRect(labelRect, alignment);

            labelRect = style.padding.Add(labelRect);


            GUI.Label(labelRect, s_TempContent, style);

        }

        static Rect AlignTextRect(Rect rect, TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperCenter:
                    rect.xMin -= rect.width * 0.5f;
                    break;
                case TextAnchor.UpperRight:
                    rect.xMin -= rect.width;
                    break;
                case TextAnchor.LowerLeft:
                    rect.yMin -= rect.height * 0.5f;
                    break;
                case TextAnchor.LowerCenter:
                    rect.xMin -= rect.width * 0.5f;
                    rect.yMin -= rect.height;
                    break;
                case TextAnchor.LowerRight:
                    rect.xMin -= rect.width;
                    rect.yMin -= rect.height;
                    break;
                case TextAnchor.MiddleLeft:
                    rect.yMin -= rect.height * 0.5f;
                    break;
                case TextAnchor.MiddleCenter:
                    rect.xMin -= rect.width * 0.5f;
                    rect.yMin -= rect.height * 0.5f;
                    break;
                case TextAnchor.MiddleRight:
                    rect.xMin -= rect.width;
                    rect.yMin -= rect.height * 0.5f;
                    break;
            }

            return rect;
        }

        #endregion

        #region Private Methods Draw GUI -> Preview 3D

        private static void DrawPreviewWindow3D(Mesh mesh, Rect rect)
        {
            DrawPreviewGUI(mesh, rect);

            ProcessMouseEvent(rect);
        }

        private static void DrawPreviewGUI(Mesh mesh, Rect rect)
        {
            Material _wireMat = wireMat;
            Material _colorMat = colorMat;
            Material showMat = ShowWireFrame ? _wireMat : _colorMat;

            PreviewGUI.BeginPreview(rect, GUIStyle.none);
            {
                PreviewGUI.DrawMesh(mesh, Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), Vector3.one * m_PreviewWindowScale3D), showMat, 0);
                PreviewGUI.camera.Render();

                var cameraTran = PreviewGUI.camera.transform;
                cameraTran.position = Vector2.zero;
                cameraTran.rotation = Quaternion.Euler(new Vector3(-m_Drag3D.y, -m_Drag3D.x, 0));
                cameraTran.position = cameraTran.forward * -6f;
                var pos = cameraTran.position;
                cameraTran.position = new Vector3(pos.x, pos.y + 0.6f, pos.z);
            }

            var texture = PreviewGUI.EndPreview();
            var previewRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            GUI.Box(previewRect, texture);
        }

        private static void ProcessMouseEvent(Rect rect)
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(s_UVPreviewWindowHash, FocusType.Passive, rect);

            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseUp:
                    MouseUp(current, controlID);
                    break;
                case EventType.MouseDown:
                    OnMouseDown(current, rect, controlID);
                    break;
                case EventType.ScrollWheel:
                    if (rect.Contains(current.mousePosition))
                    {
                        GUI.changed = true;

                        float aspect = Mathf.Min(m_ViewportRect3D.width, m_ViewportRect3D.height) / Mathf.Max(m_ViewportRect3D.width, m_ViewportRect3D.height, 1f);
                        m_PreviewWindowScale3D += current.delta.magnitude / aspect * Mathf.Sign(Vector2.Dot(current.delta, new Vector2(1.0f, -0.1f).normalized)) * (2.0f / rect.width) * (m_PreviewWindowScale3D) * 5.5f;
                        m_PreviewWindowScale3D = Mathf.Max(m_PreviewWindowScale3D, 0.01f);

                        current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUI.changed = true;

                        m_Drag3D -= current.delta * (float)(current.shift ? 3 : 1) / Mathf.Min(rect.width, rect.height) * 140f;

                        current.Use();
                    }
                    break;
                case EventType.Repaint:
                    {
                        GUI.BeginGroup(rect);
                        {
                            m_ViewportRect3D = rect;

                            m_ViewportRect3D.position = m_ViewportRect3D.position - m_ScroolPosition3D;// apply scroll

                            if (m_ViewportRect3D.position.x < 0f)
                            {
                                m_ViewportRect3D.width += m_ViewportRect3D.position.x; // -= abs(x)
                                m_ViewportRect3D.position = new Vector2(0f, m_ViewportRect3D.position.y);

                                if (m_ViewportRect3D.width <= 0f)
                                    break;
                            }
                            if (m_ViewportRect3D.position.y < 0f)
                            {
                                m_ViewportRect3D.height += m_ViewportRect3D.position.y; // -= abs(y)
                                m_ViewportRect3D.position = new Vector2(m_ViewportRect3D.position.x, 0f);

                                if (m_ViewportRect3D.height <= 0f)
                                    break;
                            }
                        }
                        GUI.EndGroup();
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Public Methods

        public static void DrawMiddlePreviewWindow(UnityEngine.Object obj, Mesh mesh, GameObject gameObject, Rect rect, UVBrush _window)
        {
            window = _window;

            if (!LoadMaterials())
            {
                Debug.LogError("UV Brush Error: shaders not found. Reimport asset.");
            }

            SetMeshInfo(obj, mesh, gameObject);

            DrawPreviewWindow2D(rect);

            if (m_UVHit && m_UVHitTooltipText != null)
            {
                Rect _rect = new Rect(Event.current.mousePosition + new Vector2(10, 0), GUIDrawerStyles.tooltip.CalcSize(new GUIContent(m_UVHitTooltipText)));
                EditorGUI.LabelField(_rect, m_UVHitTooltipText, GUIDrawerStyles.tooltip);
            }
        }

        public static void DrawRightPreviewWindow(Mesh mesh, Rect rect)
        {
            DrawPreviewWindow3D(mesh, rect);
        }

        #endregion
    }
}