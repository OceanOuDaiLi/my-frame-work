#ifndef UNIVERSAL_PLAYER_INPUT_INCLUDED
#define UNIVERSAL_PLAYER_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "HLSL/FTXInput.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseColor;

half _Cutoff;
half _Metallic;
half _Smoothness;
half _OcclusionStrength;
half _Surface;

half3 _PlayerReflectionParameter; // half2(sin, cos, Strength)
CBUFFER_END

TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);

TEXTURECUBE(_PlayerReflection); SAMPLER(sampler_PlayerReflection);

#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;

#ifdef _METALLICSPECGLOSSMAP
    specGloss = SAMPLE_METALLICSPECULAR(uv);
    specGloss.a *= _Smoothness;
#else // _METALLICSPECGLOSSMAP
    specGloss.rgb = _Metallic.rrr;
    specGloss.a = _Smoothness;
#endif

    return specGloss;
}

inline half3 SamplePlayerReflection(float3 normalWS)
{
    half2x2 m = float2x2(_PlayerReflectionParameter.y, -_PlayerReflectionParameter.x, _PlayerReflectionParameter.x, _PlayerReflectionParameter.y);
    half3 uv = half3(mul(m, normalWS.xz), normalWS.y).xzy;
    return SAMPLE_TEXTURECUBE(_PlayerReflection, sampler_PlayerReflection, uv).rgb * _PlayerReflectionParameter.z;
}

inline void InitializeStandardLitSurfaceData(float2 uv, out FTXSurfaceData outFTXSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outFTXSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    outFTXSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);

    outFTXSurfaceData.metallic = 0.0h;

    outFTXSurfaceData.smoothness = specGloss.a;

    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
    outFTXSurfaceData.normalTS = UnpackNormal(n);
    //outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

#if defined(SHADER_API_GLES)
    outFTXSurfaceData.occlusion = specGloss.g;
#else
    outFTXSurfaceData.occlusion = LerpWhiteTo(specGloss.g , _OcclusionStrength);
#endif

    outFTXSurfaceData.emission = 0.0h;//SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
