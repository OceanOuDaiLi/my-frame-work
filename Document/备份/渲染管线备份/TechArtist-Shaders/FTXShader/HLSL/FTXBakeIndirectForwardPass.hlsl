#ifndef UNIVERSAL_FORWARD_FTX_BAKE_INDIRECT_PASS_INCLUDED
#define UNIVERSAL_FORWARD_FTX_BAKE_INDIRECT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// GLES2 has limited amount of interpolators
#if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) //|| defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

// keep this file in sync with LitGBufferPass.hlsl

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    half2 texcoord      : TEXCOORD0;
    half2 lightmapUV    : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    half2 uv                        : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

    float3 normalWS                 : TEXCOORD2;

#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    float4 tangentWS                : TEXCOORD3;    // xyz: tangent, w: sign
#endif

    half4 fogFactor                 : TEXCOORD4; // x: fogFactor, yzw: vertex light
    float4 positionCS               : SV_POSITION;
    half3 viewDirWS                : TEXCOORD5;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Physically Based) shader
Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionCS = vertexInput.positionCS;
    output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    output.normalWS = normalInput.normalWS;
#if defined(_NORMALMAP)
    real sign = input.tangentOS.w * GetOddNegativeScale();
    output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
#endif
    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS, output.vertexSH);

    return output;
}

// Used in Standard (Physically Based) shader
half4 LitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half2 uv = input.uv.xy;
    half oneMinusReflectivity = OneMinusReflectivityMetallic(_Metallic);
    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    half3 color = texColor.rgb * _BaseColor.rgb * oneMinusReflectivity;
    half alpha = texColor.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);

#ifdef _ALPHAPREMULTIPLY_ON
    color *= alpha;
#endif

#if defined(_NORMALMAP)
    half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap)).xyz;
    float sgn = input.tangentWS.w;      // should be either +1 or -1
    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent, input.normalWS));
#else
    half3 normalWS = input.normalWS;
#endif
    normalWS = NormalizeNormalPerPixel(normalWS);
    color *= SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
    
#if defined(_EMISSION)
    color += SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionColor;
#endif

    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    half NoV = saturate(dot(normalWS, viewDirWS));
    half fresnelTerm = Pow4(1.0 - NoV);
#if !defined(_ENVIRONMENTREFLECTIONS_OFF)   
    half3 reflectVector = reflect(-viewDirWS, normalWS);
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, 1.0h - _Smoothness, 1.0h);
    color += indirectSpecular * fresnelTerm * _Smoothness;
#else
    half3 indirectSpecular = _GlossyEnvironmentColor.rgb;
    color += indirectSpecular * fresnelTerm * _Smoothness;
#endif    

    color = MixFog(color, input.fogFactor);
    alpha = OutputAlpha(alpha, _Surface);

    return half4(color, alpha);      
}

#endif
