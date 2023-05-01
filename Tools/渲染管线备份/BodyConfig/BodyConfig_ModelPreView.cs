using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MU.DataBase;
using MU.DataBase.Config.Obj;
using MU.DataBase.Config.Table;
using System.Linq;
using MU.Tool;
using MU.UI.Common;
using MU.Define;
using System.Reflection;

namespace AI.Tools
{
#if UNITY_EDITOR
    public class BodyConfig_ModelPreView : IModelPreView
    {
        static string playerFolder = "Assets/TResources/prefabs/player/";

        static string prefabRootFolder = "Assets/TResources/prefabs/player/";
        static string bodyPrefabName = "body/_body_76ers_Home_01.prefab";
        static string numberPrefabName = "num/mesh_number.prefab";
        static string limbPrefabName = "limb/_limb_01.prefab";
        static string shoesPrefabName = "shoes/_shoes_05.prefab";

        static string rScale = "_RScale";
        static string gScale = "_GScale";
        static string bScale = "_BScale";
        static string aScale = "_AScale";

        static string transmissionColor = "_TranslucencyColor";

        static string albedoMap = "_AlbedoMap";
        static string normalMap = "_NormalMap";
        static string metallicMap = "_MetallicMap";

        private MU.DataBase.Config.Obj.PlayerObject playerObj;
        private GameObject prefab;
        private Cskin headSin;
        private UnityEngine.Transform headNode;

        //装备节点
        private UnityEngine.Transform headbandNode;
        private UnityEngine.Transform armguardNode;
        private UnityEngine.Transform kneeguardNode;

        //Transform headbandParent;
        Transform headbandBone;
        private Vector3 headbandBone_Position_Origin = new Vector3(0, 1.8f, 5);


        private Material[] bodyMat;
        private Material[] numberMat;
        private Material[] limbMat;
        private Material[] shoesMat;
        private Material[] headMat;

        private Material[] armguardMat;
        private Material[] kneeguardMat;
        public BodyConfig_ModelPreView(PlayerObject playerCfg, BodyInfo bodyInfo)
        {
            this.playerObj = playerCfg;
            GetSkinTables(playerObj.HeadModel);

            LoadPrefab();

            LoadTextureFromPath(bodyMat, bodyInfo.bodyMap);
            LoadTextureFromPath(numberMat, bodyInfo.numberMap);
            LoadTextureFromPath(limbMat, bodyInfo.limbMap);
            LoadTextureFromPath(shoesMat, bodyInfo.shoesMap);
            LoadTextureFromPath(headMat, bodyInfo.headMap);

            ReflashValues(bodyInfo);
        }

        public void OnUpdate(BodyInfo bodyInfo)
        {
            ReflashValues(bodyInfo);
        }

        private void ReflashValues(BodyInfo bodyInfo)
        {
            SetTransformPosAndScale(bodyInfo.headOffset, bodyInfo.headScale, headNode);

            //保留装备 偏移量 控制 Position
            SetHeadbandValue(bodyInfo, headbandNode, headbandBone);
            //SetTransformPosAndScale(bodyInfo.headbandOffset, bodyInfo.headbandScale, headbandNode);
            SetTransformPosAndScale(bodyInfo.armguardOffset, Vector3.one, armguardNode);
            SetTransformPosAndScale(bodyInfo.kneeguardOffset, Vector3.one, kneeguardNode);

            //装备 缩放 控制 Shader
            SetValue(armguardMat, bodyInfo.armguardScale);
            SetValue(kneeguardMat, bodyInfo.kneeguardScale);

            SetValue(bodyMat, bodyInfo.bodyScale);
            SetValue(numberMat, bodyInfo.numberScale);
            SetValue(limbMat, bodyInfo.limbScale);
            SetValue(shoesMat, bodyInfo.shoesScale);

            SetColor(limbMat, bodyInfo.limb_transmissionColor);

            SetMapPath(headMat, bodyInfo.headMap);
            SetMapPath(bodyMat, bodyInfo.bodyMap);
            SetMapPath(numberMat, bodyInfo.numberMap);
            SetMapPath(limbMat, bodyInfo.limbMap);
            SetMapPath(shoesMat, bodyInfo.shoesMap);
        }

