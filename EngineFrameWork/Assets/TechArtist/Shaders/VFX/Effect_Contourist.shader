Shader "NBA/Effect/DefaultPlus/Contourist(AlphaTest and VFACE)"
{
	Properties
	{
		_MainTex ("Contourist Texture", 2D) = "white" {}
		[HDR]_CeilColorA ("Contourist Color A", Color) = (1,1,1,1)
		[HDR]_CeilColorB ("Contourist Color B", Color) = (1,1,1,1)
		_Power("Contourist Pow",Range(-5,500)) = 1
		_UVRotate ("UV Rotate(1:90 2:180 3:270)",Range(0,4)) = 0
		[HDR]_Color("Main Color", Color) = (1.0,1.0,1.0,1.0)
		[HDR]_Color2("Back Color", Color) = (1.0,1.0,1.0,1.0)
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_Dissolve("Dissolve Value", Range(-1,1)) = 0
		_DissolveToggle("Dissolve Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_SpeedX("Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY("Vertical Speed",Range(-5.0, 5.0)) = 0
		_SpeedX2("Noise Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY2("Noise Vertical Speed",Range(-5.0, 5.0)) = 0
		_SpeedToggle("Noise Speed Toggle",float) = 0.0
		_OutLineWidth ("OutLineWidth", Range(0.0001, 0.01)) = 0.0001
		[HDR]_OutLineColor("OutLine Color", Color) = (1.0,1.0,1.0,1.0)
		//_GradientValue("不要修改！Effect Fade Value",Float) = 1
	}
	SubShader
	{
		Tags
		{
			"IgnoreProjector"="True"
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}
		LOD 100
		//Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		//ZWrite Off
		Pass
		{
			CGPROGRAM
			#pragma vertex effectVertexDiss
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ACGameCG.cginc"
			fixed4 frag (effect_v2f_diss i, float facing : VFACE) : SV_Target
			{
				//facing
				fixed ifFrontFace = (facing >= 0 ? 1 : 0);
				fixed4 color = fixed4(0,0,0,1);
				half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
				half2 uv2 = GetAnimationUV(i.uv1.xy,i.control.zw,half2(_SpeedX2,_SpeedY2));
				//half2 uv = GetAnimationUV2(i.uv.xy);
				//half2 uv2 = GetAnimationUV(i.uv1.xy,i.control.xy);
				//fixed4 main_var = tex2D(_MainTex, uv);
				fixed posValue = tex2D(_MainTex, uv).r;
				fixed4 main_var = CeilColor(posValue, i.uv.w);
				fixed4 diss_var = tex2D(_DissolveTex,uv2);
				half dissValue = lerp(_Dissolve,i.uv.z,_DissolveToggle) * (-1.2) + 0.1;//-0.1 ~ 1.1
				float ClipAmount = diss_var.r + dissValue;
				clip(ClipAmount);
				color.rgb = lerp(_Color2.rgb,main_var.rgb,ifFrontFace);
				color.rgb = color.rgb * i.color.rgb * _Color.rgb;
				//color.a = saturate(color.a * i.color.a * _Color.a * step(0,ClipAmount));
				//color.a *= _FadeValue;
				return color;
			}
			ENDCG
		}
	}

}
