Shader "NBA/Effect/Disturbance/Wave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Range("Range",Range(-0.5,1)) = 0
		_Width("Width",Range(0,1)) = 0
		_OriginPos("Origin",Vector) = (0,0,0,0)
		_WaveTint("Wave Tint",Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 100
		//Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		ZTest Always
		Lighting Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			uniform fixed _Range;
			uniform fixed _Width;
			uniform float4 _OriginPos;
			uniform fixed4 _WaveTint;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv = i.uv - _OriginPos;
				uv.x *= _MainTex_TexelSize.y / _MainTex_TexelSize.x;//uv.x * width / height
				float2 center = float2(1,1) * _Range;
				float radius = sqrt(pow(uv.x,2)+pow(uv.y,2));//r^2 = x^2 + y^2
				fixed halfWidth = _Width/2;
				float centerR = _Range + halfWidth;
				//fixed4 col = tex2D(_MainTex, uv);
				fixed4 col = fixed4(0,0,0,0);
				float power = 1;
				if(radius > _Range &&  radius < _Range + _Width)
				{
					power = 1 - abs(radius - centerR)/halfWidth;//heave wave; width's centre > edge
					power *= 10;
					col = tex2D(_MainTex,i.uv + _MainTex_TexelSize.xy * power) * _WaveTint;
				}
				else
				{
					//col = 0;
					col = tex2D(_MainTex,i.uv);
				}
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
