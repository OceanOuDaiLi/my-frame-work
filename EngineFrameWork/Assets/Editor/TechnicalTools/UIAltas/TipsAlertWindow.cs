using UnityEngine;
using UnityEditor;
using System;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	TipsAlertWindow.cs
	Author:		DaiLi.Ou

	Descriptions:
*********************************************************************/
namespace AssetBundleBrowser
{
    public class TipsAlertWindow : EditorWindow
    {
        #region Variables

        //ShowAlertWithBtn
        static private string tipsTitle = string.Empty;
        static private string tipsContent = string.Empty;
        static private Action comfirm = null;
        static private Action cancel = null;
        #endregion


        public static void ShowAlertWithBtn(string _tipsTitle, string _tipsContent, Action _comfirm = null, Action _cancel = null)
        {
            tipsTitle = _tipsTitle;
            tipsContent = _tipsContent;
            comfirm = _comfirm;
            cancel = _cancel;

            TipsAlertWindow window = ScriptableObject.CreateInstance<TipsAlertWindow>();
            window.autoRepaintOnSceneChange = true;
            window.titleContent = new GUIContent(tipsTitle);
            window.position = new Rect(Screen.width / 2 - 200, Screen.height / 2, 350, 200);
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(tipsContent, EditorStyles.wordWrappedLabel);
            GUILayout.Space(70);


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("确定"))
            {
                comfirm?.Invoke();
                Close();
            }

            if (GUILayout.Button("取消"))
            {
                cancel?.Invoke();
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}