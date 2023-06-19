#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

public static class FBXRepeatMechChecker
{
    static Dictionary<string, MeshDataList> s_fbx2MeshDataList = null;              // key:fbx path
    static Dictionary<string, List<Mesh2FBXData>> s_mesh2FBXList = null;            // key:mesh.name
    static Dictionary<MeshDataList, List<string>> s_mutlMesh2FBXList = null;        // value:fbx path

    /// <summary>
    /// 目前Mesh是否相同，只比较顶点和三角形
    /// </summary>
    class MeshCompareData
    {
        public string nameMesh;
        public Vector3[] vertices;
        public int[] triangles;
        public long memSize;
    }

    class Mesh2FBXData
    {
        public List<string> listFBXPath;
        public MeshCompareData meshData;
    }

    class MeshDataList
    {
        public List<MeshCompareData> listMeshData = new List<MeshCompareData>();
        public override int GetHashCode()
        {
            int code = listMeshData[0].GetHashCode();
            for (int i = 1; i < listMeshData.Count; ++i)
                code ^= listMeshData[i].GetHashCode();
            return code;
        }
        public override bool Equals(object obj)
        {
            MeshDataList keyToCmp = obj as MeshDataList;
            var listMeshDataToCmp = keyToCmp.listMeshData;
            if (listMeshData.Count != listMeshDataToCmp.Count)
                return false;

            for (int i = 0; i < listMeshData.Count; ++i)
                if (!listMeshDataToCmp.Contains(listMeshData[i]))
                    return false;
            return true;
        }
    }

    [MenuItem("公共工具/资源检测/统计重复Mesh/统计Asset所有FBX")]
    public static void CheckAll()
    {
        Check();
    }

    [MenuItem("公共工具/资源检测/统计重复Mesh/统计打包路径下所有FBX")]
    public static void CheckAllTResources()
    {
        Check(true);
    }

    [MenuItem("公共工具/资源检测/统计重复Mesh/统计Asset所有FBX-前100项（测试用）")]
    public static void Check1()
    {
        Check(false, 100);
    }

    public static void Check(bool isTres = false, int checkCount = 0)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        EditorUtility.DisplayProgressBar("开始查找相同Mesh", "正在获取文件列表...", 0.0f);

        string temFileName = Path.Combine(Path.GetTempPath(), string.Format("FBXRepeatMechChecker_Result_{0}.txt", time));
        StreamWriter streamWriter = new StreamWriter(temFileName);

        //var searchPath = Application.dataPath + "/zzzOptimizeTest/FBXInportStandardShaderOptimize/";
        var searchPath = Application.dataPath + "/";
        if (isTres)
        {
            searchPath += "/ABAssets/AssetBundle";
        }

        var dirInfo = new DirectoryInfo(searchPath);
        var allFBXFiles = FBXMeshStatistics.GetAllFiles(dirInfo, "*.FBX");
        //#if !UNITY_EDITOR_WIN && !UNITY_STANDALONE_WIN
        //        allFBXFiles.AddRange(AutoExportProduct.GetAllFiles(dirInfo, "*.fbx"));
        //#endif
        var allCount = allFBXFiles.Count;

        s_mesh2FBXList = new Dictionary<string, List<Mesh2FBXData>>(allCount);
        s_fbx2MeshDataList = new Dictionary<string, MeshDataList>(allCount);
        s_mutlMesh2FBXList = new Dictionary<MeshDataList, List<string>>(1024);

        //// 处理fbx文件
        var title = string.Format("共{0}个fbx", allCount);
        var index = 0;
        foreach (string filePath in allFBXFiles)
        {
            if (checkCount > 0 && index > checkCount) break; // 调试代码，只处理一部分
            index++;
            var fPrecent = 1f * index / allCount;
            var nPrecent = (int)(fPrecent * 100);
            var progressText = string.Format("{0}% 正在检查 第 {1} 个文件", nPrecent, index);
            EditorUtility.DisplayProgressBar(title, progressText, fPrecent);

            var fullPath = filePath.Replace('\\', '/');
            if (fullPath == null || fullPath.Length < 1)
                continue;
            var assetPath = "Assets/" + FBXMeshStatistics.GetRelativePath(Application.dataPath, fullPath);

            var fbx = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            if (fbx != null)
            {
                var fbx2MeshData = new MeshDataList();
                s_fbx2MeshDataList.Add(assetPath, fbx2MeshData);
                ClassifyFBXByOneMesh(assetPath, fbx, fbx2MeshData);
            }
            else
                UnityEngine.Debug.LogErrorFormat("AssetDatabase.LoadAssetAtPath({0}) faield.", assetPath);
        }

