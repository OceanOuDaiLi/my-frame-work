Shader "NBA/Effect/DefaultPlus/Alpha Blend UVAnimation Dissolve Mask(VFACE)"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainTexBack ("Back Texture", 2D) = "white" {}
		_UVRotate ("UV Rotate(1:90 2:180 3:270)",Range(0,4)) = 0
		[HDR]_Color ("Main Color", Color) = (1,1,1,1)
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_Dissolve ("Dissolve Value", Range (-2, 2)) = 0
		_DissolveToggle("Dissolve Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_Mask ("Mask Value",Range(-1,1)) = 0
		_MaskToggle ("Mask Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_SpeedX("U Speed",Range(-5,5)) = 0
		_SpeedY("V Speed",Range(-5,5)) = 0
		_SpeedX2("Mask U Speed",Range(-5,5)) = 0
		_SpeedY2("Mask V Spedd",Range(-5,5)) = 0
		_SpeedToggle("Speed Toggle",float) = 0.0
		[Toggle]_SamplerToggle("Sampler Toggle",Int) = 0.0
		//_GradientValue("不要修改！Effect Fade Value",Float) = 1
	}
	Category
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		ZWrite Off
		Lighting Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask RGB
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex effectVertexDima
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma shader_feature _SAMPLERTOGGLE_ON
				#include "UnityCG.cginc"
				#include "ACGameCG.cginc"
				uniform sampler2D _MainTexBack;
				
				fixed4 frag (effect_v2f_dima i, float facing : VFACE) : SV_Target
				{
					//facing
					fixed ifFrontFace = (facing >= 0 ? 1 : 0);
					fixed4 color = fixed4(0,0,0,1);
					half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
					#ifdef _SAMPLERTOGGLE_ON
        				half2 uv2 = GetAnimationUV(i.uv1.xy,i.control.zw,half2(_SpeedX2,_SpeedY2));
						fixed clipValue = tex2D (_DissolveTex, uv2).r ;
						fixed mask = tex2D(_MaskTex,i.uv1.zw).r;
				    #else
				        half2 uv2 = GetAnimationUV(i.uv1.zw,i.control.zw,half2(_SpeedX2,_SpeedY2));
				        fixed clipValue = tex2D (_DissolveTex, i.uv1.xy).r;
				        fixed mask = tex2D(_MaskTex,uv2).r;
				    #endif
					//half2 uv2 = GetAnimationUV(i.uv1.xy,i.control.zw,half2(_SpeedX2,_SpeedY2));
					//fixed clipValue = tex2D (_DissolveTex, uv2).r ;
					half dissValue = lerp(_Dissolve,i.uv.z,_DissolveToggle) * (-1.2) + 0.1;//-0.1 ~ 1.1
					float ClipAmount = clipValue + dissValue;
					//fixed mask = tex2D(_MaskTex,i.uv1.zw).r;
					color = tex2D(_MainTex, uv);
					fixed4 back = tex2D(_MainTexBack, uv);
					color = lerp(back,color,ifFrontFace);
					color.rgb = color.rgb * 2.0 * _Color.rgb * i.color.rgb;
					color.a = color.a * i.color.a * saturate(mask - lerp(_Mask,i.uv.w,_MaskToggle)) * _Color.a  * step(0,ClipAmount);
					color.a = saturate(color.a);
					color.a *= (1-_GradientValue);
					return color;
				}
				ENDCG
			}
		}
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					constantColor [_TintColor]
					combine constant * primary
				}
				SetTexture [_MainTex]
				{
					combine texture * previous DOUBLE
				}
			}
		}
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					combine texture * primary
				}
			}
		}
	}
}
