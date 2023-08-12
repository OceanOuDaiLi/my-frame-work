using UnityEngine;

[ExecuteInEditMode]
public class PandaPostProcess : MonoBehaviour
{
    public Material PostProcessMat;

    public float MainAlpha = 1F;
    public float BlurFactor;
    public float LineUVScale;
    public float Chromatic;
    public float Frequency;
    public float Amplitude;
    public float VignettePower = 1.5F;
    public float VignetteScale = 1.5F;
    private void Awake()
    {
        if (PostProcessMat == null)
        {
            enabled = false;
        }
        else
        {

            PostProcessMat.mainTexture = PostProcessMat.mainTexture;
        }

    }

    //void Start()
    //{
    //    var BlurFactor = PostProcessMat.GetFloat("_BlurFactorK");
    //}



    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        PostProcessMat.SetFloat("_BlurFactorK", BlurFactor);

        PostProcessMat.SetFloat("_LineUVScaleK", LineUVScale);

        PostProcessMat.SetFloat("_MainAlphaK", MainAlpha);

        PostProcessMat.SetFloat("_zhenpinK", Frequency);

        PostProcessMat.SetFloat("_zhenfuK", Amplitude);
        PostProcessMat.SetFloat("_RedBlueFactorK", Chromatic);
        PostProcessMat.SetFloat("_VignettePowerK", VignettePower);
        PostProcessMat.SetFloat("_VignetteScaleK", VignetteScale);
        Graphics.Blit(src, dest, PostProcessMat);
    }
}
