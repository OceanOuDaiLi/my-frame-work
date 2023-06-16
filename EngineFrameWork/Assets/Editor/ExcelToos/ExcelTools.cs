using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

public class ExcelTools : EditorWindow
{
    private static string excelType = ".xlsx"; //xls

    private static string outPutPath = "";

    //Excel文件夹名
    private static string excelFlodName = "Excel";

    private static string jsonFlodName = "NotAssetBundle";

    /// <summary>
    /// 当前编辑器窗口实例
    /// </summary>
    private static ExcelTools instance;

    /// <summary>
    /// Excel文件列表
    /// </summary>
    private static List<string> excelList;

    /// <summary>
    /// 项目根路径	
    /// </summary>
    private static string pathRoot;

    /// <summary>
    /// 滚动窗口初始位置
    /// </summary>
    private static Vector2 scrollPos;

    /// <summary>
    /// 输出格式索引
    /// </summary>
    private static int indexOfFormat = 0;

    /// <summary>
    /// 输出格式
    /// </summary>
    private static string[] formatOption = new string[] { "JSON", "CSV", "XML", "LUA" };

    /// <summary>
    /// 编码索引
    /// </summary>
    private static int indexOfEncoding = 0;

    /// <summary>
    /// 编码选项
    /// </summary>
    private static string[] encodingOption = new string[] { "ASCII" };

    /// <summary>
    /// 是否保留原始文件
    /// </summary>
    private static bool keepSource = true;


    /// <summary>
    /// 是否生成到同一个文件
    /// </summary>
    private static bool isCommonJsonConvert = false;

    /// <summary>
    /// 是否生成到BaseDataDefine
    /// </summary>
    private static bool createBaseDefine = false;

    public static string typeScriptTemplet = "//文件由工具生成,修改可能会产生意外的问题,并且任何改动将在文件重新生成时丢失"
    + "\n"
    + "using Model;"
    + "\n"
    + "namespace DB {"
    ;


    /// <summary>
    /// 显示当前窗口	
    /// </summary>
    [MenuItem("Tools/配置表转Json")]
    static void ShowExcelTools()
    {
        Init();
        //加载Excel文件
        LoadExcel();
        instance.Show();
    }

    void OnGUI()
    {
        DrawOptions();
        DrawExport();
    }

    /// <summary>
    /// 绘制插件界面配置项
    /// </summary>
    private void DrawOptions()
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
        indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择编码类型:", GUILayout.Width(85));
        indexOfEncoding = EditorGUILayout.Popup(indexOfEncoding, encodingOption, GUILayout.Width(125));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tips:默认输出路径 ABAssets/NotAssetBundle/Json", GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        isCommonJsonConvert = GUILayout.Toggle(isCommonJsonConvert, "生成一个LocalData.json");

        createBaseDefine = GUILayout.Toggle(createBaseDefine, "生成BaseDataDefine");
    }

    /// <summary>
    /// 绘制插件界面输出项
    /// </summary>
    private void DrawExport()
    {
        if (excelList == null) return;
        if (excelList.Count < 1)
        {
            GUILayout.Space(30);
            EditorGUILayout.LabelField("目前没有Excel文件被选中哦!");
            EditorGUILayout.LabelField("请在Unity中单个或多个选中需要转换的Excel文档！");
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

            //输出
            if (GUILayout.Button("转换"))
            {
                Convert();
            }
        }
    }

    private static void CreateJson(ExcelUtility excel, Encoding encoding, string excelPath, int i)
    {
        excelPath = excelPath.Replace(excelFlodName, jsonFlodName);
        outPutPath = excelPath.Replace(excelType, ".json");

        //表名称
        int lastIndex = excelList[i].LastIndexOf('/');
        string xlsx = excelList[i].Substring(lastIndex).ToString();
        xlsx = xlsx.Remove(0, 1);
        xlsx = xlsx.Remove(xlsx.IndexOf('.'));

        bool isEnd = i == excelList.Count - 1;

        if (createBaseDefine)
        {
            excel.CreateBaseDataDefine(xlsx, isEnd);
        }

        if (!isCommonJsonConvert)
        {
            excel.ConvertToJson(outPutPath, encoding);
        }
        else
        {
            excel.ConvertToOneTempJson(outPutPath, encoding, xlsx, isEnd);
        }
    }


    /// <summary>
    /// 转换Excel文件
    /// </summary>
    private static void Convert()
    {
        if (createBaseDefine)
        {
            ExcelUtility.InitTypeScript();
        }

        ExcelUtility.targetClass = new Dictionary<string, object>();
        for (int i = 0; i < excelList.Count; i++)
        {
            //获取Excel文件的绝对路径
            string excelPath = System.Environment.CurrentDirectory + "/" + excelList[i];

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(excelPath);

            //编码类型
            Encoding encoding = Encoding.GetEncoding("utf-8");

            switch (indexOfFormat)
            {
                case 0:
                    CreateJson(excel, encoding, excelPath, i);
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

            var _outPutPath = isCommonJsonConvert ? outPutPath : "NotAssetBundle/Json/localdata.json";
            Debug.Log("outPutPath:        " + _outPutPath);


            //判断是否保留源文件
            if (!keepSource)
            {
                FileUtil.DeleteFileOrDirectory(excelPath);
            }

            //刷新本地资源
            AssetDatabase.Refresh();
        }

        //转换完后关闭插件
        //这样做是为了解决窗口
        //再次点击时路径错误的Bug
        if (instance)
        {
            instance.Close();
        }
    }

    /// <summary>
    /// 加载Excel
    /// </summary>
    private static void LoadExcel()
    {
        if (excelList == null)
        {
            excelList = new List<string>();
        }
        excelList.Clear();

        //获取选中的对象
        object[] selection = (object[])Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;
        //遍历每一个对象判断不是Excel文件
        foreach (Object obj in selection)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            if (objPath.EndsWith(excelType))
            {
                excelList.Add(objPath);
            }
        }
    }

    private static void Init()
    {
        //获取当前实例
        instance = EditorWindow.GetWindow<ExcelTools>();
        //初始化
        pathRoot = Application.dataPath;
        //注意这里需要对路径进行处理
        //目的是去除Assets这部分字符以获取项目目录
        //我表示Windows的/符号一直没有搞懂
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        excelList = new List<string>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
    }

    void OnSelectionChange()
    {
        //当选择发生变化时重绘窗体
        Show();
        LoadExcel();
        Repaint();
    }
}
