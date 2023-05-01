/////////////////////////////////////////////////
// SHADER FEATURE CONFIGURATION /////////////////
/////////////////////////////////////////////////
#define TANGENTSPACE
#define SHADING_METALLIC
#define CUBEMAP_REFLECTION

// DEPENDENCIES
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

// MATERIAL PARAMETERS
half   _Metallic;
half   _Glossiness;
half   _AOIntensity;
half   _NormalBias;
half   _NormalScale;
half4  _AlbedoColor;
half4  _EmissionColor;
half   _LitAttenContrast;
half   _LitAttenInt;
half   _LitAttenSub;
half   _Cutoff;
half   _DetailBumpScale;
half   _EmissionEnabled;
half   _UVScale;
half4  _RColor;
half4  _GColor;
half4  _BColor;
half4  _AColor;


UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(half, _GPUAnimation_FrameIndex)
    UNITY_DEFINE_INSTANCED_PROP(half, _m_TimeDiff)
    UNITY_DEFINE_INSTANCED_PROP(half, _AnimLen)
    UNITY_DEFINE_INSTANCED_PROP(half, _AnimFPS)
    UNITY_DEFINE_INSTANCED_PROP(half4x4, _GPUAnimation_RootMotion)
UNITY_INSTANCING_BUFFER_END(Props)


TEXTURE2D(_GPUAnimationTextureMatrix);         SAMPLER(sampler_GPUAnimationTextureMatrix);
half3 _GPUAnimation_TextureSize_NumPixelsPerFrame;

/////////////////////////////////////////////////
// FUNCTION DEFINITION //////////////////////////
/////////////////////////////////////////////////
inline half4 indexToUV(half index)
{
	int row = (int)(index / _GPUAnimation_TextureSize_NumPixelsPerFrame.x);
	half col = index - row * _GPUAnimation_TextureSize_NumPixelsPerFrame.x;
	return half4(col / _GPUAnimation_TextureSize_NumPixelsPerFrame.x, row / _GPUAnimation_TextureSize_NumPixelsPerFrame.y, 0, 0);
}

inline half4x4 getMatrix(int frameStartIndex, half boneIndex)
{
	half matStartIndex = frameStartIndex + boneIndex * 3;
	half4 row0 = SAMPLE_TEXTURE2D_LOD(_GPUAnimationTextureMatrix, sampler_GPUAnimationTextureMatrix, indexToUV(matStartIndex).xy, 0);
	half4 row1 = SAMPLE_TEXTURE2D_LOD(_GPUAnimationTextureMatrix, sampler_GPUAnimationTextureMatrix, indexToUV(matStartIndex + 1).xy, 0);
	half4 row2 = SAMPLE_TEXTURE2D_LOD(_GPUAnimationTextureMatrix, sampler_GPUAnimationTextureMatrix, indexToUV(matStartIndex + 2).xy, 0);
	half4 row3 = half4(0, 0, 0, 1);
	half4x4 mat = half4x4(row0, row1, row2, row3);
	return mat;
}

inline half getFrameStartIndex()
{
    half curframe = UNITY_ACCESS_INSTANCED_PROP(Props, _GPUAnimation_FrameIndex);
	//GetFrameIndex,计算GPU动画当前帧索引
    //#define m_TimeDiff UNITY_ACCESS_INSTANCED_PROP(Props, _m_TimeDiff)
    #define AnimLen UNITY_ACCESS_INSTANCED_PROP(Props, _AnimLen)
    #define AnimFPS UNITY_ACCESS_INSTANCED_PROP(Props, _AnimFPS)
    half time = _Time.y;
    half frameIndex = floor(fmod((time * AnimFPS)+curframe  , AnimLen * AnimFPS));
    half frameStartIndex = frameIndex * _GPUAnimation_TextureSize_NumPixelsPerFrame.z;
	return frameStartIndex;
}

inline void VertexMotionMobile(out half4 vertex,out half3 normal, Attributes v)
{
	half frameStartIndex = getFrameStartIndex();
	half4x4 mat0 = getMatrix(frameStartIndex, v.texcoord0.x);

#if ROOTON
	half4x4 root = UNITY_ACCESS_INSTANCED_PROP(Props, _GPUAnimation_RootMotion);
	vertex = mul(root, mul(mat0, v.positionOS));
	half4 normalhalf4 = half4(v.normalOS, 0);
	normalhalf4 = mul(root, mul(mat0, normalhalf4));
	normal = normalhalf4.xyz;
#else
	vertex = mul(mat0, v.positionOS);
	half4 normalhalf4 = half4(v.normalOS, 0);
	normalhalf4 = mul(mat0, normalhalf4) * 1;
	normal = normalhalf4.xyz;
#endif
}

