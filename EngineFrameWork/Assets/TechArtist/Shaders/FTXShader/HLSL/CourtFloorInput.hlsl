#ifndef UNIVERSAL_COURT_FLOOR_INPUT_INCLUDED
#define UNIVERSAL_COURT_FLOOR_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "HLSL/FTXInput.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseColor;

#ifndef _DARKFLOOR_OFF
half4 _DarkColor;
half _DarkIntensity;
half4 _DarkMap_ST;
#endif

half4 _Base2Map_ST;
half4 _Base2Color;

half _Smoothness;
half _Metallic;
half _BumpScale;
half _Surface;
CBUFFER_END

TEXTURE2D(_Base2Map);           SAMPLER(sampler_Base2Map);
TEXTURE2D(_SmoothnessMap);      SAMPLER(sampler_SmoothnessMap);
#ifndef _DARKFLOOR_OFF
TEXTURE2D(_DarkMap);            SAMPLER(sampler_DarkMap);
#endif


#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;
    specGloss.rgb = _Metallic.rrr;
    specGloss.a = _Smoothness * SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, uv).r;

    return specGloss;
}

half SampleOcclusion(float2 uv)
{
#ifdef _OCCLUSIONMAP
// TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
#if defined(SHADER_API_GLES)
    return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
#else
    half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
    return LerpWhiteTo(occ, _OcclusionStrength);
#endif
#else
    return 1.0;
#endif
}

inline void InitializeStandardLitSurfaceData(half4 uv, half2 uvDark, out FTXSurfaceData outFTXSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv.xy, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outFTXSurfaceData.alpha = albedoAlpha.a * _BaseColor.a;

#ifndef _DARKFLOOR_OFF
    half dark = SAMPLE_TEXTURE2D(_DarkMap , sampler_DarkMap , uvDark);
    dark = (1 - dark) * (1 - _DarkIntensity)  + dark;
    half3 darkColor = dark *  (half3(1,1,1) - _DarkColor) + _DarkColor;
    albedoAlpha.rgb *= darkColor;
#endif    

    half4 albedo2Alpha = SAMPLE_TEXTURE2D(_Base2Map , sampler_Base2Map , uv.zw);
    half alpha2 = albedo2Alpha.a * _Base2Color.a;

    half4 specGloss = SampleMetallicSpecGloss(uv.zw, albedoAlpha.a);
    outFTXSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb * (1 - alpha2) + albedo2Alpha.rgb * _Base2Color.rgb * alpha2;

#if _SPECULAR_SETUP
    outFTXSurfaceData.metallic = 1.0h;
    //outFTXSurfaceData.specular = specGloss.rgb;
#else
    outFTXSurfaceData.metallic = specGloss.r;
    //outFTXSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outFTXSurfaceData.smoothness = specGloss.a;

    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
    outFTXSurfaceData.normalTS = UnpackNormalScale(n, _BumpScale);
    //outFTXSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outFTXSurfaceData.occlusion = 1.0h;//SampleOcclusion(uv);
    outFTXSurfaceData.emission = 0.0h;
}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
