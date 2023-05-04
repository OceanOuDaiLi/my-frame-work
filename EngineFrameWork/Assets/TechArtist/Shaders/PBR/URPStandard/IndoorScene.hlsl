/////////////////////////////////////////////////
// SHADER FEATURE CONFIGURATION /////////////////
/////////////////////////////////////////////////
#define TANGENTSPACE
//#define PARALLAX
#define SHADING_METALLIC
#define CUBEMAP_REFLECTION
#if defined(PLANAR_REFLECTION) || defined(STIPPLE_ON)
    #define SCREEN_COORD
#endif


// DEPENDENCIES
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

// MATERIAL PARAMETERS
CBUFFER_START(UnityPerMaterial)
    half   _Metallic;
    half   _Glossiness;
    half   _AOIntensity;
    half   _NormalBias;
    half   _NormalScale;
    half4  _AlbedoColor;
    half4  _EmissionColor;
    half   _Cutoff;
    half   _EmissionEnabled;
    half   _UVScale;
    half   _NetLightness;
    half   _ShadowIntensity;
    half  _Transparency;
    half   _CURV_MOD;
    half  _MAX_POW;
    half  _Surface;
    half   _Low;
    half   _High;
    half3 _Light;
    half   _RimScale;
    half   _RimPower;
    half3  _LightColor;
    half   _BackLightScale;
    half   _FlagWaveSpeed;
    half   _FlagWaveIntensity;
    half   _FlagShadowIntensity;
    half   _FlagWaveRotation;
    half _xVar;
    half _yVar;
    half _radius;
    half _titling;
    half _ind;
    half _gentle;
    half _ellipseX;
    half _ellipseY;
    half _ReflectionInstensity;
    half _ReflectionRotation;
    half4 _ReflectionColor;
    
CBUFFER_END

half   _ReflectionAlpha;
half   _PlaneReflectionIntensity;
half   _ReflectionNoiseScale;

TEXTURE2D(_AOTex);              SAMPLER(sampler_AOTex);
TEXTURE2D(_LogoTex);            SAMPLER(sampler_LogoTex);

half circle(half2 ref, half2 center, half radius)
{
    half dist = length(ref - center);
    half outside = smoothstep(radius - _gentle, radius + _gentle, dist);
    return 1 - outside;
}
half DistLine(half3 ro, half3 rd, half3 p)
{
    return length(cross(p - ro, rd)) / length(rd);
}
half circleAround(half3 ro,half3 rd, half3 p, half radius,half gentle)
{
    half d = DistLine(ro, rd, p);
    half dCol = 1-(smoothstep(radius - gentle, _radius + gentle, d));
    return dCol;
}

/////////////////////////////////////////////////
// MATERIAL DEFINITION //////////////////////////
/////////////////////////////////////////////////
inline Material GetMaterial(Varyings input)
{
    half2 uv = input.uv0.xy;
    half4 albedoMap = SampleAlbedoMap(uv);

    half4 logoCol = SAMPLE_TEXTURE2D(_LogoTex,sampler_LogoTex,float2(-(input.uvAO.x-0.5)*5.3+0.5,-(input.uvAO.y-0.5)*3+0.5));
    albedoMap.rgb = logoCol.rgb * logoCol.a + albedoMap.rgb * (1 - logoCol.a);

    Material material;
#if defined(QUALITY_HIGH)
    half4 normalMap = SampleNormalMap(uv);
    half4 metallicMap = SampleMetallicMap(uv);
    material.normal = UnpackNormalRGB(normalMap, _NormalScale);
    material.normal = normalize(material.normal);
    material.metallic = metallicMap.r;
    material.smoothness = metallicMap.g;
    material.ao = _AOIntensity * metallicMap.b + (1 - _AOIntensity);
    material.metallic *= _Metallic;
#else
    material.normal = half3(0, 0, 0);
    material.metallic = 0;
    material.smoothness = 0.4;
    material.ao = 1;
#endif
    material.alpha = Alpha(albedoMap.a, _AlbedoColor, _Cutoff);
    material.albedo = albedoMap.rgb * _AlbedoColor;
    material.emission = 0;

#if defined (SHADING_EMISSION)
    material.emission = SampleEmissionMap(uv).rgb * _EmissionColor.rgb;
#endif

    material.transmission = 0.0h;
    material.smoothness *= _Glossiness;

    return material;
}

float3 RotateAroundYInDegrees (float3 vertex, float degrees)
{
    float alpha = degrees * 3.1415926 / 180.0;
    float sina, cosa;
    sincos(alpha, sina, cosa);
    float2x2 m = float2x2(cosa, -sina, sina, cosa);
    return float3(mul(m, vertex.xz), vertex.y).xzy;
}


