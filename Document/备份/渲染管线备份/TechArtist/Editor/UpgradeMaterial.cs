using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UpgradeMaterialTools
{
    [MenuItem("公共工具/TATools/选中材质/升级成SSS材质")]
    public static void UpgradeMaterals()
    {
        string[] guids = Selection.assetGUIDs;
        List<Material> mats = new List<Material>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                mats.Add(mat);
            }
        }

        Shader sss = Shader.Find("NBA2/URP/SkinSSS");
        if (sss == null)
        {
            return;
        }

        for (int i = 0; i < mats.Count; i++)
        {
            if (mats[i].shader.name == "NBA2/URP/SkinSSS")
                continue;

            Texture texture = mats[i].mainTexture;
            mats[i].shader = sss;
            mats[i].SetTexture("_AlbedoMap", texture);
            //mats[i].mainTexture = texture;
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("公共工具/TATools/选中材质/检测InternalErrorShader")]
    public static void ChechInternalErrorShader()
    {
        string[] guids = Selection.assetGUIDs;
        List<Material> mats = new List<Material>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                mats.Add(mat);
            }
        }

        for (int i = 0; i < mats.Count; i++)
        {
            if (mats[i].shader.name.Contains("InternalErrorShader"))
            {
                Debug.Log(mats[i].name);
            }
        }
    }

    [MenuItem("公共工具/TATools/选中材质/检测UniversalLit")]
    public static void Chech检测UniversalLit()
    {
        string[] guids = Selection.assetGUIDs;
        List<Material> mats = new List<Material>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                mats.Add(mat);
            }
        }

        for (int i = 0; i < mats.Count; i++)
        {
            if (mats[i].shader.name.Contains("Universal Render Pipeline/Lit"))
            {
                Debug.Log(mats[i].name);
            }
        }
    }

    [MenuItem("公共工具/TATools/选中预制体/添加伪阴影材质")]
    public static void AddMetarials()
    {
        string shadowMatPath = @"Assets/TechArtist/Plugins/ProjectShadow/ProjectShadowMat.mat";

        Material shadowMat = AssetDatabase.LoadAssetAtPath<Material>(shadowMatPath);

        if (shadowMat == null)
        {
            return;
        }

        string[] guids = Selection.assetGUIDs;
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < guids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (go != null)
            {
                goList.Add(go);
            }
        }


        if (goList.Count == 0)
        {
            return;
        }



        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            var sRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (sRenderer != null)
            {
                Material[] oldMats = sRenderer.sharedMaterials;

                var findResult = System.Array.Find(oldMats, (item) => { return item.Equals(shadowMat); });
                if (findResult != null)
                {
                    continue;
                }

                Material[] newMats = new Material[oldMats.Length + 1];
                System.Array.Copy(oldMats, newMats, oldMats.Length);
                newMats[newMats.Length - 1] = shadowMat;

                sRenderer.sharedMaterials = newMats;
            }
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("公共工具/TATools/选中预制体/删除伪阴影材质")]
    public static void DeleteMetarials()
    {
        string shadowMatPath = @"Assets/TechArtist/Materials/ProjectShadow/ProjectShadow.mat";

        Material shadowMat = AssetDatabase.LoadAssetAtPath<Material>(shadowMatPath);

        if (shadowMat == null)
        {
            return;
        }

        string[] guids = Selection.assetGUIDs;
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < guids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (go != null)
            {
                goList.Add(go);
            }
        }


        if (goList.Count == 0)
        {
            return;
        }



        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            var sRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (sRenderer != null)
            {
                Material[] oldMats = sRenderer.sharedMaterials;
                Material findResult = null;
                int idx = -1;
                for (int j = 0; j < oldMats.Length; j++)
                {
                    if (oldMats[j].Equals(shadowMat))
                    {
                        idx = j;
                        findResult = oldMats[j];
                        break;
                    }
                }

                if (findResult == null)
                {
                    continue;
                }

                Material[] newMats = new Material[oldMats.Length - 1];
                int newMatIdx = 0;
                for (int k = 0; k < oldMats.Length; k++)
                {
                    if (idx != k && newMatIdx < newMats.Length)
                    {
                        newMats[newMatIdx] = oldMats[k];
                        newMatIdx++;
                    }
                }

                sRenderer.sharedMaterials = newMats;
            }
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("公共工具/TATools/选中预制体/升级成SSS材质")]
    public static void UpgradeMateralsFromPrefab()
    {
        string[] guids = Selection.assetGUIDs;
        List<Material> mats = new List<Material>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            GameObject objs = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (objs != null)
            {
                SkinnedMeshRenderer tmp = objs.GetComponentInChildren<SkinnedMeshRenderer>();

                if (tmp == null)
                {
                    Debug.LogError("Can't dind meshrenderer on" + objs.name);
                    continue;
                }

                mats.Add(tmp.sharedMaterials[0]);
            }
        }

        Shader sss = Shader.Find("NBA2/URP/SkinSSS");
        if (sss == null)
        {
            return;
        }

        for (int i = 0; i < mats.Count; i++)
        {
            //if (mats[i].shader.name == "NBA2/URP/SkinSSS")
            //    continue;

            Texture texture = mats[i].mainTexture;
            mats[i].shader = sss;
            mats[i].SetTexture("_AlbedoMap", texture);

            string metallicMapPath = AssetDatabase.GUIDToAssetPath("5fd4eb7991b18e44292f325cc5b9344d");
            Texture2D metallicMap = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicMapPath);

            string norMapPath = AssetDatabase.GUIDToAssetPath("0faa516799e12724d9b55588f006b351");
            Texture2D norMap = AssetDatabase.LoadAssetAtPath<Texture2D>(norMapPath);

            mats[i].SetTexture("_MetallicMap", metallicMap);// 默认身体光照贴图
            mats[i].SetTexture("_NormalMap", norMap);// 默认身体法线贴图

            //mats[i].mainTexture = texture;
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("公共工具/TATools/选中预制体/关闭投射阴影")]
    public static void CloseShadows()
    {
        //string shadowMatPath = @"Assets/TechArtist/Plugins/ProjectShadow/ProjectShadowMat.mat";

        //Material shadowMat = AssetDatabase.LoadAssetAtPath<Material>(shadowMatPath);

        //if (shadowMat == null)
        //{
        //    return;
        //}

        string[] guids = Selection.assetGUIDs;
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < guids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (go != null)
            {
                goList.Add(go);
            }
        }


        if (goList.Count == 0)
        {
            return;
        }



        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            var sRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (sRenderer != null)
            {
                sRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("公共工具/TATools/选中预制体/身体模型FBX替换")]
    public static void ReplaceBodyFBX()
    {
        string bodyFbx = @"Assets/Art/player_figure/_body_01_skin.fbx";
        GameObject sourceFbx = AssetDatabase.LoadAssetAtPath<GameObject>(bodyFbx);
        if (sourceFbx == null)
        {
            Debug.LogError("Can't find body fbx on: " + bodyFbx);
            return;
        }

        string[] guids = Selection.assetGUIDs;
        List<GameObject> targets = new List<GameObject>();
        for (int i = 0; i < guids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (go != null)
            {
                GameObject tmp = GameObject.Instantiate(sourceFbx);
                targets.Add(tmp);
                tmp.transform.position = Vector3.zero;
                tmp.transform.rotation = new Quaternion(0, 0, 0, 0);
                tmp.transform.localScale = Vector3.one;
                tmp.name = go.name;

                SkinnedMeshRenderer tmpSk = tmp.GetComponentInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer goSk = go.GetComponentInChildren<SkinnedMeshRenderer>();

                tmpSk.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                tmpSk.sharedMaterials = goSk.sharedMaterials;

                PrefabUtility.SaveAsPrefabAssetAndConnect(tmp, prefabPath, InteractionMode.UserAction);
            }
        }

        Debug.Log("Change Count: " + targets.Count);
        AssetDatabase.SaveAssets();

        for (int i = 0; i < targets.Count; i++)
        {
            GameObject.DestroyImmediate(targets[i]);
        }
        targets.Clear();
    }

    [MenuItem("公共工具/TATools/选中模型/关闭导入材质")]
    public static void CloseImportMaterial()
    {
        string[] modelGUIDs = Selection.assetGUIDs;
        for (int i = 0; i < modelGUIDs.Length; i++)
        {
            string modelPath = AssetDatabase.GUIDToAssetPath(modelGUIDs[i]);
            ModelImporter importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
            if (importer != null)
            {
                try
                {
                    importer.materialImportMode = ModelImporterMaterialImportMode.None;
                    importer.SaveAndReimport();
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat("重新导入报错：\r\nAssetPath: {0}\r\nMessage: {1}", modelPath, e.Message);
                }
            }
        }
    }

}
