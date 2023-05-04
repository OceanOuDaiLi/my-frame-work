#ifndef STANDARD_DEPTH_NORMAL_PASS_INCLUDED
#define STANDARD_DEPTH_NORMAL_PASS_INCLUDED

// DEPENDENCIES
//#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

struct NBAAttributesDepthNormal
{
    float4 positionOS     : POSITION;
    float4 tangentOS      : TANGENT;
    float2 texcoord       : TEXCOORD0;
    float3 normal         : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct NBAVaryingsDepthNormal
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD1;
    float4 normalWS     : TEXCOORD2;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Encoding/decoding view space normals into 2D 0..1 vector
inline float2 EncodeViewNormalStereo(float3 n)
{
    float kScale = 1.7777;
    float2 enc;
    enc = n.xy / (n.z + 1);
    enc /= kScale;
    enc = enc * 0.5 + 0.5;
    return enc;
}

inline float3 DecodeViewNormalStereo(float4 enc4)
{
    float kScale = 1.7777;
    float3 nn = enc4.xyz * float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
    float g = 2.0 / dot(nn.xyz, nn.xyz);
    float3 n;
    n.xy = g * nn.xy;
    n.z = g - 1;
    return n;
}

// Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
inline float2 EncodeFloatRG(float v)
{
    float2 kEncodeMul = float2(1.0, 255.0);
    float kEncodeBit = 1.0 / 255.0;
    float2 enc = kEncodeMul * v;
    enc = frac(enc);
    enc.x -= enc.y * kEncodeBit;
    return enc;
}

inline float4 EncodeDepthNormal(float depth, float3 normal)
{
    float4 enc;
    enc.xy = EncodeViewNormalStereo(normal);
    enc.zw = EncodeFloatRG(depth);
    return enc;
}

NBAVaryingsDepthNormal DepthNormalsVertex(NBAAttributesDepthNormal input)
{
    NBAVaryingsDepthNormal output = (NBAVaryingsDepthNormal)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv         = input.texcoord * _UVScale;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    output.normalWS.xyz = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, input.normal));
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 positionVS = TransformWorldToView(positionWS);
    output.normalWS.w = -positionVS.z * _ProjectionParams.w;

    return output;
}

float4 DepthNormalsFragment(NBAVaryingsDepthNormal input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    return EncodeDepthNormal(input.normalWS.w, input.normalWS.xyz);
}

#endif