/////////////////////////////////////////////////
// FORWARD PASS /////////////////////////////////
/////////////////////////////////////////////////
Varyings LitVertForward(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    half3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    half4 positionCS = TransformWorldToHClip(positionWS);
    half3  normalWS   = TransformObjectToWorldNormal(input.normalOS);
    half3 viewDirWS   = GetCameraPositionWS() - positionWS;
    //tangent
    half3 tanOS = TransformObjectToWorldDir(input.tangentOS.xyz);
    output.tangentWS   = half4(tanOS, viewDirWS.y);

    OUTPUT_TEXCOORD0(input, output, _UVScale);
    OUTPUT_TEXCOORD2(input, output);
    output.uvAO = input.uv1;

    // OUTPUT SHADOW, NORMALS, AMBIENT GI, CLIP-, WORLD-POSITION, VIEWDIR
    OUTPUT_DEFAULT(input, output, positionWS, positionCS, normalWS, viewDirWS);
    output.bakeGISH = GIVERTEX(normalWS);
    return output;
}

half4 LitFragForward (Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    //Normalize viewDirWS
    SETUP_SHADING_DATA(input);

    // Get material properties from texture inputs, convert to world normals
    Material material = GetMaterial(input);

#if defined(QUALITY_HIGH)
    half3 normalWS = GetWorldNormal(input, material.normal);
#else
    half3 normalWS = input.normalWS.xyz;
#endif

    half3 reflectDir = GetReflectDir(input.normalWS.xyz, viewDirWS, input.positionWS.xyz);
    half3 bakedGI = GIFRAG(input, normalWS) + input.bakeGISH;

    half4 reflection = 0;

    // Get dot products and shading data
    DotProducts dotProducts = GetDotProducts(mainLight, normalWS, viewDirWS);
    BTDFData btdfData = GetBTDFData(material, dotProducts, material.alpha);

    // Calculate lighting model
#if defined(USE_PROBES)
    half3 color = LightingIndirect(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao, reflection);
#else
    half3 color = LightingIndirect3(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao, reflection);
#endif

#if defined(GSAA_ON)
    color += LightingDirectGSAA(btdfData, dotProducts, mainLight, input.tangentWS.xyz, _CURV_MOD, _MAX_POW, _ShadowIntensity,_NetLightness);
#else
    if (mainLight.distanceAttenuation > 0)
        color += LIGHTING_BRDF(btdfData, dotProducts, mainLight, _ShadowIntensity, _NetLightness, reflectDir);
#endif
    color += LightingEmissive(material.emission);

    half a = dot ( normalize( mainLight.direction + viewDirWS ) , normalize(normalWS) );
    half3 ambient = a * SampleReflectionUv( RotateAroundYInDegrees( normalize(viewDirWS - normalWS * 2 * sin(dot(viewDirWS, normalWS))) ,_ReflectionRotation)) * _ReflectionInstensity *  material.smoothness;
    color += ambient * _ReflectionColor;

#if defined(RIM_MASK)
    half rim = 1 - saturate(dot(viewDirWS, normalWS));
    //trick rim
    half3 lightDir1 = TransformObjectToTangent(half3(0, -1, 0), half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    half3 lightDir2 = TransformWorldToTangent(_Light, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    half resValue1 = RimMask(lightDir1, input.sh_1, half2(_Low, _High) / 2) * _BackLightScale;
    half resValue2 = RimMask(lightDir2, input.sh_1, half2(_Low, _High));
    #if defined(VIR_LIT)
        half3 rim2 = _LightColor * btdfData.diffuse * virAtten;
    #else
        half3 rim2 = _LightColor;
    #endif
    half3 rimColor = (pow(rim, _RimPower) * max(0.0001, _RimScale) * ((resValue1 + resValue2) * material.ao) * rim2);
    color += rimColor;
#endif

    ADDITIONAL_LIGHTS(input, color, btdfData, material.ao, reflectDir);
#if defined(IS_FLAG)
    half3 flagColor = FlagWave(input.uv0.xy, color, half4(_FlagWaveIntensity, _FlagWaveSpeed, _FlagWaveRotation, _FlagShadowIntensity));
    color = flagColor.rgb;
#endif

    APPLY_COLOR_GRADING(color);

    half AO = SAMPLE_TEXTURE2D(_AOTex , sampler_AOTex , input.uvAO);

    return half4(color, material.alpha) * AO;
}

/////////////////////////////////////////////////
// Unlit PASS //////////////////////////////////
/////////////////////////////////////////////////

VaryingsSimple LitVertUnlit(AttributesSimple input)
{
    OUTPUT_VERTSIMPLE(input)
    return output;
}

half4 LitFragUnlit(VaryingsSimple input) : SV_Target
{
    OUTPUT_FRAGSIMPLE(input);
    return half4(color, alpha);
}

/////////////////////////////////////////////////
// MAIN FUNCTION /////////////////////////////////
/////////////////////////////////////////////////

#if defined(UNITY_STANDARD_SIMPLE)
    VaryingsSimple LitVert (AttributesSimple v) { return LitVertUnlit(v); }
    half4 LitFrag (VaryingsSimple i) : SV_Target { return LitFragUnlit(i); }
#else
    Varyings LitVert(Attributes input) { return LitVertForward(input); }
    half4 LitFrag (Varyings input) : SV_Target { return LitFragForward(input); }
#endif