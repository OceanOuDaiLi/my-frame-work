using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using MU.DataBase;
using MU.DataBase.Config.Obj;

/// <summary>
/// Author：谢长烨
/// Created Time：2022/07/18
/// Descriptions：
namespace AI.Tools
{
#if UNITY_EDITOR
    public class BodyConfigEditorWindow : EditorWindow
    {
        static BodyConfigEditorWindow window;
        static BodyConfig[] _allCfgs;
        static List<PlayerObject> PlayerCfgs = new List<PlayerObject>();
        static IModelPreView _previewModel;

        BodyConfig _current;
        int selectIdx;
        string search = string.Empty;
        bool isPreviewModel = false;


        #region Initialize Windows

        public static BodyConfigEditorWindow ShowWindow()
        {
            window = (BodyConfigEditorWindow)EditorWindow.GetWindow(typeof(BodyConfigEditorWindow), false, "Body Config Window");
            window.autoRepaintOnSceneChange = true;

            Vector2 rect = new Vector2(500, 600);
            window.position = new Rect(200, 50, rect.x, rect.y);
            window.Init();
            return window;
        }

        public void Init()
        {
            searchField = new UnityEditor.IMGUI.Controls.SearchField();
            searchFieldStyle = new GUIStyle("AnimClipToolbar");
            searchFieldStyle.stretchHeight = true;
            searchFieldStyle.fixedHeight = 40;
            searchFieldStyle.margin = new RectOffset(0, 0, 0, 0);
            searchFieldStyle.padding = new RectOffset(8, 2, 0, 0);

            PlayerNameBarStyle = new GUIStyle("DropDown");
            PlayerNameBarStyle.richText = true;

            leftPanelStyle = new GUIStyle(GUIStyle.none);
            leftPanelStyle.padding = new RectOffset(8, 8, 2, 5);
            leftPanelStyle.margin = new RectOffset(0, 0, 0, 0);

            middlePanelStyle = new GUIStyle(GUIStyle.none);
            middlePanelStyle.padding = new RectOffset(8, 8, 2, 5);

            scrollStyle = new GUIStyle(GUIStyle.none);
            scrollStyle.padding = new RectOffset(5, 5, 0, 0);

            selectSetionStyle = new GUIStyle(GUIStyle.none);
            selectSetionStyle.padding = new RectOffset(10, 0, 0, 0);

            backButtonStyle = new GUIStyle("ButtonMid");

            DescStyle = new GUIStyle(GUI.skin.label);
            DescStyle.clipping = TextClipping.Clip;

            ReflashPlayersNames();
        }

        void ReflashPlayersNames()
        {
            List<PlayerObject> cfgs = DBMgr.g_Ins.V_PlayerConfig.GetAllPlayerObjectInEditor();
            PlayerCfgs = cfgs;
        }

        public static void OpenWindow(BodyConfig asset)
        {
            ShowWindow().SelectPresetHandler(asset);
        }

        public void SelectPresetHandler(BodyConfig assetIns)
        {
            BodyConfig selectedPreset = System.Array.Find(_allCfgs, x => x.Equals(assetIns));
            selectedPreset.RefreshValuesWithTable();
            _current = selectedPreset;
            SetCurrentBrushCfg(_current);
        }

        public void SetCurrentBrushCfg(BodyConfig cfg)
        {
            _current = cfg;
            selectIdx = -1;
            middleScrollPosition = Vector2.zero;
            ClosePreview();
        }

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
        #endregion

        #region Unity Calls

        private void OnEnable()
        {
            _allCfgs = GetAllInstances<BodyConfig>();
        }

        private void OnDestroy()
        {
            ClosePreview();

            EditorUtility.SetDirty(window._current);
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                DrawLeftPanel();
                DrawMiddlePanel();
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(window);
            }
        }


        #endregion

        #region Draw GUI

        float leftWidth = 200;
        Vector2 middleScrollPosition;
        UnityEditor.IMGUI.Controls.SearchField searchField;

