
using System;
using FrameWork;
using UnityEngine;

[Serializable]
public enum BuildLanguage
{
    CN = 0,
    ENGLISH = 1,
}

[Serializable]
public enum AssetZipTool
{
    SEVEN_ZIP = 0,
    //SHARP_ZIP_LIB = 1,
}

[Serializable]
public enum EnCryptAlgorithm
{
    XXTEA = 0,
    AES256 = 1,
}

[Serializable]
public enum DeviceLevel
{
    LOW = 0,
    MEDIUM = 1,
    HIGH = 2,
}

[Serializable]
public class AssetBundleBuildInfo
{
    public int JenkinsBuildId = 0;
    public string TipsName = "Please Input Build Details";
    public string CDN = "It will auto input for OutNet Build";
    public string ChanelName = "Please Input Build Chanel Name";
    public BuildLanguage buildLanguage = BuildLanguage.CN;

    public AssetZipTool assetZipTool = AssetZipTool.SEVEN_ZIP;
    public string BuildTarget = App.Env.PlatformToName(App.Env.SwitchPlatform);

    public string[] ChanelScriptingDefineSymbols = null;

    public bool IsFloadout { get; set; }        //Used by Editor Inspactor
}

[Serializable]
public class ApplicationBuildInfo
{
    [Header("编译APP公司名称")]
    public string CompanyName = "深圳市自娱科技有限责任公司";
    [Space(2)]
    [Header("编译APP展示名称")]
    public string ProductName = "Engine-Frame-Work";
    [Space(2)]
    [Header("编译APP版本号")]
    public string Version = "1.0.0";

#if UNITY_ANDROID
    [Space(2)]
    [Header("编译Apk包名")]
    public string PackageName = "com.ftx.zeus";
    [Space(2)]
    [Header("编译ApkBundle版本号")]
    public string BundleVersionCode = "88";
#endif

#if UNITY_IOS
    [Space(2)]
    [Header("编译Ipa包名")]
    public string BundleIdentifier = "com.unity.x5";
    [Space(2)]
    [Header("编译Ipa短版本号")]
    public string Build = "1.0";
#endif

    [Space(2)]
    [Header("是否进行代码资源加密")]
    public bool EncryptCode = false;
    [HideInInspector]
    public EnCryptAlgorithm EncryptCodeAlg = EnCryptAlgorithm.XXTEA;

    [Space(2)]
    [Header("是否进行Asset资源加密")]
    public bool EncryptAsset = false;
    [HideInInspector]
    public EnCryptAlgorithm EncryptAssetAlg = EnCryptAlgorithm.AES256;

    [Space(2)]
    [Header("是否进行配置资源加密")]
    public bool EncryptCfgAsset = false;
    [HideInInspector]
    public EnCryptAlgorithm EncryptCfgAlg = EnCryptAlgorithm.AES256;

    [Space(2)]
    [Header("构建应用品质")]
    public DeviceLevel DeviceLv = DeviceLevel.HIGH;

    [Space(2)]
    [Header("单个Zip大小.unit:Mb")]
    public int subZipSize = 100;    //Mb
}

[CreateAssetMenuAttribute(fileName = "Build Settings - please renamed it", menuName = "程序工具/AssetBundle Maker/Build Settings", order = 1)]
public class AssetBundleBuildSetting : ScriptableObject
{
    public ApplicationBuildInfo applicationBuildInfo = new ApplicationBuildInfo();

    [Space(10)]
    public AssetBundleBuildInfo[] AssetBundleBuildInfos = new AssetBundleBuildInfo[] { };

    public void RefreshCfg(AssetBundleBuildInfo[] infos)
    {
        AssetBundleBuildInfos = infos;
    }
}
