using UnityEditor;
using UnityEngine;

/// <summary>
/// Author：Daili.OU
/// Created Time：2022/05/06
/// Descriptions：
/// </summary>
namespace AI.Tools
{
    public class BaseBrush : EditorWindow
    {
        #region Variables

        // Styles
        public GUIStyle _titleStyle;
        public GUIStyle _labelStyle;
        public GUIStyle _badgeStyle;
        public GUIStyle _smallLabelStyle;
        public GUIStyle _objFieldLableStyle;

        // Sizings
        public float _topPanelHeight = 22.0f;
        public static float _leftPaneWidth = 200.0f;
        public static float _middlePaneWidth = 720.0f;

        // Helpers
        public bool _isLoadedBrushCfg;
        public uint _flodOutIdx;
        public bool _isHeadFlodout = true;
        public bool _isClothFlodout = true;
        public bool _isPantsFlodout = true;
        public bool _isShoesFlodout = true;
        public bool _isAutoSetFlodout;
        public bool _loadedModelObject;
        public bool _isRenderTypeFlodout;
        public bool _isEditorColorFlodout;
        public bool _isVertexColorPainting;


        public UvBrushConfig _current;
        public SkinnedMeshRenderer skRender;
        public string VERTEX_COLOR_MATERIAL_PATH = "Assets/AIModule/AILib/Editor/UVBrush/Materials/VertexColor.mat";

        private UvBrushConfig[] _allCfgs;

        #endregion

        #region Unity Call

        protected virtual void OnEnable()
        {
            wantsMouseMove = true;

            //Init GUI Style
            _titleStyle = new GUIStyle();
            _titleStyle.fontSize = 22;
            _titleStyle.normal.textColor = Color.gray;

            _labelStyle = new GUIStyle();
            _labelStyle.fontSize = 14;
            _labelStyle.normal.textColor = Color.white;

            _smallLabelStyle = new GUIStyle();
            _smallLabelStyle.normal.textColor = Color.gray;


            _allCfgs = GetAllInstances<UvBrushConfig>();

            _isLoadedBrushCfg = true;
        }

        private void OnDisable()
        {
            SetShaderDataOnClose();
            EditorUtility.SetDirty(_current);
            GUI.changed = true;

            if (UVBrushMeshEditor.PreviewGUI != null)
            {
                UVBrushMeshEditor.PreviewGUI.Cleanup();
                UVBrushMeshEditor.PreviewGUI = null;
            }
        }

        private void OnDestroy()
        {

            //if (_curObj != null)
            //{
            //    _current.RevertToDefaultColor(this);

            //    GameObject.DestroyImmediate(_curObj);
            //    GameObject.DestroyImmediate(_colorObj);

            //    _loadedModelObject = false;
            //}

            //PainterWindow
            //if (SVTXPainterEditor.SVTXPainterWindow.PainterWindow != null)
            //{
            //    SVTXPainterEditor.SVTXPainterWindow.PainterWindow.Close();
            //}


        }

        public void OnClosePaniterWindow()
        {
            //_curObj.SetActive(true);
            //_colorObj.SetActive(false);

            _isVertexColorPainting = false;

        }

