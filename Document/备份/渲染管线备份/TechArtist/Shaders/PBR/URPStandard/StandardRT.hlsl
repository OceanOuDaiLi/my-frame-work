/////////////////////////////////////////////////
// SHADER FEATURE CONFIGURATION /////////////////
/////////////////////////////////////////////////
#define TANGENTSPACE
#define SHADING_METALLIC
#define CUBEMAP_REFLECTION

// DEPENDENCIES
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

// MATERIAL PARAMETERS
CBUFFER_START(UnityPerMaterial)
    half   _Metallic;
    half   _Glossiness;
    half   _AOIntensity;
    half4  _AlbedoColor;
    half4  _EmissionColor;
    half   _Cutoff;
    half   _DetailBumpScale;
    half   _EmissionEnabled;
    half   _UVScale;
    half   _RScale;
    half   _GScale;
    half   _BScale;
    half   _AScale;
    half   _ShadowIntensity;
    half4  _DetailBump_ST;

    half    _NumberS;
    half    _NumberG;
    half	_NumberDis;
    half4	_NumBackOffSet;
    half4	_NumFrontOffSet;
CBUFFER_END

real4 _Override_SHCoefficients[7];

/////////////////////////////////////////////////
// MATERIAL DEFINITION //////////////////////////
/////////////////////////////////////////////////
inline Material GetMaterial(Varyings input)
{
    half2 uv0 = input.uv0.xy;
    half4 albedoMap = SampleAlbedoMap(uv0);
    half4 normalMap = SampleNormalMap(uv0);

    Material material;
    material.alpha = Alpha(albedoMap.a, _AlbedoColor, 0.5);
    material.normal = UnpackNormalRG(normalMap);
    material.albedo = albedoMap.rgb;
    
    material.metallic = _Metallic;
    material.emission = 0.0h;
    material.smoothness = normalMap.a * _Glossiness;
    material.transmission = 0.0h;
    material.ao = 1 - _AOIntensity + _AOIntensity * normalMap.b;

#if defined (SHADING_EMISSION)
    material.emission = SampleEmissionMap(uv0).rgb * _EmissionColor.rgb;
#endif

    material.normal = normalize(material.normal);
    material.albedo *= _AlbedoColor.rgb;

    return material;
}

void ReplaceSH()
{
#if OVERRIDE_SH
    unity_SHAr = _Override_SHCoefficients[0];
    unity_SHAg = _Override_SHCoefficients[1];
    unity_SHAb = _Override_SHCoefficients[2];
    unity_SHBr = _Override_SHCoefficients[3];
    unity_SHBg = _Override_SHCoefficients[4];
    unity_SHBb = _Override_SHCoefficients[5];
    unity_SHC = _Override_SHCoefficients[6];
#endif
}

/////////////////////////////////////////////////
// FORWARD PASS /////////////////////////////////
/////////////////////////////////////////////////
Varyings LitVert(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

#if defined(VERTEX_COLOR)
    //for muscle
    input.positionOS.xyz += input.normalOS.xyz*(
                (input.color.r)*(_RScale-1)*0.1+
                (input.color.g)*(_GScale-1)*0.1+
                (input.color.b)*(_BScale-1)*0.1+
                (input.color.a)*(_AScale-1)*0.1
                );
#endif

    half3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    half4 positionCS = TransformWorldToHClip(positionWS);
    half3 viewDirWS   = GetCameraPositionWS() - positionWS;
    half3 normalWS   = TransformObjectToWorldNormal(input.normalOS);

    // OUTPUT TEXCOORDS
    OUTPUT_TEXCOORD0(input, output, _UVScale);
    // OUTPUT SHADOW, NORMALS, AMBIENT GI, CLIP-, WORLD-POSITION, VIEWDIR
    OUTPUT_DEFAULT(input, output, positionWS, positionCS, normalWS, viewDirWS);

    return output;
}

half4 LitFrag (Varyings input) : SV_TARGET
{
    //ReplaceSH();

    //UNITY_SETUP_INSTANCE_ID(input);
    SETUP_SHADING_DATA(input);

    // Get material properties from texture inputs, convert to world normals
    Material material = GetMaterial(input);

    half3 normalWS = GetWorldNormal(input, material.normal);
    half3 reflectDir = GetReflectDir(normalWS, viewDirWS, input.positionWS.xyz);
    half3 bakedGI = GetBakedGI(input, normalWS);

    // Get dot products and shading data
    DotProducts dotProducts = GetDotProducts(mainLight, normalWS, viewDirWS);
    BTDFData btdfData = GetBTDFData(material, dotProducts, material.alpha);

    // Calculate lighting model
    half3 color = LightingIndirect(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao);
    if (mainLight.distanceAttenuation > 0)
        color += LIGHTING_BRDF(btdfData, dotProducts, mainLight, _ShadowIntensity,0, reflectDir);

    color += LightingEmissive(material.emission);

    ADDITIONAL_LIGHTS(input, color, btdfData, material.ao, reflectDir);

    //color.rgb = FogApply(color, input);

    return half4(color, material.alpha);
}