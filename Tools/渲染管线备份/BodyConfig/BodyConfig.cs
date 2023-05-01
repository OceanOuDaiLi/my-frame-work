using System;
using UnityEngine;
using UnityEditor;
using SDB;
using System.Collections.Generic;
using MU.DataBase;
using MU.DataBase.Config.Obj;

/// <summary>
/// Author：谢长烨
/// Created Time：2022/07/18
/// Descriptions：
namespace AI.Tools
{
#if UNITY_EDITOR
    [System.Serializable]
    public class BodyInfo
    {
        public int id;
        [Header("头部缩放")]
        public Vector3 headScale;
        [Header("头部偏移")]
        public Vector3 headOffset;
        [Header("四肢缩放")]
        public Vector4 limbScale;
        [Header("号码牌缩放")]
        public Vector4 numberScale;
        [Header("身体缩放")]
        public Vector4 bodyScale;
        [Header("鞋子缩放")]
        public Vector4 shoesScale;

        [Header("护手偏移")]
        public Vector3 armguardOffset = Vector3.zero;
        [Header("护手缩放")]
        public Vector4 armguardScale = Vector4.one;
        [Header("头带偏移")]
        public Vector3 headbandOffset = Vector3.zero;
        [Header("头带缩放")]
        public Vector3 headbandScale = Vector3.one;
        [Header("护膝偏移")]
        public Vector3 kneeguardOffset = Vector3.zero;
        [Header("护膝缩放")]
        public Vector4 kneeguardScale = Vector4.one;

        public Color limb_transmissionColor = Color.black;

        public BodyMapPath headMap = new BodyMapPath();
        public BodyMapPath limbMap = new BodyMapPath();
        public BodyMapPath numberMap = new BodyMapPath();
        public BodyMapPath bodyMap = new BodyMapPath();
        public BodyMapPath shoesMap = new BodyMapPath();
    }
    [System.Serializable]
    public class BodyMapPath
    {
        public string albedoMap;
        public string normalMap;
        public string metallic;
    }

    [CreateAssetMenu(fileName = "Body Config", menuName = "UV/ Body Config", order = 2)]
    public class BodyConfig : ScriptableObject
    {
        [HideInInspector] public BodyInfo[] list = new BodyInfo[0];


        #region Public Methods

        public bool Get(int id,out BodyInfo bodyInfo)
        {
            if (list == null)
            {
                bodyInfo = null;
                return false;
            }
            bodyInfo = System.Array.Find(list, (item) => { return item.id == id; });
            return bodyInfo != null;
        }
        public void RefreshValuesWithTable()
        {
            List<PlayerObject> cfgs = DBMgr.g_Ins.V_PlayerConfig.GetAllPlayerObjectInEditor();
            BodyInfo[] newList = new BodyInfo[cfgs.Count];
            for (int i = 0; i < newList.Length; i++)
            {
                newList[i] = new BodyInfo();
                newList[i].id = cfgs[i].playerId;

                BodyInfo oldItem = null;
                if (list != null && list.Length > 0)
                {
                    oldItem = Array.Find(list, (item) => { return item.id == newList[i].id; });
                }

                if (oldItem != null)
                {
                    newList[i].headScale = oldItem.headScale;
                    newList[i].headOffset = oldItem.headOffset;
                    newList[i].limbScale = oldItem.limbScale;
                    newList[i].numberScale = oldItem.numberScale;
                    newList[i].bodyScale = oldItem.bodyScale;
                    newList[i].shoesScale = oldItem.shoesScale;
                    newList[i].headMap = oldItem.headMap;
                    newList[i].limbMap = oldItem.limbMap;
                    newList[i].numberMap = oldItem.numberMap;
                    newList[i].bodyMap = oldItem.bodyMap;
                    newList[i].shoesMap = oldItem.shoesMap;

                    newList[i].headbandOffset = oldItem.headbandOffset;
                    newList[i].headbandScale = oldItem.headbandScale;
                    newList[i].armguardOffset = oldItem.armguardOffset;
                    newList[i].armguardScale = oldItem.armguardScale;
                    newList[i].kneeguardOffset = oldItem.kneeguardOffset;
                    newList[i].kneeguardScale = oldItem.kneeguardScale;
                    newList[i].limb_transmissionColor = oldItem.limb_transmissionColor;
                }
                else
                {
                    newList[i].headScale = Vector3.one;
                    newList[i].headOffset = Vector3.zero;
                    newList[i].limbScale = Vector4.one;
                    newList[i].numberScale = Vector4.one;
                    newList[i].bodyScale = Vector4.one;
                    newList[i].shoesScale = Vector4.one;

                    newList[i].headbandOffset = Vector3.zero;
                    newList[i].headbandScale = Vector3.one;
                    newList[i].armguardOffset = Vector3.zero;
                    newList[i].armguardScale = Vector4.one;
                    newList[i].kneeguardOffset = Vector3.zero;
                    newList[i].kneeguardScale = Vector4.one;
                    newList[i].limb_transmissionColor = Color.black;

                    Debug.Log("id:" + newList[i].id + "is empty data");
                }
            }
            list = new BodyInfo[newList.Length];
            System.Array.Copy(newList, list, newList.Length);
        }
        #endregion
    }
#endif
}


