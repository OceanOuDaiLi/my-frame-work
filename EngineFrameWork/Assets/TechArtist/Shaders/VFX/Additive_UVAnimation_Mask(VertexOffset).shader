Shader "NBA/Effect/DefaultPlus/Additive UVAnimation Mask(VertexOffset)"
{
	Properties
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		_UVRotate ("UV Rotate(1:90 2:180 3:270)",Range(0,4)) = 0
		[HDR]_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MaskTex ("Mask", 2D) = "white" {}
		_Mask("Mask Value",Range(-1,1)) = 0
		_MaskToggle("Mask Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_SpeedX("Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY("Vertical Speed",Range(-5.0, 5.0)) = 0
		_SpeedToggle("Speed Toggle",float) = 0.0
		_NormalFactor("Z Depth Offset",Range(-1.0,1.0)) = 0
		_DissolveTex("Offset Texture",2D) = "white" {}
		_Dissolve("Vertex Power",Range(0.0,20.0)) = 1.0
		_SpeedX2("Offset Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY2("Offset Vertical Speed",Range(-5.0, 5.0)) = 0
		//_GradientValue("不要修改！Effect Fade Value",Float) = 1
	}
	Category
	{
		Tags
		{
			"Queue"="Transparent+2"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		Blend SrcAlpha One
		Cull Off
		Lighting Off
		ZWrite Off
		SubShader
		{
			CGINCLUDE
			#include "UnityCG.cginc"
			#include "ACGameCG.cginc"
			ENDCG
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				struct appdate_eff
				{
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;//xy:main's uv; zw:dissolve and mask
					float4 control : TEXCOORD1;//xy:main's uv animation; zw:dissolve's uv animation
					float4 texcoord2 : TEXCOORD2;//
					fixed4 color : COLOR;
					float3 normal : NORMAL;
				};
				effect_v2f_diss vert(appdate_eff v)
				{
					effect_v2f_diss o = (effect_v2f_diss)0;
					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
					half2 uv = TRANSFORM_TEX(v.texcoord.xy, _DissolveTex);
					o.uv.xy = FlipUV(o.uv.xy);
					o.uv.zw = v.texcoord.zw;
					o.uv1.xy = TRANSFORM_TEX(v.texcoord2.xy,_MaskTex);
					o.color.rgba = v.color.rgba;
					o.control.xyzw = v.control.xyzw;
					//vertex Offset
					//uv = GetAnimationUV2(uv);
					uv = GetAnimationUV(uv,v.control.zw,half2(_SpeedX2,_SpeedY2));
					fixed4 offset = tex2Dlod(_DissolveTex,float4(uv,0.0,0.0));
					v.vertex.xyz += offset.xyz * v.normal.xyz * _Dissolve;
					o.vertex = UnityObjectToClipPos(v.vertex);
					//o.vertex.z += _NormalFactor;
					return o;
				}

				fixed4 frag (effect_v2f_diss i) : SV_Target
				{
					//half2 uv = GetAnimationUV(i.uv.xy,i.control.xy);
					half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
					//half2 uv2 = GetAnimationUV2(i.uv1.xy);
					half2 uv2 = i.uv1.xy;
					fixed4 color = tex2D (_MainTex, uv);
					fixed mask = tex2D (_MaskTex, uv2).r;
					color.rgb = color.rgb * _Color.rgb * i.color.rgb * 2;
					color.a = color.a * i.color.a * saturate(mask - lerp(_Mask,i.uv.w,_MaskToggle)) * _Color.a;
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
