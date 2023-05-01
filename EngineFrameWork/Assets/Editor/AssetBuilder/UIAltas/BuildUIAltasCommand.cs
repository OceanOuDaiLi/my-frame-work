using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using FrameWork.Launch;
using Core.Interface.IO;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	BuildUIAltasCommand.cs
	Author:		DaiLi.Ou

	Descriptions: According to the specification directory, pack UI Sprites into the atlas directory under AssetBundle
*********************************************************************/

public class BuildUIAltasCommand
{

    private static IDisk sourceSpriteDisk
    {
        get => IOHelper.IO.Disk(Application.dataPath + "/ArtAssets/ui");
    }
    private static IDirectory sourceSpriteDir
    {
        get => sourceSpriteDisk.Directory("sprites");
    }

    public static void StartBuildAtlas()
    {
        string[] directories = System.IO.Directory.GetDirectories(sourceSpriteDir.Path);

        IDirectory tmpSourceDir;
        for (int i = 0; i < directories.Length; i++)
        {
            string dirName = directories[i].Substring(directories[i].LastIndexOf(@"\") + 1);
            tmpSourceDir = sourceSpriteDisk.Directory("sprites/" + dirName + "/");

            BuildAtlasFromDirectory(tmpSourceDir, dirName);
        }

        AssetDatabase.Refresh();
    }

    private static void BuildAtlasFromDirectory(IDirectory directory, string dirName)
    {
        Debug.Log(string.Format($" Building <color=#7BE578> {dirName} </color> atlas .."));

        string atlasName = dirName + "_atlas" + ".spriteatlas";

        SpriteAtlas atlas = new SpriteAtlas();
        atlas.SetPackingSettings(packSetting);
        atlas.SetTextureSettings(textureSetting);
        atlas.SetPlatformSettings(importerSetting);


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
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        AssetDatabase.CreateAsset(atlas, filePath);
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
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
        sRGB = true,
        filterMode = FilterMode.Bilinear,
    };

    private static TextureImporterPlatformSettings importerSetting = new TextureImporterPlatformSettings()
    {
        maxTextureSize = 4096,
        compressionQuality = 50,
        format = TextureImporterFormat.ASTC_6x6,
    };
    #endregion
}