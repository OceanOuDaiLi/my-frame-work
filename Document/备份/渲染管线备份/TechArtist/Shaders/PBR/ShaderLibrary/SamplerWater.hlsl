/////////////////////////////////////////////////
// TEXTURES AND SAMPLER STATES //////////////////
/////////////////////////////////////////////////
TEXTURE2D(_AlbedoMap2);              SAMPLER(sampler_AlbedoMap2);
TEXTURE2D(_NormalMap2);              SAMPLER(sampler_NormalMap2);
TEXTURE2D(_ParallaxMap0);             SAMPLER(sampler_ParallaxMap0);
TEXTURE2D(_CameraDepthTexture);      SAMPLER(sampler_CameraDepthTexture);

/////////////////////////////////////////////////
// TEXTURE SAMPLERS /////////////////////////////
/////////////////////////////////////////////////
half4 SampleAlbedoMap2(half2 uv)
{
    return SAMPLE_TEXTURE2D(_AlbedoMap2, sampler_AlbedoMap2, uv);
}

half4 SampleNormalMap2(float2 uv)
{
    return SAMPLE_TEXTURE2D(_NormalMap2, sampler_NormalMap2, uv);
}

half4 SampleParallaxMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_ParallaxMap0, sampler_ParallaxMap0, uv);
}

float SampleDepthTexture(float2 uv)
{
    return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
}