Shader "NBA/Effect/Scar"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
		[HDR]_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		//_Alpha ("Alpha", Range(0,1)) = 1
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent+1" 
			"DisableBatching" = "True"
		}
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
			};

			v2f vert (float3 v : POSITION)
			{
				v2f o = (v2f)0;
				o.pos = UnityObjectToClipPos (float4(v,1));
				o.uv = v.xz+0.5;
				o.screenUV = ComputeScreenPos (o.pos);
				//o.ray = mul (UNITY_MATRIX_MV, float4(v,1)).xyz * float3(-1,-1,1);
				o.ray = UnityObjectToViewPos(float4(v,1)) . xyz * float3(-1,-1,1);
				return o;
			}

			//CBUFFER_START(UnityPerCamera2)
			// float4x4 _CameraToWorld;//此矩阵在可由代码赋值。 "renderer.material.SetMatrix("_CameraToWorld", camera.worldToCameraMatrix);"
			//CBUFFER_END

			uniform sampler2D _MainTex;
		    uniform sampler2D _CameraDepthTexture;
			uniform fixed4 _Color;
			//uniform fixed _Alpha;

			fixed4 frag(v2f i) : SV_Target
			{
			    //下面这里是将射线的长度变为整个深度大小。然后后面再乘以归一化的深度。这样做很巧妙。之前用方向 乘以具体深度值和这里思路一样。不过下面的做法计算更精简。
				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float2 uv = i.screenUV.xy / i.screenUV.w;
				// read depth and reconstruct world position  ——读取深度重新构建世界位置。
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth (depth);
				float4 vpos = float4(i.ray * depth,1);
				float3 wpos = mul (unity_CameraToWorld, vpos).xyz;
				float3 opos = mul (unity_WorldToObject, float4(wpos,1)).xyz;
				//使用clip来处理越界的像素。
				clip (float3(0.5,0.5,0.5) - abs(opos.xyz));
				//将本地位置转化为UV坐标。
				i.uv = opos.xz+0.5;
				fixed4 col = tex2D (_MainTex, i.uv);
				//col.a *= _Alpha;
				col *= _Color;
				return col;
			}
			ENDCG
		}        
	}
	FallBack "Unlit/Transparent"
}