        ClassifyFBXByMutilMesh();

        OutputMsg(streamWriter, "-----目录 \"{0}\" 下相同mesh查找开始 -------- 需要搜索{1}个FBX文件", searchPath, allCount);

        var count = OutputHasExactlySameMeshFBX(streamWriter);
        OutputMsg(streamWriter, "-----目录 \"{0}\" 下相同mesh查找结果 -------- 共{1}个完全相同的mesh.", searchPath, count);

        OutputLine(streamWriter);

        count = OutputHasOneSameMeshFBX(streamWriter);
        OutputMsg(streamWriter, "-----目录 \"{0}\" 下相同mesh查找结果 -------- 共{1}个部分或全部相同的mesh. cost time={2}", searchPath, count, stopwatch.Elapsed.ToString());

        streamWriter.Flush();
        streamWriter.Close();
        Process.Start(temFileName);

        EditorUtility.ClearProgressBar();
    }

    static void ClassifyFBXByOneMesh(string fbxPath, GameObject asset, MeshDataList fbx2MeshData)
    {
        var meshFilter = asset.GetComponentsInChildren<MeshFilter>();
        var meshRenderSkinned = asset.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (meshFilter != null && meshFilter.Length > 0)
        {
            foreach (var render in meshFilter)
            {
                CheckMesh(fbxPath, render.sharedMesh, fbx2MeshData);
            }
        }
        if (meshRenderSkinned != null && meshRenderSkinned.Length > 0)
        {
            foreach (var render in meshRenderSkinned)
            {
                CheckMesh(fbxPath, render.sharedMesh, fbx2MeshData);
            }
        }
    }

    static void CheckMesh(string fbxPath, Mesh mesh, MeshDataList fbx2MeshData)
    {
        var nameMesh = mesh.name;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var memSize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(mesh);
        List<Mesh2FBXData> mesh2FBXDataList = null;
        if (s_mesh2FBXList.TryGetValue(nameMesh, out mesh2FBXDataList))
        {
            foreach (var mesh2FBXData in mesh2FBXDataList)
            {
                var verticesMeshData = mesh2FBXData.meshData.vertices;
                var trianglesMeshData = mesh2FBXData.meshData.triangles;
                if (verticesMeshData.Length != vertices.Length || trianglesMeshData.Length != triangles.Length)
                    continue;

                bool bContinue = false;
                for (int i = 0; i < verticesMeshData.Length; ++i)
                {
                    if (verticesMeshData[i] != vertices[i])
                    {
                        bContinue = true;
                        break;
                    }
                }
                if (bContinue)
                    continue;

                for (int i = 0; i < trianglesMeshData.Length; ++i)
                {
                    if (trianglesMeshData[i] != triangles[i])
                    {
                        bContinue = true;
                        break;
                    }
                }
                if (bContinue)
                    continue;

                mesh2FBXData.listFBXPath.Add(fbxPath);
                fbx2MeshData.listMeshData.Add(mesh2FBXData.meshData);
                return;
            }
        }

        if (mesh2FBXDataList == null)
        {
            mesh2FBXDataList = new List<Mesh2FBXData>();
            s_mesh2FBXList.Add(nameMesh, mesh2FBXDataList);
        }

        var meshDataNew = new Mesh2FBXData
        {
            listFBXPath = new List<string>(),
            meshData = new MeshCompareData
            {
                nameMesh = nameMesh,
                vertices = vertices,
                triangles = triangles,
                memSize = memSize,
            },
        };
        meshDataNew.listFBXPath.Add(fbxPath);
        mesh2FBXDataList.Add(meshDataNew);
        fbx2MeshData.listMeshData.Add(meshDataNew.meshData);
    }

    static void ClassifyFBXByMutilMesh()
    {
        foreach (var pair in s_mesh2FBXList)
        {
            var meshDataList = pair.Value;
            foreach (var meshData in meshDataList)
            {
                var listFBXPath = meshData.listFBXPath;
                if (listFBXPath.Count > 1)
                {
                    foreach (var fbxPath in listFBXPath)
                    {
                        MeshDataList fbx2MeshData;
                        if (s_fbx2MeshDataList.TryGetValue(fbxPath, out fbx2MeshData))
                        {
                            List<string> listFBXPathMutil;
                            if (s_mutlMesh2FBXList.TryGetValue(fbx2MeshData, out listFBXPathMutil))
                            {
                                if (!listFBXPathMutil.Contains(fbxPath))
                                    listFBXPathMutil.Add(fbxPath);
                            }
                            else
                            {
                                listFBXPathMutil = new List<string>();
                                listFBXPathMutil.Add(fbxPath);
                                s_mutlMesh2FBXList.Add(fbx2MeshData, listFBXPathMutil);
                            }
                        }
                        else
                            UnityEngine.Debug.LogErrorFormat("_ClassifyFBXByMutilMesh get fbx meshdata failed. {0}", fbxPath);
                    }
                }
            }
        }
    }

    static void OutputLine(StreamWriter streamWriter)
    {
        if (streamWriter != null)
            streamWriter.WriteLine();
    }

    static void OutputMsg(StreamWriter streamWriter, string format, params object[] args)
    {
        if (streamWriter != null)
            streamWriter.WriteLine(format, args);
        else
            UnityEngine.Debug.LogFormat(format, args);
    }

    static int OutputHasOneSameMeshFBX(StreamWriter streamWriter)
    {
        OutputMsg(streamWriter, "index,meshName,tvertexCount,triangleCount,memSize,count,fbxpath");
        int count = 0;
        foreach (var pair in s_mesh2FBXList)
        {
            var meshDataList = pair.Value;
            foreach (var meshData in meshDataList)
            {
                var listFBXPath = meshData.listFBXPath;
                if (listFBXPath.Count > 1)
                {
                    count += 1;

                    StringBuilder sbData = new StringBuilder();
                    sbData.AppendFormat("{0},{1},{2},{3},{4},{5},", count, pair.Key, meshData.meshData.vertices.Length, meshData.meshData.triangles.Length / 3, meshData.meshData.memSize, listFBXPath.Count);

                    StringBuilder sbPath = new StringBuilder();
                    foreach (var fbxPath in listFBXPath)
                        sbPath.AppendFormat("{0};  ", fbxPath);

                    sbData.Append(sbPath);
                    OutputMsg(streamWriter, "{0}", sbData.ToString());
                }
            }
        }
        return count;
    }

    static int OutputHasExactlySameMeshFBX(StreamWriter streamWriter)
    {
        OutputMsg(streamWriter, "index,meshName,tvertexCount,triangleCount,memSize,count,fbxpath");
        int count = 0;
        foreach (var pair in s_mutlMesh2FBXList)
        {
            var listFBXPath = pair.Value;
            if (listFBXPath.Count > 1)
            {
                count += 1;
                var listMeshData = pair.Key.listMeshData;

                for (int i = 0; i < listMeshData.Count; ++i)
                {
                    StringBuilder sbData = new StringBuilder();
                    var meshData = listMeshData[i];
                    sbData.AppendFormat("{0},{1},{2},{3},{4},{5},", count, meshData.nameMesh, meshData.vertices.Length, meshData.triangles.Length / 3, meshData.memSize, listFBXPath.Count);

                    StringBuilder sbPath = new StringBuilder();
                    foreach (var fbxPath in listFBXPath)
                        sbPath.AppendFormat("{0};  ", fbxPath);

                    sbData.Append(sbPath);

                    OutputMsg(streamWriter, "{0}", sbData.ToString());
                }
            }
        }
        return count;
    }

}
#endif