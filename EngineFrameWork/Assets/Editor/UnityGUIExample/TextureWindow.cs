using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MyUnityTextureWindow : EditorWindow
{

    [MenuItem(("公共工具/UnityTextureWindow"), priority = 300)]
    static void Init()
    {
        EditorWindow.GetWindow<MyUnityTextureWindow>("MyUnityTextureWindow");
    }

    Vector2 m_Scroll;
    List<string> m_Icons = null;
    void Awake()
    {
        m_Icons = new List<string>();
        Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (Texture2D x in t)
        {
            Debug.unityLogger.logEnabled = false;
            GUIContent gc = EditorGUIUtility.IconContent(x.name);
            Debug.unityLogger.logEnabled = true;
            if (gc != null && gc.image != null)
            {
                m_Icons.Add(x.name);
            }
        }
        Debug.Log(m_Icons.Count);
    }
    void OnGUI()
    {
        m_Scroll = GUILayout.BeginScrollView(m_Scroll);
        float width = 50f;
        int count = (int)(position.width / width);
        for (int i = 0; i < m_Icons.Count; i += count)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < count; j++)
            {
                int index = i + j;
                if (index < m_Icons.Count)
                {
                    EditorGUILayout.SelectableLabel(m_Icons[index]);
                    GUILayout.Button(EditorGUIUtility.IconContent(m_Icons[index]), GUILayout.Width(width), GUILayout.Height(30));
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}
