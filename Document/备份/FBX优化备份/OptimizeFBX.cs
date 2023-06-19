using System.IO;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public enum TextureType
{
    ALBEDO,
    LIT,
    NORMAL,
}

public enum PlayerPartEnum
{
    HEAD,
    BODY,
    LIMB,
    SHOES,
    BASKETBALL,
    BASKETBALLSTAND,
    ARMGUARD,
    HEADBAND,
    KNEEGUARD,
}

public enum OptimizeFBXCompressLevel
{
    LOW = 0,
    MEDIUM = 100,
    HIGH = 200,
}

public class TextureRootPaths
{
    public List<string>[] rootPaths = { new List<string>(), new List<string>(), new List<string>() };

    public void Clear()
    {
        foreach (var paths in rootPaths)
        {
            paths.Clear();
        }
    }
}

public class PlayerPartTexturesObjs
{
    public List<Object>[] TextureFileObjs = {
        new List<Object>(),
        new List<Object>(),
        new List<Object>() };
    public List<OptimizeFBXCompressLevel>[] TextureFileObjLevels = {
        new List<OptimizeFBXCompressLevel>(),
        new List<OptimizeFBXCompressLevel>(),
        new List<OptimizeFBXCompressLevel>(),};

    public void Clear()
    {
        for (int i = 0; i < TextureFileObjs.Length; i++)
        {
            TextureFileObjs[i].Clear();
            TextureFileObjLevels[i].Clear();
        }
    }

    public bool HasEmptyObjects()
    {
        foreach (var objs in TextureFileObjs)
        {
            if (objs.Count == 0)
            {
                return true;
            }
        }
        return false;
    }
}

public class OptimizeFBX : EditorWindow
{
    //GUI
    static int toolbarindex;
    public static bool[] foldouts = new bool[9];
    static string[] toolbarContent = { "FBX模型优化", "Texture纹理优化" };

    //模型优化
    static List<string> FBXfilePaths = new List<string>();
    //头部模型根目录
    static List<string> fbxHeadRootPaths = new List<string>();
    //身体模型根目录
    static List<string> fbxBodyRootPaths = new List<string>();
    //脚部模型根目录
    static List<string> fbxShoesRootPaths = new List<string>();
    //四肢模型根目录
    static List<string> fbxLimbsRootPaths = new List<string>();
    //球模型根目录
    static List<string> fbxBasketballRootPaths = new List<string>();
    //篮架模型根目录
    static List<string> fbxBasketballStandRootPaths = new List<string>();
    //护臂模型根目录
    static List<string> fbxArmGuardRootPaths = new List<string>();
    //头带模型根目录
    static List<string> fbxHeadBandRootPaths = new List<string>();
    //护膝模型根目录
    static List<string> fbxKneeGuardRootPaths = new List<string>();

    static string fbxHeadRootpath_low = @"Assets\Art\Player\player_head\model\low\";
    static string fbxHeadRootpath_high = @"Assets\Art\Player\player_head\model\high\";

    static string fbxLimbRootpath_low = @"Assets\Art\Player\player_limb\model\low\";
    static string fbxLimbRootpath_high = @"Assets\Art\Player\player_limb\model\high\";

    static string fbxBodyRootpath_low = @"Assets\Art\Player\player_body\model\low\";
    static string fbxBodyRootpath_high = @"Assets\Art\Player\player_body\model\high\";

    static string fbxShoesRootpath_low = @"Assets\Art\Player\player_shoes\model\low\";
    static string fbxShoesRootpath_high = @"Assets\Art\Player\player_shoes\model\high\";

    static string fbxBasketballRootpath_low = @"Assets\Art\BasketBall\low\model\";
    static string fbxBasketballRootpath_high = @"Assets\Art\BasketBall\high\model\";

    static string fbxBasketballStandRootpath_low = @"Assets\Art\BasketBallStand\low\model\";
    static string fbxBasketballStandRootpath_high = @"Assets\Art\BasketBallStand\high\model\";

    static string fbxArmGuardRootpath_low = @"Assets\Art\Player\player_armguard_r\model\low\";
    static string fbxArmGuardRootpath_high = @"Assets\Art\Player\player_armguard_r\model\high\";

    static string fbxHeadBandRootpath_low = @"Assets\Art\Player\player_headband\model\low\";
    static string fbxHeadBandRootpath_high = @"Assets\Art\Player\player_headband\model\high\";

    static string fbxKneeGuardRootpath_low = @"Assets\Art\Player\player_kneeguard_l\model\low\";
    static string fbxKneeGuardRootpath_high = @"Assets\Art\Player\player_kneeguard_l\model\high\";

    //纹理优化
    static TextureRootPaths headRootPaths = new TextureRootPaths();
    static TextureRootPaths limbsRootPaths = new TextureRootPaths();
    static TextureRootPaths bodyRootPaths = new TextureRootPaths();
    static TextureRootPaths shoesRootPaths = new TextureRootPaths();
    static TextureRootPaths basketballRootPaths = new TextureRootPaths();
    static TextureRootPaths basketballStandRootPaths = new TextureRootPaths();
    static TextureRootPaths armGuardRootPaths = new TextureRootPaths();
    static TextureRootPaths headBandRootPaths = new TextureRootPaths();
    static TextureRootPaths kneeGuardRootPaths = new TextureRootPaths();

