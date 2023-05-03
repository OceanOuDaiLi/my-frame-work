Shader "NBA/Effect/Weapon(Effect)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color ("Emission Color", Color) = (1,1,1,1)
		_OutLineWidth ("OutLineWidth", Range(0.0001, 0.01)) = 0.0001
		[HDR]_OutLineColor("OutLine Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
		}
		LOD 100
		Pass
		{
			Name "Outline"
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			uniform float _OutLineWidth;
			uniform fixed4 _OutLineColor;
			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord0 : TEXCOORD0;
			};
			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			VertexOutput vert (VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv.xy = TRANSFORM_TEX(v.texcoord0.xy,_MainTex).xy;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*_OutLineWidth,1));
				return o;
			}
			fixed4 frag (VertexOutput i) : SV_Target
			{
				fixed4 color = fixed4(0,0,0,1);
				fixed3 albedo = tex2D(_MainTex,i.uv.xy);
				color.rgb += albedo;
				color.rgb = pow(color.rgb*_OutLineColor.rgb,1.5);
				return color;
			}
			ENDCG
		}
		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
				"Queue" = "Opaque"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "AutoLight.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord0 : TEXCOORD0;
			};
			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			VertexOutput vert (VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv.xy = TRANSFORM_TEX(v.texcoord0.xy,_MainTex).xy;
			  o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			fixed4 frag (VertexOutput i) : SV_Target
			{
				fixed4 color = fixed4(0,0,0,1);
				fixed3 albedo = tex2D(_MainTex,i.uv.xy);
				color.rgb += albedo;
				return color;
			}
			ENDCG
		}
	}
}
