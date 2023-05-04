using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal.Internal
{
    // Note: this pass can't be done at the same time as post-processing as it needs to be done in
    // advance in case we're doing on-tile color grading.
    /// <summary>
    /// Renders a color grading LUT texture.
    /// </summary>
    public class ColorGradingLutPass : ScriptableRenderPass
    {
        readonly Material m_LutBuilderLdr;
        readonly Material m_LutBuilderHdr;
        readonly GraphicsFormat m_HdrLutFormat;
        readonly GraphicsFormat m_LdrLutFormat;

        RenderTargetHandle m_InternalLut;
        bool m_ColorGradingInShader;
        
        CachedParams m_LastParams = new CachedParams();
        CachedParams m_Params = new CachedParams();
        RenderTexture m_Cache;

        public ColorGradingLutPass(RenderPassEvent evt, PostProcessData data)
        {
            base.profilingSampler = new ProfilingSampler(nameof(ColorGradingLutPass));
            renderPassEvent = evt;
            overrideCameraTarget = true;

            Material Load(Shader shader)
            {
                if (shader == null)
                {
                    Debug.LogError($"Missing shader. {GetType().DeclaringType.Name} render pass will not execute. Check for missing reference in the renderer resources.");
                    return null;
                }

                return CoreUtils.CreateEngineMaterial(shader);
            }

            m_LutBuilderLdr = Load(data.shaders.lutBuilderLdrPS);
            m_LutBuilderHdr = Load(data.shaders.lutBuilderHdrPS);

            // Warm up lut format as IsFormatSupported adds GC pressure...
            const FormatUsage kFlags = FormatUsage.Linear | FormatUsage.Render;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, kFlags))
                m_HdrLutFormat = GraphicsFormat.R16G16B16A16_SFloat;
            else if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, kFlags))
                m_HdrLutFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            else
                // Obviously using this for log lut encoding is a very bad idea for precision but we
                // need it for compatibility reasons and avoid black screens on platforms that don't
                // support floating point formats. Expect banding and posterization artifact if this
                // ends up being used.
                m_HdrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;

            m_LdrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;
        }

        public void Setup(in RenderTargetHandle internalLut, bool colorGradingInShader = false)
        {
            m_InternalLut = internalLut;
            m_ColorGradingInShader = colorGradingInShader;
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.ColorGradingLUT)))
            {
                // Fetch all color grading settings
                var stack = VolumeManager.instance.stack;
                var channelMixer = stack.GetComponent<ChannelMixer>();
                var colorAdjustments = stack.GetComponent<ColorAdjustments>();
                var curves = stack.GetComponent<ColorCurves>();
                var liftGammaGain = stack.GetComponent<LiftGammaGain>();
                var shadowsMidtonesHighlights = stack.GetComponent<ShadowsMidtonesHighlights>();
                var splitToning = stack.GetComponent<SplitToning>();
                var tonemapping = stack.GetComponent<Tonemapping>();
                var whiteBalance = stack.GetComponent<WhiteBalance>();

                ref var postProcessingData = ref renderingData.postProcessingData;
                bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;

                // Prepare texture & material
                int lutHeight = postProcessingData.lutSize;
                int lutWidth = lutHeight * lutHeight;
                var format = hdr ? m_HdrLutFormat : m_LdrLutFormat;
                var material = hdr ? m_LutBuilderHdr : m_LutBuilderLdr;
                var desc = new RenderTextureDescriptor(lutWidth, lutHeight, format, 0);
                desc.vrUsage = VRTextureUsage.None; // We only need one for both eyes in VR
                cmd.GetTemporaryRT(m_InternalLut.id, desc, FilterMode.Bilinear);

                // Prepare data
                var lmsColorBalance = ColorUtils.ColorBalanceToLMSCoeffs(whiteBalance.temperature.value, whiteBalance.tint.value);
                var hueSatCon = new Vector4(colorAdjustments.hueShift.value / 360f, colorAdjustments.saturation.value / 100f + 1f, colorAdjustments.contrast.value / 100f + 1f, 0f);
                var channelMixerR = new Vector4(channelMixer.redOutRedIn.value / 100f, channelMixer.redOutGreenIn.value / 100f, channelMixer.redOutBlueIn.value / 100f, 0f);
                var channelMixerG = new Vector4(channelMixer.greenOutRedIn.value / 100f, channelMixer.greenOutGreenIn.value / 100f, channelMixer.greenOutBlueIn.value / 100f, 0f);
                var channelMixerB = new Vector4(channelMixer.blueOutRedIn.value / 100f, channelMixer.blueOutGreenIn.value / 100f, channelMixer.blueOutBlueIn.value / 100f, 0f);

                var shadowsHighlightsLimits = new Vector4(
                    shadowsMidtonesHighlights.shadowsStart.value,
                    shadowsMidtonesHighlights.shadowsEnd.value,
                    shadowsMidtonesHighlights.highlightsStart.value,
                    shadowsMidtonesHighlights.highlightsEnd.value
                );

                var (shadows, midtones, highlights) = ColorUtils.PrepareShadowsMidtonesHighlights(
                    shadowsMidtonesHighlights.shadows.value,
                    shadowsMidtonesHighlights.midtones.value,
                    shadowsMidtonesHighlights.highlights.value
                );

                var (lift, gamma, gain) = ColorUtils.PrepareLiftGammaGain(
                    liftGammaGain.lift.value,
                    liftGammaGain.gamma.value,
                    liftGammaGain.gain.value
                );

                var (splitShadows, splitHighlights) = ColorUtils.PrepareSplitToning(
                    splitToning.shadows.value,
                    splitToning.highlights.value,
                    splitToning.balance.value
                );

                var lutParameters = new Vector4(lutHeight, 0.5f / lutWidth, 0.5f / lutHeight,
                    lutHeight / (lutHeight - 1f));

                // Fill in constants
                material.SetVector(ShaderConstants._Lut_Params, lutParameters);
                material.SetVector(ShaderConstants._ColorBalance, lmsColorBalance);
                material.SetVector(ShaderConstants._ColorFilter, colorAdjustments.colorFilter.value.linear);
                material.SetVector(ShaderConstants._ChannelMixerRed, channelMixerR);
                material.SetVector(ShaderConstants._ChannelMixerGreen, channelMixerG);
                material.SetVector(ShaderConstants._ChannelMixerBlue, channelMixerB);
                material.SetVector(ShaderConstants._HueSatCon, hueSatCon);
                material.SetVector(ShaderConstants._Lift, lift);
                material.SetVector(ShaderConstants._Gamma, gamma);
                material.SetVector(ShaderConstants._Gain, gain);
                material.SetVector(ShaderConstants._Shadows, shadows);
                material.SetVector(ShaderConstants._Midtones, midtones);
                material.SetVector(ShaderConstants._Highlights, highlights);
                material.SetVector(ShaderConstants._ShaHiLimits, shadowsHighlightsLimits);
                material.SetVector(ShaderConstants._SplitShadows, splitShadows);
                material.SetVector(ShaderConstants._SplitHighlights, splitHighlights);

                // YRGB curves
                material.SetTexture(ShaderConstants._CurveMaster, curves.master.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveRed, curves.red.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveGreen, curves.green.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveBlue, curves.blue.value.GetTexture());

                // Secondary curves
                material.SetTexture(ShaderConstants._CurveHueVsHue, curves.hueVsHue.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveHueVsSat, curves.hueVsSat.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveLumVsSat, curves.lumVsSat.value.GetTexture());
                material.SetTexture(ShaderConstants._CurveSatVsSat, curves.satVsSat.value.GetTexture());

                // Tonemapping (baked into the lut for HDR)
                if (hdr)
                {
                    material.shaderKeywords = null;

                    switch (tonemapping.mode.value)
                    {
                        case TonemappingMode.Neutral: material.EnableKeyword(ShaderKeywordStrings.TonemapNeutral); break;
                        case TonemappingMode.ACES: material.EnableKeyword(ShaderKeywordStrings.TonemapACES); break;
                        default: break; // None
                    }
                }

                renderingData.cameraData.xr.StopSinglePass(cmd);

                m_Params.lutSize = postProcessingData.lutSize;
                m_Params.hdr = hdr;
                m_Params.tonemappingMode = tonemapping.mode.value;
                m_Params.data[0] = lutParameters;
                m_Params.data[1] = lmsColorBalance;
                m_Params.data[2] = colorAdjustments.colorFilter.value.linear;
                m_Params.data[3] = channelMixerR;
                m_Params.data[4] = channelMixerG;
                m_Params.data[5] = channelMixerB;
                m_Params.data[6] = hueSatCon;
                m_Params.data[7] = lift;
                m_Params.data[8] = gamma;
                m_Params.data[9] = gain;
                m_Params.data[10] = shadows;
                m_Params.data[11] = midtones;
                m_Params.data[12] = highlights;
                m_Params.data[13] = shadowsHighlightsLimits;
                m_Params.data[14] = splitShadows;
                m_Params.data[15] = splitHighlights;
                
                if (m_LastParams.Update(m_Params))
                {
                    // Render the lut
                    Blit(cmd, m_InternalLut.id, m_InternalLut.id, material);

                    if (m_Cache != null && m_Cache.IsCreated())
                    {
                        m_Cache.Release();
                    }
                    m_Cache = new RenderTexture(desc);
                    m_Cache.filterMode = FilterMode.Bilinear;
                    m_Cache.Create();
                    
                    ScriptableRenderer.ConfigureActiveTarget(m_Cache, BuiltinRenderTextureType.CameraTarget);
                    cmd.Blit(m_InternalLut.id, m_Cache);
                }
                else
                {
                    ScriptableRenderer.ConfigureActiveTarget(m_InternalLut.id, BuiltinRenderTextureType.CameraTarget);
                    cmd.Blit(m_Cache, m_InternalLut.id);
                }
                
                renderingData.cameraData.xr.StartSinglePass(cmd);
            }