    static PlayerPartTexturesObjs headTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs limbsTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs bodyTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs shoesTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs basketballTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs basketballStandTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs armGuardTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs headBandTextureObjs = new PlayerPartTexturesObjs();
    static PlayerPartTexturesObjs kneeGuardTextureObjs = new PlayerPartTexturesObjs();
    //头部纹理根目录
    static string headRootpath_texture_albedo_high = @"Assets\Art\Player\player_head\textures\high";
    static string headRootpath_texture_albedo_low = @"Assets\Art\Player\player_head\textures\low";
    static string headRootpath_texture_lit_high = @"Assets\Art\Player\sss_textures\lightTex\head\high";
    static string headRootpath_texture_lit_low = @"Assets\Art\Player\sss_textures\lightTex\head\low";
    static string headRootpath_texture_normal_high = @"Assets\Art\Player\sss_textures\normalTex\head\high";
    static string headRootpath_texture_normal_low = @"Assets\Art\Player\sss_textures\normalTex\head\low";
    //四肢纹理根目录
    static string limbsRootpath_texture_albedo_high = @"Assets\Art\Player\player_limb\textures\high";
    static string limbsRootpath_texture_albedo_low = @"Assets\Art\Player\player_limb\textures\low";
    static string limbsRootpath_texture_lit_high = @"Assets\Art\Player\sss_textures\lightTex\limb\high";
    static string limbsRootpath_texture_lit_low = @"Assets\Art\Player\sss_textures\lightTex\limb\low";
    static string limbsRootpath_texture_normal_high = @"Assets\Art\Player\sss_textures\normalTex\limb\high";
    static string limbsRootpath_texture_normal_low = @"Assets\Art\Player\sss_textures\normalTex\limb\low";
    //身体纹理根目录
    static string bodyRootpath_texture_albedo_high = @"Assets\Art\Player\player_body\textures\high";
    static string bodyRootpath_texture_albedo_low = @"Assets\Art\Player\player_body\textures\low";
    static string bodyRootpath_texture_lit_high = @"Assets\Art\Player\sss_textures\lightTex\body\high";
    static string bodyRootpath_texture_lit_low = @"Assets\Art\Player\sss_textures\lightTex\body\low";
    static string bodyRootpath_texture_normal_high = @"Assets\Art\Player\sss_textures\normalTex\body\high";
    static string bodyRootpath_texture_normal_low = @"Assets\Art\Player\sss_textures\normalTex\body\low";
    //脚部纹理根目录
    static string shoesRootpath_texture_albedo_high = @"Assets\Art\Player\player_shoes\textures\high";
    static string shoesRootpath_texture_albedo_low = @"Assets\Art\Player\player_shoes\textures\low";
    static string shoesRootpath_texture_lit_high = @"Assets\Art\Player\sss_textures\lightTex\shoes\high";
    static string shoesRootpath_texture_lit_low = @"Assets\Art\Player\sss_textures\lightTex\shoes\low";
    static string shoesRootpath_texture_normal_high = @"Assets\Art\Player\sss_textures\normalTex\shoes\high";
    static string shoesRootpath_texture_normal_low = @"Assets\Art\Player\sss_textures\normalTex\shoes\low";
    //篮球纹理根目录
    static string basketballRootpath_texture_high = @"Assets\Art\BasketBall\high\textures";
    static string basketballRootpath_texture_low = @"Assets\Art\BasketBall\low\textures";
    //篮架纹理根目录
    static string basketballStandRootpath_texture_high = @"Assets\Art\BasketBallStand\high\textures";
    static string basketballStandRootpath_texture_low = @"Assets\Art\BasketBallStand\low\textures";

    //护臂纹理根目录
    static string armGuardRootpath_texture_high = @"Assets\Art\Player\player_armguard_r\textures\high";
    static string armGuardRootpath_texture_low = @"Assets\Art\Player\player_armguard_r\textures\low";
    //头带纹理根目录
    static string headBandRootpath_texture_high = @"Assets\Art\Player\player_headband\textures\high";
    static string headBandRootpath_texture_low = @"Assets\Art\Player\player_headband\textures\low";
    //护膝纹理根目录
    static string kneeGuardRootpath_texture_high = @"Assets\Art\Player\player_kneeguard_l\textures\high";
    static string kneeGuardRootpath_texture_low = @"Assets\Art\Player\player_kneeguard_l\textures\low";

    //待保存文件路径
    public static List<string> waitToSaveListfilePaths = new List<string>();

    [MenuItem("公共工具/资源优化/OptimizeFBX")]
    static void Init()
    {
        GetWindow(typeof(OptimizeFBX));
    }

    public OptimizeFBX()
    {
        this.titleContent = new GUIContent("模型|纹理优化工具");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        toolbarindex = GUILayout.Toolbar(toolbarindex, toolbarContent, GUILayout.Height(30));
        if (toolbarindex == 0)
        {
            GUILayout.Space(10);
            if (fbxHeadRootPaths.Count == 0) InitFBXRootPaths();
            ShowFBXRootPaths();

            GUILayout.Space(10);
            if (GUILayout.Button("开始模型优化", GUILayout.Height(30)))
            {
                OptimizeAllPartFBX();
            }
        }
        else if (toolbarindex == 1)
        {
            GUILayout.Space(10);
            if (CheckNeedInitTextureRootPaths()) InitTextureRootPaths();
            for (int i = 0; i < 9; i++)
            {
                ShowPartTexture((PlayerPartEnum)i);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("开始纹理优化", GUILayout.Height(30)))
            {
                OptimizeAllParttexture();
            }
        }
    }

    #region Texture Optimization

    static bool CheckNeedInitTextureRootPaths()
    {
        return headTextureObjs.HasEmptyObjects() || limbsTextureObjs.HasEmptyObjects() ||
            bodyTextureObjs.HasEmptyObjects() || shoesTextureObjs.HasEmptyObjects() ||
            basketballTextureObjs.HasEmptyObjects() || basketballStandTextureObjs.HasEmptyObjects() ||
            armGuardTextureObjs.HasEmptyObjects() || headBandTextureObjs.HasEmptyObjects() ||
            kneeGuardTextureObjs.HasEmptyObjects();
    }
    static int GetValidSize(int size)
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

