#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CanEditMultipleObjects]
[CustomEditor(typeof(NPCScan), true)]
public class NPCScanEditor : Editor
{
    #region Private Editor Variables
    static int tab = 0;
    static bool showDefault = false;

    NPCScan npcScaning;
    SerializedObject serializedNpc;
    #endregion

    #region Unity Calls
    private void Awake()
    {
        npcScaning = target as NPCScan;
        serializedNpc = new SerializedObject(npcScaning);
    }

    private static readonly string[] tabNames = { "扫描npc摆放数据" };

    List<LidneNodeInfo> allNodesInfo = new List<LidneNodeInfo>();

    public override void OnInspectorGUI()
    {

        tab = GUILayout.Toolbar(tab, tabNames);
        Undo.RecordObject(npcScaning, "Changed NpcMgr");
        switch (tab)
        {
            case 0:
                if (GUILayout.Button("重置"))
                {
                    ClearCache();
                }

                if (GUILayout.Button("点击扫描"))
                {
                    //DoClear all Cache & delete Objes.
                    ClearCache();

                    for (int i = 0; i < npcScaning.LineNoeds.Count; i++)
                    {
                        LidneNodeInfo tmp = npcScaning.LineNoeds[i].GetLinewNodeInfomation();
                        allNodesInfo.Add(tmp);
                    }

                    SetNewCache();

                    //foreach (var item in npcMgr.NpcNodes)
                    //{
                    //    item.OnRefresh();
                    //    //item.runtimeMat = npcMgr.RuntimeMat;
                    //}
                }

                if (GUILayout.Button("生成预制体"))
                {
                    CreatePrefab();
                }
                break;
            default: break;
        }

        showDefault = EditorGUILayout.Foldout(showDefault, "默认编辑器");
        if (showDefault) DrawDefaultInspector();

        if (GUI.changed)
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCompleteObjectUndo(npcScaning, "Changed NpcMgr");
            EditorUtility.SetDirty(npcScaning);
        }
    }

    void CreatePrefab()
    {
        ArrayByInstancing[] instanings = npcScaning.NpcRenderNode.GetComponentsInChildren<ArrayByInstancing>();
        NpcBatchingInfo[] npcBatchingInfos = new NpcBatchingInfo[instanings.Length];

        for (int i = 0; i < instanings.Length; i++)
        {
            npcBatchingInfos[i] = new NpcBatchingInfo();
            npcBatchingInfos[i].OriMesh = instanings[i].OriMesh;
            npcBatchingInfos[i].OriMeshRenderer = instanings[i].OriMeshRenderer;
            npcBatchingInfos[i].TarPos = new Vector3[instanings[i].TarTrans.Length];
            npcBatchingInfos[i].TarRot = new Quaternion[instanings[i].TarTrans.Length];
            for (int j = 0; j < instanings[i].TarTrans.Length; j++)
            {
                npcBatchingInfos[i].TarPos[j] = instanings[i].TarTrans[j].position;
                npcBatchingInfos[i].TarRot[j] = instanings[i].TarTrans[j].rotation;
            }
        }

        npcScaning.npcBatchingConfig.RefreshCfg(npcBatchingInfos);

        GameObject prefab = new GameObject("NpcBatching");
        prefab.transform.SetParent(null);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.rotation = new Quaternion();

        GameObject sourceNode = new GameObject("Source Node");
        sourceNode.transform.SetParent(prefab.transform);
        sourceNode.transform.position = npcScaning.transform.position;
        sourceNode.transform.rotation = new Quaternion();

        for (int i = 0; i < npcBatchingInfos.Length; i++)
        {
            //create source node instance
            GameObject sourceOriMeshRenderer = GameObject.Instantiate(npcBatchingInfos[i].OriMeshRenderer.gameObject);
            sourceOriMeshRenderer.name = npcBatchingInfos[i].OriMeshRenderer.name;
            sourceOriMeshRenderer.transform.SetParent(sourceNode.transform);
            sourceOriMeshRenderer.transform.SetPositionAndRotation
                (
                    npcBatchingInfos[i].OriMeshRenderer.transform.position,
                    npcBatchingInfos[i].OriMeshRenderer.transform.rotation
                );
            npcBatchingInfos[i].OriMeshRenderer = sourceOriMeshRenderer.GetComponent<MeshRenderer>();


            GameObject tmp = new GameObject(npcBatchingInfos[i].OriMeshRenderer.name);
            ArrayByInstancing arrayByInstancing = tmp.AddComponent<ArrayByInstancing>();
            arrayByInstancing.BatchingInfo = npcBatchingInfos[i];
            arrayByInstancing.OriMesh = npcBatchingInfos[i].OriMesh;
            arrayByInstancing.OriMeshRenderer = npcBatchingInfos[i].OriMeshRenderer;

            tmp.transform.SetParent(prefab.transform);
            tmp.transform.localPosition = Vector3.zero;
            tmp.transform.rotation = new Quaternion();
        }

        string path = string.Format("{0}/TResources/NPC/NpcBatching.prefab", Application.dataPath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, path, InteractionMode.AutomatedAction);

    }

    void ClearCache()
    {
        #region 重置女npc Cache数据
        //foreach (var item in npcMgr.GirlType_1_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_1_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_1_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_1_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_1_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_1_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_1_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_1_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_2_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_2_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_2_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_2_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_2_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_2_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_2_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_2_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_3_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_3_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_3_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_3_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_3_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_3_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_3_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_3_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_4_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_4_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_4_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_4_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_4_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_4_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.GirlType_4_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.GirlType_4_Sit2_Trans = new List<Transform>();
        #endregion

        #region 重置男npc Cache数据
        //foreach (var item in npcMgr.BoyType_1_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_1_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_1_Idle_Cheer2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Idle_Cheer2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_1_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_1_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_1_Sit3_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_1_Sit3_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Idle_Cheer2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Idle_Cheer2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_2_Sit3_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_2_Sit3_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Idle_Cheer2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Idle_Cheer2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_3_Sit3_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_3_Sit3_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Idle_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Idle_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Idle_Cheer_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Idle_Cheer_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Idle_Cheer2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Idle_Cheer2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Sit_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Sit_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Sit2_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Sit2_Trans = new List<Transform>();

        //foreach (var item in npcMgr.BoyType_4_Sit3_Trans) { GameObject.DestroyImmediate(item.gameObject); }
        npcScaning.BoyType_4_Sit3_Trans = new List<Transform>();
        #endregion


        while (npcScaning.NpcRenderNode.transform.childCount > 0)
        {
            GameObject.DestroyImmediate(npcScaning.NpcRenderNode.transform.GetChild(0).gameObject);
        }
    }

    void SetNewCache()
    {
        List<Transform> boysNodesTrans = new List<Transform>();
        List<Transform> girlsNodesTrans = new List<Transform>();

        //Get aLl boy & girl Npc.
        for (int i = 0; i < allNodesInfo.Count; i++)
        {
            for (int j = 0; j < allNodesInfo[i].mesh.Length; j++)
            {
                if (allNodesInfo[i].mesh[j].sharedMesh.name.Equals(npcScaning.BoyOriMesh.name))
                {
                    boysNodesTrans.Add(allNodesInfo[i].mesh[j].transform);
                }
                else if (allNodesInfo[i].mesh[j].sharedMesh.name.Equals(npcScaning.GrilOriMesh.name))
                {
                    girlsNodesTrans.Add(allNodesInfo[i].mesh[j].transform);
                }
            }
        }

        Debug.Log("boysNodesTrans.count: " + boysNodesTrans.Count);
        Debug.Log("girlsNodesTrans.count: " + girlsNodesTrans.Count);

        CreateHightestNpcs(boysNodesTrans, girlsNodesTrans);
        //CreateMidHighNpcs(boysNodesTrans, girlsNodesTrans);
        //CreateLowNpcs(boysNodesTrans, girlsNodesTrans);
        //CreateLowestNpcs(boysNodesTrans, girlsNodesTrans);
    }

    /// <summary>
    /// 男性角色： 4种皮肤 *  6种动画 = 24种材质
    /// 女性角色： 4种皮肤 *  4种动画 = 16种材质
    /// DrawaCall: 
    /// 24 * 2 + 16 * 2 = 80 (默认2个，82个dc)
    /// </summary>
    /// <param name="boysNodesTrans"></param>
    /// <param name="girlsNodesTrans"></param>
    void CreateHightestNpcs(List<Transform> boysNodesTrans, List<Transform> girlsNodesTrans)
    {
        MeshRenderer tmpMR = null;
        for (int i = 0; i < girlsNodesTrans.Count; i++)
        {
            tmpMR = girlsNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Girl_Audience_GPUAnimat_01_Idle":
                    npcScaning.GirlType_1_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Idle_Cheer":
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit":
                    npcScaning.GirlType_1_Sit_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit_2":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_02_Idle":
                    npcScaning.GirlType_2_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Idle_Cheer":
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit":
                    npcScaning.GirlType_2_Sit_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit_2":
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_03_Idle":
                    npcScaning.GirlType_3_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Idle_Cheer":
                    npcScaning.GirlType_3_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit":
                    npcScaning.GirlType_3_Sit_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit_2":
                    npcScaning.GirlType_3_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_04_Idle":
                    npcScaning.GirlType_4_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Idle_Cheer":
                    npcScaning.GirlType_4_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit":
                    npcScaning.GirlType_4_Sit_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit_2":
                    npcScaning.GirlType_4_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }


        #region Create Girl Npc Node
        CreateRenderNode("GirlType_1_Idle_Trans", npcScaning.GirlType_1_Idle_Trans, "GirlType_1_Idle_", npcScaning.GirlType_1_Idle);
        CreateRenderNode("GirlType_1_Idle_Cheer_Trans", npcScaning.GirlType_1_Idle_Cheer_Trans, "GirlType_1_Idle_Cheer", npcScaning.GirlType_1_Idle_Cheer);
        CreateRenderNode("GirlType_1_Sit_Trans", npcScaning.GirlType_1_Sit_Trans, "GirlType_1_Sit", npcScaning.GirlType_1_Sit);
        CreateRenderNode("GirlType_1_Sit2_Trans", npcScaning.GirlType_1_Sit2_Trans, "GirlType_1_Sit2", npcScaning.GirlType_1_Sit2);

        CreateRenderNode("GirlType_2_Idle_Trans", npcScaning.GirlType_2_Idle_Trans, "GirlType_2_Idle_", npcScaning.GirlType_2_Idle);
        CreateRenderNode("GirlType_2_Idle_Cheer_Trans", npcScaning.GirlType_2_Idle_Cheer_Trans, "GirlType_2_Idle_Cheer", npcScaning.GirlType_2_Idle_Cheer);
        CreateRenderNode("GirlType_2_Sit_Trans", npcScaning.GirlType_2_Sit_Trans, "GirlType_2_Sit", npcScaning.GirlType_2_Sit);
        CreateRenderNode("GirlType_2_Sit2_Trans", npcScaning.GirlType_2_Sit2_Trans, "GirlType_2_Sit2", npcScaning.GirlType_2_Sit2);

        CreateRenderNode("GirlType_3_Idle_Trans", npcScaning.GirlType_3_Idle_Trans, "GirlType_3_Idle_", npcScaning.GirlType_3_Idle);
        CreateRenderNode("GirlType_3_Idle_Cheer_Trans", npcScaning.GirlType_3_Idle_Cheer_Trans, "GirlType_3_Idle_Cheer", npcScaning.GirlType_3_Idle_Cheer);
        CreateRenderNode("GirlType_3_Sit_Trans", npcScaning.GirlType_3_Sit_Trans, "GirlType_3_Idle_Sit", npcScaning.GirlType_3_Sit);
        CreateRenderNode("GirlType_3_Sit2_Trans", npcScaning.GirlType_3_Sit2_Trans, "GirlType_3_Idle_Sit2", npcScaning.GirlType_3_Sit2);

        CreateRenderNode("GirlType_4_Idle_Trans", npcScaning.GirlType_4_Idle_Trans, "GirlType_4_Idle_", npcScaning.GirlType_4_Idle);
        CreateRenderNode("GirlType_4_Idle_Cheer_Trans", npcScaning.GirlType_4_Idle_Cheer_Trans, "GirlType_4_Idle_Cheer_", npcScaning.GirlType_4_Idle_Cheer);
        CreateRenderNode("GirlType_4_Sit_Trans", npcScaning.GirlType_4_Sit_Trans, "GirlType_4_Sit_", npcScaning.GirlType_4_Sit);
        CreateRenderNode("GirlType_4_Sit2_Trans", npcScaning.GirlType_4_Sit2_Trans, "GirlType_4_Sit2_", npcScaning.GirlType_4_Sit2);

        #endregion

        for (int i = 0; i < boysNodesTrans.Count; i++)
        {
            tmpMR = boysNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Boy_Audience_GPUAnima_01_Idle":
                    npcScaning.BoyType_1_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer":
                    npcScaning.BoyType_1_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer_2":
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit":
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_2":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_3":
                    npcScaning.BoyType_1_Sit3_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_02_Idle":
                    npcScaning.BoyType_2_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer":
                    npcScaning.BoyType_2_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer_2":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit":
                    npcScaning.BoyType_2_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_3":
                    npcScaning.BoyType_2_Sit3_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_03_Idle":
                    npcScaning.BoyType_3_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer":
                    npcScaning.BoyType_3_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer_2":
                    npcScaning.BoyType_3_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit":
                    npcScaning.BoyType_3_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_2":
                    npcScaning.BoyType_3_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_3":
                    npcScaning.BoyType_3_Sit3_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_04_Idle":
                    npcScaning.BoyType_4_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer":
                    npcScaning.BoyType_4_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer_2":
                    npcScaning.BoyType_4_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit":
                    npcScaning.BoyType_4_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_2":
                    npcScaning.BoyType_4_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_3":
                    npcScaning.BoyType_4_Sit3_Trans.Add(boysNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }


        #region Create Boy NPC Info
        CreateRenderNode("BoyType_1_Idle_Trans", npcScaning.BoyType_1_Idle_Trans, "BoyType_1_Idle_", npcScaning.BoyType_1_Idle);
        CreateRenderNode("BoyType_1_Idle_Cheer_Trans", npcScaning.BoyType_1_Idle_Cheer_Trans, "BoyType_1_Idle_Cheer_", npcScaning.BoyType_1_Idle_Cheer);
        CreateRenderNode("BoyType_1_Idle_Cheer2_Trans", npcScaning.BoyType_1_Idle_Cheer2_Trans, "BoyType_1_Idle_Cheer2_", npcScaning.BoyType_1_Idle_Cheer2);
        CreateRenderNode("BoyType_1_Sit_Trans", npcScaning.BoyType_1_Sit_Trans, "BoyType_1_Sit_", npcScaning.BoyType_1_Sit);
        CreateRenderNode("BoyType_1_Sit2_Trans", npcScaning.BoyType_1_Sit2_Trans, "BoyType_1_Sit2_", npcScaning.BoyType_1_Sit2);
        CreateRenderNode("BoyType_1_Sit3_Trans", npcScaning.BoyType_1_Sit3_Trans, "BoyType_1_Sit3_", npcScaning.BoyType_1_Sit3);


        CreateRenderNode("BoyType_2_Idle_Trans", npcScaning.BoyType_2_Idle_Trans, "BoyType_2_Idle_", npcScaning.BoyType_2_Idle);
        CreateRenderNode("BoyType_2_Idle_Cheer_Trans", npcScaning.BoyType_2_Idle_Cheer_Trans, "BoyType_2_Idle_Cheer_", npcScaning.BoyType_2_Idle_Cheer);
        CreateRenderNode("BoyType_2_Idle_Cheer2_Trans", npcScaning.BoyType_2_Idle_Cheer2_Trans, "BoyType_2_Idle_Cheer2_", npcScaning.BoyType_2_Idle_Cheer2);
        CreateRenderNode("BoyType_2_Sit_Trans", npcScaning.BoyType_2_Sit_Trans, "BoyType_2_Sit_", npcScaning.BoyType_2_Sit);
        CreateRenderNode("BoyType_2_Sit2_Trans", npcScaning.BoyType_2_Sit2_Trans, "BoyType_2_Sit2_", npcScaning.BoyType_2_Sit2);
        CreateRenderNode("BoyType_2_Sit3_Trans", npcScaning.BoyType_2_Sit3_Trans, "BoyType_2_Sit3_", npcScaning.BoyType_2_Sit3);


        CreateRenderNode("BoyType_3_Idle_Trans", npcScaning.BoyType_3_Idle_Trans, "BoyType_3_Idle_", npcScaning.BoyType_3_Idle);
        CreateRenderNode("BoyType_3_Idle_Cheer_Trans", npcScaning.BoyType_3_Idle_Cheer_Trans, "BoyType_3_Idle_Cheer_", npcScaning.BoyType_3_Idle_Cheer);
        CreateRenderNode("BoyType_3_Idle_Cheer2_Trans", npcScaning.BoyType_3_Idle_Cheer2_Trans, "BoyType_3_Idle_Cheer2_", npcScaning.BoyType_3_Idle_Cheer2);
        CreateRenderNode("BoyType_3_Sit_Trans", npcScaning.BoyType_3_Sit_Trans, "BoyType_3_Sit_", npcScaning.BoyType_3_Sit);
        CreateRenderNode("BoyType_3_Sit2_Trans", npcScaning.BoyType_3_Sit2_Trans, "BoyType_3_Sit2_", npcScaning.BoyType_3_Sit2);
        CreateRenderNode("BoyType_3_Sit3_Trans", npcScaning.BoyType_3_Sit3_Trans, "BoyType_3_Sit3_", npcScaning.BoyType_3_Sit3);


        CreateRenderNode("BoyType_4_Idle_Trans", npcScaning.BoyType_4_Idle_Trans, "BoyType_4_Idle_", npcScaning.BoyType_4_Idle);
        CreateRenderNode("BoyType_4_Idle_Cheer_Trans", npcScaning.BoyType_4_Idle_Cheer_Trans, "BoyType_4_Idle_Cheer_", npcScaning.BoyType_4_Idle_Cheer);
        CreateRenderNode("BoyType_4_Idle_Cheer2_Trans", npcScaning.BoyType_4_Idle_Cheer2_Trans, "BoyType_4_Idle_Cheer2_", npcScaning.BoyType_4_Idle_Cheer2);
        CreateRenderNode("BoyType_4_Sit_Trans", npcScaning.BoyType_4_Sit_Trans, "BoyType_4_Sit_", npcScaning.BoyType_4_Sit);
        CreateRenderNode("BoyType_4_Sit2_Trans", npcScaning.BoyType_4_Sit2_Trans, "BoyType_4_Sit2_", npcScaning.BoyType_4_Sit2);
        CreateRenderNode("BoyType_4_Sit3_Trans", npcScaning.BoyType_4_Sit3_Trans, "BoyType_4_Sit3_", npcScaning.BoyType_4_Sit3);
        #endregion
    }

    /// <summary>
    /// 男性角色： 2种皮肤 *  3种动画 = 6种材质
    /// 女性角色： 2种皮肤 *  3种动画 = 6种材质
    /// DrawaCall: 
    /// 12 * 2 = 24(默认2个，26个dc)
    /// </summary>
    /// <param name="boysNodesTrans"></param>
    /// <param name="girlsNodesTrans"></param>
    void CreateMidHighNpcs(List<Transform> boysNodesTrans, List<Transform> girlsNodesTrans)
    {
        MeshRenderer tmpMR = null;
        for (int i = 0; i < girlsNodesTrans.Count; i++)
        {
            tmpMR = girlsNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Girl_Audience_GPUAnimat_01_Idle":
                    npcScaning.GirlType_1_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Idle_Cheer":
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit":
                    //npcMgr.GirlType_1_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit_2":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_02_Idle":
                    npcScaning.GirlType_2_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Idle_Cheer":
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit":
                    //npcMgr.GirlType_2_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit_2":
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_03_Idle":
                    //npcMgr.GirlType_3_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Idle_Cheer":
                    //npcMgr.GirlType_3_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit":
                    //npcMgr.GirlType_3_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit_2":
                    //npcMgr.GirlType_3_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_04_Idle":
                    //npcMgr.GirlType_4_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Idle_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Idle_Cheer":
                    //npcMgr.GirlType_4_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit":
                    //npcMgr.GirlType_4_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit_2":
                    //npcMgr.GirlType_4_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }


        #region Create Girl Npc Node
        CreateRenderNode("GirlType_1_Idle_Trans", npcScaning.GirlType_1_Idle_Trans, "GirlType_1_Idle_", npcScaning.GirlType_1_Idle);
        CreateRenderNode("GirlType_1_Idle_Cheer_Trans", npcScaning.GirlType_1_Idle_Cheer_Trans, "GirlType_1_Idle_Cheer", npcScaning.GirlType_1_Idle_Cheer);
        CreateRenderNode("GirlType_1_Sit_Trans", npcScaning.GirlType_1_Sit_Trans, "GirlType_1_Sit", npcScaning.GirlType_1_Sit);
        CreateRenderNode("GirlType_1_Sit2_Trans", npcScaning.GirlType_1_Sit2_Trans, "GirlType_1_Sit2", npcScaning.GirlType_1_Sit2);

        CreateRenderNode("GirlType_2_Idle_Trans", npcScaning.GirlType_2_Idle_Trans, "GirlType_2_Idle_", npcScaning.GirlType_2_Idle);
        CreateRenderNode("GirlType_2_Idle_Cheer_Trans", npcScaning.GirlType_2_Idle_Cheer_Trans, "GirlType_2_Idle_Cheer", npcScaning.GirlType_2_Idle_Cheer);
        CreateRenderNode("GirlType_2_Sit_Trans", npcScaning.GirlType_2_Sit_Trans, "GirlType_2_Sit", npcScaning.GirlType_2_Sit);
        CreateRenderNode("GirlType_2_Sit2_Trans", npcScaning.GirlType_2_Sit2_Trans, "GirlType_2_Sit2", npcScaning.GirlType_2_Sit2);

        CreateRenderNode("GirlType_3_Idle_Trans", npcScaning.GirlType_3_Idle_Trans, "GirlType_3_Idle_", npcScaning.GirlType_3_Idle);
        CreateRenderNode("GirlType_3_Idle_Cheer_Trans", npcScaning.GirlType_3_Idle_Cheer_Trans, "GirlType_3_Idle_Cheer", npcScaning.GirlType_3_Idle_Cheer);
        CreateRenderNode("GirlType_3_Sit_Trans", npcScaning.GirlType_3_Sit_Trans, "GirlType_3_Idle_Sit", npcScaning.GirlType_3_Sit);
        CreateRenderNode("GirlType_3_Sit2_Trans", npcScaning.GirlType_3_Sit2_Trans, "GirlType_3_Idle_Sit2", npcScaning.GirlType_3_Sit2);

        CreateRenderNode("GirlType_4_Idle_Trans", npcScaning.GirlType_4_Idle_Trans, "GirlType_4_Idle_", npcScaning.GirlType_4_Idle);
        CreateRenderNode("GirlType_4_Idle_Cheer_Trans", npcScaning.GirlType_4_Idle_Cheer_Trans, "GirlType_4_Idle_Cheer_", npcScaning.GirlType_4_Idle_Cheer);
        CreateRenderNode("GirlType_4_Sit_Trans", npcScaning.GirlType_4_Sit_Trans, "GirlType_4_Sit_", npcScaning.GirlType_4_Sit);
        CreateRenderNode("GirlType_4_Sit2_Trans", npcScaning.GirlType_4_Sit2_Trans, "GirlType_4_Sit2_", npcScaning.GirlType_4_Sit2);

        #endregion

        for (int i = 0; i < boysNodesTrans.Count; i++)
        {
            tmpMR = boysNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Boy_Audience_GPUAnima_01_Idle":
                    npcScaning.BoyType_1_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer":
                    npcScaning.BoyType_1_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer_2":
                    //npcMgr.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit":
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_2":
                    //npcMgr.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_3":
                    //npcMgr.BoyType_1_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_02_Idle":
                    npcScaning.BoyType_2_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer":
                    //npcMgr.BoyType_2_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer_2":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit":
                    //npcMgr.BoyType_2_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_3":
                    //npcMgr.BoyType_2_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_03_Idle":
                    //npcMgr.BoyType_3_Idle_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer":
                    //npcMgr.BoyType_3_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer_2":
                    //npcMgr.BoyType_3_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit":
                    //npcMgr.BoyType_3_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_2":
                    //npcMgr.BoyType_3_Sit2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_3":
                    //npcMgr.BoyType_3_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_04_Idle":
                    //npcMgr.BoyType_4_Idle_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Idle_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer":
                    //npcMgr.BoyType_4_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer_2":
                    //npcMgr.BoyType_4_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit":
                    //npcMgr.BoyType_4_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_2":
                    //npcMgr.BoyType_4_Sit2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_3":
                    //npcMgr.BoyType_4_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }

        #region Create Boy NPC Info
        CreateRenderNode("BoyType_1_Idle_Trans", npcScaning.BoyType_1_Idle_Trans, "BoyType_1_Idle_", npcScaning.BoyType_1_Idle);
        CreateRenderNode("BoyType_1_Idle_Cheer_Trans", npcScaning.BoyType_1_Idle_Cheer_Trans, "BoyType_1_Idle_Cheer_", npcScaning.BoyType_1_Idle_Cheer);
        CreateRenderNode("BoyType_1_Idle_Cheer2_Trans", npcScaning.BoyType_1_Idle_Cheer2_Trans, "BoyType_1_Idle_Cheer2_", npcScaning.BoyType_1_Idle_Cheer2);
        CreateRenderNode("BoyType_1_Sit_Trans", npcScaning.BoyType_1_Sit_Trans, "BoyType_1_Sit_", npcScaning.BoyType_1_Sit);
        CreateRenderNode("BoyType_1_Sit2_Trans", npcScaning.BoyType_1_Sit2_Trans, "BoyType_1_Sit2_", npcScaning.BoyType_1_Sit2);
        CreateRenderNode("BoyType_1_Sit3_Trans", npcScaning.BoyType_1_Sit3_Trans, "BoyType_1_Sit3_", npcScaning.BoyType_1_Sit3);


        CreateRenderNode("BoyType_2_Idle_Trans", npcScaning.BoyType_2_Idle_Trans, "BoyType_2_Idle_", npcScaning.BoyType_2_Idle);
        CreateRenderNode("BoyType_2_Idle_Cheer_Trans", npcScaning.BoyType_2_Idle_Cheer_Trans, "BoyType_2_Idle_Cheer_", npcScaning.BoyType_2_Idle_Cheer);
        CreateRenderNode("BoyType_2_Idle_Cheer2_Trans", npcScaning.BoyType_2_Idle_Cheer2_Trans, "BoyType_2_Idle_Cheer2_", npcScaning.BoyType_2_Idle_Cheer2);
        CreateRenderNode("BoyType_2_Sit_Trans", npcScaning.BoyType_2_Sit_Trans, "BoyType_2_Sit_", npcScaning.BoyType_2_Sit);
        CreateRenderNode("BoyType_2_Sit2_Trans", npcScaning.BoyType_2_Sit2_Trans, "BoyType_2_Sit2_", npcScaning.BoyType_2_Sit2);
        CreateRenderNode("BoyType_2_Sit3_Trans", npcScaning.BoyType_2_Sit3_Trans, "BoyType_2_Sit3_", npcScaning.BoyType_2_Sit3);


        CreateRenderNode("BoyType_3_Idle_Trans", npcScaning.BoyType_3_Idle_Trans, "BoyType_3_Idle_", npcScaning.BoyType_3_Idle);
        CreateRenderNode("BoyType_3_Idle_Cheer_Trans", npcScaning.BoyType_3_Idle_Cheer_Trans, "BoyType_3_Idle_Cheer_", npcScaning.BoyType_3_Idle_Cheer);
        CreateRenderNode("BoyType_3_Idle_Cheer2_Trans", npcScaning.BoyType_3_Idle_Cheer2_Trans, "BoyType_3_Idle_Cheer2_", npcScaning.BoyType_3_Idle_Cheer2);
        CreateRenderNode("BoyType_3_Sit_Trans", npcScaning.BoyType_3_Sit_Trans, "BoyType_3_Sit_", npcScaning.BoyType_3_Sit);
        CreateRenderNode("BoyType_3_Sit2_Trans", npcScaning.BoyType_3_Sit2_Trans, "BoyType_3_Sit2_", npcScaning.BoyType_3_Sit2);
        CreateRenderNode("BoyType_3_Sit3_Trans", npcScaning.BoyType_3_Sit3_Trans, "BoyType_3_Sit3_", npcScaning.BoyType_3_Sit3);


        CreateRenderNode("BoyType_4_Idle_Trans", npcScaning.BoyType_4_Idle_Trans, "BoyType_4_Idle_", npcScaning.BoyType_4_Idle);
        CreateRenderNode("BoyType_4_Idle_Cheer_Trans", npcScaning.BoyType_4_Idle_Cheer_Trans, "BoyType_4_Idle_Cheer_", npcScaning.BoyType_4_Idle_Cheer);
        CreateRenderNode("BoyType_4_Idle_Cheer2_Trans", npcScaning.BoyType_4_Idle_Cheer2_Trans, "BoyType_4_Idle_Cheer2_", npcScaning.BoyType_4_Idle_Cheer2);
        CreateRenderNode("BoyType_4_Sit_Trans", npcScaning.BoyType_4_Sit_Trans, "BoyType_4_Sit_", npcScaning.BoyType_4_Sit);
        CreateRenderNode("BoyType_4_Sit2_Trans", npcScaning.BoyType_4_Sit2_Trans, "BoyType_4_Sit2_", npcScaning.BoyType_4_Sit2);
        CreateRenderNode("BoyType_4_Sit3_Trans", npcScaning.BoyType_4_Sit3_Trans, "BoyType_4_Sit3_", npcScaning.BoyType_4_Sit3);
        #endregion
    }

    /// <summary>
    /// 男性角色： 2种皮肤 *  2种动画 = 4种材质
    /// 女性角色： 2种皮肤 *  2种动画 = 4种材质
    /// DrawaCall: 
    /// 8*2 = 16(默认2个，18个dc)
    /// </summary>
    /// <param name="boysNodesTrans"></param>
    /// <param name="girlsNodesTrans"></param>
    void CreateLowNpcs(List<Transform> boysNodesTrans, List<Transform> girlsNodesTrans)
    {
        MeshRenderer tmpMR = null;
        for (int i = 0; i < girlsNodesTrans.Count; i++)
        {
            tmpMR = girlsNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Girl_Audience_GPUAnimat_01_Idle":
                    //npcMgr.GirlType_1_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Idle_Cheer":
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit":
                    //npcMgr.GirlType_1_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit_2":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                case "Girl_Audience_GPUAnimat_02_Idle":
                    //npcMgr.GirlType_2_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Idle_Cheer":
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit":
                    //npcMgr.GirlType_2_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit_2":
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_03_Idle":
                    //npcMgr.GirlType_3_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Idle_Cheer":
                    //npcMgr.GirlType_3_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit":
                    //npcMgr.GirlType_3_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit_2":
                    //npcMgr.GirlType_3_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_04_Idle":
                    //npcMgr.GirlType_4_Idle_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Idle_Cheer":
                    //npcMgr.GirlType_4_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit":
                    //npcMgr.GirlType_4_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit_2":
                    //npcMgr.GirlType_4_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }


        #region Create Girl Npc Node
        CreateRenderNode("GirlType_1_Idle_Trans", npcScaning.GirlType_1_Idle_Trans, "GirlType_1_Idle_", npcScaning.GirlType_1_Idle);
        CreateRenderNode("GirlType_1_Idle_Cheer_Trans", npcScaning.GirlType_1_Idle_Cheer_Trans, "GirlType_1_Idle_Cheer", npcScaning.GirlType_1_Idle_Cheer);
        CreateRenderNode("GirlType_1_Sit_Trans", npcScaning.GirlType_1_Sit_Trans, "GirlType_1_Sit", npcScaning.GirlType_1_Sit);
        CreateRenderNode("GirlType_1_Sit2_Trans", npcScaning.GirlType_1_Sit2_Trans, "GirlType_1_Sit2", npcScaning.GirlType_1_Sit2);

        CreateRenderNode("GirlType_2_Idle_Trans", npcScaning.GirlType_2_Idle_Trans, "GirlType_2_Idle_", npcScaning.GirlType_2_Idle);
        CreateRenderNode("GirlType_2_Idle_Cheer_Trans", npcScaning.GirlType_2_Idle_Cheer_Trans, "GirlType_2_Idle_Cheer", npcScaning.GirlType_2_Idle_Cheer);
        CreateRenderNode("GirlType_2_Sit_Trans", npcScaning.GirlType_2_Sit_Trans, "GirlType_2_Sit", npcScaning.GirlType_2_Sit);
        CreateRenderNode("GirlType_2_Sit2_Trans", npcScaning.GirlType_2_Sit2_Trans, "GirlType_2_Sit2", npcScaning.GirlType_2_Sit2);

        CreateRenderNode("GirlType_3_Idle_Trans", npcScaning.GirlType_3_Idle_Trans, "GirlType_3_Idle_", npcScaning.GirlType_3_Idle);
        CreateRenderNode("GirlType_3_Idle_Cheer_Trans", npcScaning.GirlType_3_Idle_Cheer_Trans, "GirlType_3_Idle_Cheer", npcScaning.GirlType_3_Idle_Cheer);
        CreateRenderNode("GirlType_3_Sit_Trans", npcScaning.GirlType_3_Sit_Trans, "GirlType_3_Idle_Sit", npcScaning.GirlType_3_Sit);
        CreateRenderNode("GirlType_3_Sit2_Trans", npcScaning.GirlType_3_Sit2_Trans, "GirlType_3_Idle_Sit2", npcScaning.GirlType_3_Sit2);

        CreateRenderNode("GirlType_4_Idle_Trans", npcScaning.GirlType_4_Idle_Trans, "GirlType_4_Idle_", npcScaning.GirlType_4_Idle);
        CreateRenderNode("GirlType_4_Idle_Cheer_Trans", npcScaning.GirlType_4_Idle_Cheer_Trans, "GirlType_4_Idle_Cheer_", npcScaning.GirlType_4_Idle_Cheer);
        CreateRenderNode("GirlType_4_Sit_Trans", npcScaning.GirlType_4_Sit_Trans, "GirlType_4_Sit_", npcScaning.GirlType_4_Sit);
        CreateRenderNode("GirlType_4_Sit2_Trans", npcScaning.GirlType_4_Sit2_Trans, "GirlType_4_Sit2_", npcScaning.GirlType_4_Sit2);

        #endregion

        for (int i = 0; i < boysNodesTrans.Count; i++)
        {
            tmpMR = boysNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Boy_Audience_GPUAnima_01_Idle":
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer":
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer_2":
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit":
                    //npcMgr.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_2":
                    //npcMgr.BoyType_1_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_3":
                    //npcMgr.BoyType_1_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_02_Idle":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer":
                    //npcMgr.BoyType_2_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer_2":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit":
                    //npcMgr.BoyType_2_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_3":
                    //npcMgr.BoyType_2_Sit3_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_03_Idle":
                    //npcMgr.BoyType_3_Idle_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer":
                    //npcMgr.BoyType_3_Idle_Cheer_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer_2":
                    //npcMgr.BoyType_3_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit":
                    //npcMgr.BoyType_3_Sit_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_2":
                    //npcMgr.BoyType_3_Sit2_Trans.Add(boysNodesTrans[i]);
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_3":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_04_Idle":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer_2":
                    npcScaning.BoyType_2_Idle_Cheer2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_3":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }

        #region Create Boy NPC Info
        CreateRenderNode("BoyType_1_Idle_Trans", npcScaning.BoyType_1_Idle_Trans, "BoyType_1_Idle_", npcScaning.BoyType_1_Idle);
        CreateRenderNode("BoyType_1_Idle_Cheer_Trans", npcScaning.BoyType_1_Idle_Cheer_Trans, "BoyType_1_Idle_Cheer_", npcScaning.BoyType_1_Idle_Cheer);
        CreateRenderNode("BoyType_1_Idle_Cheer2_Trans", npcScaning.BoyType_1_Idle_Cheer2_Trans, "BoyType_1_Idle_Cheer2_", npcScaning.BoyType_1_Idle_Cheer2);
        CreateRenderNode("BoyType_1_Sit_Trans", npcScaning.BoyType_1_Sit_Trans, "BoyType_1_Sit_", npcScaning.BoyType_1_Sit);
        CreateRenderNode("BoyType_1_Sit2_Trans", npcScaning.BoyType_1_Sit2_Trans, "BoyType_1_Sit2_", npcScaning.BoyType_1_Sit2);
        CreateRenderNode("BoyType_1_Sit3_Trans", npcScaning.BoyType_1_Sit3_Trans, "BoyType_1_Sit3_", npcScaning.BoyType_1_Sit3);


        CreateRenderNode("BoyType_2_Idle_Trans", npcScaning.BoyType_2_Idle_Trans, "BoyType_2_Idle_", npcScaning.BoyType_2_Idle);
        CreateRenderNode("BoyType_2_Idle_Cheer_Trans", npcScaning.BoyType_2_Idle_Cheer_Trans, "BoyType_2_Idle_Cheer_", npcScaning.BoyType_2_Idle_Cheer);
        CreateRenderNode("BoyType_2_Idle_Cheer2_Trans", npcScaning.BoyType_2_Idle_Cheer2_Trans, "BoyType_2_Idle_Cheer2_", npcScaning.BoyType_2_Idle_Cheer2);
        CreateRenderNode("BoyType_2_Sit_Trans", npcScaning.BoyType_2_Sit_Trans, "BoyType_2_Sit_", npcScaning.BoyType_2_Sit);
        CreateRenderNode("BoyType_2_Sit2_Trans", npcScaning.BoyType_2_Sit2_Trans, "BoyType_2_Sit2_", npcScaning.BoyType_2_Sit2);
        CreateRenderNode("BoyType_2_Sit3_Trans", npcScaning.BoyType_2_Sit3_Trans, "BoyType_2_Sit3_", npcScaning.BoyType_2_Sit3);


        CreateRenderNode("BoyType_3_Idle_Trans", npcScaning.BoyType_3_Idle_Trans, "BoyType_3_Idle_", npcScaning.BoyType_3_Idle);
        CreateRenderNode("BoyType_3_Idle_Cheer_Trans", npcScaning.BoyType_3_Idle_Cheer_Trans, "BoyType_3_Idle_Cheer_", npcScaning.BoyType_3_Idle_Cheer);
        CreateRenderNode("BoyType_3_Idle_Cheer2_Trans", npcScaning.BoyType_3_Idle_Cheer2_Trans, "BoyType_3_Idle_Cheer2_", npcScaning.BoyType_3_Idle_Cheer2);
        CreateRenderNode("BoyType_3_Sit_Trans", npcScaning.BoyType_3_Sit_Trans, "BoyType_3_Sit_", npcScaning.BoyType_3_Sit);
        CreateRenderNode("BoyType_3_Sit2_Trans", npcScaning.BoyType_3_Sit2_Trans, "BoyType_3_Sit2_", npcScaning.BoyType_3_Sit2);
        CreateRenderNode("BoyType_3_Sit3_Trans", npcScaning.BoyType_3_Sit3_Trans, "BoyType_3_Sit3_", npcScaning.BoyType_3_Sit3);


        CreateRenderNode("BoyType_4_Idle_Trans", npcScaning.BoyType_4_Idle_Trans, "BoyType_4_Idle_", npcScaning.BoyType_4_Idle);
        CreateRenderNode("BoyType_4_Idle_Cheer_Trans", npcScaning.BoyType_4_Idle_Cheer_Trans, "BoyType_4_Idle_Cheer_", npcScaning.BoyType_4_Idle_Cheer);
        CreateRenderNode("BoyType_4_Idle_Cheer2_Trans", npcScaning.BoyType_4_Idle_Cheer2_Trans, "BoyType_4_Idle_Cheer2_", npcScaning.BoyType_4_Idle_Cheer2);
        CreateRenderNode("BoyType_4_Sit_Trans", npcScaning.BoyType_4_Sit_Trans, "BoyType_4_Sit_", npcScaning.BoyType_4_Sit);
        CreateRenderNode("BoyType_4_Sit2_Trans", npcScaning.BoyType_4_Sit2_Trans, "BoyType_4_Sit2_", npcScaning.BoyType_4_Sit2);
        CreateRenderNode("BoyType_4_Sit3_Trans", npcScaning.BoyType_4_Sit3_Trans, "BoyType_4_Sit3_", npcScaning.BoyType_4_Sit3);
        #endregion
    }

    /// <summary>
    /// 男性角色： 2种皮肤 *  1种动画 = 2种材质
    /// 女性角色： 2种皮肤 *  1种动画 = 2种材质
    /// DrawaCall: 
    /// 4*2 = 8(默认2个，10个dc)
    /// </summary>
    /// <param name="boysNodesTrans"></param>
    /// <param name="girlsNodesTrans"></param>
    void CreateLowestNpcs(List<Transform> boysNodesTrans, List<Transform> girlsNodesTrans)
    {
        MeshRenderer tmpMR = null;
        for (int i = 0; i < girlsNodesTrans.Count; i++)
        {
            tmpMR = girlsNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Girl_Audience_GPUAnimat_01_Idle":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Idle_Cheer":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_01_Sit_2":
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                case "Girl_Audience_GPUAnimat_02_Idle":
                    //npcMgr.GirlType_2_Idle_Trans.Add(girlsNodesTrans[i]);
                    //npcMgr.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Idle_Cheer":
                    //npcMgr.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit":
                    //npcMgr.GirlType_2_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_02_Sit_2":
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_03_Idle":
                    //npcMgr.GirlType_3_Idle_Trans.Add(girlsNodesTrans[i]);
                    //npcMgr.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Idle_Cheer":
                    //npcMgr.GirlType_3_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    //npcMgr.GirlType_1_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit":
                    //npcMgr.GirlType_3_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_03_Sit_2":
                    //npcMgr.GirlType_3_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_1_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;


                case "Girl_Audience_GPUAnimat_04_Idle":
                    //npcMgr.GirlType_4_Idle_Trans.Add(girlsNodesTrans[i]);
                    //npcMgr.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Idle_Cheer":
                    //npcMgr.GirlType_4_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    //npcMgr.GirlType_2_Idle_Cheer_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit":
                    //npcMgr.GirlType_4_Sit_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;
                case "Girl_Audience_GPUAnimat_04_Sit_2":
                    //npcMgr.GirlType_4_Sit2_Trans.Add(girlsNodesTrans[i]);
                    npcScaning.GirlType_2_Sit2_Trans.Add(girlsNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }


        #region Create Girl Npc Node
        CreateRenderNode("GirlType_1_Idle_Trans", npcScaning.GirlType_1_Idle_Trans, "GirlType_1_Idle_", npcScaning.GirlType_1_Idle);
        CreateRenderNode("GirlType_1_Idle_Cheer_Trans", npcScaning.GirlType_1_Idle_Cheer_Trans, "GirlType_1_Idle_Cheer", npcScaning.GirlType_1_Idle_Cheer);
        CreateRenderNode("GirlType_1_Sit_Trans", npcScaning.GirlType_1_Sit_Trans, "GirlType_1_Sit", npcScaning.GirlType_1_Sit);
        CreateRenderNode("GirlType_1_Sit2_Trans", npcScaning.GirlType_1_Sit2_Trans, "GirlType_1_Sit2", npcScaning.GirlType_1_Sit2);

        CreateRenderNode("GirlType_2_Idle_Trans", npcScaning.GirlType_2_Idle_Trans, "GirlType_2_Idle_", npcScaning.GirlType_2_Idle);
        CreateRenderNode("GirlType_2_Idle_Cheer_Trans", npcScaning.GirlType_2_Idle_Cheer_Trans, "GirlType_2_Idle_Cheer", npcScaning.GirlType_2_Idle_Cheer);
        CreateRenderNode("GirlType_2_Sit_Trans", npcScaning.GirlType_2_Sit_Trans, "GirlType_2_Sit", npcScaning.GirlType_2_Sit);
        CreateRenderNode("GirlType_2_Sit2_Trans", npcScaning.GirlType_2_Sit2_Trans, "GirlType_2_Sit2", npcScaning.GirlType_2_Sit2);

        CreateRenderNode("GirlType_3_Idle_Trans", npcScaning.GirlType_3_Idle_Trans, "GirlType_3_Idle_", npcScaning.GirlType_3_Idle);
        CreateRenderNode("GirlType_3_Idle_Cheer_Trans", npcScaning.GirlType_3_Idle_Cheer_Trans, "GirlType_3_Idle_Cheer", npcScaning.GirlType_3_Idle_Cheer);
        CreateRenderNode("GirlType_3_Sit_Trans", npcScaning.GirlType_3_Sit_Trans, "GirlType_3_Idle_Sit", npcScaning.GirlType_3_Sit);
        CreateRenderNode("GirlType_3_Sit2_Trans", npcScaning.GirlType_3_Sit2_Trans, "GirlType_3_Idle_Sit2", npcScaning.GirlType_3_Sit2);

        CreateRenderNode("GirlType_4_Idle_Trans", npcScaning.GirlType_4_Idle_Trans, "GirlType_4_Idle_", npcScaning.GirlType_4_Idle);
        CreateRenderNode("GirlType_4_Idle_Cheer_Trans", npcScaning.GirlType_4_Idle_Cheer_Trans, "GirlType_4_Idle_Cheer_", npcScaning.GirlType_4_Idle_Cheer);
        CreateRenderNode("GirlType_4_Sit_Trans", npcScaning.GirlType_4_Sit_Trans, "GirlType_4_Sit_", npcScaning.GirlType_4_Sit);
        CreateRenderNode("GirlType_4_Sit2_Trans", npcScaning.GirlType_4_Sit2_Trans, "GirlType_4_Sit2_", npcScaning.GirlType_4_Sit2);

        #endregion

        for (int i = 0; i < boysNodesTrans.Count; i++)
        {
            tmpMR = boysNodesTrans[i].GetComponent<MeshRenderer>();
            switch (tmpMR.sharedMaterials[0].name)
            {
                case "Boy_Audience_GPUAnima_01_Idle":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Idle_Cheer_2":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_2":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_01_Sit_3":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_02_Idle":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Idle_Cheer_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_02_Sit_3":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_03_Idle":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Idle_Cheer_2":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_2":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_03_Sit_3":
                    npcScaning.BoyType_1_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;


                case "Boy_Audience_GPUAnima_04_Idle":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Idle_Cheer_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_2":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;
                case "Boy_Audience_GPUAnima_04_Sit_3":
                    npcScaning.BoyType_2_Sit2_Trans.Add(boysNodesTrans[i]);
                    break;

                default:
                    break;
            }
        }

        #region Create Boy NPC Info
        CreateRenderNode("BoyType_1_Idle_Trans", npcScaning.BoyType_1_Idle_Trans, "BoyType_1_Idle_", npcScaning.BoyType_1_Idle);
        CreateRenderNode("BoyType_1_Idle_Cheer_Trans", npcScaning.BoyType_1_Idle_Cheer_Trans, "BoyType_1_Idle_Cheer_", npcScaning.BoyType_1_Idle_Cheer);
        CreateRenderNode("BoyType_1_Idle_Cheer2_Trans", npcScaning.BoyType_1_Idle_Cheer2_Trans, "BoyType_1_Idle_Cheer2_", npcScaning.BoyType_1_Idle_Cheer2);
        CreateRenderNode("BoyType_1_Sit_Trans", npcScaning.BoyType_1_Sit_Trans, "BoyType_1_Sit_", npcScaning.BoyType_1_Sit);
        CreateRenderNode("BoyType_1_Sit2_Trans", npcScaning.BoyType_1_Sit2_Trans, "BoyType_1_Sit2_", npcScaning.BoyType_1_Sit2);
        CreateRenderNode("BoyType_1_Sit3_Trans", npcScaning.BoyType_1_Sit3_Trans, "BoyType_1_Sit3_", npcScaning.BoyType_1_Sit3);


        CreateRenderNode("BoyType_2_Idle_Trans", npcScaning.BoyType_2_Idle_Trans, "BoyType_2_Idle_", npcScaning.BoyType_2_Idle);
        CreateRenderNode("BoyType_2_Idle_Cheer_Trans", npcScaning.BoyType_2_Idle_Cheer_Trans, "BoyType_2_Idle_Cheer_", npcScaning.BoyType_2_Idle_Cheer);
        CreateRenderNode("BoyType_2_Idle_Cheer2_Trans", npcScaning.BoyType_2_Idle_Cheer2_Trans, "BoyType_2_Idle_Cheer2_", npcScaning.BoyType_2_Idle_Cheer2);
        CreateRenderNode("BoyType_2_Sit_Trans", npcScaning.BoyType_2_Sit_Trans, "BoyType_2_Sit_", npcScaning.BoyType_2_Sit);
        CreateRenderNode("BoyType_2_Sit2_Trans", npcScaning.BoyType_2_Sit2_Trans, "BoyType_2_Sit2_", npcScaning.BoyType_2_Sit2);
        CreateRenderNode("BoyType_2_Sit3_Trans", npcScaning.BoyType_2_Sit3_Trans, "BoyType_2_Sit3_", npcScaning.BoyType_2_Sit3);


        CreateRenderNode("BoyType_3_Idle_Trans", npcScaning.BoyType_3_Idle_Trans, "BoyType_3_Idle_", npcScaning.BoyType_3_Idle);
        CreateRenderNode("BoyType_3_Idle_Cheer_Trans", npcScaning.BoyType_3_Idle_Cheer_Trans, "BoyType_3_Idle_Cheer_", npcScaning.BoyType_3_Idle_Cheer);
        CreateRenderNode("BoyType_3_Idle_Cheer2_Trans", npcScaning.BoyType_3_Idle_Cheer2_Trans, "BoyType_3_Idle_Cheer2_", npcScaning.BoyType_3_Idle_Cheer2);
        CreateRenderNode("BoyType_3_Sit_Trans", npcScaning.BoyType_3_Sit_Trans, "BoyType_3_Sit_", npcScaning.BoyType_3_Sit);
        CreateRenderNode("BoyType_3_Sit2_Trans", npcScaning.BoyType_3_Sit2_Trans, "BoyType_3_Sit2_", npcScaning.BoyType_3_Sit2);
        CreateRenderNode("BoyType_3_Sit3_Trans", npcScaning.BoyType_3_Sit3_Trans, "BoyType_3_Sit3_", npcScaning.BoyType_3_Sit3);


        CreateRenderNode("BoyType_4_Idle_Trans", npcScaning.BoyType_4_Idle_Trans, "BoyType_4_Idle_", npcScaning.BoyType_4_Idle);
        CreateRenderNode("BoyType_4_Idle_Cheer_Trans", npcScaning.BoyType_4_Idle_Cheer_Trans, "BoyType_4_Idle_Cheer_", npcScaning.BoyType_4_Idle_Cheer);
        CreateRenderNode("BoyType_4_Idle_Cheer2_Trans", npcScaning.BoyType_4_Idle_Cheer2_Trans, "BoyType_4_Idle_Cheer2_", npcScaning.BoyType_4_Idle_Cheer2);
        CreateRenderNode("BoyType_4_Sit_Trans", npcScaning.BoyType_4_Sit_Trans, "BoyType_4_Sit_", npcScaning.BoyType_4_Sit);
        CreateRenderNode("BoyType_4_Sit2_Trans", npcScaning.BoyType_4_Sit2_Trans, "BoyType_4_Sit2_", npcScaning.BoyType_4_Sit2);
        CreateRenderNode("BoyType_4_Sit3_Trans", npcScaning.BoyType_4_Sit3_Trans, "BoyType_4_Sit3_", npcScaning.BoyType_4_Sit3);
        #endregion
    }

    void CreateRenderNode(string tarName, List<Transform> childTrans, string childName, Transform sourceTrans)
    {
        if (childTrans.Count > 0)
        {

            Transform parent = npcScaning.NpcRenderNode.transform;
            GameObject tar = new GameObject(tarName);
            tar.transform.SetParent(parent);
            tar.transform.localPosition = Vector3.zero;
            tar.transform.localScale = Vector3.one;
            tar.transform.rotation = new Quaternion();

            ArrayByInstancing batching = tar.AddComponent<ArrayByInstancing>();

            for (int i = 0; i < childTrans.Count; i++)
            {
                string name = childName + i.ToString();

                GameObject tmpChild = new GameObject(name);
                tmpChild.transform.SetParent(tar.transform);
                tmpChild.transform.position = childTrans[i].position;
                tmpChild.transform.localScale = Vector3.one;
                tmpChild.transform.rotation = childTrans[i].rotation;
            }

            batching.RefreshTarTrans();
            batching.OriMeshRenderer = sourceTrans.GetComponent<MeshRenderer>();
            batching.OriMesh = sourceTrans.GetComponent<MeshFilter>().sharedMesh;

        }
    }
    #endregion
}
#endif
