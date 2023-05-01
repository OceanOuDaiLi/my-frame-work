//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class NpcNodeEditor : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}



#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(NpcNode), true)]
public class NpcNodeEditor : Editor
{
    #region Private Editor Variables
    static int tab = 0;
    static bool showDefault = false;

    NpcNode instance;
    SerializedObject serializedBatching;
    #endregion

    #region Unity Calls
    private void Awake()
    {
        instance = target as NpcNode;
        serializedBatching = new SerializedObject(instance);
    }


    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            MeshRenderer render = instance.gameObject.GetComponent<MeshRenderer>();
            instance.BaseMap = render.sharedMaterials[0].GetTexture("_MainTex");
            instance.AnimationMap = render.sharedMaterials[0].GetTexture("_AniMap");
            instance.render = render;
        }

        showDefault = EditorGUILayout.Foldout(showDefault, "д╛хо");
        if (showDefault) DrawDefaultInspector();

        if (GUI.changed)
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCompleteObjectUndo(instance, "Changed NpcMgr");
            EditorUtility.SetDirty(instance);
        }
    }

    #endregion
}
#endif

