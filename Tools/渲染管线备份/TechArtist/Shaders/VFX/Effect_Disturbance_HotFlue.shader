Shader "NBA/Effect/Disturbance/HotFlue"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaTex ("Mask Texture",2D) = "white" {}
		[HDR]_TintColor("Tint Color", Color) = (1,1,1,1)
		_NoiseTex ("Noise Texture",2D) = "white" {}
		_Speed("Speed",Vector) = (0,0,0,0)
		_Force("Force",Range(0,0.2)) = 0
		//_GradientValue("不要修改！Effect Fade Value",Float) = 1
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
		}
		Blend SrcAlpha One
		ZWrite Off
		Cull Off
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _AlphaTex;
			uniform fixed4 _TintColor;
			uniform sampler2D _NoiseTex;
			uniform float4 _Speed;
			uniform fixed _Force;
			uniform fixed _GradientValue;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color * _TintColor;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				half2 offsetColor1 = tex2D(_NoiseTex, i.uv + _Time.xz * _Speed.x).xy;
				half2 offsetColor2 = tex2D(_NoiseTex, i.uv + _Time.yx * _Speed.y).xy;
				i.uv.x += ((offsetColor1.r + offsetColor2.r) - 1) * _Force;
				i.uv.y += ((offsetColor1.g + offsetColor2.g) - 1) * _Force;
				fixed4 finalColor = tex2D(_MainTex, i.uv);
				fixed mask = tex2D(_AlphaTex, i.uv).r;
				finalColor *= i.color;
				finalColor.a = finalColor.a * mask;
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				finalColor.a *= (1-_GradientValue);
				return finalColor;
			}
			ENDCG
		}
	}
}
