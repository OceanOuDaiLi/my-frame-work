/////////////////////////////////////////////////
// TEXTURES AND SAMPLER STATES //////////////////
/////////////////////////////////////////////////
TEXTURE2D(_MaskTex);              SAMPLER(sampler_MaskTex);
TEXTURE2D(_IrisColorTex);         SAMPLER(sampler_IrisColorTex);
TEXTURECUBE(_CubemapEvn);         SAMPLER(sampler_CubemapEvn);


/////////////////////////////////////////////////
// TEXTURE SAMPLERS /////////////////////////////
/////////////////////////////////////////////////
half4 SampleMaskTex(float2 uv)
{
    return SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv);
}

half4 SampleIrisColorTex(float2 uv)
{
    return SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, uv);
}

half4 SampleCubemapEvn(half3 reflectDir, half lod)
{
    return SAMPLE_TEXTURECUBE_LOD(_CubemapEvn, sampler_CubemapEvn, reflectDir, lod);
}