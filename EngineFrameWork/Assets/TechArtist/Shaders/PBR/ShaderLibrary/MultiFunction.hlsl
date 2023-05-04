
float FlagWaveW(half2 uv, half3 flagVariable) 
{
    float2 uv1;
    uv1 = (uv * 2 - 1);
    uv1 += 0.5;
    float cosAngle = cos(flagVariable.z);
    float sinAngle = sin(flagVariable.z);
    uv1 = mul(float2x2(cosAngle, -sinAngle, sinAngle, cosAngle), uv1);
    uv1 -= 0.5;
    float uvadd = uv1.x + uv1.y;
    uv1.x = uv1.x * 1.5;
    uv1.y = uv1.y * 4.5;
    float wavein = sin(uv1.x + uv1.y) * 3.141592 * flagVariable.x;
    float speed = uvadd - (_Time.x * flagVariable.y);
    float w = sin((speed + wavein) * 2 * 3.141592 * 0.6);
    return w;
}

half2 FlagWaveUV(half2 uv, half3 flagVariable)
{
    float w = FlagWaveW(uv, flagVariable);
    half2 uv2 = (0.02 - (w * 0.02) ) * saturate(1.3-uv.y) + uv;
    return uv2;
}

float3 FlagWave(half2 uv, half3 albedoColor,half4 flagVariable)
{
    
    float w = FlagWaveW(uv, flagVariable.xyz);
    float shain = w * 2 + flagVariable.w;
    shain=lerp(1,shain,saturate(1.3-uv.y));
    float vf = (1 - uv.x) * uv.x * 16 * uv.y * (1 - uv.y);
    float shain2 = 1 - (pow(2, vf * -1.75) * 0.6);
    float3 shadow = lerp(albedoColor, albedoColor * shain, 0.3);
    shadow *= shain2;
    float3 color = clamp(shadow, 0, 1);
    color += color;

    return color;
}


half4 GradientAlpha(half4 control,half3 edgeColor,Varyings input,half3 color,half4 scrappy)
{

#if defined(GRADIENT_ALPHA)
    float alphaPos = scrappy.y;
    float c = alphaPos - input.positionWS.y;
    float2 uv = float2(input.uv0.xy * control.w + float2(0, _Time.x * control.x * 10));
    float3 mask = SAMPLE_TEXTURE2D(_EffectMaskTex, sampler_EffectMaskTex, uv) - 0.01;
    float upWidth = control.y / 15;
    float widClip = step(alphaPos - upWidth, input.positionWS.y) * step(input.positionWS.y, alphaPos);
    float upWidGra = smoothstep(0, upWidth, alphaPos - input.positionWS.y);
    upWidGra = abs(upWidGra - 1) ;
    float downWidth = scrappy.x / 15;
    float downWidClip = step(alphaPos - upWidth - downWidth, input.positionWS.y) * step(input.positionWS.y, alphaPos - upWidth);
    //float downWidGra = smoothstep(0, downWidth / 2, alphaPos - upWidth - input.positionWS.y) * smoothstep(0, downWidth / 2, input.positionWS.y - (alphaPos - upWidth - downWidth)) * downWidClip;
    float downWidGra = smoothstep(0, downWidth, alphaPos - upWidth - input.positionWS.y);
    downWidGra = abs(downWidGra - 1);
    float maskC = lerp(abs(mask.g-1), mask.r, smoothstep(alphaPos - downWidth / 2 - upWidth, alphaPos - upWidth / 2, input.positionWS.y));
    float maskGra = lerp(downWidGra, upWidGra, smoothstep(alphaPos - downWidth / 2 - upWidth, alphaPos - upWidth / 2, input.positionWS.y));
    c = lerp(c, mask.r, widClip );
    clip(c - 0.5 * upWidGra);
    float edge = step(control.z / 5, lerp(0, abs(maskC - 1), (widClip + downWidClip) * saturate(maskGra + 0.2)));
    color = lerp(color.rgb, edgeColor, edge);

    float3 viewDirWS = float3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
    float fenel = 1 - max(0, dot(input.normalWS.rgb, normalize(viewDirWS.rgb)));
    fenel = saturate(pow(fenel, scrappy.z));
    color += fenel * edgeColor * abs(1 - smoothstep(scrappy.w, 2.5, scrappy.y));
    return half4(color, c - 0.5 * upWidGra);
#endif
    return half4(color, 1);
}