/////////////////////////////////////////////////
// MATERIAL DEFINITION //////////////////////////
/////////////////////////////////////////////////
inline Material GetMaterial(Varyings input)
{
    half2 uv = input.uv0.xy;
    half4 albedoMap = SampleAlbedoMap(uv);
    half4 normalMap = SampleNormalMap(uv, _NormalBias);
    half4 metallicMap = SampleMetallicMap(uv);

    Material material;
    material.alpha = Alpha(albedoMap.a, _AlbedoColor, _Cutoff);
    material.normal = UnpackNormalRGB(normalMap, _NormalScale);
    material.albedo = albedoMap.rgb;
    material.metallic = metallicMap.r;
    material.emission = 0.0h;
    material.smoothness = metallicMap.g;
    material.transmission = 0.0h;
    material.ao = 1 - _AOIntensity + _AOIntensity * metallicMap.b;

    // if(_EmissionEnabled)
    // {
    //     material.emission = SampleEmissionMap(uv).rgb * _EmissionColor.rgb;
    // }

#if defined (SHADING_EMISSION)
    material.emission = SampleEmissionMap(uv).rgb * _EmissionColor.rgb;
#endif

#if defined (DETAIL_MICRO)
    half2 uv1 = uv * _DetailBumpScale;
    half4 microNormalMap = SampleDetailNormalMap(uv1);
    material.normal = BlendNormal(material.normal, UnpackNormalRGB(microNormalMap, _DetailBumpScale));
#endif

    material.normal = normalize(material.normal);
    material.albedo *= _AlbedoColor.rgb;
    material.metallic *= _Metallic;
    material.smoothness *= _Glossiness;

#if defined(VERTEX_COLOR)
    half3 Vertr = lerp(material.albedo, _RColor, input.color0.r);
    half3 Vertg = lerp(material.albedo, _GColor, input.color0.g);
    half3 Vertb = lerp(material.albedo, _BColor, input.color0.b);
    half3 Verta = lerp(material.albedo, _AColor, 1 - input.color0.a);
    material.albedo = lerp(material.albedo, Vertr + Vertg + Vertb + Verta, input.color0.r + input.color0.g + input.color0.b + 1 - input.color0.a);
#endif

    return material;
}

/////////////////////////////////////////////////
// FORWARD PASS /////////////////////////////////
/////////////////////////////////////////////////
Varyings LitVert(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    //VertexMotionMobile(input.positionOS, input.normalOS, input);

    half3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    half4 positionCS = TransformWorldToHClip(positionWS);
    half3  normalWS   = TransformObjectToWorldNormal(input.normalOS);
    half3 viewDirWS   = GetCameraPositionWS() - positionWS;
    //viewDirWS = normalize(viewDirWS);

    // OUTPUT TEXCOORDS
    OUTPUT_TEXCOORD0(input, output, _UVScale);

    // OUTPUT SHADOW, NORMALS, AMBIENT GI, CLIP-, WORLD-POSITION, VIEWDIR
    OUTPUT_DEFAULT(input, output, positionWS, positionCS, normalWS, viewDirWS);
    output.bakeGISH = GIVERTEX(normalWS);
    return output;
}

half4 LitFrag (Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    //Normalize viewDirWS
    SETUP_SHADING_DATA(input);

    // Get material properties from texture inputs, convert to world normals
    Material material = GetMaterial(input);

    half3 normalWS = GetWorldNormal(input, material.normal);
    half3 reflectDir = GetReflectDir(normalWS, viewDirWS, input.positionWS.xyz);
    half3 bakedGI = GIFRAG(input, normalWS) + input.bakeGISH;

    // Get dot products and shading data
    DotProducts dotProducts = GetDotProducts(mainLight, normalWS, viewDirWS);
    BTDFData btdfData = GetBTDFData(material, dotProducts, material.alpha);

    // Calculate lighting model
    //half3 color = 0;
    half3 color = LightingIndirect(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao);
    if (mainLight.distanceAttenuation > 0)
        color += LIGHTING_BRDF(btdfData, dotProducts, mainLight,0,0, reflectDir);
    color += LightingEmissive(material.emission);

    ADDITIONAL_LIGHTS(input, color, btdfData, material.ao, reflectDir);
    color.rgb = FogApply(color, input);

    return half4(color, 1);
}