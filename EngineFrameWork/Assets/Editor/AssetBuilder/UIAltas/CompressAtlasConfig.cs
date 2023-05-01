using UnityEngine;
using System.Collections.Generic;
using static CompressUIAtlasCommand;

[CreateAssetMenu(fileName = "CompressAtlasSettings", menuName = "程序工具/CompressAtlasConfig")]
public class CompressAtlasConfig : ScriptableObject
{
    [Header("图片检查")]
    public bool isCheckAlpha;
    public bool isCheckWidthHeight_IOS = false;
    public bool isCheckWidthHeight_android = false;
    [Header("图片压缩")]
    public CompressLevel level = CompressLevel.LOW;
    public List<Object> fileObjs = new List<Object>();
    public List<Object> whiteListFileObjs = new List<Object>();
    public List<CompressLevel> whiteListFileObjsLevels = new List<CompressLevel>();
}
