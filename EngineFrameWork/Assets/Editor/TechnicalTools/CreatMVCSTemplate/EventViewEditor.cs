#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using AssetBundleBrowser;

[CanEditMultipleObjects]
[CustomEditor(typeof(EventView), true)]
public class EventViewEditor : Editor
{
    EventView eView;
    SpriteAtlas[] previewAtlas;
    SerializedObject serializedView;

    List<string> _atlasDependence = null;
    bool _buildAtlas = false;
    bool _isBuildFlodout = false;
    string pathRoot = "Assets/ABAssets/AssetBundle/ui/prefabs/";

    private void Awake()
    {
        eView = target as EventView;
        serializedView = new SerializedObject(eView);
    }

    private void OnEnable()
    {
        _isBuildFlodout = true;
        RefreshDependence();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DrawToogle();

        if (_isBuildFlodout)
        {
            DrawAtlasInfo();
        }

        if (GUI.changed)
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCompleteObjectUndo(eView, $"Changed {eView.gameObject.name} View");
            EditorUtility.SetDirty(eView);
        }
    }

    private void DrawToogle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.toolbar);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 14;

        string tips = "Preview Atlas";
        string caption = (_isBuildFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
        _isBuildFlodout = GUILayout.Toggle(_isBuildFlodout, caption, style);
    }

    private void DrawAtlasInfo()
    {
        GUILayout.Space(5);

        GUIStyle tipStyle = new GUIStyle(EditorStyles.toolbarButton);
        tipStyle.fontSize = 13;
        tipStyle.alignment = TextAnchor.MiddleCenter;
        tipStyle.normal.textColor = Color.white;
        tipStyle.hover.textColor = Color.green;

        if (GUILayout.Button("Refresh & Show", tipStyle))
        {
            RefreshDependence();
        }
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;

        if (_atlasDependence == null)
        {
            _atlasDependence = new List<string>();
        }

        if (!_buildAtlas)
        {
            PreviewAtlas();
        }
    }

    private void RefreshDependence()
    {
        GameObject view = eView.gameObject;
        var imgs = view.GetComponentsInChildren<Image>(true);
        var rImgs = view.GetComponentsInChildren<RawImage>(true);

        _atlasDependence = new List<string>();

        foreach (var item in imgs)
        {
            string path = AssetDatabase.GetAssetPath(item.sprite);

            if (path.Contains("unity_builtin_extra") || item.sprite == null) { continue; }         // flit unity default resources.

            if (!string.IsNullOrEmpty(path))
            {
                string dir = System.IO.Path.GetDirectoryName(path);
                dir = dir.Remove(0, dir.LastIndexOf(@"\") + 1);

                if (!_atlasDependence.Contains(dir))
                    _atlasDependence.Add(dir);

                // Debug.Log($"path:{dir} ");
            }
            else
            {
                Debug.LogError($"Sprite Gen Error: Can't find {item.sprite.name} file path.");
            }
        }

        foreach (var item in rImgs)
        {
            string path = AssetDatabase.GetAssetPath(item.texture);

            if (path.Contains("unity_builtin_extra") || path.Contains("Assets/Resources") || item.texture == null) { continue; }         // flit unity default resources.

            if (!string.IsNullOrEmpty(path))
            {
                string dir = System.IO.Path.GetDirectoryName(path);
                dir = dir.Remove(0, dir.LastIndexOf(@"\") + 1);

                if (!_atlasDependence.Contains(dir))
                    _atlasDependence.Add(dir);

                // Debug.LogWarning($"path:{path}  ");
            }
            else
            {
                Debug.LogError($"Sprite Gen Error: Can't find {item.texture.name} file path.");
            }
        }

        CheckAtlasExit();
    }

    private void CheckAtlasExit()
    {
        string assetPath = "{0}{1}/{2}_atlas.spriteatlas";

        previewAtlas = new SpriteAtlas[_atlasDependence.Count];
        for (int i = 0; i < _atlasDependence.Count; i++)
        {
            var item = _atlasDependence[i];
            string path = string.Format(assetPath, pathRoot, item, item);
            SpriteAtlas tmp = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);

            if (tmp == null)
            {
                TipsAlertWindow.ShowAlertWithBtn("提示", "图集未构建" + "\n" + "是否构建图集？", () =>
                {
                    BuildAltasCommand.BuildUIAtlas();
                });
                _buildAtlas = true;
                break;
            }
            else
            {
                previewAtlas[i] = tmp;
            }
        }
    }

    private void PreviewAtlas()
    {
        if (previewAtlas == null) { return; }

        eView._atlasDependence = new List<string>();
        for (int i = 0; i < previewAtlas.Length; i++)
        {
            var item = previewAtlas[i];
            if (item == null) { continue; }
            EditorGUILayout.ObjectField(item, typeof(SpriteAtlas), false);
            eView._atlasDependence.Add(item.name);
        }
    }
}

#endif