    static void OptimizeAllParttexture()
    {
        for (int i = 0; i < 9; i++)
        {
            OptimizePartTexture((PlayerPartEnum)i);
        }
        //TODO:保存更改
        AssetDatabase.SaveAssets();
        DoAssetReimport(ref waitToSaveListfilePaths, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }

    static void ClearTextureObjs()
    {
        headTextureObjs.Clear();
        limbsTextureObjs.Clear();
        bodyTextureObjs.Clear();
        shoesTextureObjs.Clear();
        armGuardTextureObjs.Clear();
        headBandTextureObjs.Clear();
        kneeGuardTextureObjs.Clear();
        basketballTextureObjs.Clear();
        basketballStandTextureObjs.Clear();
    }

    private static void InitTextureRootPaths()
    {
        ClearTextureObjs();

        headRootPaths.Clear();
        limbsRootPaths.Clear();
        bodyRootPaths.Clear();
        shoesRootPaths.Clear();
        armGuardRootPaths.Clear();
        headBandRootPaths.Clear();
        kneeGuardRootPaths.Clear();
        basketballRootPaths.Clear();
        basketballStandRootPaths.Clear();

        headRootPaths.rootPaths[0].Add(headRootpath_texture_albedo_high);
        headRootPaths.rootPaths[0].Add(headRootpath_texture_albedo_low);
        headRootPaths.rootPaths[1].Add(headRootpath_texture_lit_high);
        headRootPaths.rootPaths[1].Add(headRootpath_texture_lit_low);
        headRootPaths.rootPaths[2].Add(headRootpath_texture_normal_high);
        headRootPaths.rootPaths[2].Add(headRootpath_texture_normal_low);

        limbsRootPaths.rootPaths[0].Add(limbsRootpath_texture_albedo_high);
        limbsRootPaths.rootPaths[0].Add(limbsRootpath_texture_albedo_low);
        limbsRootPaths.rootPaths[1].Add(limbsRootpath_texture_lit_high);
        limbsRootPaths.rootPaths[1].Add(limbsRootpath_texture_lit_low);
        limbsRootPaths.rootPaths[2].Add(limbsRootpath_texture_normal_high);
        limbsRootPaths.rootPaths[2].Add(limbsRootpath_texture_normal_low);

        bodyRootPaths.rootPaths[0].Add(bodyRootpath_texture_albedo_high);
        bodyRootPaths.rootPaths[0].Add(bodyRootpath_texture_albedo_low);
        bodyRootPaths.rootPaths[1].Add(bodyRootpath_texture_lit_high);
        bodyRootPaths.rootPaths[1].Add(bodyRootpath_texture_lit_low);
        bodyRootPaths.rootPaths[2].Add(bodyRootpath_texture_normal_high);
        bodyRootPaths.rootPaths[2].Add(bodyRootpath_texture_normal_low);

        shoesRootPaths.rootPaths[0].Add(shoesRootpath_texture_albedo_high);
        shoesRootPaths.rootPaths[0].Add(shoesRootpath_texture_albedo_low);
        shoesRootPaths.rootPaths[1].Add(shoesRootpath_texture_lit_high);
        shoesRootPaths.rootPaths[1].Add(shoesRootpath_texture_lit_low);
        shoesRootPaths.rootPaths[2].Add(shoesRootpath_texture_normal_high);
        shoesRootPaths.rootPaths[2].Add(shoesRootpath_texture_normal_low);

        armGuardRootPaths.rootPaths[0].Add(armGuardRootpath_texture_high);
        armGuardRootPaths.rootPaths[0].Add(armGuardRootpath_texture_low);
        armGuardRootPaths.rootPaths[1].Add(armGuardRootpath_texture_high);
        armGuardRootPaths.rootPaths[1].Add(armGuardRootpath_texture_low);
        armGuardRootPaths.rootPaths[2].Add(armGuardRootpath_texture_high);
        armGuardRootPaths.rootPaths[2].Add(armGuardRootpath_texture_low);

        headBandRootPaths.rootPaths[0].Add(headBandRootpath_texture_high);
        headBandRootPaths.rootPaths[0].Add(headBandRootpath_texture_low);
        headBandRootPaths.rootPaths[1].Add(headBandRootpath_texture_high);
        headBandRootPaths.rootPaths[1].Add(headBandRootpath_texture_low);
        headBandRootPaths.rootPaths[2].Add(headBandRootpath_texture_high);
        headBandRootPaths.rootPaths[2].Add(headBandRootpath_texture_low);

        kneeGuardRootPaths.rootPaths[0].Add(kneeGuardRootpath_texture_high);
        kneeGuardRootPaths.rootPaths[0].Add(kneeGuardRootpath_texture_low);
        kneeGuardRootPaths.rootPaths[1].Add(kneeGuardRootpath_texture_high);
        kneeGuardRootPaths.rootPaths[1].Add(kneeGuardRootpath_texture_low);
        kneeGuardRootPaths.rootPaths[2].Add(kneeGuardRootpath_texture_high);
        kneeGuardRootPaths.rootPaths[2].Add(kneeGuardRootpath_texture_low);

        basketballRootPaths.rootPaths[0].Add(basketballRootpath_texture_high);
        basketballRootPaths.rootPaths[0].Add(basketballRootpath_texture_low);
        basketballRootPaths.rootPaths[1].Add(basketballRootpath_texture_high);
        basketballRootPaths.rootPaths[1].Add(basketballRootpath_texture_low);
        basketballRootPaths.rootPaths[2].Add(basketballRootpath_texture_high);
        basketballRootPaths.rootPaths[2].Add(basketballRootpath_texture_low);

        basketballStandRootPaths.rootPaths[0].Add(basketballStandRootpath_texture_high);
        basketballStandRootPaths.rootPaths[0].Add(basketballStandRootpath_texture_low);
        basketballStandRootPaths.rootPaths[1].Add(basketballStandRootpath_texture_high);
        basketballStandRootPaths.rootPaths[1].Add(basketballStandRootpath_texture_low);
        basketballStandRootPaths.rootPaths[2].Add(basketballStandRootpath_texture_high);
        basketballStandRootPaths.rootPaths[2].Add(basketballStandRootpath_texture_low);

        //添加默认路径至纹理对象池
        for (int i = 0; i < 9; i++)
        {
            AddRootPathsToTextureObjs((PlayerPartEnum)i);
        }
    }

    static void ShowPartTexture(PlayerPartEnum playerPart)
    {
        var objs = GetPlayerPartTextureObjs(playerPart);
        var rootpaths = GetTextureRootPaths(playerPart);
        GUIStyle toggle1Style = new GUIStyle(EditorStyles.foldout);
        toggle1Style.fontSize = 12;
        toggle1Style.alignment = TextAnchor.UpperLeft;
        string toggle2Tips = $"{playerPart}纹理路径：";


        EditorGUILayout.BeginHorizontal();
        foldouts[(int)playerPart] = GUILayout.Toggle(foldouts[(int)playerPart], toggle2Tips, toggle1Style);
        EditorGUILayout.EndHorizontal();
        if (foldouts[(int)playerPart])
        {
            ShowCurTextureRootPaths(rootpaths, TextureType.ALBEDO);
            ShowObjField(objs.TextureFileObjs[0], objs.TextureFileObjLevels[0], "albedo");
            ShowCurTextureRootPaths(rootpaths, TextureType.LIT);
            ShowObjField(objs.TextureFileObjs[1], objs.TextureFileObjLevels[1], "lighting map");
            ShowCurTextureRootPaths(rootpaths, TextureType.NORMAL);
            ShowObjField(objs.TextureFileObjs[2], objs.TextureFileObjLevels[2], "normal");
        }
        GUILayout.Space(10);
    }

    static string[] GetfilePathsByPart(PlayerPartEnum playerPart, TextureType textureType, string assetPath)
    {
        string[] filePaths = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
        switch (playerPart)
        {
            case PlayerPartEnum.HEAD:
            case PlayerPartEnum.BODY:
            case PlayerPartEnum.LIMB:
            case PlayerPartEnum.SHOES:
                break;
            case PlayerPartEnum.BASKETBALL:
                var filename = "D.tga";
                bool isMatch = false;
                switch (textureType)
                {
                    case TextureType.ALBEDO:
                        filename = "D.tga";
                        break;
                    case TextureType.LIT:
                        filename = "M.tga";
                        break;
                    case TextureType.NORMAL:
                        filename = "N.tga";
                        break;
                    default:
                        break;
                }
                foreach (var filePath in filePaths)
                {
                    if (filePath.EndsWith(filename))
                    {
                        isMatch = true;
                        filePaths = new string[] { filePath };
                        break;
                    }
                }
                if (!isMatch) filePaths = null;
                break;
            case PlayerPartEnum.BASKETBALLSTAND:
            case PlayerPartEnum.ARMGUARD:
            case PlayerPartEnum.HEADBAND:
            case PlayerPartEnum.KNEEGUARD:
                if (textureType != TextureType.ALBEDO)
                {
                    filePaths = null;
                }
                break;
            default:
                break;
        }
        return filePaths;
    }

    static void OptimizePartTexture(PlayerPartEnum playerPart)
    {
        int optimizeCount = 0;
        var textureFileObjs = GetPlayerPartTextureObjs(playerPart);
        for (int l = 0; l < textureFileObjs.TextureFileObjs.Length; l++)
        {
            var texetureType = (TextureType)l;
            var fileObjs = textureFileObjs.TextureFileObjs[l];
            var fileObjsLevels = textureFileObjs.TextureFileObjLevels[l];

            for (int i = 0; i < fileObjs.Count; i++)
            {
                string[] filePaths;
                var compressLevel = fileObjsLevels[i];
                var asstPath = AssetDatabase.GetAssetPath(fileObjs[i]);
                try
                {
                    //获取文件夹路径
                    filePaths = GetfilePathsByPart(playerPart, texetureType, asstPath);
                }
                catch
                {
                    Debug.Log($"文件夹路径不合法 {asstPath}");
                    continue;
                }
                if (filePaths == null) continue;
                for (int j = 0; j < filePaths.Length; j++)
                {
                    var filePath = filePaths[j].Replace("\\", "/");
                    if (filePath.EndsWith(".png") || filePath.EndsWith(".PNG")
                        || filePath.EndsWith(".tga") || filePath.EndsWith(".TGA"))
                    {
                        if (!(filePath.EndsWith(".tga") || filePath.EndsWith(".TGA")))
                        {
                            //Debug.LogError($"[文件格式不合法] 不是.tga格式 {filePath}");
                        }

                        TextureImporterPlatformSettings textureSettings = null;
                        EditorUtility.DisplayProgressBar("处理中>>>", filePath, (float)j / (float)filePaths.Length);

                        TextureImporter textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
                        //分纹理类型优化
                        OptimizeTextureByPartByTextureType(ref textureImporter, playerPart, texetureType);
                        //纹理大小
                        GetTextureOriginalSize(textureImporter, out int width, out int height);
                        int textureSize = Mathf.Max(height, width);
                        var targetSize = GetRelatedMaxSize(textureSize, playerPart, compressLevel, texetureType);
                        var targetFormat = GetRelatedTextureFormat(playerPart, compressLevel, texetureType);
                        //压缩格式
                        // 安卓 or 苹果
                        for (int k = 0; k < 2; k++)
                        {
                            string curPlatformStr = (k == 0) ? "Android" : "iPhone";
                            textureSettings = textureImporter.GetPlatformTextureSettings(curPlatformStr);
                            textureSettings.overridden = true;

                            //配置最优无需压缩
                            if (targetSize == textureSettings.maxTextureSize
                            && targetFormat == textureSettings.format)
                            {
                                optimizeCount++;
                            }
                            else
                            {
                                textureSettings.format = targetFormat;
                                textureSettings.maxTextureSize = targetSize;
                                textureImporter.SetPlatformTextureSettings(textureSettings);
                            }
                        }
                        string log = optimizeCount == 2 ? "[纹理已优化 无需设置]" : "[优化纹理]";
                        optimizeCount = 0;
                        Debug.Log($"{log} 格式：{textureSettings.format} 纹理尺寸：{textureSettings.maxTextureSize} 路径: {filePath}");

                        waitToSaveListfilePaths.Add(filePath);
                    }
                }
            }
        }

    }

    static TextureImporterFormat GetRelatedTextureFormat(PlayerPartEnum playerPart, OptimizeFBXCompressLevel level, TextureType textureType)
    {
        var format = TextureImporterFormat.ASTC_6x6;
        var targetFormat = (level == OptimizeFBXCompressLevel.LOW) ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_4x4;

        switch (level)
        {
            case OptimizeFBXCompressLevel.LOW:

            case OptimizeFBXCompressLevel.MEDIUM:
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                    case PlayerPartEnum.BODY:
                    case PlayerPartEnum.LIMB:
                    case PlayerPartEnum.SHOES:
                    case PlayerPartEnum.BASKETBALL:
                    case PlayerPartEnum.ARMGUARD:
                    case PlayerPartEnum.HEADBAND:
                    case PlayerPartEnum.KNEEGUARD:
                        format = TextureImporterFormat.ASTC_8x8;
                        break;
                    case PlayerPartEnum.BASKETBALLSTAND:
                        break;
                    default:
                        break;
                }
                break;
            case OptimizeFBXCompressLevel.HIGH:
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                    case PlayerPartEnum.BODY:
                    case PlayerPartEnum.LIMB:
                    case PlayerPartEnum.SHOES:
                    case PlayerPartEnum.BASKETBALL:
                    case PlayerPartEnum.ARMGUARD:
                    case PlayerPartEnum.HEADBAND:
                    case PlayerPartEnum.KNEEGUARD:
                        format = TextureImporterFormat.ASTC_6x6;
                        break;
                    case PlayerPartEnum.BASKETBALLSTAND:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        return format;
    }

    static int GetRelatedMaxSize(int textureSize, PlayerPartEnum playerPart, OptimizeFBXCompressLevel level, TextureType textureType)
    {
        int maxSize = GetValidSize(textureSize);

        switch (level)
        {
            case OptimizeFBXCompressLevel.LOW:

            case OptimizeFBXCompressLevel.MEDIUM:
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                    case PlayerPartEnum.BODY:
                    case PlayerPartEnum.LIMB:
                        maxSize = 512;
                        break;
                    case PlayerPartEnum.SHOES:
                        maxSize = 256;
                        break;
                    case PlayerPartEnum.BASKETBALL:
                        maxSize = 512;
                        break;
                    case PlayerPartEnum.BASKETBALLSTAND:
                        break;
                    case PlayerPartEnum.ARMGUARD:
                    case PlayerPartEnum.HEADBAND:
                    case PlayerPartEnum.KNEEGUARD:
                        maxSize = 256;
                        break;
                    default:
                        break;
                }
                break;
            case OptimizeFBXCompressLevel.HIGH:
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                    case PlayerPartEnum.BODY:
                    case PlayerPartEnum.LIMB:
                        maxSize = 1024;
                        break;
                    case PlayerPartEnum.SHOES:
                        maxSize = 512;
                        break;
                    case PlayerPartEnum.BASKETBALL:
                        maxSize = 512;
                        break;
                    case PlayerPartEnum.BASKETBALLSTAND:
                        break;
                    case PlayerPartEnum.ARMGUARD:
                    case PlayerPartEnum.HEADBAND:
                    case PlayerPartEnum.KNEEGUARD:
                        maxSize = 512;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        return maxSize;
    }

    static void AddRootPathsToTextureObjs(PlayerPartEnum playerPart)
    {
        var rootPaths = GetTextureRootPaths(playerPart);
        var textureFileObjs = GetPlayerPartTextureObjs(playerPart);

        for (int i = 0; i < rootPaths.rootPaths.Length; i++)
        {
            for (int j = 0; j < rootPaths.rootPaths[i].Count; j++)
            {
                var path = rootPaths.rootPaths[i][j].Replace("\\", "/");
                var file = AssetDatabase.LoadAssetAtPath(path.Replace("\\", "/"), typeof(DefaultAsset));
                textureFileObjs.TextureFileObjs[i].Add(file);
                if (path.EndsWith("high") || path.EndsWith("high\textures"))
                {
                    textureFileObjs.TextureFileObjLevels[i].Add(OptimizeFBXCompressLevel.HIGH);
                }
                else
                {
                    textureFileObjs.TextureFileObjLevels[i].Add(OptimizeFBXCompressLevel.LOW);
                }
            }
        }
    }

    static TextureRootPaths GetTextureRootPaths(PlayerPartEnum playerPart)
    {
        switch (playerPart)
        {
            default:
            case PlayerPartEnum.HEAD:
                return headRootPaths;
            case PlayerPartEnum.BODY:
                return bodyRootPaths;
            case PlayerPartEnum.LIMB:
                return limbsRootPaths;
            case PlayerPartEnum.SHOES:
                return shoesRootPaths;
            case PlayerPartEnum.BASKETBALL:
                return basketballRootPaths;
            case PlayerPartEnum.BASKETBALLSTAND:
                return basketballStandRootPaths;
            case PlayerPartEnum.ARMGUARD:
                return armGuardRootPaths;
            case PlayerPartEnum.HEADBAND:
                return headBandRootPaths;
            case PlayerPartEnum.KNEEGUARD:
                return kneeGuardRootPaths;
        }
    }
    static PlayerPartTexturesObjs GetPlayerPartTextureObjs(PlayerPartEnum playerPart)
    {
        PlayerPartTexturesObjs texturesObjs = new PlayerPartTexturesObjs();
        switch (playerPart)
        {
            default:
            case PlayerPartEnum.HEAD:
                texturesObjs = headTextureObjs;
                break;
            case PlayerPartEnum.BODY:
                texturesObjs = bodyTextureObjs;
                break;
            case PlayerPartEnum.LIMB:
                texturesObjs = limbsTextureObjs;
                break;
            case PlayerPartEnum.SHOES:
                texturesObjs = shoesTextureObjs;
                break;
            case PlayerPartEnum.BASKETBALL:
                texturesObjs = basketballTextureObjs;
                break;
            case PlayerPartEnum.BASKETBALLSTAND:
                texturesObjs = basketballStandTextureObjs;
                break;
            case PlayerPartEnum.ARMGUARD:
                texturesObjs = armGuardTextureObjs;
                break;
            case PlayerPartEnum.HEADBAND:
                texturesObjs = headBandTextureObjs;
                break;
            case PlayerPartEnum.KNEEGUARD:
                texturesObjs = kneeGuardTextureObjs;
                break;
        }
        return texturesObjs;
    }

    static void GetTextureOriginalSize(TextureImporter ti, out int width, out int height)
    {
        if (ti == null)
        {
            width = 0;
            height = 0;
            return;
        }
        object[] args = { 0, 0 };
        MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(ti, args);
        width = (int)args[0];
        height = (int)args[1];
    }

    static void ShowCurTextureRootPaths(TextureRootPaths rootPaths, TextureType textureType)
    {
        var paths = rootPaths.rootPaths[0];
        switch (textureType)
        {
            case TextureType.ALBEDO:
                paths = rootPaths.rootPaths[0];
                break;
            case TextureType.LIT:
                paths = rootPaths.rootPaths[1];
                break;
            case TextureType.NORMAL:
                paths = rootPaths.rootPaths[2];
                break;
            default:
                break;
        }
        EditorGUILayout.LabelField($"{textureType}默认纹理路径：");
        foreach (var path in paths)
        {
            EditorGUILayout.LabelField(path);
        }
    }

    static void ShowObjField(List<Object> fileObjs, List<OptimizeFBXCompressLevel> levels, string tips)
    {
        //白名单
        UnityEngine.Object[] whiteObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        //文件夹列表
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(tips);
        EditorGUILayout.LabelField($"{fileObjs.Count}", GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < fileObjs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_toggle_mixed_bg"), GUILayout.Width(25), GUILayout.Height(25)))
            {
                fileObjs.Remove(fileObjs[i]);
                continue;
            }
            fileObjs[i] = EditorGUILayout.ObjectField(fileObjs[i], typeof(Object), true, GUILayout.Height(25));
            EditorGUILayout.LabelField("机型品质", GUILayout.Width(50));
            levels[i] = (OptimizeFBXCompressLevel)EditorGUILayout.EnumPopup(levels[i]);
            EditorGUILayout.EndHorizontal();
        }
        //增加按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_CreateAddNew"), GUILayout.Height(25)))
        {
            fileObjs.Add(new Object());
            levels.Add(OptimizeFBXCompressLevel.MEDIUM);
        }
        EditorGUILayout.EndHorizontal();
    }

    static void OptimizeTextureByPartByTextureType(ref TextureImporter textureImporter, PlayerPartEnum playerPart, TextureType textureType)
    {
        switch (textureType)
        {
            case TextureType.ALBEDO:
                DefaultOptimizeAlbedoTexture(textureImporter);

                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                        HeadOptimizeAlbedoTexture(textureImporter);
                        break;
                    case PlayerPartEnum.BODY:
                        BodyOptimizeAlbedoTexture(textureImporter);
                        break;
                    case PlayerPartEnum.LIMB:
                        LimbsOptimizeAlbedoTexture(textureImporter);
                        break;
                    case PlayerPartEnum.SHOES:
                        ShoesOptimizeAlbedoTexture(textureImporter);
                        break;
                    default:
                        break;
                }
                break;
            case TextureType.LIT:
                DefaultOptimizeLitTexture(textureImporter);
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                        HeadOptimizeLitTexture(textureImporter);
                        break;
                    case PlayerPartEnum.BODY:
                        BodyOptimizeLitTexture(textureImporter);
                        break;
                    case PlayerPartEnum.LIMB:
                        LimbsOptimizeLitTexture(textureImporter);
                        break;
                    case PlayerPartEnum.SHOES:
                        ShoesOptimizeLitTexture(textureImporter);
                        break;
                    default:
                        break;
                }
                break;
            case TextureType.NORMAL:
                DefaultOptimizeNormalTexture(textureImporter);
                switch (playerPart)
                {
                    case PlayerPartEnum.HEAD:
                        HeadOptimizeNormalTexture(textureImporter);
                        break;
                    case PlayerPartEnum.BODY:
                        BodyOptimizeNormalTexture(textureImporter);
                        break;
                    case PlayerPartEnum.LIMB:
                        LimbsOptimizeNormalTexture(textureImporter);
                        break;
                    case PlayerPartEnum.SHOES:
                        ShoesOptimizeNormalTexture(textureImporter);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    //Albedo
    static void DefaultOptimizeAlbedoTexture(TextureImporter textureImporter)
    {
        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.textureShape = TextureImporterShape.Texture2D;
        textureImporter.sRGBTexture = true;
        textureImporter.alphaSource = TextureImporterAlphaSource.None;
        textureImporter.alphaIsTransparency = true;
        textureImporter.ignorePngGamma = false;

        textureImporter.isReadable = false;
        textureImporter.streamingMipmaps = true;
        textureImporter.streamingMipmapsPriority = 127;
        textureImporter.vtOnly = false;
        textureImporter.mipmapEnabled = true;
        textureImporter.borderMipmap = true;
        textureImporter.mipmapFilter = TextureImporterMipFilter.BoxFilter;
        textureImporter.mipMapsPreserveCoverage = false;
        textureImporter.fadeout = false;
        textureImporter.wrapMode = TextureWrapMode.Repeat;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.anisoLevel = 0;
    }

    static void HeadOptimizeAlbedoTexture(TextureImporter textureImporter)
    {

    }

    static void LimbsOptimizeAlbedoTexture(TextureImporter textureImporter)
    {
        textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
        textureImporter.alphaIsTransparency = false;
        textureImporter.ignorePngGamma = false;
        textureImporter.streamingMipmapsPriority = 124;
        textureImporter.anisoLevel = 1;
    }

    static void BodyOptimizeAlbedoTexture(TextureImporter textureImporter)
    {
        textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
        textureImporter.alphaIsTransparency = false;
        textureImporter.ignorePngGamma = false;
        textureImporter.streamingMipmapsPriority = 122;
        textureImporter.anisoLevel = 1;
    }

    static void ShoesOptimizeAlbedoTexture(TextureImporter textureImporter)
    {
        textureImporter.alphaSource = TextureImporterAlphaSource.None;
        textureImporter.alphaIsTransparency = false;
        textureImporter.ignorePngGamma = false;
        textureImporter.streamingMipmapsPriority = 119;
        textureImporter.anisoLevel = 8;
    }

    //lighting map
    static void DefaultOptimizeLitTexture(TextureImporter textureImporter)
    {
        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.textureShape = TextureImporterShape.Texture2D;
        textureImporter.sRGBTexture = true;
        textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
        textureImporter.alphaIsTransparency = false;
        textureImporter.ignorePngGamma = false;

        textureImporter.isReadable = false;
        textureImporter.streamingMipmaps = true;
        textureImporter.streamingMipmapsPriority = 126;
        textureImporter.vtOnly = false;
        textureImporter.mipmapEnabled = true;
        textureImporter.borderMipmap = true;
        textureImporter.mipmapFilter = TextureImporterMipFilter.BoxFilter;
        textureImporter.mipMapsPreserveCoverage = false;
        textureImporter.fadeout = false;
        textureImporter.wrapMode = TextureWrapMode.Repeat;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.anisoLevel = 1;
    }

    static void HeadOptimizeLitTexture(TextureImporter textureImporter)
    {

    }

    static void LimbsOptimizeLitTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 123;
    }

    static void BodyOptimizeLitTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 122;
        textureImporter.anisoLevel = 4;
    }

