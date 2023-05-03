using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SSSSSRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class SSSSSRenderSettings
    {
        [Range(0, 1)]
        public float downscale = 1.0f;
        [Range(1, 3)]
        public int blurIterations = 1;
        [Range(0.01f, 1.6f)]
        public float scatterDistance = 0.4f;
        [Range(0f, 2f)]
        public float scatterIntensity = 1f;
        [Range(0f, 1f)]
        public float affectDirect = 0.5f;
        public Color SSSColor = Color.red;
    }
    public SSSSSRenderSettings sssssRenderSettings = new SSSSSRenderSettings();

    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingTransparents;
    SSSSSRenderPass sssssPass;

    public override void Create()
    {
        sssssPass = new SSSSSRenderPass(sssssRenderSettings, Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var cam = renderingData.cameraData.camera;
        sssssPass.SetUp(sssssRenderSettings, cam);
        renderer.EnqueuePass(sssssPass);
    }
}

public class SSSSSRenderPass : ScriptableRenderPass
{
    private SSSSSRenderFeature.SSSSSRenderSettings sssssRenderSettings;
    private Material SSSSSMat;
    private Material copyMat;
    private const string NameOfCommandBuffer = "SSSSS Render Feature";
    private RenderTargetIdentifier depth { get; set; }
    private Camera camera { get; set; }
    Rect rect;
    static readonly int blurRT1 = Shader.PropertyToID("_SSSSSBlur1");
    static readonly int blurRT2 = Shader.PropertyToID("_SSSSSBlur2");
    //static readonly int src = Shader.PropertyToID("_SSSSSSource");
    static readonly int colorTexture = Shader.PropertyToID("_CameraColorTexture");

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(SSSSSMat == null || copyMat == null)
            return;

        var cmd = CommandBufferPool.Get(NameOfCommandBuffer);
        cmd.Clear();
        SSSSSPass(cmd);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void SSSSSPass(CommandBuffer cmd)
    {
        int width = (int)(Screen.width * sssssRenderSettings.downscale);
        int height = (int)(Screen.height * sssssRenderSettings.downscale);

        cmd.GetTemporaryRT(blurRT1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
        cmd.GetTemporaryRT(blurRT2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.RGB111110Float);

        cmd.Blit(colorTexture, blurRT2 ,copyMat ,0);

        //multipass pass blur
        for (int k = 1; k <= sssssRenderSettings.blurIterations; k++)
        {
            cmd.SetGlobalFloat("_BlurStr", Mathf.Clamp01(sssssRenderSettings.scatterDistance * 0.12f - k * 0.02f));
            cmd.SetGlobalVector("_BlurVec", new Vector4(1, 0, 0, 0));

            DrawTri(cmd, blurRT2, blurRT1, SSSSSMat, 0);
            cmd.SetGlobalVector("_BlurVec", new Vector4(0, 1, 0, 0));
            DrawTri(cmd, blurRT1, blurRT2, SSSSSMat, 0);
            cmd.SetGlobalVector("_BlurVec", new Vector4(1, 1, 0, 0).normalized);
            DrawTri(cmd, blurRT2, blurRT1, SSSSSMat, 0);
            cmd.SetGlobalVector("_BlurVec", new Vector4(-1, 1, 0, 0).normalized);
            DrawTri(cmd, blurRT1, blurRT2, SSSSSMat, 0);
        }
        cmd.SetGlobalFloat("_EffectStr", sssssRenderSettings.scatterIntensity);
        cmd.SetGlobalFloat("_PreserveOriginal", 1 - sssssRenderSettings.affectDirect);       
        cmd.SetGlobalColor("_SSSSSColor", sssssRenderSettings.SSSColor);
        BlitSp(camera, cmd, blurRT2, colorTexture, depth, SSSSSMat, 1);

        cmd.ReleaseTemporaryRT(blurRT1);
        cmd.ReleaseTemporaryRT(blurRT2);
    }

    public SSSSSRenderPass(SSSSSRenderFeature.SSSSSRenderSettings setings, RenderPassEvent renderPassEvent)
    {
        sssssRenderSettings = setings;
        SSSSSMat = new Material(Shader.Find("Hidden/SSSSSShader"));
        copyMat = new Material(Shader.Find("Hidden/BlitCopy"));
        this.renderPassEvent = renderPassEvent;
        rect = new Rect(0,0, Screen.width, Screen.height);
        this.depth = Shader.PropertyToID("_CameraDepthAttachment");
    }

    public void SetUp(SSSSSRenderFeature.SSSSSRenderSettings setings, Camera camera)
    {
        sssssRenderSettings = setings;
        this.camera = camera;
    }

    private void BlitSp(Camera camera, CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest,
            RenderTargetIdentifier depth, Material mat, int passIndex, MaterialPropertyBlock mpb = null)
    {
        cmd.SetGlobalTexture("_BlurTex", source);
        cmd.SetRenderTarget(dest, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
            depth, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        if(rect.width != Screen.width && rect.height != Screen.height)
        {
            rect = new Rect(0, 0, Screen.width, Screen.height);
        }
        cmd.SetViewport(rect);
        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat, 0, passIndex, mpb);
        cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        //CoreUtils.Swap(ref source, ref dest);
    }

    /// <summary>
    /// 绘制三角形面片，可减少Frag开销
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="from"></param>
    /// <param name="target"></param>
    /// <param name="mat"></param>
    /// <param name="pass"></param>
    private void DrawTri(CommandBuffer cmd, RenderTargetIdentifier from, RenderTargetIdentifier target, Material mat, int pass = -1)
    {
        cmd.SetGlobalTexture("_MainTex", from);
        cmd.SetRenderTarget(target, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        cmd.DrawProcedural(Matrix4x4.identity, mat, pass, MeshTopology.Triangles, 3);
    }
}

