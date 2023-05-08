/////////////////////////////////////////////////
// SHADER DEPENDENCIES //////////////////////////
/////////////////////////////////////////////////
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

struct MetaAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv0 : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 uv2 : TEXCOORD2;
};

struct MetaVaryings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

MetaVaryings MetaVert (MetaAttributes input)
{
    MetaVaryings output;

    output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
    output.uv = input.uv0;
    return output;
}

half4 MetaFrag (MetaVaryings input) : SV_Target
{
    float2 uv0 = input.uv * _UVScale;
    half4 albedoMap = SampleAlbedoMap(uv0);
    half4 normalMap = SampleNormalMap(uv0);
    half4 metallicMap = SampleMetallicMap(uv0);
    half3 emission = 0.0h;

    half3 diffuse = albedoMap.rgb * _AlbedoColor.rgb;
    half3 specular = lerp (kDieletricSpec.rgb, diffuse, normalMap.r * _Metallic);
    half smoothness = metallicMap.g * _Glossiness;

    half perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
    half roughness = max(PerceptualRoughnessToRoughness(perceptualRoughness), HALF_MIN);

#if defined (SHADING_EMISSION)
    emission = SampleEmissionMap(uv0).rgb * _EmissionColor.rgb;
#endif
    
    MetaInput metaInput;
    metaInput.Albedo = diffuse + specular * roughness * 0.5;
    metaInput.SpecularColor = specular;
    metaInput.Emission = emission;

    return MetaFragment(metaInput);
}