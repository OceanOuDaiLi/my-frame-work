using FrameWork;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEditor.U2D;
using UnityEngine.U2D;
using FrameWork.Launch;
using Core.Interface.IO;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	BuildAltasCommand.cs
	Author:		DaiLi.Ou

	Descriptions: According to the specification directory, pack UI Sprites into the atlas directory under AssetBundle
*********************************************************************/
public class BuildAltasCommand
{
    private static IDisk sourceScriptsDisk
    {
        get => IOHelper.IO.Disk(Application.dataPath + "/Scripts/Code/Logic/UI/Core");
    }
    private static IDirectory sourceScriptsDir
    {
        get => sourceScriptsDisk.Directory("Utils");
    }

    private static IDisk sourceSpriteDisk
    {
        get => IOHelper.IO.Disk(Application.dataPath + "/ArtAssets/ui");
    }
    private static IDirectory sourceSpriteDir
    {
        get => sourceSpriteDisk.Directory("sprites");
    }

    private static IDisk sourcePlayerSpriteDisk
    {
        get => IOHelper.IO.Disk(Application.dataPath + "/ArtAssets/characters");
    }

    private static IDisk sourceMapMaskSpriteDisk
    {
        get => IOHelper.IO.Disk(Application.dataPath + "/ArtAssets/mapMask");
    }

    #region Atlas Default Settings
    static SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
    {
        blockOffset = 1,
        padding = 2,
        enableRotation = false,
        enableTightPacking = false
    };

    private static SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
    {
        sRGB = false,
        filterMode = FilterMode.Bilinear,
        generateMipMaps = true,
        anisoLevel = 1,
    };

    private static TextureImporterPlatformSettings importerSetting = new TextureImporterPlatformSettings()
    {
        maxTextureSize = 2048,
        compressionQuality = 50,
        format = TextureImporterFormat.ASTC_4x4,
    };
    #endregion

    #region UI Atlas

    public static void GenUIAtlasConfig()
    {
        string sourceUIPrefabDir = "Assets/ABAssets/AssetBundle/ui/prefabs";

        var allPrefabLst = AssetDatabase.FindAssets("t:Prefab", new[] { sourceUIPrefabDir });

        List<GameObject> objs = new List<GameObject>();

        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        foreach (var guid in allPrefabLst)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (!go) continue;

            var eView = go.GetComponent<EventView>();
            if (eView == null)
            {
                Debug.LogWarning($"提示：{go.name} 未挂载View脚本。将不进行自动化构建图集。");
                GameObject.DestroyImmediate(go);
                continue;
            }
            eView.RefreshForGenAtlasConfig();
            dic.Add(go.name, eView._atlasDependence);

            go.transform.SetParent(null, false);

            objs.Add(go);
        }

        foreach (var item in objs)
        {
            GameObject.DestroyImmediate(item.gameObject);
        }

        IFile csFile = sourceScriptsDir.File("AtlasConfig.cs");
        var byteData = csFile.Read();
        string sourContent = Encoding.UTF8.GetString(byteData);

        string template =
            "using System.Collections.Generic;\r\n\r\nnamespace UI\r\n{\r\n    public class AtlasConfig\r\n    {\r\n        public static Dictionary<string, List<string>> PrefabAtlasDependenceDic = new Dictionary<string, List<string>>()\r\n        {\r\n//_replace_\r\n        };\r\n\r\n\r\n    }\r\n}";

        string replaceContent = string.Empty;

        string valueTempLate = "{\"aa\" , new List<string>(){bb} },";

        foreach (var item in dic)
        {
            string tips = string.Empty;

            for (int i = 0; i < item.Value.Count; i++)
            {
                string dot = i == item.Value.Count - 1 ? "" : ", ";
                tips += '"' + item.Value[i].ToString() + '"' + "  " + dot;
            }

            var tmp = valueTempLate.Replace("aa", item.Key);
            tmp = tmp.Replace("bb", tips);

            replaceContent += "            " + tmp + "\n";
        }


        template = template.Replace("//_replace_", replaceContent);

