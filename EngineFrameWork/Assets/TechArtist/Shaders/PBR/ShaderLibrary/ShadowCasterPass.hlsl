#ifndef STANDARD_SHADOW_CASTER_PASS_INCLUDED
#define STANDARD_SHADOW_CASTER_PASS_INCLUDED

// DEPENDENCIES
//#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

VaryingsShadow ShadowVert(AttributesShadow input)
{
    VaryingsShadow output = (VaryingsShadow)0;
    UNITY_SETUP_INSTANCE_ID(input);

    float3 normalWS   = TransformObjectToWorldNormal(input.normalOS);
#if defined(VERTEX_COLOR)
    //for muscle
    input.positionOS.xyz += input.normalOS.xyz * (
        (input.color.r) * (_RScale - 1) * 0.1 +
        (input.color.g) * (_GScale - 1) * 0.1 +
        (input.color.b) * (_BScale - 1) * 0.1 +
        (input.color.a) * (_AScale - 1) * 0.1
        );
#endif
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.positionCS = GetShadowPositionHClip(positionWS, normalWS);
    output.uv = input.texcoord * _UVScale;
    return output;
}

half4 ShadowFrag(VaryingsShadow input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    Alpha(SampleAlbedoMap(input.uv.xy).a, _AlbedoColor, _Cutoff);
    return 0;
}

half4 HairShadowFrag(VaryingsShadow input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    Alpha(SampleAlbedoMap(input.uv.xy).a, _AlbedoColor, 0.1);
    return 0;
}
#endif
