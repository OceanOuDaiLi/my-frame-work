// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "NBA/Effect/Cover(Add)" {
Properties {
	_MainTex ("太阳光晕形状", 2D) = "white" {}
	[HDR]_Color ("光晕颜色", Color) = (1, 1, 1, 1)
	_SunDistance ("阳光浓度", Float) = 5.0
	[Enum(None, 0, X, 1, Y, 2, Z, 3)] _Direction ("光晕朝向", Float) = 0
	//_GradientColor("不要修改！Effect Fade Color",Color) = (1.0,1.0,1.0,1.0)
}

SubShader {
	Tags {"Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="PostProcessing"}

	ZWrite Off
	ZTest Off
	Cull Off
	Blend SrcAlpha One

	Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half4 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex; sampler2D _CameraDepthTexture;
			float4 _MainTex_ST;
			fixed4 _Color;
			half _SunDistance;
			fixed _Direction;
			uniform fixed4 _GradientColor;

			v2f vert (appdata_t v)
			{
				v2f o;
				// 计算billboard
				half3 cameraPos = mul((float3x3)unity_WorldToObject, _WorldSpaceCameraPos);
				half3 viewDir = normalize(cameraPos);
				half3 upDir = half3(0, 1, 0);// 模型空间上
				half3 rightDir = normalize(cross(upDir, viewDir));// 视野右方向
				upDir = normalize(cross(viewDir, rightDir));// 视野上方向
				half3 vertexOffset = v.vertex.xyz;// 后续做billboard顶点色合批 v.vertex + v.color
				half3 localPos = v.vertex.xyz;
				// 广告牌轴向适配3dsMax的右手坐标系
				if(_Direction == 1)
				// X轴朝向摄像机
				localPos = -rightDir * vertexOffset.z + upDir * vertexOffset.y + viewDir * vertexOffset.x;
				else if(_Direction == 2)
				// Y轴朝向摄像机
				localPos = rightDir * vertexOffset.x + upDir * vertexOffset.z - viewDir * vertexOffset.y;
				else if(_Direction == 3)
				// Z轴朝向摄像机
				localPos = rightDir * vertexOffset.x - upDir * vertexOffset.y - viewDir * vertexOffset.z;

				o.vertex = UnityObjectToClipPos(float4(localPos, 1));
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				// 计算屏幕坐标,用于采样深度
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
				// 读取摄像机深度
				half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				depth = Linear01Depth(depth);
				depth = saturate(depth * _SunDistance);
				col *= depth;
				col.rgb *= fixed3(1-_GradientColor.r,1-_GradientColor.g,1-_GradientColor.b);
				//return depth;
				return col;
			}
		ENDCG
	}
}

}
