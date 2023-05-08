Shader "Universal Render Pipeline/DistortBlit"
{
    	Properties
	{
		[HideInInspector]_MainTex("Main Texture", 2D) = "white" {}
		[HideInInspector]_DistortTex("DistortTex", 2D) = "white" {}

	}
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Blit"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment
            #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
            #pragma multi_compile _ _USE_DRAW_PROCEDURAL

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            TEXTURE2D_X(_SourceTex);
            SAMPLER(sampler_SourceTex);

            TEXTURE2D_X(_DistortTex);
            SAMPLER(sampler_DistortTex);

			sampler2D _MainTex;
			float4 _MainTex_ST;



            half4 Fragment(Varyings input) : SV_Target
            {
                //UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                //half4 col = SAMPLE_TEXTURE2D_X(_SourceTex, sampler_SourceTex, input.uv);
                float4 col = tex2D( _MainTex, input.uv );


                //float isFrontFace = (facing >= 0 ? 1 : 0);
                //float faceSign = (facing >= 0 ? 1 : -1);
                //float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));

               	//float2 screenPos01 =(i.screenPos.xy / i.screenPos.w) + (float2(_MainTex_var.r,_MainTex_var.g)*_MainTex_var.a*i.vertexColor.a*_Intensity);

                //float4 sceneColor = SAMPLE_TEXTURE2D(_AfterPostProcessTexture, sampler_AfterPostProcessTexture, screenPos01);
                //float4 sceneColor = SAMPLE_TEXTURE2D(_AfterPostProcessTexture, sampler_AfterPostProcessTexture, screenPos01);


                //float3 finalColor = 0;
                //half4 finalRGBA = half4(lerp(sceneColor.rgb, finalColor,0.0),1);

            #ifdef _LINEAR_TO_SRGB_CONVERSION
                col = LinearToSRGB(col);
             #endif

                return col;
            }
            ENDHLSL
        }
    }
}
