using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class CompressUIAtlasCommand : EditorWindow
{
    public enum MaxSize
    {
        LESS_256 = 0,
        LESS_512 = 10,
        LESS_1024 = 20,
        GREATER_EQUAL_1024 = 30,
    }
    public enum HasAlpha
    {
        NOALPHA = 0,
        HASALPHA = 1,
    }
    public enum CompressLevel
    {
        LOW = 0,
        MEDIUM = 100,
        HIGH = 200,
    }
    public enum CompressPlatform
    {
        ANDROID = 0,
        IPHONE = 1000,
        DEFAULT = 2000,
    }

    public static string[] dirPaths;
    public static int fileCount = 0;
    public static float halveRate = 1f;
    public static string[] selectDirPaths;
    public static int CompressQuality = 50;
    public static int compressToolBarIndex;
    public static bool isCheckAlpha = false;
    public static CompressAtlasConfig config;
    public static Object fileObj = new Object();
    public static bool isOverridePlatform = false;
    public static bool isCheckWidthHeight_IOS = false;
    public static bool isCheckWidthHeight_android = false;
    public static CompressLevel level = CompressLevel.LOW;
    public static List<Object> fileObjs = new List<Object>();
    public static List<string> filePaths = new List<string>();
    public static string[] compressToolBar = { "图片检查", "图片压缩" };
    public static CompressPlatform platform = CompressPlatform.ANDROID;
    public static Vector2 scrollview1 = new Vector2();
    public static bool foldout1 = false;
    public static bool foldout2 = false;
    //白名单
    public static List<Object> whiteListFileObjs = new List<Object>();
    public static List<string> whiteListfilePaths = new List<string>();
    public static List<string> whiteListfileNames = new List<string>();
    public static List<string> whiteListfolderPaths = new List<string>();
    public static List<string> whiteListfolderNames = new List<string>();
    public static List<CompressLevel> whiteListFileObjsLevels = new List<CompressLevel>();
    static Dictionary<string, CompressLevel> whiteListLevels = new Dictionary<string, CompressLevel>();
    //和合法性检测
    public static List<string> illegalAlphafilePaths = new List<string>();
    public static List<string> illegalWidthHeightfilePaths_IOS = new List<string>();
    public static List<string> illegalWidthHeightfilePaths_Android = new List<string>();
    public static TextureImporterFormat textureImporterFormat = TextureImporterFormat.ASTC_6x6;
    static Dictionary<int, TextureImporterFormat> platformFormat = new Dictionary<int, TextureImporterFormat>();
    static string configSOPath = "Assets/Editor/AssetBuilder/AppBuildResources/CompressAtlasSettings.asset";

    [MenuItem("公共工具/图片MaxSize压缩工具")]
    static void Init()
    {
        GetWindow(typeof(CompressUIAtlasCommand));
    }

    CompressUIAtlasCommand()
    {
        this.titleContent = new GUIContent("图片MaxSize压缩工具");
    }

    private void OnGUI()
    {
        if (config == null) LoadConfigFromSO();

        scrollview1 = EditorGUILayout.BeginScrollView(scrollview1);

        GUILayout.Space(10);
        compressToolBarIndex = GUILayout.Toolbar(compressToolBarIndex, compressToolBar);

        isOverridePlatform = true;
        GUILayout.Space(10);
        if (compressToolBarIndex == 1)
        {
            EditorGUILayout.LabelField("覆写平台", "Android & IOS");
            level = (CompressLevel)EditorGUILayout.EnumPopup("机型品质", level);
            GUILayout.Space(10);
            if (GUILayout.Button("开始压缩", GUILayout.Height(25)))
            {
                if (platformFormat.Count == 0)
                {
                    InitTextureFormatLibrary();
                }
                SetAtlas();
            }
        }
        else if (compressToolBarIndex == 0)
        {
            isCheckAlpha = EditorGUILayout.Toggle("检查Alpha", isCheckAlpha);
            isCheckWidthHeight_IOS = EditorGUILayout.Toggle("检查宽高合法性 IOS", isCheckWidthHeight_IOS);
            isCheckWidthHeight_android = EditorGUILayout.Toggle("检查宽高合法性 Android", isCheckWidthHeight_android);
            GUILayout.Space(10);
            if (GUILayout.Button("开始检查", GUILayout.Height(25)))
            {
                SetAtlas();
                ShowIllegalFiles();
            }
        }
        GUILayout.Space(10);
        //非白名单
        UnityEngine.Object[] objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        //文件夹列表
        foldout1 = EditorGUILayout.Foldout(foldout1, "当前选择文件夹目录: (请在Project面板拖拽文件夹目录进来) ");
        if (foldout1)
        {
            for (int i = 0; i < fileObjs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_toggle_mixed_bg"), GUILayout.Width(25), GUILayout.Height(25)))
                {
                    fileObjs.Remove(fileObjs[i]);
                    continue;
                }
                fileObjs[i] = EditorGUILayout.ObjectField(fileObjs[i], typeof(Object), true, GUILayout.Height(25));
                EditorGUILayout.EndHorizontal();

            }
        }
        //增加按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_CreateAddNew"), GUILayout.Height(25)))
        {
            fileObjs.Add(new Object());
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        //白名单
        UnityEngine.Object[] whiteObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        //文件夹列表
        foldout2 = EditorGUILayout.Foldout(foldout2, "白名单文件/文件夹目录: (请在Project面板拖拽文件夹目录进来) ");
        if (foldout2)
        {
            for (int i = 0; i < whiteListFileObjs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_toggle_mixed_bg"), GUILayout.Width(25), GUILayout.Height(25)))
                {
                    whiteListFileObjs.Remove(whiteListFileObjs[i]);
                    continue;
                }
                whiteListFileObjs[i] = EditorGUILayout.ObjectField(whiteListFileObjs[i], typeof(Object), true, GUILayout.Height(25));
                EditorGUILayout.LabelField("机型品质", GUILayout.Width(50));
                whiteListFileObjsLevels[i] = (CompressLevel)EditorGUILayout.EnumPopup(whiteListFileObjsLevels[i]);
                EditorGUILayout.EndHorizontal();

            }
        }
        //增加按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_CreateAddNew"), GUILayout.Height(25)))
        {
            whiteListFileObjs.Add(new Object());
            whiteListFileObjsLevels.Add(CompressLevel.MEDIUM);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        //保存按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存配置", GUILayout.Height(25)))
        {
            SaveConfigToSO();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(60);
        EditorGUILayout.EndScrollView();

    }

    public static void SetAtlas()
    {
        UnityEngine.Object[] objects = fileObjs.ToArray();

        dirPaths = new string[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            var dirPath = AssetDatabase.GetAssetPath(objects[i]).Replace("\\", "/");
            if (!Directory.Exists(dirPath))
            {
                EditorUtility.DisplayDialog("错误", "选择正确文件夹", "好的");
                dirPaths = null;
                break;
            }
            dirPaths[i] = dirPath;
        }
        //TODO:添加白名单文件目录
        whiteListfileNames.Clear();
        whiteListfilePaths.Clear();
        whiteListfolderNames.Clear();
        whiteListfolderPaths.Clear();
        UnityEngine.Object[] whiteObjects = whiteListFileObjs.ToArray();
        for (int i = 0; i < whiteObjects.Length; i++)
        {
            var dirPath = AssetDatabase.GetAssetPath(whiteObjects[i]).Replace("\\", "/");
            if (dirPath.EndsWith(".png"))
            {
                var fileName = dirPath.Split('/');
                whiteListfilePaths.Add(dirPath);
                whiteListfileNames.Add(fileName[fileName.Length - 1]);
                whiteListLevels[fileName[fileName.Length - 1]] = whiteListFileObjsLevels[i];
            }
            else
            {
                if (!Directory.Exists(dirPath))
                {
                    EditorUtility.DisplayDialog("错误", "选择正确文件夹", "好的");
                    whiteListfolderPaths.Clear();
                    break;
                }
                var folderName = dirPath.Split('/');
                whiteListfolderPaths.Add(dirPath);
                whiteListfolderNames.Add(folderName[folderName.Length - 1]);
                whiteListLevels[folderName[folderName.Length - 1]] = whiteListFileObjsLevels[i];
            }

        }

        if (dirPaths.Length == 0)
        {
            Debug.Log($"文件夹路径为空");
        }
        else
        {
            //非白名单文件夹
            for (int i = 0; i < dirPaths.Length; i++)
            {
                SetSprite(dirPaths[i], false);
            }
            //白名单文件
            for (int i = 0; i < whiteListfilePaths.Count; i++)
            {
                SetSprite(whiteListfilePaths[i], true);
            }
            //白名单文件夹
            for (int i = 0; i < whiteListfolderPaths.Count; i++)
            {
                SetSprite(whiteListfolderPaths[i], true);
            }

            AssetDatabase.SaveAssets();
            for (int i = 0; i < filePaths.Count; i++)
            {
                DoAssetReimport(filePaths[i], ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("成功", "处理完成！", "好的");
            Debug.Log($"[图片压缩完成]: 共处理 {fileCount} 个文件");
            whiteListLevels.Clear();
            filePaths.Clear();
            fileCount = 0;
        }
    }
    static void SaveConfigToSO()
    {
        var to = AssetDatabase.LoadAssetAtPath<CompressAtlasConfig>(configSOPath);
        if (to == null) return;
        to.level = level;
        to.fileObjs = fileObjs;
        to.isCheckAlpha = isCheckAlpha;
        to.whiteListFileObjs = whiteListFileObjs;
        to.isCheckWidthHeight_IOS = isCheckWidthHeight_IOS;
        to.whiteListFileObjsLevels = whiteListFileObjsLevels;
        to.isCheckWidthHeight_android = isCheckWidthHeight_android;
        EditorUtility.SetDirty(to);
        AssetDatabase.SaveAssets();
    }
    static void LoadConfigFromSO()
    {
        config = AssetDatabase.LoadAssetAtPath<CompressAtlasConfig>(configSOPath);
        if (config == null) return;
        level = config.level;
        fileObjs = config.fileObjs;
        isCheckAlpha = config.isCheckAlpha;
        whiteListFileObjs = config.whiteListFileObjs;
        isCheckWidthHeight_IOS = config.isCheckWidthHeight_IOS;
        whiteListFileObjsLevels = config.whiteListFileObjsLevels;
        isCheckWidthHeight_android = config.isCheckWidthHeight_android;
    }

    private static void ShowIllegalFiles()
    {
        Debug.Log($"[Alpha] Alpha通道检测");
        foreach (var filePath in illegalAlphafilePaths)
        {
            Debug.Log($"[Alpha] 图片没有Alpha通道! {filePath}");
        }
        Debug.Log($"[Android] 图片宽高合法性检测");
        foreach (var filePath in illegalWidthHeightfilePaths_Android)
        {
            Debug.Log($"[Android] 图片宽高不合法 不是2的n次幂! {filePath}");
        }
        Debug.Log($"[IOS] 图片宽高合法性检测");
        foreach (var filePath in illegalWidthHeightfilePaths_IOS)
        {
            Debug.Log($"[IOS] 图片宽高不合法 不是4的倍数! {filePath}");
        }

        illegalAlphafilePaths.Clear();
        illegalWidthHeightfilePaths_IOS.Clear();
        illegalWidthHeightfilePaths_Android.Clear();
    }

    /// <summary>
    /// 初始化材质格式库
    /// </summary>
    static void InitTextureFormatLibrary()
    {
        //android
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.LOW, HasAlpha.NOALPHA, TextureImporterFormat.ETC_RGB4, TextureImporterFormat.ETC_RGB4, TextureImporterFormat.ETC_RGB4, TextureImporterFormat.ETC_RGB4);
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.LOW, HasAlpha.HASALPHA, TextureImporterFormat.ETC2_RGB4, TextureImporterFormat.ETC2_RGB4, TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ETC2_RGBA8);
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.MEDIUM, HasAlpha.NOALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.MEDIUM, HasAlpha.HASALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.HIGH, HasAlpha.NOALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.ANDROID, CompressLevel.HIGH, HasAlpha.HASALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        //iphone
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.LOW, HasAlpha.NOALPHA, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_6x6);
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.LOW, HasAlpha.HASALPHA, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_8x8, TextureImporterFormat.ASTC_6x6);
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.MEDIUM, HasAlpha.NOALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.MEDIUM, HasAlpha.HASALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.HIGH, HasAlpha.NOALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
        SetTextureFormatForAllSize(CompressPlatform.IPHONE, CompressLevel.HIGH, HasAlpha.HASALPHA, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_6x6, TextureImporterFormat.ASTC_4x4);
    }

    private static int GetValidSize(int size)
    {
        int result = 0;
        if (size <= 32)
            result = 32;
        else if (size <= 64)
            result = 64;
        else if (size <= 128)
            result = 128;
        else if (size <= 256)
            result = 256;
        else if (size <= 512)
            result = 512;
        else if (size <= 1024)
            result = 1024;
        else if (size <= 2048)
            result = 2048;
        else
            result = 4096;

        return result;
    }

    public static void ShowSelectAtlasAddress()
    {
        UnityEngine.Object[] objects = fileObjs.ToArray();// Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        selectDirPaths = new string[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            var dirPath = AssetDatabase.GetAssetPath(objects[i]).Replace("\\", "/");
            if (!Directory.Exists(dirPath))
            {
                continue;
            }
            selectDirPaths[i] = dirPath;
        }
    }

    private static MaxSize GetMaxSizeEnum(int size)
    {
        var maxSize = MaxSize.LESS_256;
        if (size < 256)
            maxSize = MaxSize.LESS_256;
        else if (size < 512)
            maxSize = MaxSize.LESS_512;
        else if (size < 1024)
            maxSize = MaxSize.LESS_1024;
        else if (size >= 1024)
            maxSize = MaxSize.GREATER_EQUAL_1024;
        return maxSize;
    }

    private static bool CheckIsTwoToThePowerN(int num)
    {
        if (num < 2) return false;
        while (num % 2 == 0)
        {
            if (num == 2) return true;
            num /= 2;
        }
        return false;
    }

    private static bool CheckIsMultipleOfFour(int num)
    {
        if (num < 4) return false;
        return (num % 4 == 0) ? true : false;
    }

    static string[] GetFileAndFolderName(string filePath)
    {
        var fileSplit = filePath.Split('/');
        var fileName = fileSplit[fileSplit.Length - 1];
        var folderName = fileSplit[fileSplit.Length - 2];
        return new string[] { fileName, folderName };
    }

    static bool CheckAndRemoveInWhiteList(string filePath)
    {
        //TODO:剔除白名单文件夹及文件
        var fileSplit = GetFileAndFolderName(filePath);
        var fileName = fileSplit[0];
        var folderName = fileSplit[1];
        //剔除白名单文件
        for (int j = 0; j < whiteListfileNames.Count; j++)
        {
            if (fileName.Equals(whiteListfileNames[j]))
            {
                whiteListfileNames.Remove(fileName);
                return true;
            }
        }
        //剔除白名单文件夹
        for (int j = 0; j < whiteListfolderNames.Count; j++)
        {
            if (folderName.Equals(whiteListfolderNames[j]))
            {
                return true;
            }
        }
        return false;
    }

    private static bool CheckHasAlpha(TextureImporter textureImporter)
    {
        return textureImporter.DoesSourceTextureHaveAlpha();
    }

    static string[] GetAllFilesInPath(string dirPath, out bool isFolder)
    {
        string[] files;
        //文件夹 or 文件
        isFolder = dirPath.Split('.').Length <= 1;
        if (isFolder)
        {
            files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
        }
        else
        {
            files = new string[] { dirPath };
        }
        return files;
    }

    private static void SetSprite(string dirPath, bool isWhiteList = false)
    {
        string[] files = GetAllFilesInPath(dirPath, out bool isFolder);
        string tips2 = isFolder ? "文件夹" : "文件";
        string tips1 = isWhiteList ? "白名单" : "非白名单";
        Debug.Log($"[开始压缩{tips1}{tips2}] {dirPath}");
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            filePath = filePath.Replace("\\", "/");

            if (filePath.EndsWith(".png"))
            {
                //TODO:剔除非白名单文件中的白名单文件夹及文件
                if (!isWhiteList && CheckAndRemoveInWhiteList(filePath)) continue;

                filePaths.Add(filePath);
                EditorUtility.DisplayProgressBar("处理中>>>", filePath, (float)i / (float)files.Length);
                //Debug.Log($"处理文件: {filePath}");

                TextureImporter textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (textureImporter == null) return;

                //textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;

                AssetDatabase.ImportAsset(filePath);

                GetTextureOriginalSize(textureImporter, out int width, out int height);
                //Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);

                int textureSize = Mathf.Max(height, width);

                if (isCheckAlpha || isCheckWidthHeight_android || isCheckWidthHeight_IOS)
                {
                    CheckLegality(width, height, filePath, textureImporter);
                }
                else
                {
                    switch (platform)
                    {
                        //移动平台
                        case CompressPlatform.ANDROID:
                            CompressMobilePNG(textureSize, filePath, isWhiteList, ref textureImporter);
                            break;

                        //默认平台
                        case CompressPlatform.DEFAULT:

                            TextureImporterFormat defaultTextureFormat = textureImporterFormat;
                            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                            settings = textureImporter.GetPlatformTextureSettings("default");

                            textureImporter.textureType = TextureImporterType.Default;
                            int defaultMaxTextureSize = settings.maxTextureSize;
                            defaultMaxTextureSize = Mathf.Min(textureSize, defaultMaxTextureSize);
                            defaultMaxTextureSize = (int)(defaultMaxTextureSize * halveRate);
                            settings.format = defaultTextureFormat;
                            settings.maxTextureSize = GetValidSize(defaultMaxTextureSize);
                            break;
                    }
                }

            }
        }

    }

    public static void DoAssetReimport(string path, ImportAssetOptions options)
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            AssetDatabase.ImportAsset(path, options);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    static bool GetWhiteListCompressLevel(string filePath, out CompressLevel level)
    {
        CompressLevel curLevel = CompressLevel.LOW;
        var names = GetFileAndFolderName(filePath);
        var fileName = names[0];
        var folderName = names[1];
        if (!whiteListLevels.TryGetValue(fileName, out curLevel))
        {
            Debug.LogWarning($"[读取白名单文件品质]: 文件 {fileName} 品质 {curLevel} 路径 {filePath}");
            level = curLevel;
            return true;
        }
        if (!whiteListLevels.TryGetValue(folderName, out curLevel))
        {
            Debug.LogWarning($"[读取白名单文件夹品质]: 文件夹 {folderName} 品质 {curLevel} 路径 {filePath}");
            level = curLevel;
            return true;
        }
        level = curLevel;
        return false;
    }

    public static void GetTextureOriginalSize(TextureImporter ti, out int width, out int height)
    {
        if (ti == null)
        {
            width = 0;
            height = 0;
            return;
        }

        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(ti, args);

        width = (int)args[0];
        height = (int)args[1];
    }

    private static void CheckLegality(int width, int height, string filePath, TextureImporter textureImporter)
    {
        //检测是否带alpha通道 
        if (isCheckAlpha)
        {
            if (!CheckHasAlpha(textureImporter))
            {
                illegalAlphafilePaths.Add(filePath);
            }
        }
        //Android检测宽高是否不是2的n次幂
        if (isCheckWidthHeight_android)
        {
            bool isWidthLegal = CheckIsTwoToThePowerN(width);
            bool isHeightLegal = CheckIsTwoToThePowerN(height);
            if (!isWidthLegal || !isHeightLegal)
            {
                illegalWidthHeightfilePaths_Android.Add(filePath);
            }
        }
        //IOS检测宽高是否不是4的倍数
        if (isCheckWidthHeight_IOS)
        {
            bool isWidthLegal = CheckIsMultipleOfFour(width);
            bool isHeightLegal = CheckIsMultipleOfFour(height);
            if (!isWidthLegal || !isHeightLegal)
            {
                illegalWidthHeightfilePaths_IOS.Add(filePath);
            }
        }
    }

    private static void CompressMobilePNG(int textureSize, string filePath, bool isWhiteList, ref TextureImporter textureImporter)
    {
        if (platform == CompressPlatform.DEFAULT) return;
        var compressLevel = level;
        //读取白名单压缩品质
        if (isWhiteList && GetWhiteListCompressLevel(filePath, out var curlevel))
        {
            compressLevel = curlevel;
        }
        int isOptimized = 0;
        TextureImporterPlatformSettings textureSettings = null;
        // 安卓 or 苹果
        for (int i = 0; i < 2; i++)
        {
            int maxTextureSize = 0;
            TextureImporterFormat textureFormat = TextureImporterFormat.ASTC_6x6;
            var curPlatformEnum = i == 0 ? CompressPlatform.ANDROID : CompressPlatform.IPHONE;
            string curPlatformStr = (i == (int)CompressPlatform.ANDROID) ? "Android" : "iPhone";
            bool isOverWrite = textureImporter.GetPlatformTextureSettings(curPlatformStr, out maxTextureSize, out textureFormat);
            if (isOverWrite || isOverridePlatform)
            {
                maxTextureSize = textureSize;//Mathf.Min(textureSize, maxTextureSize);
                maxTextureSize = (int)(maxTextureSize * halveRate);
                maxTextureSize = GetValidSize(maxTextureSize);

                textureSettings = textureImporter.GetPlatformTextureSettings(curPlatformStr);

                textureSettings.overridden = true;
                var targetSize = maxTextureSize;
                textureSettings.compressionQuality = CompressQuality;
                HasAlpha hasAlpha = textureImporter.DoesSourceTextureHaveAlpha() ? HasAlpha.HASALPHA : HasAlpha.NOALPHA;
                var targetFormat = GetTextureFormat(curPlatformEnum, compressLevel, GetMaxSizeEnum(maxTextureSize), hasAlpha);

                //配置最优无需压缩
                if (targetSize == textureSettings.maxTextureSize
                && targetFormat == textureSettings.format)
                {
                    isOptimized++;
                    filePaths.Remove(filePath);
                    Debug.Log($"[{curPlatformEnum}压缩最优 无需设置] 格式：{textureSettings.format} 材质尺寸：{textureSettings.maxTextureSize} 路径: {filePath}");
                }
                else
                {
                    textureSettings.format = targetFormat;
                    textureSettings.maxTextureSize = targetSize;
                    textureImporter.SetPlatformTextureSettings(textureSettings);
                }

            }
            else
            {
                Debug.Log($"文件未启用{platform}平台 filePath: {filePath}");
            }
        }
        if (textureSettings != null)
        {
            fileCount++;
            if (isOptimized != 2)
            {
                Debug.Log($"压缩图片: {filePath} 格式：{textureSettings.format} 材质尺寸：{textureSettings.maxTextureSize}");
            }
        }
        else
        {
            Debug.LogWarning($"压缩图片: {filePath} 出错!");
        }
    }

    static TextureImporterFormat GetTextureFormat(CompressPlatform platform, CompressLevel level, MaxSize maxSize, HasAlpha hasAlpha)
    {
        var key = (int)platform + (int)level + (int)maxSize + (int)hasAlpha;
        if (!platformFormat.TryGetValue(key, out TextureImporterFormat format))
        {
            var defaultFormat = TextureImporterFormat.ETC_RGB4;
            Debug.LogWarning($"读取对应压缩格式失败！platform:{platform} level:{level} maxSize:{maxSize} hasAlpha:{hasAlpha} 设置为默认模式 {defaultFormat}");
            return defaultFormat;
        }
        return format;
    }

    static void SetTextureFormat(CompressPlatform platform, CompressLevel level, MaxSize maxSize, HasAlpha hasAlpha, TextureImporterFormat format)
    {
        //千位 platform 百位 level 十位 maxSize 个位 hasAlpha
        var key = (int)platform + (int)level + (int)maxSize + (int)hasAlpha;
        platformFormat[key] = format;
    }

    static void SetTextureFormatForAllSize(CompressPlatform platform, CompressLevel level, HasAlpha hasAlpha, params TextureImporterFormat[] formats)
    {
        if (formats.Length == 0) return;
        int i = 0;
        foreach (var maxsize in System.Enum.GetValues(typeof(MaxSize)))
        {
            if (i > formats.Length - 1) return;
            SetTextureFormat(platform, level, (MaxSize)maxsize, hasAlpha, formats[i++]);
        }
    }
}



