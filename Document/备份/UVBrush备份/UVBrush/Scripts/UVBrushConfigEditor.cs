using UnityEditor;
using UnityEngine;

/// <summary>
/// Author��Daili.OU
/// Created Time��2022/05/06
/// Descriptions��
/// </summary>
namespace AI.Tools
{
    [CustomEditor(typeof(UvBrushConfig))]
    public class UVBrushConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open UV Brush Window", GUILayout.MinHeight(40.0f)))
            {
                UVBrush.OpenBrushConfig(target.name);
            }
        }
    }
}