        public void OnInspectorGUI()
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_current);
            }
        }

        #endregion

        #region Draw GUI Left Part

        public void DrawPreset()
        {
            //Begine Set GUIStyle
            _objFieldLableStyle = new GUIStyle(EditorStyles.label);
            _objFieldLableStyle.alignment = TextAnchor.MiddleLeft;
            _objFieldLableStyle.fontSize = 14;
            _objFieldLableStyle.fontStyle = FontStyle.Italic;
            _objFieldLableStyle.normal.textColor = new Color(0.639f, 0.65f, 0.678f);

            _badgeStyle = new GUIStyle(EditorStyles.toolbarButton);
            _badgeStyle.normal.textColor = Color.white;
            _badgeStyle.active.textColor = Color.white;
            _badgeStyle.focused.textColor = Color.white;
            _badgeStyle.hover.textColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            {
                var prev = _current;
                _current = (UvBrushConfig)EditorGUILayout.ObjectField(_current, typeof(UvBrushConfig), false);
                if (!System.Object.ReferenceEquals(_current, prev))
                {
                    SetCurrentBrushCfg(_current);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /*
        public void DrawToggleItem(string tips)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(500));
            {
                string caption = string.Empty;
                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 14;

                switch (_flodOutIdx)
                {
                    case 0:
                        caption = (_isHeadFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
                        _isHeadFlodout = GUILayout.Toggle(_isHeadFlodout, caption, style);
                        break;
                    case 1:
                        caption = (_isClothFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
                        _isClothFlodout = GUILayout.Toggle(_isClothFlodout, caption, style);
                        break;
                    case 2:
                        caption = (_isPantsFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
                        _isPantsFlodout = GUILayout.Toggle(_isPantsFlodout, caption, style);
                        break;
                    case 3:
                        caption = (_isShoesFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
                        _isShoesFlodout = GUILayout.Toggle(_isShoesFlodout, caption, style);
                        break;
                    default:
                        caption = (_isHeadFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
                        _isHeadFlodout = GUILayout.Toggle(_isHeadFlodout, caption, style);
                        break;
                }
            }
            GUILayout.EndHorizontal();
        }

        public void DrawObjectFieldItem(BaseBrushItem[] items, BrushModelLevel curLevel, bool[] selectLevel, BrushModelType modelType)
        {
            GUILayout.BeginVertical();

            var c = GUI.color;
            for (int i = 0; i < items.Length; i++)
            {
                GUILayout.Label(GetObjectFieldLabel(i), _objFieldLableStyle);

                GUILayout.BeginHorizontal();
                items[i].oriObj = (GameObject)EditorGUILayout.ObjectField(items[i].oriObj, typeof(GameObject), true);

                GUI.color = c * ((selectLevel[i]) ? Color.green : Color.red);

                if (GUILayout.Button(ToStr(selectLevel[i]), _badgeStyle, GUILayout.MaxWidth(20.0f))) { }

                GUILayout.EndHorizontal();
                GUI.color = c;
            }

            //DrawMat;
            GUILayout.Label("UseingMat", _objFieldLableStyle);
            switch (modelType)
            {
                case BrushModelType.HEAD:
                    break;
                case BrushModelType.CLOTH:
                    if (_current.cloth.curMat == null)
                        _current.cloth.LoadMat(_current.DEFAULT_CLOTH_MATERIAL_PATH);

                    _current.cloth.curMat = (Material)EditorGUILayout.ObjectField(_current.cloth.curMat, typeof(Material), true);
                    break;
                case BrushModelType.LIMBS:
                    if (_current.limbs.curMat == null)
                        _current.limbs.LoadMat(_current.DEFAULT_LIMB_MATERIAL_PATH);

                    _current.limbs.curMat = (Material)EditorGUILayout.ObjectField(_current.limbs.curMat, typeof(Material), true);
                    break;
                case BrushModelType.SHOES:
                    if (_current.shoes.curMat == null)
                        _current.shoes.LoadMat(_current.DEFAULT_SHOE_MATERIAL_PATH);

                    _current.shoes.curMat = (Material)EditorGUILayout.ObjectField(_current.shoes.curMat, typeof(Material), true);
                    break;
                default:
                    break;
            }

            GUILayout.EndVertical();

            EditorUtility.SetDirty(_current);
            GUI.changed = true;
        }

        public string GetObjectFieldLabel(int index)
        {
            string tips = string.Empty;
            switch (index)
            {
                case 0:
                    tips = "Default";
                    break;
                case 1:
                    tips = "Slim";
                    break;
                case 2:
                    tips = "Bigger";
                    break;
                default:
                    tips = "Default";
                    break;
            }

            return tips;
        }

        public void DrawAutoSetModelFigure()
        {
            string caption = "选择体型";
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;

            caption = (_isAutoSetFlodout) ? string.Format("▼ {0}", caption) : string.Format("► {0}", caption);
            _isAutoSetFlodout = GUILayout.Toggle(_isAutoSetFlodout, caption, style);

            if (!_isAutoSetFlodout) { return; }

            GUILayout.BeginVertical(EditorStyles.objectField);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Default", _badgeStyle))
                {
                    _current.SetModelFigure(BrushModelFigure.DEFAULT);
                    _current.CurLevel = BrushModelFigure.DEFAULT;

                    EditorUtility.SetDirty(_current);
                    GUI.changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Slim", _badgeStyle))
                {
                    _current.SetModelFigure(BrushModelFigure.SLIM);
                    _current.CurLevel = BrushModelFigure.SLIM;

                    EditorUtility.SetDirty(_current);
                    GUI.changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Bigger", _badgeStyle))
                {
                    _current.SetModelFigure(BrushModelFigure.BIGGER);
                    _current.CurLevel = BrushModelFigure.BIGGER;

                    EditorUtility.SetDirty(_current);
                    GUI.changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        public void DrawLoad()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("模型加载", _badgeStyle))
                {
                    //if (_curObj != null)
                    //{
                    //    TipsAlertWindow.ShowAlertWithBtn("提示", "已创建模型，是否覆盖？", ConfirmAlert);
                    //    return;
                    //}

                    DoLoadModel();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public void DrawRenderType()
        {
            string caption = "选择渲染模式";
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;

            caption = (_isRenderTypeFlodout) ? string.Format("▼ {0}", caption) : string.Format("► {0}", caption);
            _isRenderTypeFlodout = GUILayout.Toggle(_isRenderTypeFlodout, caption, style);

            if (!_isRenderTypeFlodout) { return; }

            var c = GUI.color;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Wireframe", _labelStyle);
                GUI.color = c * (UVBrushMeshEditor.ShowWireFrame ? Color.green : Color.red);
                if (GUILayout.Button(ToStr(UVBrushMeshEditor.ShowWireFrame), _badgeStyle, GUILayout.MaxWidth(20.0f)))
                {
                    UVBrushMeshEditor.ShowWireFrame = !UVBrushMeshEditor.ShowWireFrame;
                }
            }
            GUILayout.EndHorizontal();


            GUI.color = c;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Vertex Color", _labelStyle);
                GUI.color = c * (UVBrushMeshEditor.ShowWireFrame ? Color.red : Color.green);
                if (GUILayout.Button(ToStr(UVBrushMeshEditor.ShowWireFrame), _badgeStyle, GUILayout.MaxWidth(20.0f)))
                {
                    UVBrushMeshEditor.ShowWireFrame = !UVBrushMeshEditor.ShowWireFrame;
                }
            }
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        public void DrawColorEditor()
        {
            string caption = "选择顶点色编辑模式";
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;

            caption = (_isEditorColorFlodout) ? string.Format("▼ {0}", caption) : string.Format("► {0}", caption);
            _isEditorColorFlodout = GUILayout.Toggle(_isEditorColorFlodout, caption, style);

            if (!_isEditorColorFlodout) { return; }

            string tips = _isVertexColorPainting ? "编辑中" : "开始编辑";
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button(tips, _badgeStyle))
                {
                    // Do Start Editor.
                    _isVertexColorPainting = true;

                    //_curObj.SetActive(false);
                    //_colorObj.SetActive(true);

                    //SVTXPainterEditor.SVTXPainterWindow.LauchVertexPainter(this);

                    EditorUtility.SetDirty(_current);
                    GUI.changed = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        public void DrawObject()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                //_curObj = (GameObject)EditorGUILayout.ObjectField(_curObj, typeof(GameObject), true);
            }
            EditorGUILayout.EndHorizontal();
        }
        */
        #endregion

        #region Public Methods

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            T[] a = new T[guids.Length];
            for (int i = 0, n = guids.Length; i < n; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

        public void SelectPresetHandler(object aPresetName)
        {
            UvBrushConfig selectedPreset = System.Array.Find(_allCfgs, x => x.name.Equals(aPresetName.ToString()));

            if (selectedPreset.cloth.list.Length < 1)
            {
                selectedPreset.SetDefault();
            }

            _current = selectedPreset;
            SetCurrentBrushCfg(selectedPreset);
        }

        public void SetCurrentBrushCfg(UvBrushConfig cfg)
        {
            _current = cfg;

            if (_current == null)
            {
                _current = new UvBrushConfig();
                _current.SetDefault();
                return;
            }

            _current.UpdateModel(cfg.heads, cfg.cloth, cfg.limbs, cfg.shoes);

            _current.UpdateVertexColor(cfg);
        }

        public void DoLoadModel()
        {
            //_curObj = new GameObject("UvBrush_Painter_Object");
            //_curObj.transform.SetParent(null);

            //instance body part
            //GameObject tmpHead = null;
            //GameObject tmpCloth = null;
            //GameObject tmpPants = null;
            //GameObject tmpShoes = null;

            //_current.LoadModelBody(ref tmpHead, ref tmpCloth, ref tmpPants, ref tmpShoes);
            //_headObj = GameObject.Instantiate(tmpHead, Vector3.zero, new Quaternion(), _curObj.transform);
            //_clothObj = GameObject.Instantiate(tmpCloth, Vector3.zero, new Quaternion(), _curObj.transform);
            //_limbsObj = GameObject.Instantiate(tmpPants, Vector3.zero, new Quaternion(), _curObj.transform);
            //_shoesObj = GameObject.Instantiate(tmpShoes, Vector3.zero, new Quaternion(), _curObj.transform);

            //_headObj.name = "head_" + _current.CurLevel.ToString().ToLower();
            //_clothObj.name = "cloth_" + _current.CurLevel.ToString().ToLower();
            //_limbsObj.name = "pants_" + _current.CurLevel.ToString().ToLower();
            //_shoesObj.name = "shoes_" + _current.CurLevel.ToString().ToLower();

            //RemoveAnimator(_headObj.GetComponent<Animator>());
            //RemoveAnimator(_clothObj.GetComponent<Animator>());
            //RemoveAnimator(_limbsObj.GetComponent<Animator>());
            //RemoveAnimator(_shoesObj.GetComponent<Animator>());

            //headRender = _headObj.GetComponentInChildren<SkinnedMeshRenderer>();
            //clothRender = _clothObj.GetComponentInChildren<SkinnedMeshRenderer>();
            //limbsRender = _limbsObj.GetComponentInChildren<SkinnedMeshRenderer>();
            //shoesRender = _shoesObj.GetComponentInChildren<SkinnedMeshRenderer>();

            //headRender = GameObject.Instantiate(headRender);
            //clothRender = GameObject.Instantiate(clothRender);
            //limbsRender = GameObject.Instantiate(limbsRender);
            //shoesRender = GameObject.Instantiate(shoesRender);

            //ResetMat(clothRender, BrushModelType.CLOTH);
            //ResetMat(limbsRender, BrushModelType.LIMBS);
            //ResetMat(shoesRender, BrushModelType.SHOES);

            //if (!_current.SetDefaultColor(this))
            //{
            //    _current.SetToConfigColor(this);
            //}

            //Dictionary<PlayerModPart, GameObject> _EditorObjDic = new Dictionary<PlayerModPart, GameObject>();
            //_EditorObjDic[PlayerModPart.Head] = _headObj;
            //_EditorObjDic[PlayerModPart.Body] = _clothObj;
            //_EditorObjDic[PlayerModPart.Limb] = _limbsObj;
            //_EditorObjDic[PlayerModPart.Shoes] = _shoesObj;

            //Dictionary<PlayerModPart, Texture2D> _normalDic = new Dictionary<PlayerModPart, Texture2D>();
            //_normalDic[PlayerModPart.Head] = _current.heads.headNormal;
            //_normalDic[PlayerModPart.Body] = _current.cloth.clothNormal;
            //_normalDic[PlayerModPart.Limb] = _current.limbs.limbNormal;
            //_normalDic[PlayerModPart.Shoes] = _current.shoes.shoesNormal;

            //PlayerHelper.g_Ins.GetPlayerGoForEditor(_EditorObjDic, _normalDic, (info) =>
            //{
            //    //Editor 下 删除 加载日志信息
            //    {
            //        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            //        System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            //        System.Reflection.MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            //        clearConsoleMethod.Invoke(new object(), null);
            //    }

            //    _colorObj = _curObj;
            //    _colorObj.SetActive(false);

            //    ReplaceMat(headRender, BrushModelType.HEAD);
            //    ReplaceMat(clothRender, BrushModelType.CLOTH);
            //    ReplaceMat(limbsRender, BrushModelType.LIMBS);
            //    ReplaceMat(shoesRender, BrushModelType.SHOES);

            //    _curObj = info.myObj;
            //    _curObj.name = "UvBrush_Object";
            //    _curObj.transform.position = Vector3.zero;

            //    SetShaderParamByCfg();

            //    _isHeadFlodout = false;
            //    _isClothFlodout = false;
            //    _isPantsFlodout = false;
            //    _isShoesFlodout = false;

            //    _loadedModelObject = true;
            //    _isRenderTypeFlodout = true;
            //    _isEditorColorFlodout = true;
            //});
        }

        public void SetShaderDataOnClose()
        {
            //if (_curObj == null) { return; }

            //_current._ScaleParamLimb = skRender.sharedMaterial.GetFloat("_ScaleParamLimb");
            //_current._ScaleParamPants = skRender.sharedMaterial.GetFloat("_ScaleParamPants");

            //EditorUtility.SetDirty(_current);
            //GUI.changed = true;
        }

        public void SetShaderParamByCfg()
        {
            //skRender = _curObj.GetComponentInChildren<SkinnedMeshRenderer>();
            //skRender.sharedMaterial.SetFloat("_ScaleParamLimb", _current._ScaleParamLimb);
            //skRender.sharedMaterial.SetFloat("_ScaleParamPants", _current._ScaleParamPants);
        }

        public void RemoveAnimator(Animator tar)
        {
            if (tar != null)
                GameObject.DestroyImmediate(tar);
        }

        public void ResetMat(SkinnedMeshRenderer renderer, BrushModelType modelType)
        {
            renderer.materials = new Material[1];
            switch (modelType)
            {
                case BrushModelType.HEAD:
                    break;
                case BrushModelType.CLOTH:
                    renderer.material = _current.cloth.curMat;
                    renderer.sharedMaterial = _current.cloth.curMat;
                    break;
                case BrushModelType.LIMBS:
                    renderer.material = _current.limbs.curMat;
                    renderer.sharedMaterial = _current.limbs.curMat;
                    break;
                case BrushModelType.SHOES:
                    renderer.material = _current.shoes.curMat;
                    renderer.sharedMaterial = _current.shoes.curMat;
                    break;
                default:
                    break;
            }
        }

        public void ReplaceMat(SkinnedMeshRenderer renderer, BrushModelType modelType)
        {
            renderer.materials = new Material[1];
            var tmpMat = AssetDatabase.LoadAssetAtPath(VERTEX_COLOR_MATERIAL_PATH, typeof(Material)) as Material;

            Material mat = GameObject.Instantiate(tmpMat) as Material;

            switch (modelType)
            {
                case BrushModelType.HEAD:
                    renderer.material = mat;
                    renderer.sharedMaterial = mat;
                    break;
                case BrushModelType.CLOTH:
                    renderer.material = mat;
                    renderer.sharedMaterial = mat;
                    break;
                case BrushModelType.LIMBS:
                    renderer.material = mat;
                    renderer.sharedMaterial = mat;
                    break;
                case BrushModelType.SHOES:
                    renderer.material = mat;
                    renderer.sharedMaterial = mat;
                    break;
                default:
                    break;
            }

            //renderer.gameObject.AddComponent<SVTXPainter.SVTXObject>();
        }

        public void ConfirmAlert()
        {
            //GameObject.DestroyImmediate(_curObj);
            //GameObject.DestroyImmediate(_colorObj);
            //DoLoadModel();
        }

        #endregion

        #region GUI Style Methods

        private const string _true = "●";
        private const string _false = "○";

        private string ToStr(bool aValue)
        {
            return (aValue) ? _true : _false;
        }

        #endregion
    }
}