        private void SetTransformPosAndScale(Vector3 Position, Vector3 Scale, Transform transform)
        {
            transform.position = prefab.transform.position + (prefab.transform.rotation * Position);
            Vector3 factor = new Vector3(transform.lossyScale.x / transform.localScale.x, transform.lossyScale.y / transform.localScale.y, transform.lossyScale.z / transform.localScale.z);
            transform.localScale = new Vector3(Scale.x / factor.x, Scale.y / factor.y, Scale.z / factor.z);
        }

        private void SetHeadbandValue(BodyInfo info, Transform headbandNode, Transform headbandbone)
        {
            Vector3 offset = info.headbandOffset;
            Vector3 scale = info.headbandScale;

            headbandbone.localPosition = offset;
            headbandbone.localScale = scale;
        }

        private void SetColor(Material[] mats, Color transColor)
        {
            if (mats == null)
                return;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null)
                {
                    continue;
                }
                mats[i].SetColor(transmissionColor, transColor);
            }
        }

        private void SetValue(Material[] mats, Vector4 scale)
        {
            if (mats == null)
                return;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null)
                {
                    continue;
                }
                mats[i].SetFloat(rScale, scale.x);
                mats[i].SetFloat(gScale, scale.y);
                mats[i].SetFloat(bScale, scale.z);
                mats[i].SetFloat(aScale, scale.w);
            }
        }

        private void SetMapPath(Material[] mats, BodyMapPath bodyMapPath)
        {
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].HasProperty(albedoMap))
                {
                    Texture albedoMapText = mats[i].GetTexture(albedoMap);
                    if (albedoMapText != null)
                    {
                        bodyMapPath.albedoMap = AssetDatabase.GetAssetPath(albedoMapText);
                    }
                }

                if (mats[i].HasProperty(normalMap))
                {
                    Texture normalMapText = mats[i].GetTexture(normalMap);
                    if (normalMapText != null)
                    {
                        bodyMapPath.normalMap = AssetDatabase.GetAssetPath(normalMapText);
                    }
                }

                if (mats[i].HasProperty(metallicMap))
                {
                    Texture metallicMapText = mats[i].GetTexture(metallicMap);
                    if (metallicMapText != null)
                    {
                        bodyMapPath.metallic = AssetDatabase.GetAssetPath(metallicMapText);
                    }
                }
            }
        }


        private void LoadTextureFromPath(Material[] mats, BodyMapPath bodyMapPath)
        {
            for (int i = 0; i < mats.Length; i++)
            {
                if (!string.IsNullOrEmpty(bodyMapPath.albedoMap) && mats[i].HasProperty(albedoMap))
                {
                    //替换新路径
                    bodyMapPath.albedoMap = OldPathToNewPath(bodyMapPath.albedoMap, typeof(Texture));
                    Texture albedoMapText = AssetDatabase.LoadAssetAtPath<Texture>(bodyMapPath.albedoMap);
                    mats[i].SetTexture(albedoMap, albedoMapText);
                }

                if (!string.IsNullOrEmpty(bodyMapPath.normalMap) && mats[i].HasProperty(normalMap))
                {
                    //替换新路径
                    bodyMapPath.normalMap = OldPathToNewPath(bodyMapPath.normalMap, typeof(Texture));
                    Texture normalMapText = AssetDatabase.LoadAssetAtPath<Texture>(bodyMapPath.normalMap);
                    mats[i].SetTexture(normalMap, normalMapText);
                }

                if (!string.IsNullOrEmpty(bodyMapPath.metallic) && mats[i].HasProperty(albedoMap))
                {
                    //替换新路径
                    bodyMapPath.metallic = OldPathToNewPath(bodyMapPath.metallic, typeof(Texture));
                    Texture metallicMapText = AssetDatabase.LoadAssetAtPath<Texture>(bodyMapPath.metallic);
                    mats[i].SetTexture(metallicMap, metallicMapText);
                }
            }
        }

        private void LoadPrefab()
        {

            prefab = new GameObject(playerObj.Name);
            string bodyPath = System.IO.Path.Combine(prefabRootFolder, bodyPrefabName);
            string numberPath = System.IO.Path.Combine(prefabRootFolder, numberPrefabName);
            string limbPath = System.IO.Path.Combine(prefabRootFolder, limbPrefabName);
            string shoesPath = System.IO.Path.Combine(prefabRootFolder, shoesPrefabName);
            string headPath = System.IO.Path.Combine(playerFolder, headSin.ModelPath + ".prefab");

            string headbandPath = System.IO.Path.Combine(playerFolder, "head_band/_headband_01_black.prefab");
            string armguardPath = System.IO.Path.Combine(playerFolder, "arm_r/_armguard_r_01_green.prefab");
            string kneeguardPath = System.IO.Path.Combine(playerFolder, "kne_l/_kneeguard_l_01_green.prefab");

            Transform bodyPrefab = AssetDatabase.LoadAssetAtPath<Transform>(bodyPath);
            Transform bodyNode = GameObject.Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform numberPrefab = AssetDatabase.LoadAssetAtPath<Transform>(numberPath);
            Transform numberNode = GameObject.Instantiate(numberPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform limbrPrefab = AssetDatabase.LoadAssetAtPath<Transform>(limbPath);
            Transform limbNode = GameObject.Instantiate(limbrPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform shoesPrefab = AssetDatabase.LoadAssetAtPath<Transform>(shoesPath);
            Transform shoesNode = GameObject.Instantiate(shoesPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform headPrefab = AssetDatabase.LoadAssetAtPath<Transform>(headPath);
            Transform headNode = GameObject.Instantiate(headPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            this.headNode = headNode;



            Transform headbandPrefab = AssetDatabase.LoadAssetAtPath<Transform>(headbandPath);
            headbandNode = GameObject.Instantiate(headbandPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform armguardPrefab = AssetDatabase.LoadAssetAtPath<Transform>(armguardPath);
            armguardNode = GameObject.Instantiate(armguardPrefab, Vector3.zero, Quaternion.identity, prefab.transform);
            Transform kneeguardPrefab = AssetDatabase.LoadAssetAtPath<Transform>(kneeguardPath);
            kneeguardNode = GameObject.Instantiate(kneeguardPrefab, Vector3.zero, Quaternion.identity, prefab.transform);

            prefab.transform.position = new Vector3(0, 0, 5);
            prefab.transform.eulerAngles = new Vector3(0, 180, 0);

            headbandBone = headbandNode.GetComponentInChildren<SkinnedMeshRenderer>().bones[0];

            Renderer renderCom = null;
            renderCom = bodyNode.GetComponentInChildren<Renderer>();
            bodyMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = bodyMat;

            renderCom = numberNode.GetComponentInChildren<Renderer>();
            numberMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = numberMat;

            renderCom = limbNode.GetComponentInChildren<Renderer>();
            limbMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = limbMat;

            renderCom = shoesNode.GetComponentInChildren<Renderer>();
            shoesMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = shoesMat;

            renderCom = headNode.GetComponentInChildren<Renderer>();
            headMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = headMat;

            renderCom = armguardNode.GetComponentInChildren<Renderer>();
            armguardMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = armguardMat;

            renderCom = kneeguardNode.GetComponentInChildren<Renderer>();
            kneeguardMat = CloneMats(renderCom.sharedMaterials);
            renderCom.sharedMaterials = kneeguardMat;
        }



        public void UnLoad()
        {
            if (prefab != null)
            {
                GameObject.DestroyImmediate(prefab);
            }
            DestroyMats(bodyMat);
            DestroyMats(numberMat);
            DestroyMats(limbMat);
            DestroyMats(shoesMat);
            DestroyMats(headMat);

            DestroyMats(armguardMat);
            DestroyMats(kneeguardMat);

            headNode = null;
            bodyMat = null;
            numberMat = null;
            limbMat = null;
            shoesMat = null;
            headMat = null;

            armguardMat = null;
            kneeguardMat = null;


            headbandNode = null;
            armguardNode = null;
            kneeguardNode = null;
        }

        static Material[] CloneMats(Material[] array)
        {
            Material[] mats = new Material[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                    mats[i] = new Material(array[i]);
            }
            return mats;
        }

        static void DestroyMats(Material[] array)
        {
            if (array == null)
                return;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                    GameObject.DestroyImmediate(array[i]);
            }
        }

        private void GetSkinTables(string skinId)
        {
            headSin = DBMgr.g_Ins.V_SkinsConfig.GetSkinDataById(skinId);
        }

        static string OldPathToNewPath(string oldPath, System.Type assetType)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(oldPath, assetType);

            if (asset == null)
            {
                string[] strs = oldPath.Split('/');
                string fileName = strs[strs.Length - 1];
                fileName = fileName.Split('.').First();

                string[] results = AssetDatabase.FindAssets(fileName);
                for (int i = 0; i < results.Length; i++)
                {
                    string resultPath = AssetDatabase.GUIDToAssetPath(results[i]);
                    asset = AssetDatabase.LoadAssetAtPath(resultPath, assetType);
                    if (asset != null)
                    {
                        return resultPath;
                    }
                }
                return string.Empty;
            }
            else
            {
                return oldPath;
            }
        }
    }

    public static class TransformExtend
    {
        public static Transform FindChildenByName(this Transform self, string name)
        {
            var find = self.Find(name);
            if (find != null)
                return find;
            for (int i = 0; i < self.childCount; i++)
            {
                var c = self.GetChild(i);
                find = c.FindChildenByName(name);
                if (find != null)
                    return find;
            }
            return null;
        }

        public static T[] GetComponentsByChildrens<T>(this Transform self) where T : Component
        {
            List<T> result = new List<T>();
            GetComponentsByChildrens_Internal(self, result);
            return result.ToArray();
        }

        private static void GetComponentsByChildrens_Internal<T>(Transform self, List<T> result) where T : Component
        {
            result.AddRange(self.GetComponents<T>());
            for (int i = 0; i < self.childCount; i++)
            {
                var c = self.GetChild(i);
                GetComponentsByChildrens_Internal(c, result);
            }
        }
    }


    public interface IModelPreView
    {
        void OnUpdate(BodyInfo bodyInfo);
        void UnLoad();
    }

    public class ModelPreView_New : IModelPreView
    {
        delegate void SetScale(PlayerModPart pPart);

        PlayerObject playerObject;
        BodyInfo curbodyConfig;
        CachePlayerInfo cache;
        UIPlayerModel playerView;
        LoadManager myLoadManager;
        SetScale setScale;

        IValueToString valueToString;

        public ModelPreView_New(PlayerObject playerCfg, BodyInfo bodyInfo)
        {
            playerObject = playerCfg;
            curbodyConfig = bodyInfo;
            valueToString = BodyConfig2Excel.GetTostringInterface();
            Dictionary<PlayerModPart, string> pPartInfo = new Dictionary<PlayerModPart, string>() 
            {
                { PlayerModPart.Shoes,"4005"},
                { PlayerModPart.Leg,"7005"},
                { PlayerModPart.Arm,"6005"},
                { PlayerModPart.HeadBand,"5005"},
            };
            PlayerHelper.g_Ins.GetPlayerGO(112, true, bodyInfo.id, OnFinish, pPartInfo, true, false);
        }
        private void OnFinish(CachePlayerInfo cachePlayerInfo)
        {
            myLoadManager = typeof(PlayerHelper).
               GetField("myLoadManager", BindingFlags.NonPublic | BindingFlags.Static)
               .GetValue(PlayerHelper.g_Ins) as LoadManager;

            setScale = typeof(LoadManager).
                GetMethod("InitScalePosInfo",BindingFlags.NonPublic | BindingFlags.Instance).
                CreateDelegate(typeof(SetScale), myLoadManager) as SetScale;

            myLoadManager.m_PlayInfo = playerObject;
            myLoadManager.m_BoneObj = cachePlayerInfo.myObj.transform.FindChildenByName("bodyRoot").gameObject;
            cache = cachePlayerInfo;
            playerView = cache.myObj.AddComponent<UIPlayerModel>();
            playerView.ResLoadFinsh(cache.myObj);
            playerView.transform.position = Vector3.zero;
            playerView.transform.eulerAngles = new Vector3(0, -180, 0);

            FindSkinMeshRenderer(myLoadManager.m_PlayerMeshRenders, myLoadManager.m_BoneObj.transform.parent.gameObject);
            ApplyConfig(playerObject, curbodyConfig);
        }

        private void FindSkinMeshRenderer(Dictionary<PlayerModPart, SkinnedMeshRenderer> dic,GameObject obj)
        {
            SkinnedMeshRenderer[] comps = obj.transform.GetComponentsByChildrens<SkinnedMeshRenderer>();

            foreach(var sm in comps)
            {
                string name = sm.name;

                int a = name.IndexOf("_limb_");

                if (name.IndexOf("_Head_") >=0)
                {
                    dic[PlayerModPart.Head] = sm;
                }
                else if (name.IndexOf("_limb_") >= 0)
                {
                    dic[PlayerModPart.Limb] = sm;
                }
                else if (name.IndexOf("_body_") >= 0)
                {
                    dic[PlayerModPart.Body] = sm;
                }
                else if (name.IndexOf("_shoes_") >= 0)
                {
                    dic[PlayerModPart.Shoes] = sm;
                }
                else if (name.IndexOf("_kneeguard_") >= 0)
                {
                    dic[PlayerModPart.Leg] = sm;
                }
                else if (name.IndexOf("_armguard_") >= 0)
                {
                    dic[PlayerModPart.Arm] = sm;
                }
            }

        }


        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        private void SetPropertys(PlayerObject playerCfg, BodyInfo bodyInfo)
        {
            keyValuePairs["headbandOffset"] = valueToString.ValueToString(bodyInfo.headbandOffset);
            keyValuePairs["headbandScale"] = valueToString.ValueToString(bodyInfo.headbandScale);
            keyValuePairs["headOffset"] = valueToString.ValueToString(bodyInfo.headOffset);
            keyValuePairs["headScale"] = valueToString.ValueToString(bodyInfo.headScale);
            keyValuePairs["limbScale"] = valueToString.ValueToString(bodyInfo.limbScale);
            keyValuePairs["bodyScale"] = valueToString.ValueToString(bodyInfo.bodyScale);
            keyValuePairs["shoesScale"] = valueToString.ValueToString(bodyInfo.shoesScale);
            keyValuePairs["armguardScale"] = valueToString.ValueToString(bodyInfo.armguardScale);
            keyValuePairs["kneeguardScale"] = valueToString.ValueToString(bodyInfo.kneeguardScale);
            playerCfg.SetBodyConfigInEditor(keyValuePairs);
        }

        private void ApplyConfig(PlayerObject playerCfg, BodyInfo bodyInfo)
        {
            SetPropertys(playerCfg, bodyInfo);
            setScale.Invoke(PlayerModPart.Head);
            setScale.Invoke(PlayerModPart.HeadBand);
            setScale.Invoke(PlayerModPart.Limb);
            setScale.Invoke(PlayerModPart.Body);
            setScale.Invoke(PlayerModPart.Shoes);
            setScale.Invoke(PlayerModPart.Arm);
            setScale.Invoke(PlayerModPart.Leg);

            playerView.m_Animator.Update(0);
        }
        public void OnUpdate(BodyInfo bodyInfo)
        {
            curbodyConfig = bodyInfo;
            ApplyConfig(playerObject, curbodyConfig);
        }

        public void UnLoad()
        {
            if (cache == null)
                return;
            myLoadManager = null;
            setScale = null;
            GameObject.DestroyImmediate(cache.myObj);
            playerView = null;
            cache = null;
        }
    }
#endif
}