        if (!sourContent.Equals(template))
        {
            csFile.Delete();
            csFile.Create(Encoding.UTF8.GetBytes(template));
        }
        AssetDatabase.Refresh();
    }

    public static void BuildUIAtlas()
    {
        GenUIAtlasConfig();

        IDirectory tmpSourceDir;
        string[] directories = System.IO.Directory.GetDirectories(sourceSpriteDir.Path);
        for (int i = 0; i < directories.Length; i++)
        {
            string dirName = directories[i].Substring(directories[i].LastIndexOf(@"\") + 1);
            tmpSourceDir = sourceSpriteDisk.Directory("sprites/" + dirName + "/");

            BuildUIAtlasFromDirectory(tmpSourceDir, dirName);
        }

        AssetDatabase.Refresh();
    }

    private static void BuildUIAtlasFromDirectory(IDirectory directory, string dirName)
    {
        Debug.Log(string.Format($" Building <color=#7BE578> {dirName} </color> atlas .."));

        string atlasName = dirName + "_atlas" + ".spriteatlas";

        SpriteAtlas atlas = new SpriteAtlas();

        TextureImporterPlatformSettings imp;
        switch (App.Env.SwitchPlatform)
        {
            case RuntimePlatform.WindowsEditor:
                break;
            case RuntimePlatform.IPhonePlayer:
                imp = atlas.GetPlatformSettings("IPhone");
                break;
            case RuntimePlatform.Android:
                imp = atlas.GetPlatformSettings("Android");
                break;
            default:
                break;
        }
        imp = atlas.GetPlatformSettings(App.Env.PlatformToName(App.Env.SwitchPlatform));
        imp.maxTextureSize = 2048;
        imp.compressionQuality = 50;
        imp.format = TextureImporterFormat.ASTC_4x4;
        //imp.overridden = true;

        atlas.SetPackingSettings(packSetting);
        atlas.SetTextureSettings(textureSetting);
        atlas.SetPlatformSettings(imp);
        atlas.SetIncludeInBuild(false);


        IFile[] tmpFile = directory.GetFiles();
        Sprite sprite;
        List<Sprite> spriteList = new List<Sprite>();
        string assetPath = string.Empty;
        for (int i = 0; i < tmpFile.Length; i++)
        {
            if (tmpFile[i].Extension.Contains(".meta"))
            {
                continue;
            }
            EditorUtility.DisplayProgressBar(string.Format($"Start Build {dirName} atlas .."), tmpFile[i].Name, i / tmpFile.Length);

            assetPath = "Assets/ArtAssets/ui/sprites/" + dirName + "/" + tmpFile[i].Name;
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                spriteList.Add(sprite);
            }
        }
        if (spriteList.Count > 0) atlas.Add(spriteList.ToArray());

        string dir = "Assets/ABAssets/AssetBundle/ui/prefabs/" + dirName;
        string filePath = dir + "/" + atlasName;
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        AssetDatabase.CreateAsset(atlas, filePath);
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    #endregion

    #region Character Atlas 

    public static void BuildPlayerAnimaAtlas()
    {
        IDirectory rootDir = sourcePlayerSpriteDisk.Root;
        IDirectory[] allDir = rootDir.ChildrenDirectorys;

        string rootPath = rootDir.Path.Replace("/", @"\") + @"\";
        foreach (IDirectory dir in allDir)
        {
            if (dir.ChildrenDirectorys.Length > 0)
            {
                continue;
            }

            var rePath = dir.Path.Replace(rootPath, "");
            var dirName = rePath.Substring(rePath.LastIndexOf(@"\") + 1);
            var characterName = rePath.Split('\\')[0];
            var animName = rePath.Split('\\')[1];
            Debug.Log(string.Format($" Building {characterName} <color=#7BE578> {rePath} </color>  <color=#DEDD23> {dirName} </color>  atlas .."));
            BuildPlayerAtlasFromDirectory(dir, dirName, characterName, animName);
        }

        AssetDatabase.Refresh();
    }

    private static void BuildPlayerAtlasFromDirectory(IDirectory directory, string dirName, string characterName, string animName)
    {

        string atlasName = animName + "_" + dirName + "_atlas" + ".spriteatlas";

        SpriteAtlas atlas = new SpriteAtlas();


        TextureImporterPlatformSettings imp;
        switch (App.Env.SwitchPlatform)
        {
            case RuntimePlatform.WindowsEditor:
                break;
            case RuntimePlatform.IPhonePlayer:
                imp = atlas.GetPlatformSettings("IPhone");
                break;
            case RuntimePlatform.Android:
                imp = atlas.GetPlatformSettings("Android");
                break;
            default:
                break;
        }

        imp = atlas.GetPlatformSettings(App.Env.PlatformToName(App.Env.SwitchPlatform));
        imp.maxTextureSize = 1024;
        imp.compressionQuality = 50;
        imp.format = TextureImporterFormat.ASTC_4x4;
        imp.overridden = true;

        atlas.SetPlatformSettings(imp);
        atlas.SetPackingSettings(packSetting);
        atlas.SetTextureSettings(textureSetting);
        atlas.SetIncludeInBuild(true);


        IFile[] tmpFile = directory.GetFiles();
        if (tmpFile.Length > 0)
        {
            Sprite sprite;
            List<Sprite> spriteList = new List<Sprite>();
            string assetPath = string.Empty;
            string dataPath = Application.dataPath.Replace("/", @"\");
            string rootPath = "Assets" + directory.Path.Replace(dataPath, "");
            for (int i = 0; i < tmpFile.Length; i++)
            {
                if (tmpFile[i].Extension.Contains(".meta"))
                {
                    continue;
                }
                EditorUtility.DisplayProgressBar(string.Format($"Start Build {dirName} atlas .."), tmpFile[i].Name, i / tmpFile.Length);

                assetPath = rootPath + "/" + tmpFile[i].Name;
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite != null)
                {
                    spriteList.Add(sprite);
                }
            }
            if (spriteList.Count > 0) atlas.Add(spriteList.ToArray());

            string dir = $"Assets/ABAssets/AssetBundle/characters/{characterName}/animations/";
            string filePath = dir + atlasName;
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            AssetDatabase.CreateAsset(atlas, filePath);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
    }

    #endregion

    #region Map Mask Atlas

    public static void BuildMapMaskAtlas()
    {
        IDirectory rootDir = sourceMapMaskSpriteDisk.Root;
        IDirectory[] allDir = rootDir.ChildrenDirectorys;
        string rootPath = rootDir.Path.Replace("/", @"\") + @"\";
        foreach (IDirectory dir in allDir)
        {
            if (dir.ChildrenDirectorys.Length > 0)
            {
                continue;
            }

            var rePath = dir.Path.Replace(rootPath, "");
            var dirName = rePath.Substring(rePath.LastIndexOf(@"\") + 1);
            var characterName = rePath.Split('\\')[0];
            Debug.Log(string.Format($" Building <color=#7BE578> {rePath} </color>  <color=#DEDD23> {dirName} </color>  atlas .."));
            BuildMapMaskAtlasFromDirectory(dir, dirName);
        }

        AssetDatabase.Refresh();
    }

    public static void BuildMapMaskAtlasFromDirectory(IDirectory directory, string dirName)
    {
        string atlasName = dirName + "_mask_atlas" + ".spriteatlas";

        SpriteAtlas atlas = new SpriteAtlas();

        TextureImporterPlatformSettings imp;
        switch (App.Env.SwitchPlatform)
        {
            case RuntimePlatform.WindowsEditor:
                break;
            case RuntimePlatform.IPhonePlayer:
                imp = atlas.GetPlatformSettings("IPhone");
                break;
            case RuntimePlatform.Android:
                imp = atlas.GetPlatformSettings("Android");
                break;
            default:
                break;
        }
        imp = atlas.GetPlatformSettings(App.Env.PlatformToName(App.Env.SwitchPlatform));
        imp.maxTextureSize = 2048;
        imp.compressionQuality = 50;
        imp.format = TextureImporterFormat.ASTC_4x4;
        imp.overridden = true;

        atlas.SetPlatformSettings(imp);
        atlas.SetPackingSettings(packSetting);
        atlas.SetTextureSettings(textureSetting);
        atlas.SetIncludeInBuild(true);


        IFile[] tmpFile = directory.GetFiles();
        Sprite sprite;
        List<Sprite> spriteList = new List<Sprite>();
        string assetPath = string.Empty;
        string dataPath = Application.dataPath.Replace("/", @"\");
        string rootPath = "Assets" + directory.Path.Replace(dataPath, "");
        for (int i = 0; i < tmpFile.Length; i++)
        {
            if (tmpFile[i].Extension.Contains(".meta"))
            {
                continue;
            }
            EditorUtility.DisplayProgressBar(string.Format($"Start Build {dirName} atlas .."), tmpFile[i].Name, i / tmpFile.Length);

            assetPath = rootPath + "/" + tmpFile[i].Name;
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);


            if (sprite != null)
            {
                spriteList.Add(sprite);
            }
        }
        if (spriteList.Count > 0) atlas.Add(spriteList.ToArray());

        string dir = $"Assets/ABAssets/AssetBundle/map/{dirName}/";
        string filePath = dir + atlasName;
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        AssetDatabase.CreateAsset(atlas, filePath);
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    #endregion
}