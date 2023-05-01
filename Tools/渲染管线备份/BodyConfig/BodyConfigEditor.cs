
using UnityEngine;
using UnityEditor;

/// <summary>
/// Author£∫–ª≥§Ï«
/// Created Time£∫2022/07/18
/// Descriptions£∫
namespace AI.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BodyConfig))]
    public class BodyConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Body Editor Window", GUILayout.MinHeight(40.0f)))
            {
                BodyConfigEditorWindow.OpenWindow(target as BodyConfig);
            }
        }
    }
#endif
}

