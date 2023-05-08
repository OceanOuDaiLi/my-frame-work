Shader "NBA/Effect/Disturbance/Diffusion"
{
	Properties
	{
		_MainTex ("Diffusion Texture", 2D) = "white" {}
		_DissolveToggle("Intensity Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_Dissolve ("Intensity", Range(-1,1)) = 0
		_SpeedX("Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY("Vertical Speed",Range(-5.0, 5.0)) = 0
		_SpeedToggle("Speed Toggle",float) = 0.0
		// _GradientValue("不要修改！Effect Fade Value",Float) = 1
	}
		SubShader
	{
		Tags
		{
			"IgnoreProjector"="True"
			"Queue"="Transparent+500"
			"LightMode" = "UniversalForward"
			"RenderType"="Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100
		GrabPass
		{
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ACGameCG.cginc"
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 uv : TEXCOORD0;
				float4 control : TEXCOORD1;
				float4 projPos : TEXCOORD2;
				fixed4 color : COLOR;
			};
			uniform sampler2D _CameraOpaqueTexture;
			v2f vert (effect_in_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = v.texcoord.zw;
				o.color.rgba = v.color.rgba;
				o.control.xyzw = v.control.xyzw;
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				//half2 uv = half2(i.uv.x + _Time.y * _SpeedX,i.uv.y + _Time.y * _SpeedY);
				half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
				fixed4 col = tex2D(_MainTex, uv);
				
				clip(col.a - 0.05);
				// return col.a;
				float2 sceneUVs = i.projPos.xy/i.projPos.w + col.xy*col.a * i.color.a * lerp(_Dissolve,i.uv.z,_DissolveToggle);
				fixed4 Optex = tex2D(_CameraOpaqueTexture, sceneUVs);
				col.a *= (1-_GradientValue);
				Optex.a *= col.a;
				return Optex;
			}
			ENDCG
		}
	}


	SubShader
	{
		Tags
		{
			"IgnoreProjector"="True"
			"Queue"="Transparent+500"
			"RenderType"="Transparent"
			
		}
		LOD 100
		GrabPass
		{
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ACGameCG.cginc"
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 uv : TEXCOORD0;
				float4 control : TEXCOORD1;
				float4 projPos : TEXCOORD2;
				fixed4 color : COLOR;
			};
			uniform sampler2D _GrabTexture;
			v2f vert (effect_in_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = v.texcoord.zw;
				o.color.rgba = v.color.rgba;
				o.control.xyzw = v.control.xyzw;
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				//half2 uv = half2(i.uv.x + _Time.y * _SpeedX,i.uv.y + _Time.y * _SpeedY);
				half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
				fixed4 col = tex2D(_MainTex, uv);
				clip(col.a - 0.05);
				float2 sceneUVs = i.projPos.xy/i.projPos.w + col.xy*col.a * i.color.a * lerp(_Dissolve,i.uv.z,_DissolveToggle);
				col = tex2D(_GrabTexture, sceneUVs);
				col.a *= (1-_GradientValue);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Mobile/Particles/Additive"
}
