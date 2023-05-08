Shader "NBA/Effect/Disturbance/Base"
{
	Properties
	{
		_MainTex ("扰动图", 2D) = "white" {}
		//_MoveForce  ("扰动强度", range (0,1)) = 0.5
		_Dissolve ("扰动强度", Range(-1,1)) = 0
		_DissolveToggle("Disturbance Toggle(0 : Value; 1 : Custom)",float) = 0.0
		_SpeedX("Horizontal Speed", Range(-5.0, 5.0)) = 0
		_SpeedY("Vertical Speed",Range(-5.0, 5.0)) = 0
		_SpeedToggle("Speed Toggle",float) = 0.0
		//_GradientValue("不要修改！Effect Fade Value",Float) = 1
	}

	Category
	{
		Tags
		{
			"IgnoreProjector" = "True"
			"Queue"="Transparent+500"
			"RenderType"="Transparent"
		}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off

		SubShader
		{
			GrabPass
			{
				Name "BASE"
			}

			Pass
			{
				Name "BASE"
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#include "ACGameCG.cginc"
				struct v2f
				{
					float4 vertex : POSITION;
					half4 uv : TEXCOORD0;
					float4 control : TEXCOORD1;
					half4 uvGrab : TEXCOORD2;
					fixed alpha :COLOR;
				};
				uniform sampler2D _GrabTexture;
				v2f vert (effect_in_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
					#else
					float scale = 1.0;
					#endif
					o.uvGrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
					o.uvGrab.zw = o.vertex.zw;
					o.alpha = v.color.a;
					o.control.xyzw = v.control.xyzw;
					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
					o.uv.zw = v.texcoord.zw;
					return o;
				}

				half4 frag( v2f i ) : COLOR
				{
					half2 uv = GetAnimationUV(i.uv.xy,i.control.xy,half2(_SpeedX,_SpeedY));
					half2 offsetColor = tex2D(_MainTex, uv).rg;
					i.uvGrab.xy += offsetColor.rg * i.alpha * lerp(_Dissolve,i.uv.z,_DissolveToggle);
					half4 finalColor = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvGrab));
					finalColor.a = 1;
					finalColor.a *= (1-_GradientValue);
					return  finalColor;
				}
				ENDCG
			}
		}
		FallBack "Mobile/Particles/Additive"
	}
}
