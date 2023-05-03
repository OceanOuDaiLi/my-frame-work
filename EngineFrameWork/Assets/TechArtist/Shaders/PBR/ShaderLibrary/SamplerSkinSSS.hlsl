/////////////////////////////////////////////////
// TEXTURES AND SAMPLER STATES //////////////////
/////////////////////////////////////////////////
TEXTURE2D(_BSSRDFMap);              SAMPLER(sampler_BSSRDFMap);
TEXTURE2D(_SweatMap);               SAMPLER(sampler_SweatMap);

/////////////////////////////////////////////////
// TEXTURE SAMPLERS /////////////////////////////
/////////////////////////////////////////////////
half4 SampleBSSRDFMap(half2 uv)
{
    return SAMPLE_TEXTURE2D(_BSSRDFMap, sampler_BSSRDFMap, uv);
}

half4 SampleSweatMap(float2 uv)
{
    return SAMPLE_TEXTURE2D(_SweatMap, sampler_SweatMap, uv);
}