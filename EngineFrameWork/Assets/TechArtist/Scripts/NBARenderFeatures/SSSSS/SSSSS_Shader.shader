Shader "Hidden/SSSSSShader"
{
	Properties
	{
		//_MainTex("Texture", 2D) = "" {}
	}

	HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		struct v2f
		{			
			float4 positionCS		:	SV_POSITION;
			half2 uv				:	VAR_SCREEN_UV;
		};

		v2f vert(uint vertexID : SV_VertexID)
		{
			v2f o;
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
		
	ENDHLSL

	SubShader
	{
		Stencil 
		{  
			Ref 2 
			Comp Equal
			Pass keep
			Fail keep
			ZFail keep
		} 

		// No culling or depth
		Cull Off 
		ZWrite On 
		ZTest Always

		//Separable depth based blur
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			CBUFFER_START(UnityPerMaterial)				
				half4 _BlurVec;
				float _BlurStr;
				float _SoftDepthBias;
			CBUFFER_END
            
			TEXTURE2D (_MainTex);
			TEXTURE2D (_CameraDepthTexture);		
			SAMPLER(sampler_LinearClamp);

			inline float SAMPLE_INVERSE_DEPTH(float2 uvs) 
			{
				float t = unity_CameraProjection._m11;
				float z = LinearEyeDepth(SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_LinearClamp, uvs).r,_ZBufferParams);
				
				//Voodoo math on visible size multiplier here that i dont understand, but it works
				
				float size = 0.5/(z + 0.5);
				return size * t * 0.6;
			}

			inline float SAMPLE_INVERSE_DEPTH_LINEAR(float2 uvs) 
			{
				return saturate(1.0 - Linear01Depth(SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_LinearClamp, uvs).r,_ZBufferParams));
			}

			float4 frag (v2f i) : SV_Target
			{
				//If theres nothing in the mask texture, then we dont need to blur/process it
				//half testMask = SAMPLE_TEXTURE2D(_MaskTex,sampler_MaskTex, i.uv).r;
				//if (testMask.r + testMask.g + testMask.b  < 0.005) discard;

				half4 col = SAMPLE_TEXTURE2D(_MainTex,sampler_LinearClamp, i.uv);
				half2 blurvec = _BlurVec.xy;
				float d = SAMPLE_INVERSE_DEPTH(i.uv);
				float str = _BlurStr * d;

				float dlin = SAMPLE_INVERSE_DEPTH_LINEAR(i.uv);
				//if (dlin < 0.1) discard;

				//Gaussian blur				
				col *= 0.38;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv + blurvec * 1 * str) * 0.18;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv - blurvec * 1 * str) * 0.18;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv + blurvec * 2 * str) * 0.09;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv - blurvec * 2 * str) * 0.09;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv + blurvec * 3 * str) * 0.04;
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, i.uv - blurvec * 3 * str) * 0.04;
				

				//Blur with depth check
				/**/
				float sum = 1;
				float diff = 0;
				float cont = 0;

				//HACK:
				//We multiply the depth sample vector by a 1.06 to get rid of the 1 pixel wide lines, 
				//that show up on the edges where the depth difference between samples is significant, 
				//i have no clue whats causing it and how to fix it properly :(
				//for (uint n = 1; n <= 2; n++) 
				//{
				//	float contrib_base = 0.99 / (n + 2);

				//	half4 colr = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.uv + blurvec * n * str);
				//	float dr = SAMPLE_INVERSE_DEPTH_LINEAR(i.uv + blurvec * n * str * 1.06);
				//	diff = abs(dlin - dr);
				//	cont = 1.0-saturate(diff / _SoftDepthBias);
				//	cont *= contrib_base;
				//	col.rgb += colr.rgb*cont;
				//	sum += cont;

				//	half4 coll = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.uv - blurvec * n * str);
				//	float dl = SAMPLE_INVERSE_DEPTH_LINEAR(i.uv - blurvec * n * str * 1.06);
				//	diff = abs(dlin - dl);
				//	cont = 1.0 - saturate(diff / _SoftDepthBias);
				//	cont *= contrib_base;
				//	col.rgb += coll.rgb*cont;
				//	sum += cont;
				//}
				//col.rgb /= sum;
				//return testMask;
				return col;
			}
			ENDHLSL
		}

		//Combine pass
		Pass 
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			CBUFFER_START(UnityPerMaterial)
				half _EffectStr;
				half _PreserveOriginal;
				half4 _SSSSSColor;
			CBUFFER_END
		   
			TEXTURE2D (_CameraOpaqueTexture);
			TEXTURE2D (_BlurTex);				
			SAMPLER(sampler_LinearClamp);

			float4 frag(v2f i) : SV_Target
			{
				half4 src = SAMPLE_TEXTURE2D(_CameraOpaqueTexture ,  sampler_LinearClamp, i.uv);
				float fac = 1 - pow(saturate(max(max(src.r, src.g), src.b) * 1), 0.5);
				half4 blr = SAMPLE_TEXTURE2D(_BlurTex , sampler_LinearClamp , i.uv);
				//return blr;
				//return src;
				blr = clamp(blr - src * _PreserveOriginal, 0, 50);
				blr *= _SSSSSColor;
				return src + blr * fac * _EffectStr;
			}
			ENDHLSL
		}
	}
}
