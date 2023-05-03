Shader "Hidden/BloomShader"
{
	Properties
	{
        //_MainTex("Source", 2D) = "black" {}
	}

	HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

    #define _LUMINACE       half3(0.2125,0.7154,0.0721)

    CBUFFER_START(UnityPerMaterial)

        half _BloomThreshold;
        half _BloomIntensity;
        half _BloomScatter;
        half4 _BloomColor;
        half4 _MainTex_TexelSize;
        
    CBUFFER_END
        
    TEXTURE2D(_MainTex);
    TEXTURE2D(_BloomTex);
    SAMPLER(sampler_LinearClamp);

    struct Varyings
    {
        float4 positionCS 	:	SV_POSITION;
        half2 uv 	        :	VAR_SCREEN_UV;
    };

    struct VaryingsBlur
    {
        float4 positionCS 	:	SV_POSITION;
        half2 uv 	        :	VAR_SCREEN_UV;
        half2 uvBlur        :   TEXCOORD1;
    };

    Varyings vert(uint vertexID : SV_VertexID)
    {
        Varyings o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.positionCS = float4(
		    vertexID <= 1 ? -1.0 : 3.0,
            vertexID == 1 ? -3.0 : 1.0,
            0.0 , 1.0 
        );
        o.uv = float2(
            vertexID <= 1 ? 0.0 : 2.0,
            vertexID == 1 ? 2.0 : 0.0
        );
        return o;
    }

    VaryingsBlur vertBlur(uint vertexID : SV_VertexID)
    {
        VaryingsBlur o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.positionCS = float4(
            vertexID <= 1 ? -1.0 : 3.0,
            vertexID == 1 ? -3.0 : 1.0,
            0.0 , 1.0 
        );
        o.uv = float2(
            vertexID <= 1 ? 0.0 : 2.0,
            vertexID == 1 ? 2.0 : 0.0
        );
        o.uvBlur = half2(_MainTex_TexelSize.x * _BloomScatter , 2 * _MainTex_TexelSize.x * _BloomScatter);
        return o;
    }
    
    half3 SAMPLE_TEXTURE2D_Luminance(half2 uv)
    {
        half3 col = SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , uv);
        half bloom = clamp(dot(_LUMINACE , col) - _BloomThreshold , 0.0 , 1.0);
        return bloom * col;
    }

    half3 GEtSourceBicubic(half2 screenUV)
    {
        return SampleTexture2DBicubic(TEXTURE2D_ARGS(_MainTex , sampler_LinearClamp),screenUV,_MainTex_TexelSize.zwxy,1.0,0.0).rgb;
    }

	ENDHLSL

	Subshader
	{
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		ZTest Always 
		ZWrite Off 
		Cull Off

		Pass
		{
            Name "Bloom Blur Horizontal"           

			HLSLPROGRAM

			#pragma vertex vertBlur
			#pragma fragment fragHorizontal

			half4 fragHorizontal(VaryingsBlur i) : SV_Target
			{
                half3 col;
                col = 0.0545 * SAMPLE_TEXTURE2D_Luminance(i.uv + half2(-i.uvBlur.y , 0));
                col += 0.2442 * SAMPLE_TEXTURE2D_Luminance(i.uv + half2(-i.uvBlur.x , 0));
                col += 0.4026 * SAMPLE_TEXTURE2D_Luminance(i.uv);
                col += 0.2442 * SAMPLE_TEXTURE2D_Luminance(i.uv + half2(i.uvBlur.x , 0));    
                col += 0.0545 * SAMPLE_TEXTURE2D_Luminance(i.uv + half2(i.uvBlur.y , 0));
				return half4(col,0);
			}
		  	ENDHLSL
		}

		Pass
		{
            Name "Bloom Blur Vertical"

            HLSLPROGRAM

            #pragma vertex vertBlur
            #pragma fragment fragVertical

            half4 fragVertical(VaryingsBlur i) : SV_Target
			{
                half3 col;
                col = 0.0545 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(0 , -i.uvBlur.y));
                col += 0.2442 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(0 , -i.uvBlur.x));
                col += 0.4026 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv);
                col += 0.2442 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(0 , i.uvBlur.x));    
                col += 0.0545 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(0 , i.uvBlur.y));
				return half4(col,0);
			}
            ENDHLSL
		}

		Pass
		{
            Name "Bloom Downsample"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment fragDownsample

            half4 fragDownsample(VaryingsBlur i) : SV_Target
			{
                half3 col;
                half2 uv = i.uv;

                half2 uv1 = half2((uv.y - 0.6667f) * 3.0f , (uv.x - 0.75f) * 4.0f);
                half2 uv2 = half2(uv.x * 1.5f , (uv.y - 0.64f) * 2.7778f);
                half2 uv3 = half2(uv.x , uv.y * 1.6667f);

                half t = step(0.6667f, uv.x);
                uv1 = lerp(uv2 , uv1 , t);

                half t2 = step(0.6f, uv.y);
                uv = lerp(uv3 , uv1 ,t2);   

                half b = step(1 , uv.x);
                b += step(uv.x , 0);
                b += step(1 , uv.y);
                b += step(uv.y , 0);
                b *= 0.25f;
                b = step(b , 0);

                half3 col1 = half3(0,0,0);
                half3 col2 = SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , uv) * 4.0f;
                col2 += SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , half2(uv.x - _MainTex_TexelSize.x, uv.y - _MainTex_TexelSize.y));
                col2 += SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , half2(uv.x + _MainTex_TexelSize.x, uv.y - _MainTex_TexelSize.y));
                col2 += SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , half2(uv.x - _MainTex_TexelSize.x, uv.y + _MainTex_TexelSize.y));
                col2 += SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , half2(uv.x + _MainTex_TexelSize.x, uv.y + _MainTex_TexelSize.y));
                col2 *= 0.125f;

                col = lerp(col1 , col2 , b);

				return half4(col,0);
			}
            ENDHLSL
		}

		Pass
		{
            Name "Bloom Atlas Blur Horizontal"

            HLSLPROGRAM

            #pragma vertex vertBlur
            #pragma fragment fragAtlasHorizontal

            half4 fragAtlasHorizontal(VaryingsBlur i) : SV_Target
            {
                half3 col;
                col = 0.0545 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(-i.uvBlur.y , 0)).rgb;
                col += 0.2442 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(-i.uvBlur.x , 0)).rgb;
                col += 0.4026 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv);
                col += 0.2442 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(i.uvBlur.x , 0)).rgb;    
                col += 0.0545 * SAMPLE_TEXTURE2D_X(_MainTex , sampler_LinearClamp , i.uv + half2(i.uvBlur.y , 0)).rgb;
                return half4(col,0);
            }
            ENDHLSL
		}

        Pass
		{
            Name "Bloom Atlas Blur Vertical"

            HLSLPROGRAM

            #pragma vertex vertBlur
            #pragma fragment fragAtlasVertical

            half4 fragAtlasVertical(VaryingsBlur i) : SV_Target
            {
                half3 col;
                col = 0.0545 * GEtSourceBicubic(i.uv + half2(0 , -i.uvBlur.y));
                col += 0.2442 * GEtSourceBicubic(i.uv + half2(0 , -i.uvBlur.x));
                col += 0.4026 * GEtSourceBicubic(i.uv);
                col += 0.2442 * GEtSourceBicubic(i.uv + half2(0 , i.uvBlur.x));    
                col += 0.0545 * GEtSourceBicubic(i.uv + half2(0 , i.uvBlur.y));            
                return half4(col,0);
            }
            ENDHLSL
		}

        Pass
		{
            Name "Bloom Upsample"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment fragUpsample

            half4 fragUpsample(VaryingsBlur i) : SV_Target
            {
                half3 col;
                col = GEtSourceBicubic(half2(i.uv.x , i.uv.y * 0.6f)) * 0.8f;
                col += GEtSourceBicubic(half2(i.uv.x * 0.6667f , i.uv.y * 0.36f + 0.64f)) * 0.6f;
                col += GEtSourceBicubic(half2(i.uv.y * 0.25f + 0.75f , i.uv.x * 0.3333f + 0.6667f)) * 0.2f;

                half3 dualCol;
                dualCol = SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x + _MainTex_TexelSize.x * 2.0f , i.uv.y)).rgb;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x - _MainTex_TexelSize.x * 2.0f , i.uv.y)).rgb;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x , i.uv.y + _MainTex_TexelSize.y * 2.0)).rgb;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x , i.uv.y - _MainTex_TexelSize.y * 2.0)).rgb;

                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x + _MainTex_TexelSize.x , i.uv.y + _MainTex_TexelSize.y)).rgb * 2.0f;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x - _MainTex_TexelSize.x , i.uv.y + _MainTex_TexelSize.y)).rgb * 2.0f;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x + _MainTex_TexelSize.x , i.uv.y - _MainTex_TexelSize.y)).rgb * 2.0f;
                dualCol += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , half2(i.uv.x - _MainTex_TexelSize.x , i.uv.y - _MainTex_TexelSize.y)).rgb * 2.0f;

                dualCol *= 0.0833f;
                
				return half4(col + dualCol , 0);
            }
            ENDHLSL
        }

        Pass
		{
            Name "Bloom Add"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment fragAdd

            half4 fragAdd(VaryingsBlur i) : SV_Target
            {
                half3 col;
                col = GEtSourceBicubic(i.uv);
                col += SAMPLE_TEXTURE2D_X(_BloomTex , sampler_LinearClamp , i.uv);
				return half4(col , 0);
            }
            ENDHLSL
        }
	}
	Fallback off
}