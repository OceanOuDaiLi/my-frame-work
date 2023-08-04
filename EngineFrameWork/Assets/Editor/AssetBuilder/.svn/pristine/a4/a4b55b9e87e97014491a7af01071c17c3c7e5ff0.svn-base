using System;
using FrameWork;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SplitBundleInfo
{
    public string BuildTarget = App.Env.PlatformToName(App.Env.SwitchPlatform);
    public string[] AccompanyFiles = new string[1];
    public string[] AccompanyDirectorys = new string[1];

    public bool IsFloadout { get; set; }            //Used by Editor Inspactor
    public bool IsFloadoutDir { get; set; }         //Used by Editor Inspactor
    public bool IsFloadoutFiles { get; set; }       //Used by Editor Inspactor
}

[Serializable]
public class UnZipInfo
{
    public enum EquipmentLevel
    {
        LOW = 0,
        MEDIUM = 1,
        HIGH = 2,
    }

    public int UnZipSpace = 1024;   // 每次解压的空间/KB
    public int UnZipThreadNum = 1;  // 使用解压的线程数
    public int CleanThreadNum = 1;  // 清理旧资源的线程数

    public EquipmentLevel Level = EquipmentLevel.LOW;
}

[CreateAssetMenuAttribute(fileName = "Split Bundle Settings - please renamed it", menuName = "程序工具/AssetBundle Maker/Split AssetBundle Settings", order = 2)]
public class AssetBundleSplitSetting : ScriptableObject
{
    public SplitBundleInfo[] SplitInfos = null;

    public SplitBundleInfo GetSplitInfoByBuildTarget(string buildTarget)
    {
        for (int i = 0; i < SplitInfos.Length; i++)
        {
            if (SplitInfos[i].BuildTarget.Equals(buildTarget))
            {
                return SplitInfos[i];
            }
        }

        Debug.LogErrorFormat($"There are no {buildTarget} config setting.");
        return null;
    }

    public bool JudgeAccompanyFileContainsToDirectory(int splitIdx, int tarIdx, out string containedDir)
    {
        containedDir = string.Empty;
        string filePath = SplitInfos[splitIdx].AccompanyFiles[tarIdx];
        for (int i = 0; i < SplitInfos[splitIdx].AccompanyDirectorys.Length; i++)
        {
            var dirPath = SplitInfos[splitIdx].AccompanyDirectorys[i];
            if (filePath.Contains(dirPath) && !string.IsNullOrEmpty(dirPath))
            {
                containedDir = filePath + "\n" + "属于分包文件夹" + "\n" + dirPath + "\n";
                return true;
            }
        }

        return false;
    }

    public void AddFileInfo(int splitIdx)
    {
        Add(ref SplitInfos[splitIdx].AccompanyFiles, "");
    }

    public void RemoveFileInfo(int splitIdx, int tarIdx)
    {
        RemoveAt(ref SplitInfos[splitIdx].AccompanyFiles, tarIdx);
    }

    public void RemoveFilesGroupInfo(Dictionary<int, List<int>> warningDic)
    {
        // warningDic  Key: splitIndex  Value: List<tarIndex>
        foreach (var item in warningDic)
        {
            List<string> newAccompanyFiles = new List<string>();
            for (int i = 0; i < SplitInfos[item.Key].AccompanyFiles.Length; i++)
            {
                if (!item.Value.Contains(i))
                {
                    newAccompanyFiles.Add(SplitInfos[item.Key].AccompanyFiles[i]);
                }
            }
            SplitInfos[item.Key].AccompanyFiles = newAccompanyFiles.ToArray();
        }
    }

    public void AddDirectoryInfo(int splitIdx)
    {
        Add(ref SplitInfos[splitIdx].AccompanyDirectorys, "");
    }

    public void RemoveDirectoryInfo(int splitIdx, int tarIdx)
    {
        RemoveAt(ref SplitInfos[splitIdx].AccompanyDirectorys, tarIdx);
    }

    private void Add<T>(ref T[] aSource, T aValue)
    {
        var newArray = new T[aSource.Length + 1];
        for (int i = 0, n = aSource.Length; i < n; i++)
        {
            newArray[i] = aSource[i];
        }

        newArray[newArray.Length - 1] = aValue;
        aSource = newArray;
    }

    private void RemoveAt<T>(ref T[] aSource, int aDelIndex)
    {
        int curIndex = 0;
        var newArray = new T[aSource.Length - 1];
        for (int i = 0, n = aSource.Length; i < n; i++)
        {
            if (i != aDelIndex)
            {
                newArray[curIndex] = aSource[i];
                curIndex++;
            }
        }

        aSource = newArray;
    }

    public bool Contains(string[] aSource, string aValue)
    {
        // true  : chould find.
        // false : chould not find.
        for (int i = 0; i < aSource.Length; i++)
        {
            if (aSource[i].Equals(aValue))
            {
                return true;
            }
        }

        return false;
    }
}
