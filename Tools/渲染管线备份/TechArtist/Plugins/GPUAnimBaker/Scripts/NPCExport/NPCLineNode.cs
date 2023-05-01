using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidneNodeInfo
{
#if UNITY_EDITOR
    public string nodeName;
    public int childCount;
#endif

    public string[] matName;
    public MeshFilter[] mesh;
}

public class NPCLineNode : MonoBehaviour
{
#if UNITY_EDITOR
    public LidneNodeInfo LineNodeInfomation;

    public LidneNodeInfo GetLinewNodeInfomation()
    {
        LineNodeInfomation = new LidneNodeInfo();

        LineNodeInfomation.childCount = transform.childCount;
        LineNodeInfomation.nodeName = gameObject.name;

        LineNodeInfomation.matName = new string[transform.childCount];
        LineNodeInfomation.mesh = new MeshFilter[transform.childCount];

        Transform tmpTrans;
        MeshFilter tmpMesh;
        MeshRenderer tmpRender;
        for (int i = 0; i < transform.childCount; i++)
        {
            tmpTrans = transform.GetChild(i);
            tmpMesh = tmpTrans.GetComponent<MeshFilter>();

            if (tmpMesh == null) 
            {
                Debug.Log(tmpTrans.name);
            }

            tmpRender = tmpTrans.GetComponent<MeshRenderer>();

            LineNodeInfomation.matName[i] = tmpRender.sharedMaterials[0].name;
            LineNodeInfomation.mesh[i] = tmpMesh;
        }

        return LineNodeInfomation;
    }
#endif
}
