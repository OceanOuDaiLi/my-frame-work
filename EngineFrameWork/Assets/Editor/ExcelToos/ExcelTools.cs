﻿using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

public class ExcelTools : EditorWindow
{
    private static string outPutPath;
    private static string excelType = ".xlsx";                  // xlsx or others ..
    private static string excelFlodName = "Excel";
    private static string excelFloderPath = string.Empty;
    private static string jsonFlodName = "NotAssetBundle";

    private static ExcelTools instance;
    private static List<string> excelList;

    private static Vector2 scrollPos;
    private static int indexOfFormat = 0;
    private static bool isCombineOneFile = true;
    private static string[] formatOption = new string[] { "JSON", "CSV", "XML", "LUA" };

    public static string cSharpTemplet = "// Dynamically generated by config tool. Do not modify the text content manually."
    + "\n"
    + "\n"
    + "namespace Model.DB {";

    [MenuItem("公共工具/配置转表")]
    static void ShowExcelTools()
    {
        instance = EditorWindow.GetWindow<ExcelTools>();
        excelList = new List<string>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        excelFloderPath = Application.dataPath + "/ABAssets/" + excelFlodName;

        LoadExcel();

        instance.Show();
    }

    private static void CreateToJson(ExcelUtility excel, Encoding encoding, string excelPath, int i)
    {
        excelPath = excelPath.Replace(excelFlodName, jsonFlodName);
        outPutPath = excelPath.Replace(excelType, ".json");

        //表名称
        int lastIndex = excelList[i].LastIndexOf(@"\");
        string xlsx = excelList[i].Substring(lastIndex).ToString();

        xlsx = xlsx.Remove(0, 1);
        xlsx = xlsx.Remove(xlsx.IndexOf('.'));

        bool isEnd = i == excelList.Count - 1;

        excel.CreateCSharpBaseDateFile(xlsx, isEnd);

        if (!isCombineOneFile)
        {
            excel.ConvertToJson(outPutPath, encoding);
        }
        else
        {
            excel.ConvertToOneFileJson(outPutPath, encoding, xlsx, isEnd);
        }
    }

    private static void Convert()
    {
        ExcelUtility.targetClass = new Dictionary<string, object>();
        for (int i = 0; i < excelList.Count; i++)
        {
            string excelPath = excelList[i];

            ExcelUtility excel = new ExcelUtility(excelPath);

            // 固定编码类型 utf-8
            Encoding encoding = Encoding.GetEncoding("utf-8");

            switch (indexOfFormat)
            {
                case 0:
                    ExcelUtility.InitCSharpScript();
                    CreateToJson(excel, encoding, excelPath, i);
                    break;
                case 1:
                    outPutPath = excelPath.Replace(excelType, ".csv");
                    excel.ConvertToCSV(outPutPath, encoding);
                    break;
                case 2:
                    outPutPath = excelPath.Replace(excelType, ".xml");
                    excel.ConvertToXml(outPutPath);
                    break;
                case 3:
                    outPutPath = excelPath.Replace(excelType, ".lua");
                    excel.ConvertToLua(outPutPath, encoding);
                    break;
                default:
                    Debug.LogError("indexOfFormat" + indexOfFormat);
                    break;
            }


            AssetDatabase.Refresh();
        }

        if (instance)
        {
            instance.Close();
        }
    }

    private static void LoadExcel()
    {
        if (excelList == null)
        {
            excelList = new List<string>();
        }
        excelList.Clear();

        // Get All Excels.
        string[] allFiles = Directory.GetFiles(excelFloderPath, "*.*", SearchOption.AllDirectories);
        foreach (var item in allFiles)
        {
            if (item.EndsWith(excelType))
            {
                excelList.Add(item);
            }
        }
    }

    #region GUI Methods

    private void OnGUI()
    {
        DrawOptions();

        DrawExport();
    }

    private void DrawExport()
    {
        if (excelList == null) return;

        if (excelList.Count < 1)
        {
            GUILayout.Space(30);
            EditorGUILayout.LabelField("目前没有Excel文件被选中!");
        }
        else
        {
            EditorGUILayout.LabelField("下列项目将被转换为" + formatOption[indexOfFormat] + ":");

            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(150));
            foreach (string s in excelList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Toggle(true, s);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            if (GUILayout.Button("导出配置"))
            {
                Convert();
            }
        }
    }

    private void DrawOptions()
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
        indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tips:默认输出路径 ABAssets/NotAssetBundle/Json", GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (indexOfFormat == 0)
        {
            isCombineOneFile = GUILayout.Toggle(isCombineOneFile, "生成一个LocalData.json");
            GUILayout.Space(20);
        }
    }


    //private void OnSelectionChange()
    //{
    //    //当选择发生变化时重绘窗体
    //    Show();
    //    LoadExcel();
    //    Repaint();
    //}

    #endregion
}