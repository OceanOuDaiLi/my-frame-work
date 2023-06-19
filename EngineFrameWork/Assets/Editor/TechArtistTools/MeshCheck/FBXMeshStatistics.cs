#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public static class FBXMeshStatistics
{
    enum StatType
    {
        Triangle,
        Vertex,
    };

    [MenuItem("公共工具/资源检测/统计Mesh面数与顶点数/面数Top20", false, 800)]
    public static void CheckTriangles()
    {
        _Counting(StatType.Triangle, 20);
    }

    [MenuItem("公共工具/资源检测/统计Mesh面数与顶点数/顶点数Top20", false, 801)]
    public static void CheckVertex()
    {
        _Counting(StatType.Vertex, 20);
    }

    static Dictionary<int, List<string>> s_meshList = null;
    private static void _Counting(StatType cType, int count)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        EditorUtility.DisplayProgressBar("开始统计", "正在获取文件列表...", 0.0f);

        string temFileName = Path.Combine(Path.GetTempPath(), string.Format("FBXMeshStatistics_Result_{0}.txt", time));
        StreamWriter streamWriter = new StreamWriter(temFileName);

        var searchPath = Application.dataPath + "/";
        var dirInfo = new DirectoryInfo(searchPath);
        var allFBXFiles = GetAllFiles(dirInfo, "*.FBX");
        //#if !UNITY_EDITOR_WIN && !UNITY_STANDALONE_WIN
        //        allFBXFiles.AddRange(AutoExportProduct.GetAllFiles(dirInfo, "*.fbx"));
        //#endif
        var allCount = allFBXFiles.Count;

        s_meshList = new Dictionary<int, List<string>>(1024);

        //// 处理fbx文件
        var title = string.Format("共{0}个fbx", allCount);
        int index = 0;
        foreach (string filePath in allFBXFiles)
        {
            var fPrecent = 1f * (++index) / allCount;
            var nPrecent = (int)(fPrecent * 100);
            var progressText = string.Format("{0}% 正在检查 第 {1} 个文件", nPrecent, index);
            EditorUtility.DisplayProgressBar(title, progressText, fPrecent);

            var fullPath = filePath.Replace('\\', '/');
            if (fullPath == null || fullPath.Length < 1)
                continue;

            var assetPath = "Assets/" + GetRelativePath(Application.dataPath, fullPath);
            int stat = _GetFbxData(assetPath, cType);
            if (stat <= 0)
                continue;

            List<string> fileList = null;
            if (!s_meshList.TryGetValue(stat, out fileList))
            {
                fileList = new List<string>();
                fileList.Add(stat.ToString());
                s_meshList.Add(stat, fileList);
            }

            fileList.Add(assetPath);
        }

        _OutputMsg(streamWriter, "-----目录 \"{0}\" 下统计Mesh {1} -------- 需要搜索{2}个FBX文件", searchPath, cType, allCount);
        _OutputResult(cType, streamWriter, count);
        _OutputMsg(streamWriter, "-----目录 \"{0}\" 下统计Mesh {1} -------- cost time={2}", searchPath, cType, stopwatch.Elapsed.ToString());

        streamWriter.Flush();
        streamWriter.Close();
        Process.Start(temFileName);

        EditorUtility.ClearProgressBar();
    }

    static int _GetFbxData(string assetPath, StatType cType)
    {
        var fbx = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
        if (fbx == null)
        {
            UnityEngine.Debug.LogErrorFormat("AssetDatabase.LoadAssetAtPath({0}) faield.", assetPath);
            return 0;
        }

        int stat = 0;

        var meshFilter = fbx.GetComponentsInChildren<MeshFilter>();
        if (meshFilter != null && meshFilter.Length > 0)
        {
            foreach (var render in meshFilter)
                stat += _GetData(render.sharedMesh, cType);
        }

        var meshRenderSkinned = fbx.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (meshRenderSkinned != null && meshRenderSkinned.Length > 0)
        {
            foreach (var render in meshRenderSkinned)
                stat += _GetData(render.sharedMesh, cType);
        }

        return stat;
    }

    static int _GetData(Mesh mesh, StatType cType)
    {
        switch (cType)
        {
            case StatType.Triangle: return mesh.triangles.Length;
            case StatType.Vertex: return mesh.vertices.Length;
            default: return 0;
        }
    }

    static void _OutputResult(StatType cType, StreamWriter writer, int count)
    {
        int index = 0;
        List<List<string>> resultList = s_meshList.OrderByDescending(kp => kp.Key).Select(kp => kp.Value).ToList();
        foreach (var data in resultList)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                if (i == 0)
                    _OutputMsg(writer, "{0} = {1}", cType.ToString(), data[i]);
                else
                    _OutputMsg(writer, "\t{0}", data[i]);
            }

            if (++index > count) break;
        }
    }

    static void _OutputMsg(StreamWriter streamWriter, string format, params object[] args)
    {
        if (streamWriter != null)
            streamWriter.WriteLine(format, args);
        else
            UnityEngine.Debug.LogFormat(format, args);
    }

    public static ArrayList GetAllFiles(DirectoryInfo dir, string searchPattern)
    {
        var result = new ArrayList();

        var allFile = dir.GetFiles(searchPattern);
        foreach (var fi in allFile)
        {
            result.Add(Path.Combine(fi.DirectoryName, fi.Name));
        }

        var allDir = dir.GetDirectories();
        foreach (var d in allDir)
        {
            result.AddRange(GetAllFiles(d, searchPattern));
        }

        return result;
    }

    public static string GetRelativePath(string customDir, string fullFilePath)
    {
        var strResult = fullFilePath;
        var strTempMainDir = customDir;

        if (!strTempMainDir.EndsWith("/"))
            strTempMainDir += "/";

        int intIndex = -1, intpos = strTempMainDir.IndexOf("/");
        while (intpos >= 0)
        {
            intpos++;
            if (string.Compare(strTempMainDir, 0, fullFilePath, 0, intpos, true) != 0)
                break;

            intIndex = intpos;
            intpos = strTempMainDir.IndexOf("/", intpos);
        }

        if (intIndex >= 0)
        {
            strResult = fullFilePath.Substring(intIndex);
            intpos = strTempMainDir.IndexOf("/", intIndex);

            while (intpos >= 0)
            {
                strResult = "../" + strResult;
                intpos = strTempMainDir.IndexOf("/", intpos + 1);
            }
        }

        return strResult;
    }

}

#endif