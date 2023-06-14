#ifndef UNIVERSAL_SKIN_SSS_INPUT_INCLUDED
#define UNIVERSAL_SKIN_SSS_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
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

 //SSS
half _SSSPower;
half _SSSFactor;
half4 _SSSColor;

//Rim
half _RimSmoothness;
half _RimWidth;
half4 _RimColor;

//Transmission
half _TransmissionWidth;
half _TransmissionRamp;
half _TransmissionIndencity;
half4 _TransmissionColor;
CBUFFER_END

TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_SSSLutMap);          SAMPLER(sampler_SSSLutMap);
TEXTURE2D(_RimMaskMap);          SAMPLER(sampler_RimMaskMap);

#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

#define SAMPLE_SSSLUTMAP(uv) SAMPLE_TEXTURE2D(_SSSLutMap, sampler_SSSLutMap, uv)
#define SAMPLE_RIMMASKMAP(uv) SAMPLE_TEXTURE2D(_RimMaskMap, sampler_RimMaskMap, uv)

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

void ApplyPerPixelDisplacement(half3 viewDirTS, inout float2 uv)
{
#if defined(_PARALLAXMAP)
    uv += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap, sampler_ParallaxMap), viewDirTS, _Parallax, uv);
#endif
}

// Used for scaling detail albedo. Main features:
// - Depending if detailAlbedo brightens or darkens, scale magnifies effect.
// - No effect is applied if detailAlbedo is 0.5.
half3 ScaleDetailAlbedo(half3 detailAlbedo, half scale)
{
    // detailAlbedo = detailAlbedo * 2.0h - 1.0h;
    // detailAlbedo *= _DetailAlbedoMapScale;
    // detailAlbedo = detailAlbedo * 0.5h + 0.5h;
    // return detailAlbedo * 2.0f;

    // A bit more optimized
    return 2.0h * detailAlbedo * scale - scale + 1.0h;
}

half3 ApplyDetailAlbedo(float2 detailUv, half3 albedo, half detailMask)
{
#if defined(_DETAIL)
    half3 detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedoMap, sampler_DetailAlbedoMap, detailUv).rgb;

    // In order to have same performance as builtin, we do scaling only if scale is not 1.0 (Scaled version has 6 additional instructions)
#if defined(_DETAIL_SCALED)
    detailAlbedo = ScaleDetailAlbedo(detailAlbedo, _DetailAlbedoMapScale);
#else
    detailAlbedo = 2.0h * detailAlbedo;
#endif

    return albedo * LerpWhiteTo(detailAlbedo, detailMask);
#else
    return albedo;
#endif
}

half3 ApplyDetailNormal(float2 detailUv, half3 normalTS, half detailMask)
{
#if defined(_DETAIL)
#if BUMP_SCALE_NOT_SUPPORTED
    half3 detailNormalTS = UnpackNormal(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailUv));
#else
    half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailUv), _DetailNormalMapScale);
#endif

    // With UNITY_NO_DXT5nm unpacked vector is not normalized for BlendNormalRNM
    // For visual consistancy we going to do in all cases
    detailNormalTS = normalize(detailNormalTS);

    return lerp(normalTS, BlendNormalRNM(normalTS, detailNormalTS), detailMask); // todo: detailMask should lerp the angle of the quaternion rotation, not the normals
#else
    return normalTS;
#endif
}

inline void InitializeStandardLitSurfaceData(float2 uv, out SkinSurfaceData outSkinSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSkinSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    outSkinSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);

    outSkinSurfaceData.metallic = 0.0h;
    outSkinSurfaceData.thickness = specGloss.r;

    outSkinSurfaceData.smoothness = specGloss.a;

    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
    outSkinSurfaceData.normalTS = UnpackNormal(n);
    //outSkinSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

#if defined(SHADER_API_GLES)
    outSkinSurfaceData.occlusion = specGloss.g;
#else
    outSkinSurfaceData.occlusion = LerpWhiteTo(specGloss.g , _OcclusionStrength);
#endif

    outSkinSurfaceData.emission = 0.0h;//SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));

#if defined(_DETAIL)
    half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
    float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
    outSkinSurfaceData.albedo = ApplyDetailAlbedo(detailUv, outSkinSurfaceData.albedo, detailMask);
    outSkinSurfaceData.normalTS = ApplyDetailNormal(detailUv, outSkinSurfaceData.normalTS, detailMask);

#endif
}

half3 CalculateSSSTerm(half NdotL)
{
    half3 sssTerm = SAMPLE_SSSLUTMAP(half2((NdotL * 0.5 + 0.5), _SSSFactor)).rgb;
    sssTerm.r = saturate(pow(sssTerm.r, _SSSPower * (1 - _SSSColor.r)));
    sssTerm.g = saturate(pow(sssTerm.g, _SSSPower * (1 - _SSSColor.g)));
    sssTerm.b = saturate(pow(sssTerm.b, _SSSPower * (1 - _SSSColor.b)));
    return sssTerm;
}

half3 CalculateRimTerm(half3 viewDirWS, half3 normalWS, half2 uv)
{ 
    half rim = 1.0 - saturate(dot(viewDirWS, normalWS));
    rim = smoothstep(1-_RimWidth, 1, rim); 
    rim = smoothstep(0, _RimSmoothness, rim); 
    half3 rimCol = rim * _RimColor.rgb; 
    rimCol *= SAMPLE_RIMMASKMAP(uv).r; 
    return rimCol;
}

half3 CalculateTransmissionTerm(Light light, half3 normalWS, half3 viewDirWS, half thickness, half occlusion)
{
    half NdotLNoClamped = dot(normalWS, light.direction);
    half3 H = normalize(light.direction) + normalWS * _TransmissionWidth;
    half3 V = viewDirWS;
    half transDot = pow(saturate(dot(-H, V)), _TransmissionRamp) *_TransmissionIndencity;
    half3 transmission = saturate(transDot * thickness * light.shadowAttenuation * (1-saturate(NdotLNoClamped)) * light.color * _TransmissionColor.rgb * occlusion);

    return transmission;
}


#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
