using UnityEngine;

public class NpcNode : MonoBehaviour
{
    public Texture BaseMap;
    //[SerializeField] Texture2D NormalMap;
    public Texture AnimationMap;
    public MeshRenderer render;

    //public Material runtimeMat;
    //MaterialPropertyBlock block;


    //private void Awake()
    //{
    //    block = new MaterialPropertyBlock();
    //    render.GetPropertyBlock(block);
    //}
    //private void Start()
    //{
    //    block = new MaterialPropertyBlock();
    //    render.GetPropertyBlock(block);

    //    block.SetTexture("_MainTex", BaseMap);
    //    //block.SetTexture("_NorampMap", NormalMap);
    //    block.SetTexture("_AniMap", AnimationMap);
    //    render.SetPropertyBlock(block);
    //}

#if UNITY_EDITOR
    public void OnRefresh()
    {
        render = gameObject.GetComponent<MeshRenderer>();
        BaseMap = render.sharedMaterials[0].GetTexture("_MainTex");
        AnimationMap = render.sharedMaterials[0].GetTexture("_AniMap");

        render.sharedMaterials = new Material[1];
    }
#endif
}
