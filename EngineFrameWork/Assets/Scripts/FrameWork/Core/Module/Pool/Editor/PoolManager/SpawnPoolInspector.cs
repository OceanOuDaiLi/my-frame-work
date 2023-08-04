#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

// Only compile if not using Unity iPhone
[CustomEditor(typeof(SpawnPool))]
public class SpawnPoolInspector : Editor
{
    public bool expandPrefabs = true;

    public override void OnInspectorGUI()
    {
        var script = (SpawnPool)target;

        EditorGUI.indentLevel = 0;
        PGEditorUtils.LookLikeControls();

        script.poolName = EditorGUILayout.TextField("Pool Name", script.poolName);
        script.dontDestroyOnLoad = EditorGUILayout.Toggle("Don't Destroy On Load", script.dontDestroyOnLoad);
        script.logMessages = EditorGUILayout.Toggle("Log Messages", script.logMessages);

        this.expandPrefabs = PGEditorUtils.SerializedObjFoldOutList<PrefabPool>
                            (
                                "Per-Prefab Pool Options",
                                script._perPrefabPoolOptions,
                                this.expandPrefabs,
                                ref script._editorListItemStates,
                                true
                            );

        // Flag Unity to save the changes to to the prefab to disk
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}
#endif