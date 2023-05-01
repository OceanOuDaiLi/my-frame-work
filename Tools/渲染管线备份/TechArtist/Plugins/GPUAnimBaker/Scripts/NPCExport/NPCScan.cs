using System.Collections.Generic;
using UnityEngine;

public class NPCScan : MonoBehaviour
{

#if UNITY_EDITOR
    public List<NPCLineNode> LineNoeds = new List<NPCLineNode>();

    //public List<NpcNode> NpcNodes = new List<NpcNode>();

    //public Material RuntimeMat = null;


    public NpcBatchingConfig npcBatchingConfig;

    public GameObject NpcRenderNode;

    public Mesh BoyOriMesh;
    public Mesh GrilOriMesh;


    [Space(5)]
    public Transform GirlType_1_Idle;
    public Transform GirlType_1_Idle_Cheer;
    public Transform GirlType_1_Sit;
    public Transform GirlType_1_Sit2;
    [Space(5)]
    public Transform GirlType_2_Idle;
    public Transform GirlType_2_Idle_Cheer;
    public Transform GirlType_2_Sit;
    public Transform GirlType_2_Sit2;
    [Space(5)]
    public Transform GirlType_3_Idle;
    public Transform GirlType_3_Idle_Cheer;
    public Transform GirlType_3_Sit;
    public Transform GirlType_3_Sit2;
    [Space(5)]
    public Transform GirlType_4_Idle;
    public Transform GirlType_4_Idle_Cheer;
    public Transform GirlType_4_Sit;
    public Transform GirlType_4_Sit2;
    //渲染指定女npc的空节点
    [HideInInspector]
    public List<Transform> GirlType_1_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_1_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_1_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_1_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_2_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_2_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_2_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_2_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_3_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_3_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_3_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_3_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_4_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_4_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_4_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> GirlType_4_Sit2_Trans = new List<Transform>();


    [Space(5)]
    public Transform BoyType_1_Idle;
    public Transform BoyType_1_Idle_Cheer;
    public Transform BoyType_1_Idle_Cheer2;
    public Transform BoyType_1_Sit;
    public Transform BoyType_1_Sit2;
    public Transform BoyType_1_Sit3;
    [Space(5)]
    public Transform BoyType_2_Idle;
    public Transform BoyType_2_Idle_Cheer;
    public Transform BoyType_2_Idle_Cheer2;
    public Transform BoyType_2_Sit;
    public Transform BoyType_2_Sit2;
    public Transform BoyType_2_Sit3;
    [Space(5)]
    public Transform BoyType_3_Idle;
    public Transform BoyType_3_Idle_Cheer;
    public Transform BoyType_3_Idle_Cheer2;
    public Transform BoyType_3_Sit;
    public Transform BoyType_3_Sit2;
    public Transform BoyType_3_Sit3;
    [Space(5)]
    public Transform BoyType_4_Idle;
    public Transform BoyType_4_Idle_Cheer;
    public Transform BoyType_4_Idle_Cheer2;
    public Transform BoyType_4_Sit;
    public Transform BoyType_4_Sit2;
    public Transform BoyType_4_Sit3;

    //渲染指定男npc的空节点
    [HideInInspector]
    public List<Transform> BoyType_1_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_1_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_1_Idle_Cheer2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_1_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_1_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_1_Sit3_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Idle_Cheer2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_2_Sit3_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Idle_Cheer2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_3_Sit3_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Idle_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Idle_Cheer_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Idle_Cheer2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Sit_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Sit2_Trans = new List<Transform>();
    [HideInInspector]
    public List<Transform> BoyType_4_Sit3_Trans = new List<Transform>();
#endif

    public void NpcSwitchCheer()
    {
        //Todo:
        //Switching 'Sit' AnimationMap to 'Idle_Cheer' AnimationMap
    }
}
