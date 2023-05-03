#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

TEXTURE2D(_InternalLut);
TEXTURE2D(_UserLut);
SAMPLER(sampler_LinearClamp);

float4 _Lut_Params;
float4 _UserLut_Params;
float _ColorGradingInShader;

#define LutParams               _Lut_Params.xyz
#define PostExposure            _Lut_Params.w
#define UserLutParams           _UserLut_Params.xyz
#define UserLutContribution     _UserLut_Params.w

#define APPLY_COLOR_GRADING(color) \
    if (_ColorGradingInShader > 0.0) \
        color.rgb = ApplyColorGrading(color.rgb, PostExposure, TEXTURE2D_ARGS(_InternalLut, sampler_LinearClamp), LutParams, TEXTURE2D_ARGS(_UserLut, sampler_LinearClamp), UserLutParams, UserLutContribution);

half3 ApplyColorGrading(half3 input, float postExposure, TEXTURE2D_PARAM(lutTex, lutSampler), float3 lutParams, TEXTURE2D_PARAM(userLutTex, userLutSampler), float3 userLutParams, float userLutContrib)
{
    // Artist request to fine tune exposure in post without affecting bloom, dof etc
    input *= postExposure;

    // HDR Grading:
    //   - Apply internal LogC LUT
    //   - (optional) Clamp result & apply user LUT
    {
        float3 inputLutSpace = saturate(LinearToLogC(input)); // LUT space is in LogC
        input = ApplyLut2D(TEXTURE2D_ARGS(lutTex, lutSampler), inputLutSpace, lutParams);

        UNITY_BRANCH
        if (userLutContrib > 0.0)
        {
            input = saturate(input);
            input.rgb = LinearToSRGB(input.rgb); // In LDR do the lookup in sRGB for the user LUT
            half3 outLut = ApplyLut2D(TEXTURE2D_ARGS(userLutTex, userLutSampler), input, userLutParams);
            input = lerp(input, outLut, userLutContrib);
            input.rgb = SRGBToLinear(input.rgb);
        }
    }

    return input;
}

