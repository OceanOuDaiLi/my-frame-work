/////////////////////////////////////////////////
// TEXTURES AND SAMPLER STATES //////////////////
/////////////////////////////////////////////////
TEXTURE2D(_AlbedoMap);              SAMPLER(sampler_AlbedoMap);
TEXTURE2D(_NormalMap);              SAMPLER(sampler_NormalMap);
TEXTURE2D(_MetallicMap);            SAMPLER(sampler_MetallicMap);
TEXTURE2D(_EmissionMap);            SAMPLER(sampler_EmissionMap);
TEXTURE2D(_DetailBump);             SAMPLER(sampler_DetailBump);
TEXTURE2D(_CameraOpaqueTexture);    SAMPLER(sampler_ScreenTextures_linear_clamp);
TEXTURE2D(_ReflectionTex);
//TEXTURE2D(_ReflectionDepthTex);     
TEXTURE2D(_ReflectionTex2);
TEXTURECUBE(_ReflectionUvCube);         
TEXTURE2D(_EffectMaskTex);                SAMPLER(sampler_EffectMaskTex);

TEXTURE2D(_NumTex);              SAMPLER(sampler_NumTex);
TEXTURE2D(_NumTex2);             SAMPLER(sampler_NumTex2);
TEXTURE2D(_ZMTex);               SAMPLER(sampler_ZMTex);
TEXTURE2D(_ZMTex2);              SAMPLER(sampler_ZMTex2);
/////////////////////////////////////////////////
// TEXTURE SAMPLERS /////////////////////////////
/////////////////////////////////////////////////
half4 SampleAlbedoMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, uv);
}

half4 SampleNumberTexture(float2 uv)
{
    return SAMPLE_TEXTURE2D(_NumTex, sampler_NumTex, uv);
}

half4 SampleNumberTexture2(float2 uv)
{
    return SAMPLE_TEXTURE2D(_NumTex2, sampler_NumTex2, uv);
}

half4 SampleZMTexture(float2 uv)
{
    return SAMPLE_TEXTURE2D(_ZMTex, sampler_ZMTex, uv);
}

half4 SampleZMTexture2(float2 uv)
{
    return SAMPLE_TEXTURE2D(_ZMTex2, sampler_ZMTex2, uv);
}


half4 SampleNormalMap(float2 uv, float bias = 0)
{
    return SAMPLE_TEXTURE2D_BIAS(_NormalMap, sampler_NormalMap, uv, bias);
}

half4 SampleMetallicMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, uv);
}

half4 SampleEmissionMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);
}

half4 SampleDetailNormalMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_DetailBump, sampler_DetailBump, uv);
}

half4 SampleCameraOpaqueTexture (float4 projection, float2 distortion)
{
    float2 uv = (projection.xy / projection.w) + distortion;
    return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_ScreenTextures_linear_clamp, uv);
}

half4 SampleReflectionTexture(float2 screenUV)
{
    return SAMPLE_TEXTURE2D(_ReflectionTex, sampler_AlbedoMap, screenUV);
}

//half4 SampleReflectionDepthTexture(float2 screenUV)
//{
//    return SAMPLE_TEXTURE2D(_ReflectionDepthTex, sampler_AlbedoMap, screenUV);
//}

half4 SampleReflectionTexture2(float2 screenUV)
{
    return SAMPLE_TEXTURE2D(_ReflectionTex2, sampler_AlbedoMap, screenUV);
}

half4 SampleReflectionUv(float3 reflectUV)
{
    return SAMPLE_TEXTURECUBE(_ReflectionUvCube, sampler_AlbedoMap, reflectUV);
}

half Alpha(half albedoAlpha, half4 color, half cutoff)
{
    half alpha = albedoAlpha * color.a;

#if defined(ALPHATEST_ON)
    clip(alpha - cutoff);
#endif
    return alpha;
}

half Stipple(Varyings input, half alpha, half transparency)
{
#if defined(STIPPLE_ON)
    float4x4 thresholdMatrix =
    {
        1.0, 9.0, 3.0, 11.0,
        13.0, 5.0, 15.0, 7.0,
        4.0, 12.0, 2.0, 10.0,
        16.0, 8.0, 14.0, 6.0

    };
    float4x4 _RawAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
    float2 pos = input.screenCoord.xy / input.screenCoord.w;
    pos *= _ScreenParams.xy;
    #if defined(STANDARD_SHADER)
        clip(transparency - thresholdMatrix[fmod(pos.x, 4)] * _RawAccess[fmod(pos.y, 4)] / 17);
    #else
        alpha *= saturate((transparency - thresholdMatrix[fmod(pos.x, 4)] * _RawAccess[fmod(pos.y, 4)]/17));
    #endif
#endif
    return alpha;
}

half StippleBySurface(Varyings input, half alpha, half transparency, half surfaceType)
{
#if defined(STIPPLE_ON)
    float4x4 thresholdMatrix =
    {
        1.0, 9.0, 3.0, 11.0,
        13.0, 5.0, 15.0, 7.0,
        4.0, 12.0, 2.0, 10.0,
        16.0, 8.0, 14.0, 6.0

    };
    float4x4 _RawAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
    float2 pos = input.screenCoord.xy / input.screenCoord.w;
    pos *= _ScreenParams.xy;
    float4 result = transparency - thresholdMatrix[fmod(pos.x, 4)] * _RawAccess[fmod(pos.y, 4)]/17;
    if(surfaceType == 1)
    {
        alpha *= saturate(result);
    }
    else
    {
        clip(result);
    }

#endif
    return alpha;
}