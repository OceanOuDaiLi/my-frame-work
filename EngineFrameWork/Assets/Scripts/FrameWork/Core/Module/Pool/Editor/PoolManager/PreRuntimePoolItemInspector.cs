#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


// Only compile if not using Unity iPhone
[CustomEditor(typeof(PreRuntimePoolItem))]
public class PreRuntimePoolItemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (PreRuntimePoolItem)target;

        EditorGUI.indentLevel = 0;
        PGEditorUtils.LookLikeControls();

        script.poolName = EditorGUILayout.TextField("Pool Name", script.poolName);
        script.prefabName = EditorGUILayout.TextField("Prefab Name", script.prefabName);
        script.despawnOnStart = EditorGUILayout.Toggle("Despawn On Start", script.despawnOnStart);
        script.doNotReparent = EditorGUILayout.Toggle("Do Not Reparent", script.doNotReparent);

        // Flag Unity to save the changes to to the prefab to disk
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

#endif