    static void ShoesOptimizeLitTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 118;
    }

    //normal
    static void DefaultOptimizeNormalTexture(TextureImporter textureImporter)
    {
        textureImporter.textureType = TextureImporterType.NormalMap;
        textureImporter.textureShape = TextureImporterShape.Texture2D;
        textureImporter.sRGBTexture = true;
        textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
        textureImporter.alphaIsTransparency = false;
        textureImporter.ignorePngGamma = false;

        textureImporter.isReadable = false;
        textureImporter.streamingMipmaps = true;
        textureImporter.streamingMipmapsPriority = 126;
        textureImporter.vtOnly = false;
        textureImporter.mipmapEnabled = true;
        textureImporter.borderMipmap = true;
        textureImporter.mipmapFilter = TextureImporterMipFilter.BoxFilter;
        textureImporter.mipMapsPreserveCoverage = false;
        textureImporter.fadeout = false;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.anisoLevel = 1;
    }

    static void HeadOptimizeNormalTexture(TextureImporter textureImporter)
    {

    }

    static void LimbsOptimizeNormalTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 125;
        textureImporter.wrapMode = TextureWrapMode.Repeat;
        textureImporter.anisoLevel = 4;
    }

    static void BodyOptimizeNormalTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 123;
        textureImporter.wrapMode = TextureWrapMode.Repeat;
        textureImporter.anisoLevel = 4;
    }

    static void ShoesOptimizeNormalTexture(TextureImporter textureImporter)
    {
        textureImporter.streamingMipmapsPriority = 117;
        textureImporter.wrapMode = TextureWrapMode.Repeat;
        textureImporter.anisoLevel = 1;
    }
    #endregion


    #region FBX Optimization
    private static void InitFBXRootPaths()
    {
        fbxHeadRootPaths.Clear();
        fbxBodyRootPaths.Clear();
        fbxShoesRootPaths.Clear();
        fbxLimbsRootPaths.Clear();
        fbxArmGuardRootPaths.Clear();
        fbxHeadBandRootPaths.Clear();
        fbxKneeGuardRootPaths.Clear();
        fbxBasketballRootPaths.Clear();
        fbxBasketballStandRootPaths.Clear();

        fbxHeadRootPaths.Add(fbxHeadRootpath_low);
        fbxHeadRootPaths.Add(fbxHeadRootpath_high);
        fbxBodyRootPaths.Add(fbxBodyRootpath_low);
        fbxBodyRootPaths.Add(fbxBodyRootpath_high);
        fbxLimbsRootPaths.Add(fbxLimbRootpath_low);
        fbxLimbsRootPaths.Add(fbxLimbRootpath_high);
        fbxShoesRootPaths.Add(fbxShoesRootpath_low);
        fbxShoesRootPaths.Add(fbxShoesRootpath_high);
        fbxArmGuardRootPaths.Add(fbxArmGuardRootpath_low);
        fbxArmGuardRootPaths.Add(fbxArmGuardRootpath_high);
        fbxHeadBandRootPaths.Add(fbxHeadBandRootpath_low);
        fbxHeadBandRootPaths.Add(fbxHeadBandRootpath_high);
        fbxKneeGuardRootPaths.Add(fbxKneeGuardRootpath_low);
        fbxKneeGuardRootPaths.Add(fbxKneeGuardRootpath_high);
        fbxBasketballRootPaths.Add(fbxBasketballRootpath_low);
        fbxBasketballRootPaths.Add(fbxBasketballRootpath_high);
        fbxBasketballStandRootPaths.Add(fbxBasketballStandRootpath_low);
        fbxBasketballStandRootPaths.Add(fbxBasketballStandRootpath_high);

    }
    static void ShowFBXRootPaths()
    {
        for (int i = 0; i < 9; i++)
        {
            ShowPartFBXRootPaths((PlayerPartEnum)i);
        }
    }

    static string GetPartFBXRootPathsLabel(PlayerPartEnum playerPart)
    {
        string label = "";
        switch (playerPart)
        {
            case PlayerPartEnum.HEAD:
                label = "头部";
                break;
            case PlayerPartEnum.BODY:
                label = "身体";
                break;
            case PlayerPartEnum.LIMB:
                label = "四肢";
                break;
            case PlayerPartEnum.SHOES:
                label = "鞋子";
                break;
            case PlayerPartEnum.BASKETBALL:
                label = "篮球";
                break;
            case PlayerPartEnum.BASKETBALLSTAND:
                label = "篮架";
                break;
            case PlayerPartEnum.ARMGUARD:
                label = "护臂";
                break;
            case PlayerPartEnum.HEADBAND:
                label = "头带";
                break;
            case PlayerPartEnum.KNEEGUARD:
                label = "护膝";
                break;
            default:
                break;
        }
        return label + "模型路径：";
    }

    static void ShowPartFBXRootPaths(PlayerPartEnum playerPart)
    {
        var label = GetPartFBXRootPathsLabel(playerPart);
        var rootpaths = GetFBXRootPath(playerPart);
        EditorGUILayout.LabelField(label);
        foreach (var path in rootpaths)
        {
            EditorGUILayout.LabelField(path);
        }
    }

    static void OptimizeAllPartFBX()
    {
        InitFBXRootPaths();
        for (int i = 0; i < 9; i++)
        {
            OptimizeFBXSelectPart((PlayerPartEnum)i);
        }

        //TODO:保存更改
        AssetDatabase.SaveAssets();
        DoAssetReimport(ref FBXfilePaths, ImportAssetOptions.ForceUpdate);
    }

    static void OptimizeFBXSelectPart(PlayerPartEnum part)
    {
        //TODO:遍历文件
        var rootPath = GetFBXRootPath(part);
        for (int i = 0; i < rootPath.Count; i++)
        {
            var assetpath = rootPath[i].Replace("\\", "/");
            string[] filepaths;
            try
            {
                filepaths = Directory.GetFiles(assetpath, ".", SearchOption.AllDirectories);
            }
            catch (System.Exception)
            {

                Debug.Log($"{assetpath} 文件夹路径不存在！");
                continue;
            }

            for (int j = 0; j < filepaths.Length; j++)
            {
                var fbxpath = filepaths[j].Replace("\\", "/");
                if (fbxpath.EndsWith("fbx") || fbxpath.EndsWith("FBX"))
                {
                    EditorUtility.DisplayProgressBar("处理中>>>", fbxpath, (float)j / (float)filepaths.Length);
                    ModelImporter modelImporter = AssetImporter.GetAtPath(fbxpath) as ModelImporter;

                    OptimizeRelatedFBX(part, ref modelImporter);
                    Debug.Log($"[模型文件处理完成] {fbxpath}");
                    FBXfilePaths.Add(fbxpath);
                }
            }
        }
    }

    static void DefaultOptimizeFBX(ref ModelImporter modelImporter)
    {
        //TODO:优化模型
        //场景标签 Scene Tab
        modelImporter.globalScale = 1.0f;
        modelImporter.useFileUnits = true;
        modelImporter.bakeAxisConversion = false;
        modelImporter.importBlendShapes = false;
        modelImporter.importVisibility = false;
        modelImporter.importCameras = false;
        modelImporter.importLights = false;
        modelImporter.preserveHierarchy = false;
        modelImporter.sortHierarchyByName = true;
        //网格标签 Meshes Tab
        modelImporter.meshCompression = ModelImporterMeshCompression.Medium;
        modelImporter.isReadable = false;
        modelImporter.optimizeMeshPolygons = true;
        modelImporter.optimizeMeshVertices = true;
        modelImporter.addCollider = false;
        //几何标签 Geometry Tab
        modelImporter.keepQuads = false;
        modelImporter.weldVertices = true;
        modelImporter.indexFormat = ModelImporterIndexFormat.Auto;
        modelImporter.importNormals = ModelImporterNormals.Import;
        modelImporter.importBlendShapeNormals = ModelImporterNormals.Import;
        modelImporter.normalCalculationMode = ModelImporterNormalCalculationMode.AreaAndAngleWeighted;
        modelImporter.normalSmoothingSource = ModelImporterNormalSmoothingSource.PreferSmoothingGroups;
        modelImporter.normalSmoothingAngle = 60.0f;
        modelImporter.importTangents = ModelImporterTangents.None;
        modelImporter.swapUVChannels = false;
        modelImporter.generateSecondaryUV = false;

        //TODO:优化骨骼
        modelImporter.animationType = ModelImporterAnimationType.Generic;
        modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        modelImporter.skinWeights = ModelImporterSkinWeights.Custom;
        modelImporter.maxBonesPerVertex = 4;
        modelImporter.minBoneWeight = 0.001f;
        modelImporter.optimizeGameObjects = false;
        var extraExposedTransformPaths = modelImporter.extraExposedTransformPaths;
        modelImporter.extraExposedTransformPaths = extraExposedTransformPaths;

        //TODO:优化动画
        modelImporter.importConstraints = false;
        modelImporter.importAnimation = false;
        //TODO:优化材料
        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
    }

    static void OptimizeHeadFBX(ref ModelImporter modelImporter)
    {
        modelImporter.indexFormat = ModelImporterIndexFormat.UInt16;

        modelImporter.maxBonesPerVertex = 1;

    }

    static void OptimizeLimbsFBX(ref ModelImporter modelImporter)
    {
        modelImporter.indexFormat = ModelImporterIndexFormat.UInt16;

        modelImporter.maxBonesPerVertex = 4;
    }

    static void OptimizeBodyFBX(ref ModelImporter modelImporter)
    {
        modelImporter.sortHierarchyByName = false;
        modelImporter.indexFormat = ModelImporterIndexFormat.Auto;

        modelImporter.maxBonesPerVertex = 3;
    }

    static void OptimizeShoesFBX(ref ModelImporter modelImporter)
    {
        modelImporter.indexFormat = ModelImporterIndexFormat.UInt16;

        modelImporter.maxBonesPerVertex = 1;
    }
    private static void OptimizeBasketballFBX(ref ModelImporter modelImporter)
    {
    }

    private static void OptimizeBasketballStandFBX(ref ModelImporter modelImporter)
    {
        modelImporter.isReadable = true;
    }
    private static void OptimizeKneeGuardFBX(ref ModelImporter modelImporter)
    {
    }

    private static void OptimizeHeadBandFBX(ref ModelImporter modelImporter)
    {
    }

    private static void OptimizeArmGuardFBX(ref ModelImporter modelImporter)
    {
    }

    private static List<string> GetFBXRootPath(PlayerPartEnum part)
    {
        switch (part)
        {
            case PlayerPartEnum.HEAD:
                return fbxHeadRootPaths;
            case PlayerPartEnum.BODY:
                return fbxBodyRootPaths;
            case PlayerPartEnum.LIMB:
                return fbxLimbsRootPaths;
            case PlayerPartEnum.SHOES:
                return fbxShoesRootPaths;
            case PlayerPartEnum.BASKETBALL:
                return fbxBasketballRootPaths;
            case PlayerPartEnum.BASKETBALLSTAND:
                return fbxBasketballStandRootPaths;
            case PlayerPartEnum.ARMGUARD:
                return fbxArmGuardRootPaths;
            case PlayerPartEnum.HEADBAND:
                return fbxHeadBandRootPaths;
            case PlayerPartEnum.KNEEGUARD:
                return fbxKneeGuardRootPaths;
        }
        return fbxHeadRootPaths;
    }

    static ModelImporter OptimizeRelatedFBX(PlayerPartEnum playerPart, ref ModelImporter modelImporter)
    {
        DefaultOptimizeFBX(ref modelImporter);

        switch (playerPart)
        {
            case PlayerPartEnum.HEAD:
                OptimizeHeadFBX(ref modelImporter);
                break;
            case PlayerPartEnum.LIMB:
                OptimizeLimbsFBX(ref modelImporter);
                break;
            case PlayerPartEnum.BODY:
                OptimizeBodyFBX(ref modelImporter);
                break;
            case PlayerPartEnum.SHOES:
                OptimizeShoesFBX(ref modelImporter);
                break;
            case PlayerPartEnum.BASKETBALL:
                OptimizeBasketballFBX(ref modelImporter);
                break;
            case PlayerPartEnum.BASKETBALLSTAND:
                OptimizeBasketballStandFBX(ref modelImporter);
                break;
            case PlayerPartEnum.ARMGUARD:
                OptimizeArmGuardFBX(ref modelImporter);
                break;
            case PlayerPartEnum.HEADBAND:
                OptimizeHeadBandFBX(ref modelImporter);
                break;
            case PlayerPartEnum.KNEEGUARD:
                OptimizeKneeGuardFBX(ref modelImporter);
                break;
        }
        return modelImporter;
    }
    #endregion

    public static void DoAssetReimport(ref List<string> paths, ImportAssetOptions options)
    {
        foreach (var path in paths)
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
        Debug.Log($"[文件优化] 优化完成 共处理 {paths.Count} 个文件");
        paths.Clear();
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("成功", "处理完成！", "好的");
    }
}
