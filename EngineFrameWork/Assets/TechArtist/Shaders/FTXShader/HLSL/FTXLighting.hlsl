#ifndef UNIVERSAL_FTX_LIGHTING_INCLUDED
#define UNIVERSAL_FTX_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#ifdef UNIVERSAL_SKIN_SSS_INPUT_INCLUDED
half3 FTX_LightingPhysicallyBased_SKin(BRDFData brdfData, Light light, half3 normalWS, half3 viewDirectionWS, half thickness, half occlusion, half curvature, bool specularHighlightsOff)
{
    half NdotL = saturate(dot(normalWS, light.direction));
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation;// 没有 * NdotL，下面补上了
    
    half3 brdf = brdfData.diffuse * CalculateSSSTerm(NdotL, curvature);
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
        brdf += brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, light.direction, viewDirectionWS)  * NdotL * _SpecularHighlightIntensity;
    }
#endif // _SPECULARHIGHLIGHTS_OFF
    brdf *= radiance;

    brdf += CalculateTransmissionTerm(light, normalWS, viewDirectionWS, thickness ,occlusion);

    return brdf;    
}

half4 FTX_UniversalFragmentPBR_Skin(InputData inputData, SkinSurfaceData skinSurfaceData)
{
#ifdef _SPECULARHIGHLIGHTS_OFF
    bool specularHighlightsOff = true;
#else
    bool specularHighlightsOff = false;
#endif

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(skinSurfaceData.albedo, skinSurfaceData.metallic, half3(0.0h, 0.0h, 0.0h), skinSurfaceData.smoothness, skinSurfaceData.alpha, brdfData);

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
#elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
#else
    half4 shadowMask = half4(1, 1, 1, 1);
#endif

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

    #if defined(_SCREEN_SPACE_OCCLUSION)
        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
        mainLight.color *= aoFactor.directAmbientOcclusion;
        skinSurfaceData.occlusion = min(skinSurfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
    #endif

    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIllumination(brdfData, brdfData, 0.0h,
                                     inputData.bakedGI, skinSurfaceData.occlusion,
                                     inputData.normalWS, inputData.viewDirectionWS);
    color += FTX_LightingPhysicallyBased_SKin(brdfData, mainLight,
                                              inputData.normalWS, inputData.viewDirectionWS,
                                              skinSurfaceData.thickness, skinSurfaceData.occlusion, skinSurfaceData.curvature,
                                              specularHighlightsOff);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
        #if defined(_SCREEN_SPACE_OCCLUSION)
            light.color *= aoFactor.directAmbientOcclusion;
        #endif
        color += FTX_LightingPhysicallyBased_SKin(brdfData, light,
                                                  inputData.normalWS, inputData.viewDirectionWS,
                                                  skinSurfaceData.thickness, skinSurfaceData.occlusion, skinSurfaceData.curvature,
                                                  specularHighlightsOff);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += skinSurfaceData.emission;

    return half4(color, skinSurfaceData.alpha);
}
#endif

half3 FTX_LightingPhysicallyBased(BRDFData brdfData, Light light, half3 normalWS, half3 viewDirectionWS, bool specularHighlightsOff)
{
    half NdotL = saturate(dot(normalWS, light.direction));
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * NdotL;
    
    half3 brdf = brdfData.diffuse;
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
        brdf += brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, light.direction, viewDirectionWS);
    }
#endif // _SPECULARHIGHLIGHTS_OFF

    return brdf * radiance;    
}

#if defined (UNIVERSAL_PLAYER_INPUT_INCLUDED) || defined (UNIVERSAL_NUMBER_CLOTH_INPUT_INCLUDED)

half3 FTX_LightingPhysicallyBased_Play(BRDFData brdfData, Light light, half3 normalWS, half3 viewDirectionWS, bool specularHighlightsOff)
{
    half NdotL = saturate(dot(normalWS, light.direction));
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * (NdotL * (1.0h - _MainLightCompensation) + _MainLightCompensation);
    
    half3 brdf = brdfData.diffuse;
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
        brdf += brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, light.direction, viewDirectionWS);
    }
#endif // _SPECULARHIGHLIGHTS_OFF

    return brdf * radiance;    
}


half4 FTX_UniversalFragmentPBR_Player(InputData inputData, FTXSurfaceData FTXSurfaceData)
{
#ifdef _SPECULARHIGHLIGHTS_OFF
    bool specularHighlightsOff = true;
#else
    bool specularHighlightsOff = false;
#endif

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(FTXSurfaceData.albedo, FTXSurfaceData.metallic, half3(0.0h, 0.0h, 0.0h), FTXSurfaceData.smoothness, FTXSurfaceData.alpha, brdfData);

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
#elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
#else
    half4 shadowMask = half4(1, 1, 1, 1);
#endif

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

    #if defined(_SCREEN_SPACE_OCCLUSION)
        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
        mainLight.color *= aoFactor.directAmbientOcclusion;
        FTXSurfaceData.occlusion = min(FTXSurfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
    #endif

    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIllumination(brdfData, brdfData, 0.0h,
                                     inputData.bakedGI, FTXSurfaceData.occlusion,
                                     inputData.normalWS, inputData.viewDirectionWS);
    color += FTX_LightingPhysicallyBased_Play(brdfData, mainLight,
                                              inputData.normalWS, inputData.viewDirectionWS,                                             
                                              specularHighlightsOff);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
        #if defined(_SCREEN_SPACE_OCCLUSION)
            light.color *= aoFactor.directAmbientOcclusion;
        #endif
        color += FTX_LightingPhysicallyBased_Play(brdfData, light,
                                                  inputData.normalWS, inputData.viewDirectionWS,                                                
                                                  specularHighlightsOff);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

#ifdef PLAYERREFLECTION
    color += brdfData.diffuse * SamplePlayerReflection(inputData.normalWS);
#endif

    color += FTXSurfaceData.emission;

    return half4(color, FTXSurfaceData.alpha);
}
#endif

half4 FTX_UniversalFragmentPBR(InputData inputData, FTXSurfaceData FTXSurfaceData)
{
#ifdef _SPECULARHIGHLIGHTS_OFF
    bool specularHighlightsOff = true;
#else
    bool specularHighlightsOff = false;
#endif

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(FTXSurfaceData.albedo, FTXSurfaceData.metallic, half3(0.0h, 0.0h, 0.0h), FTXSurfaceData.smoothness, FTXSurfaceData.alpha, brdfData);

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
#elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
#else
    half4 shadowMask = half4(1, 1, 1, 1);
#endif

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

    #if defined(_SCREEN_SPACE_OCCLUSION)
        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
        mainLight.color *= aoFactor.directAmbientOcclusion;
        FTXSurfaceData.occlusion = min(FTXSurfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
    #endif

    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIllumination(brdfData, brdfData, 0.0h,
                                     inputData.bakedGI, FTXSurfaceData.occlusion,
                                     inputData.normalWS, inputData.viewDirectionWS);
    color += FTX_LightingPhysicallyBased(brdfData, mainLight,
                                         inputData.normalWS, inputData.viewDirectionWS,                                             
                                         specularHighlightsOff);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
        #if defined(_SCREEN_SPACE_OCCLUSION)
            light.color *= aoFactor.directAmbientOcclusion;
        #endif
        color += FTX_LightingPhysicallyBased(brdfData, light,
                                             inputData.normalWS, inputData.viewDirectionWS,                                                
                                             specularHighlightsOff);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += FTXSurfaceData.emission;

    return half4(color, FTXSurfaceData.alpha);
}

#endif
