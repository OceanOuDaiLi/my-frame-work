#define VECTOR_UP float3(0.0, 1.0, 0.0)
#define NORMAL_TS half3(0.5, 0.5, 1.0)
#define DEFAULT_BLACK half4(0.0, 0.0, 0.0, 1.0)
#define DEFAULT_NORMAL half4(0.5, 0.5, 1.0, 1.0)

/////////////////////////////////////////////////
// UTILITY FUNCTIONS ////////////////////////////
/////////////////////////////////////////////////
half DotClamped(half3 a, half3 b)
{
	return saturate(dot(a, b));
}

half DotAbs(half3 a, half3 b)
{
	return abs(dot(a, b));
}

half SmoothCurve(half x)
{
    return x * x * (3.0h - 2.0h * x);
}

half TriangleWave(half x)
{
    return abs(frac(x + 0.5h) * 2.0h - 1.0h);
}

half SmoothTriangleWave(half x)
{
    return SmoothCurve(TriangleWave(x));
}

// // Normal Blending Modes
// inline half3 BlendNormal(inout half3 a, half3 b)
// {
// #if defined (NORMAL_BLEND_WHITEOUT)
//     a = normalize (half3 (a.xy + b.xy, a.z * b.z));
// #else
//     a = normalize (half3 (a.xy + b.xy, a.z));
// #endif
//     return a;
// }

inline void BlendHighPass(inout half a, half b)
{
    a += (b - 0.5h) * 2.0h;
}

inline void BlendHighPass(inout half3 a, half3 b)
{
    a += (b - 0.5h) * 2.0h;
}

/////////////////////////////////////////////////
// VARYINGS HELPER FUNCTIONS ////////////////////
/////////////////////////////////////////////////
half3 UnpackNormalRG(half4 normalMap, half scale = 1.0h)
{
    half3 normal = 0.0h;

    normal.xy = normalMap.rg * 2.0h - 1.0h;
    normal.xy *= scale;
    normal.z  = sqrt(1.0h - saturate(dot(normal.xy, normal.xy)));

    return normal;
}

half3 UnpackNormalRGBNormalize(half4 packedNormal, half scale = 1.0h)
{
    half3 normal;
    normal.xyz = packedNormal.rgb * 2.0 - 1.0;
    normal.xy *= scale;
    return normalize(normal);
}

half3 GetWorldNormal (Varyings input, half3 normalTS = NORMAL_TS)
{
#if defined(TANGENTSPACE)
    half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    return SafeNormalize(normalWS);
#else
    return SafeNormalize(input.normalWS);
#endif
}

half3 GetViewDir (Varyings input)
{
#if defined(TANGENTSPACE)
    return half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
#else
    return input.viewDirWS;
#endif
}

half3 GetBakedGI (Varyings input, half3 normalWS)
{
#if defined(LIGHTMAP_ON)
    return SampleLightmap(input.uv0.zw, normalWS);
#else
    return FastSampleSH(normalWS);
#endif
}

half3 GetBakedGIL(Varyings input, half3 normalWS)
{
#if defined(LIGHTMAP_ON)
    return SampleLightmap(input.uv0.zw, normalWS);
#else
    return half3(0, 0, 0);
#endif
}

half3 GetBakedGISH(half3 normalWS)
{
#if defined(LIGHTMAP_ON)
    return half3(0, 0, 0);
#else
    return FastSampleSH(normalWS);
#endif
}

half3 GetBakedGIH(half3 normalWS)
{
    return half3(0, 0, 0);
}

/////////////////////////////////////////////////
// MATH FUNCTIONS ///////////////////////////////
/////////////////////////////////////////////////
half Sum(half3 c)
{
    return c.r + c.g + c.b;
}

half Sum(half4 c)
{
    return c.r + c.g + c.b;
}

half Pow2(half x)
{
    return x * x;
}

half3 Pow2(half3 x)
{
    return x * x;
}

half Pow3(half x)
{
    return (x * x) * x;
}

half3 Pow3(half3 x)
{
    return (x * x) * x;
}

// half Pow4(half x)
// {
//     return (x * x) * (x * x);
// }

half2 Pow4(half2 x)
{
    return (x * x) * (x * x);
}

half3 Pow4(half3 x)
{
    return (x * x) * (x * x);
}

half3 Pow5(half3 x)
{
    return (x * x) * (x * x) * x;
}

half Pow5(half x)
{
    return (x * x) * (x * x) * x;
}

//shift tangents 切线扰动   --horacezhao
float3 HairShiftTangent(in float3 T, in float3 N, in float shift)
{
    float3 shiftedT = T + shift * N;
    return normalize(shiftedT);
}

//头发各向异性高光   --horacezhao
float HairStrandSpecular(in float3 T, in float3 H, in float exponent)
{
    float HoT = dot(H, T);
    float sinTH = max(0.01, sqrt(1.0 - HoT * HoT));
    float dirAtten = smoothstep(-1.0, 0.0, HoT);
    return dirAtten * pow(sinTH, exponent);
}

/////////////////////////////////////////////////
// SHADOW FUNCTIONS /////////////////////////////
/////////////////////////////////////////////////
float3 _LightDirection;

float4 GetShadowPositionHClip(float3 positionWS, float3 normalWS)
{
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif

    return positionCS;
}

/////////////////////////////////////////////////
// LOGIC OPERATIONS /////////////////////////////
// http://theorangeduck.com/page/avoiding-shader-conditionals
/////////////////////////////////////////////////
half IsEqual (half a, half b)
{
    return 1.0h - abs(sign(a - b));
}

half IsNotEqual(half a, half b)
{
    return abs(sign(a - b));
}

half IsGreater(half a, half b)
{
    return max(sign(a - b), 0.0h);
}

half IsGreaterOrEqual(half a, half b)
{
    return 1.0h - max(sign(a - b), 0.0h);
}

half IsLess(half a, half b)
{
    return max(sign(b - a), 0.0h);
}

half IsLessOrEqual(half a, half b)
{
    return 1.0h - max(sign(a - b), 0.0h);
}

/////////////////////////////////////////////////
// DEBUGGING METHODS ////////////////////////////
/////////////////////////////////////////////////
float3 AssertIsClamped(float3 color)
{
    float value = color.r + color.g + color.g;

    if(value > 3.0 || value < 0.0)
    {
        return float3(1.0, 0.0, 0.0);
    }

    return float3(0.0, 1.0, 0.0);
}

float3 AssertIsClamped(float value)
{
    if(value > 1.0 || value < 0.0)
    {
        return float3(1.0, 0.0, 0.0);
    }

    return float3(0.0, 1.0, 0.0);
}