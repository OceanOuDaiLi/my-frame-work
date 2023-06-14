#ifndef UNIVERSAL_FTX_LIT_META_PASS_INCLUDED
#define UNIVERSAL_FTX_LIT_META_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 uv0          : TEXCOORD0;
    float2 uv1          : TEXCOORD1;
    float2 uv2          : TEXCOORD2;
#ifdef _TANGENT_TO_WORLD
    float4 tangentOS     : TANGENT;
#endif
};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
};

Varyings UniversalVertexMeta(Attributes input)
{
    Varyings output;
    output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
    output.uv.xy = TRANSFORM_TEX(input.uv0, _BaseMap);

    return output;
}

half4 UniversalFragmentMeta(Varyings input) : SV_Target
{
    FTXSurfaceData FTXSurfaceData;
    InitializeStandardLitSurfaceData(input.uv, FTXSurfaceData);

    BRDFData brdfData;
    InitializeBRDFData(FTXSurfaceData.albedo, FTXSurfaceData.metallic, half3(0.0, 0.0, 0.0), FTXSurfaceData.smoothness, FTXSurfaceData.alpha, brdfData);

    MetaInput metaInput;
    metaInput.Albedo = brdfData.diffuse + brdfData.specular * brdfData.roughness * 0.5;
    metaInput.SpecularColor = half3(0.0, 0.0, 0.0);
    metaInput.Emission = FTXSurfaceData.emission;

    return MetaFragment(metaInput);
}


//LWRP -> Universal Backwards Compatibility
Varyings LightweightVertexMeta(Attributes input)
{
    return UniversalVertexMeta(input);
}

half4 LightweightFragmentMeta(Varyings input) : SV_Target
{
    return UniversalFragmentMeta(input);
}

#endif
