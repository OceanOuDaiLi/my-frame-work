#if UNITY_EDITOR

using GameEngine;
using UnityEngine;
using UnityEditor;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	CharacterEditor.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/

[CanEditMultipleObjects]
[CustomEditor(typeof(Character), true)]
public class CharacterEditor : Editor
{
    #region Private Editor Variables
    static int tab = 0;
    static bool showDefault = false;

    Character character;
    SerializedObject serializedCharacter;
    #endregion

    #region Unity Calls
    private void Awake()
    {
        character = target as Character;
        serializedCharacter = new SerializedObject(character);
    }

    private static readonly string[] tabNames = { "属性", "技能编辑器", "动画", "待定" };

    public override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        tab = GUILayout.Toolbar(tab, tabNames);

        Undo.RecordObject(character, "Changed Character");

        switch (tab)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default: break;
        }

        showDefault = EditorGUILayout.Foldout(showDefault, "默认编辑器");
        if (showDefault) DrawDefaultInspector();

        if (GUI.changed)
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCompleteObjectUndo(character, "Changed Character");
            EditorUtility.SetDirty(character);
        }
#endif

    }

    #endregion
}
#endif