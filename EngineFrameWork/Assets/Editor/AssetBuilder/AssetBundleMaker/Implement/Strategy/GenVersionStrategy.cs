﻿using FrameWork;
using System.Text;
using Core.AutoUpdate;
using Core.AssetBuilder;
using Core.Interface.IO;
using Core.Interface.INI;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;
using FrameWork.Launch;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	    GenVersionStrategy.cs
Author:		    DaiLi.Ou
Descriptions:   Build PipeLine Generate Version Strategy.

*********************************************************************/
public class GenVersionStrategy : IBuildStrategy
{
    public BuildProcess Process => BuildProcess.GenVersion;

    public void Build(IBuildContext context)
    {
        if (context.StopBuild) { return; }

        IFile VersionFile = AssetBundlesMaker._platformDir.File(AssetBundlesMaker._verFileName);
        UnityEngine.Debug.Log("###VersionFile.Exists:  " + VersionFile.Exists);

        if (VersionFile.Exists)
        {
            CreateVersionFileHotPackage(VersionFile);
        }
        else
        {
            CreateVersionFileFirstPackage(VersionFile);
        }

        UnityEngine.Debug.Log("### Gen Version Strategy Successed ###");
    }

    private void CreateVersionFileHotPackage(IFile VersionFile)
    {
        string versionCode = string.Empty;
        string zFile = string.Empty;
        string uFile = string.Empty;

        IIniResult result = App.Ini.Load(VersionFile);
        Dictionary<string, string> dic = result.Get("Version");
        string pastVCode = dic["VersionCode"];
        versionCode = AddVersionCode(pastVCode);

        string zFileRelativelyPath;
        string uFileRelativelyPath;
        bool zipFileExists = GetVersionFileInfomation(out zFileRelativelyPath, out uFileRelativelyPath);

        string title = "[Version]";
        string vCode = string.Format($"VersionCode={versionCode}");
        zFile = zipFileExists ? string.Format($"ZipFile={zFileRelativelyPath}") : "ZipFile=Empty";
        uFile = string.Format($"UpdateFile={uFileRelativelyPath}");
        string content = title + "\n" + vCode + "\n" + zFile + "\n" + uFile;

        VersionFile.Delete();
        VersionFile.Create(Encoding.UTF8.GetBytes(content));
    }

    private void CreateVersionFileFirstPackage(IFile VersionFile)
    {
        string zFileRelativelyPath;
        string uFileRelativelyPath;
        bool zipFileExists = GetVersionFileInfomation(out zFileRelativelyPath, out uFileRelativelyPath);


        string title = "[Version]";
        string vCode = "VersionCode=1.0.0.0";
        string zFile = zipFileExists ? string.Format($"ZipFile={zFileRelativelyPath}") : "ZipFile=Empty";
        string uFile = string.Format($"UpdateFile={uFileRelativelyPath}");
        string content = title + "\n" + vCode + "\n" + zFile + "\n" + uFile;

        VersionFile.Create(Encoding.UTF8.GetBytes(content));


    }

    private bool GetVersionFileInfomation(out string zFileRelativelyPath, out string uFileRelativelyPath)
    {
        IFile zipFile = AssetBundlesMaker._curBuildDir.File(AssetBundlesMaker._zipName);
        IFile updateFile = AssetBundlesMaker._curBuildDir.File(UpdateFileStore.FILE_NAME);
        string rootPath = AssetBundlesMaker._curBuildDir.Path.Replace("/", @"\");
        rootPath = rootPath.Remove(rootPath.LastIndexOf(@"\") + 1);
        zFileRelativelyPath = zipFile.FullName.Replace(rootPath, string.Empty);
        uFileRelativelyPath = updateFile.FullName.Replace(rootPath, string.Empty);

        //UnityEngine.Debug.Log("###rootPath: " + rootPath);
        //UnityEngine.Debug.Log("###ZipPath: " + zipFile.FullName);
        //UnityEngine.Debug.Log("###UpdatePath: " + updateFile.FullName);
        //UnityEngine.Debug.Log("###replaceZipPath: " + zFilePath);
        //UnityEngine.Debug.Log("###replaceUpdatePath: " + uFilePath);
        return zipFile.Exists;
    }

    private string AddVersionCode(string versionCode)
    {
        string[] splitChar = versionCode.Split('.');
        string qianStr = splitChar[0];
        string baiStr = splitChar[1];
        string shiStr = splitChar[2];
        string geStr = splitChar[3];

        int qian = 0, bai = 0, shi = 0, ge = 0;
        int.TryParse(geStr, out ge);
        int.TryParse(shiStr, out shi);
        int.TryParse(baiStr, out bai);
        int.TryParse(qianStr, out qian);
        int total = qian * 1000 + bai * 100 + shi * 10 + ge;
        total++;

        qian = total / 1000;
        bai = (total % 1000) / 100;
        shi = (total % 100) / 10;
        ge = total % 10;
        if (qian > 9)
        {
            UnityEngine.Debug.LogError("版本号 9.9.9.9 => 打了8999个版本, 九九归一, 重新来吧.");
            qian = 1;
            bai = 0;
            shi = 0;
            ge = 0;
        }

        return string.Format($"{qian}.{bai}.{shi}.{ge}");
    }
}
