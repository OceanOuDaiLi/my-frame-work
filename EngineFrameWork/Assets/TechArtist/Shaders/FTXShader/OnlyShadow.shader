Shader "FTX Game/Character/Only Shadow"
{
    Properties
    {
        _ShadowColor("Shadow Color", Color) = (0,0,0,1)
        _ShadowStrength("Shadow Strength", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags{"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "Queue"="Transparent"}
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
         
            CBUFFER_START(UnityPerMaterial)

                half3 _ShadowColor;
                half _ShadowStrength;  

            CBUFFER_END

            struct Attributes
            {
                float3 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;   
            };
                 
     
            Varyings vert(Attributes inputData)
            {
                Varyings outputData = (Varyings)0;
                outputData.positionWS = TransformObjectToWorld(inputData.positionOS);
                outputData.positionCS = TransformWorldToHClip(outputData.positionWS);
                return outputData;
            }

            half4 frag(Varyings inputData) : SV_TARGET 
            {  
                half4 shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                half shadow = 1 - MainLightRealtimeShadow(shadowCoord);
                return half4(_ShadowColor, shadow * _ShadowStrength);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