#if !NO_DEBUG
            if (RenderThreadIncreaseTime > 0.0f)
            {
                cmd.IssuePluginEvent(pSleep, 0);
            }
#endif

            if (m_ColorGradingInShader)
            {
                SetupColorGrading(cmd, ref renderingData);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

#if !NO_DEBUG
        public static float RenderThreadIncreaseTime = -1.0f;
        static IntPtr pSleep = Marshal.GetFunctionPointerForDelegate(new Action<int>(Test));
        [MonoPInvokeCallback(typeof(Action<int>))]
        private static void Test(int eventID)
        {
            long timeStart = DateTime.Now.Ticks;
            float timeElapsed = 0.0f;
            while (timeElapsed < RenderThreadIncreaseTime)
            {
                timeElapsed = (DateTime.Now.Ticks - timeStart) / 10000.0f;
            }
        }
#endif
        
        void SetupColorGrading(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var postProcessingData = ref renderingData.postProcessingData;
            bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
            int lutHeight = postProcessingData.lutSize;
            int lutWidth = lutHeight * lutHeight;
            
            var stack = VolumeManager.instance.stack;
            var colorAdjustments = stack.GetComponent<ColorAdjustments>();
            var colorLookup = stack.GetComponent<ColorLookup>();
            var tonemapping = stack.GetComponent<Tonemapping>();
            
            cmd.SetGlobalFloat(ShaderConstants._ColorGradingInShader, 1);

            // Source material setup
            float postExposureLinear = Mathf.Pow(2f, colorAdjustments.postExposure.value);
            cmd.SetGlobalTexture(ShaderConstants._InternalLut, m_InternalLut.Identifier());
            cmd.SetGlobalVector(ShaderConstants._Lut_Params, new Vector4(1f / lutWidth, 1f / lutHeight, lutHeight - 1f, postExposureLinear));
            cmd.SetGlobalTexture(ShaderConstants._UserLut, colorLookup.texture.value);
            cmd.SetGlobalVector(ShaderConstants._UserLut_Params, !colorLookup.IsActive()
                ? Vector4.zero
                : new Vector4(1f / colorLookup.texture.value.width,
                    1f / colorLookup.texture.value.height,
                    colorLookup.texture.value.height - 1f,
                    colorLookup.contribution.value)
            );
        }

        /// <inheritdoc/>
        public override void OnFinishCameraStackRendering(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_InternalLut.id);
            cmd.SetGlobalFloat("_ColorGradingInShader", 0);
        }

        public void Cleanup()
        {
            CoreUtils.Destroy(m_LutBuilderLdr);
            CoreUtils.Destroy(m_LutBuilderHdr);

            if (m_Cache != null && m_Cache.IsCreated())
            {
                m_Cache.Release();
            }
        }

        // Precomputed shader ids to same some CPU cycles (mostly affects mobile)
        static class ShaderConstants
        {
            public static readonly int _Lut_Params        = Shader.PropertyToID("_Lut_Params");
            public static readonly int _ColorBalance      = Shader.PropertyToID("_ColorBalance");
            public static readonly int _ColorFilter       = Shader.PropertyToID("_ColorFilter");
            public static readonly int _ChannelMixerRed   = Shader.PropertyToID("_ChannelMixerRed");
            public static readonly int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");
            public static readonly int _ChannelMixerBlue  = Shader.PropertyToID("_ChannelMixerBlue");
            public static readonly int _HueSatCon         = Shader.PropertyToID("_HueSatCon");
            public static readonly int _Lift              = Shader.PropertyToID("_Lift");
            public static readonly int _Gamma             = Shader.PropertyToID("_Gamma");
            public static readonly int _Gain              = Shader.PropertyToID("_Gain");
            public static readonly int _Shadows           = Shader.PropertyToID("_Shadows");
            public static readonly int _Midtones          = Shader.PropertyToID("_Midtones");
            public static readonly int _Highlights        = Shader.PropertyToID("_Highlights");
            public static readonly int _ShaHiLimits       = Shader.PropertyToID("_ShaHiLimits");
            public static readonly int _SplitShadows      = Shader.PropertyToID("_SplitShadows");
            public static readonly int _SplitHighlights   = Shader.PropertyToID("_SplitHighlights");
            public static readonly int _CurveMaster       = Shader.PropertyToID("_CurveMaster");
            public static readonly int _CurveRed          = Shader.PropertyToID("_CurveRed");
            public static readonly int _CurveGreen        = Shader.PropertyToID("_CurveGreen");
            public static readonly int _CurveBlue         = Shader.PropertyToID("_CurveBlue");
            public static readonly int _CurveHueVsHue     = Shader.PropertyToID("_CurveHueVsHue");
            public static readonly int _CurveHueVsSat     = Shader.PropertyToID("_CurveHueVsSat");
            public static readonly int _CurveLumVsSat     = Shader.PropertyToID("_CurveLumVsSat");
            public static readonly int _CurveSatVsSat     = Shader.PropertyToID("_CurveSatVsSat");
            public static readonly int _ColorGradingInShader     = Shader.PropertyToID("_ColorGradingInShader");
            public static readonly int _InternalLut     = Shader.PropertyToID("_InternalLut");
            public static readonly int _UserLut     = Shader.PropertyToID("_UserLut");
            public static readonly int _UserLut_Params     = Shader.PropertyToID("_UserLut_Params");
        }

        class CachedParams
        {
            public int lutSize;
            public bool hdr;
            public TonemappingMode tonemappingMode;
            public Vector4[] data = new Vector4[16];

            public bool Update(CachedParams c)
            {
                bool changed = false;
                
                if (lutSize != c.lutSize)
                {
                    lutSize = c.lutSize;
                    changed = true;
                }
                
                if (hdr != c.hdr)
                {
                    hdr = c.hdr;
                    changed = true;
                }
                
                if (tonemappingMode != c.tonemappingMode)
                {
                    tonemappingMode = c.tonemappingMode;
                    changed = true;
                }

                for (int i = 0; i < data.Length; ++i)
                {
                    if (data[i] != c.data[i])
                    {
                        data[i] = c.data[i];
                        changed = true;
                    }
                }

                return changed;
            }
        }
    }
}
