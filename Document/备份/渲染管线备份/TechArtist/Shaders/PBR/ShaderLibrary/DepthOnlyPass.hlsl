#ifndef STANDARD_DEPTH_ONLY_PASS_INCLUDED
#define STANDARD_DEPTH_ONLY_PASS_INCLUDED

// DEPENDENCIES
//#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

VaryingsDepthOnly DepthOnlyVert(AttributesDepthOnly input)
{
    VaryingsDepthOnly output = (VaryingsDepthOnly)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    output.uv = input.texcoord * _UVScale;
#if defined(VERTEX_COLOR)
    //for muscle
    input.positionOS.xyz += input.normalOS.xyz * (
        (input.color.r) * (_RScale - 1) * 0.1 +
        (input.color.g) * (_GScale - 1) * 0.1 +
        (input.color.b) * (_BScale - 1) * 0.1 +
        (input.color.a) * (_AScale - 1) * 0.1
        );
#endif
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);


    return output;
}

half4 DepthOnlyFrag(VaryingsDepthOnly input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    Alpha(SampleAlbedoMap(input.uv.xy).a, _AlbedoColor, _Cutoff);
    return 0;
}
#endif