        GUIStyle leftPanelStyle;
        GUIStyle middlePanelStyle;
        GUIStyle scrollStyle;
        GUIStyle selectSetionStyle;
        GUIStyle PlayerNameBarStyle;
        GUIStyle searchFieldStyle;
        GUIStyle backButtonStyle;
        GUIStyle DescStyle;
        private void DrawLeftPanel()
        {
            GUILayout.BeginVertical(leftPanelStyle, GUILayout.Width(leftWidth));
            {
                var lastConfig = _current;
                _current = (BodyConfig)EditorGUILayout.ObjectField(_current, typeof(BodyConfig), false);
                if (!System.Object.ReferenceEquals(lastConfig, _current))
                {
                    SetCurrentBrushCfg(_current);
                }
                GUILayout.Space(5);
                if (GUILayout.Button(new GUIContent("预览模型")))
                {
                    //TODO:预览模型
                    if (selectIdx >= 0 && selectIdx < _current.list.Length)
                    {
                        OpenPreview();
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawMiddlePanel()
        {
            GUILayout.BeginVertical();
            {
                if (isPreviewModel)
                {
                    DrawPreModelPark();
                }
                else
                {
                    DrawPlayerList();
                }
            }
            GUILayout.EndVertical();

            //GuiDrawer.DrawLine(
            //        new Vector2(leftWidth + 1, 0.0f),
            //        new Vector2(leftWidth + 1, position.height),
            //        Color.white,
            //        2.0f);
        }

        private void DrawPlayerList()
        {
            GUILayout.BeginHorizontal(searchFieldStyle);
            search = searchField.OnGUI(search);
            GUILayout.EndHorizontal();

            middleScrollPosition = GUILayout.BeginScrollView(middleScrollPosition, scrollStyle);
            {
                GUILayout.BeginVertical(middlePanelStyle);
                {
                    for (int i = 0; i < _current.list.Length; i++)
                    {
                        string playerName = string.Empty;
                        var cfg = PlayerCfgs.Find((item) => { return item.playerId == _current.list[i].id; });
                        if (cfg != null)
                        {
                            playerName = cfg.Name;
                        }
                        bool select = selectIdx == i;
                        var item = _current.list[i];
                        string caption = string.Format("{0} {1}: {2}", select ? "<color=green>●</color>" : "○", item.id, playerName);
                        if (!caption.ToLower().Contains(search.ToLower()))
                        {
                            continue;
                        }
                        select = GUILayout.Toggle(select, caption, PlayerNameBarStyle);
                        if (selectIdx == i && !select)
                        {
                            selectIdx = -1;
                        }
                        if (select)
                        {
                            selectIdx = i;
                            DrawBodyConfig(cfg, item);
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DrawPreModelPark()
        {
            var cfg = PlayerCfgs.Find((item) => { return item.playerId == _current.list[selectIdx].id; });
            var item = _current.list[selectIdx];
            if (GUILayout.Button(cfg.Name, backButtonStyle))
            {
                ClosePreview();
            }
            DrawBodyConfig(cfg, item);
            if (isPreviewModel)
            {
                UpdatePreview();
            }
        }

        private void DrawBodyConfig(PlayerObject cfg, BodyInfo item)
        {
            GUILayout.BeginVertical(selectSetionStyle);
            {
                GUILayout.Label(string.Format("ID: {0}", cfg != null ? cfg.playerId.ToString() : string.Empty));
                GUILayout.Label(string.Format("Desc: {0}", cfg != null && cfg.Describe != null ? cfg.Describe.ToString() : string.Empty), DescStyle);

                GUILayout.Label("体型参数；");
                item.headScale = EditorGUILayout.Vector3Field("头部缩放:", item.headScale);
                item.headOffset = EditorGUILayout.Vector3Field("头部偏移:", item.headOffset);
                item.limbScale = EditorGUILayout.Vector4Field("四肢缩放:", item.limbScale);
                item.bodyScale = EditorGUILayout.Vector4Field("身体缩放:", item.bodyScale);
                item.numberScale = EditorGUILayout.Vector4Field("号码牌缩放:", item.numberScale);
                item.shoesScale = EditorGUILayout.Vector4Field("鞋子缩放:", item.shoesScale);
                item.limb_transmissionColor = EditorGUILayout.ColorField("四肢颜色", item.limb_transmissionColor);

                GUILayout.Space(5);
                GUILayout.Label("装备参数；");
                item.headbandOffset = EditorGUILayout.Vector3Field("头带偏移:", item.headbandOffset);
                item.headbandScale = EditorGUILayout.Vector3Field("头带缩放:", item.headbandScale);
                //item.armguardOffset = EditorGUILayout.Vector3Field("护肘偏移:", item.armguardOffset);
                item.armguardScale = EditorGUILayout.Vector4Field("护肘缩放:", item.armguardScale);
                //item.kneeguardOffset = EditorGUILayout.Vector3Field("护膝偏移:", item.kneeguardOffset);
                item.kneeguardScale = EditorGUILayout.Vector4Field("护膝缩放:", item.kneeguardScale);
            }
            GUILayout.EndVertical();
        }
        #endregion

        #region Private Methold
        private void OpenPreview()
        {
            if (selectIdx < 0 || selectIdx >= _current.list.Length || isPreviewModel)
            {
                return;
            }

            if (_previewModel != null)
            {
                ClosePreview();
            }

            isPreviewModel = true;
            var info = _current.list[selectIdx];
            var cfg = PlayerCfgs.Find((item) => { return item.playerId == info.id; });
            //_previewModel = new BodyConfig_ModelPreView(cfg, info);
            _previewModel = new ModelPreView_New(cfg, info);
        }

        private void ClosePreview()
        {
            if (_previewModel != null)
            {
                _previewModel.UnLoad();
                _previewModel = null;
            }

            isPreviewModel = false;
        }

        private void UpdatePreview()
        {
            if (_previewModel != null && _current.list.Length > selectIdx && selectIdx >= 0)
            {
                _previewModel.OnUpdate(_current.list[selectIdx]);
            }
        }
        #endregion
    }
#endif
}

