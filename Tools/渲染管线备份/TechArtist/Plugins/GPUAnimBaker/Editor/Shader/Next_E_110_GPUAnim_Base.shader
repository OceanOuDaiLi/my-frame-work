Shader "Next_E/11_GPUAnim_Base"
{
	Properties
	{
 
		_AnimMap ("动画缓存图", 2D) = "black" {}
		_AnimSpeed ("播放速度", Range(-100.0, 100.0)) = 10.0
 		_MainTex ("颜色贴图", 2D) = "white" {}

		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	Category
	{
		Tags
		{
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
	        "RenderType"="Transparent"
		}
		Blend [_SrcBlend] [_DstBlend]
		Lighting Off
		Cull Back
		ZWrite [_ZWrite]

		// tex2Dlod方法需要sm3.0支持
		SubShader
		{
			pass
			{
				CGPROGRAM
            	#pragma vertex vert
            	#pragma fragment frag
            	#include "UnityCG.cginc"
				#pragma target 3.0

				uniform sampler2D _MainTex; 
				uniform float4 _MainTex_ST;

				// 动画缓存图及其尺寸参数
				uniform sampler2D _AnimMap;	
				uniform float4 _AnimMap_TexelSize;
 	
				uniform half _AnimSpeed;
	 
				// AnimMap采样UV计算方法
				//frac 返回标量或矢量的小数
				float2 GetAnimUV(float2 indexUV, float animMapTexelSizeX, float evaluationValue)
				{
					float vertIndex = indexUV.x;
					float meshIndex = indexUV.y;

					float2 animUV = float2((vertIndex + 0.5) * _AnimMap_TexelSize.x , 0.0);
					animUV.y = frac(evaluationValue);
					return animUV;
				}

				struct VertexInput {
	                float2 texcoord : TEXCOORD0;
					// uv4为定点序号信息 X-顶点序号 Y-SubMesh序号 GPUAnimBaker.BakeMesh
					float2 texcoord3 : TEXCOORD3;
	            };
				
				//SV_前缀的变量代表system value
				// SV_POSTION一旦被作为vertex shader的输出语义，那么这个最终的顶点位置就被固定了,直接进入光栅化处理
				//如果作为fragment shader的输入语义那么和POSITION是一样的
	            struct VertexOutput {
	                float4 pos : SV_POSITION;
	                float2 uv : TEXCOORD0;
					 
	            };

	            VertexOutput vert (VertexInput v) {

					// 初始化 
	                VertexOutput o = (VertexOutput)0;


					/*
					https://docs.unity3d.com/cn/current/Manual/SL-UnityShaderVariables.html
					//t是自该场景加载开始所经过的时间，4个分量分别是 (t/20, t, t*2, t*3) 一直增加
						_Time   float4  time (t/20, t, t*2, t*3),  
						
						//t 是时间的正弦值，4个分量分别是 (t/8, t/4, t/2, t)
						_SinTime    float4  Sine of time: (t/8, t/4, t/2, t). SinTime.w 等价于 sin(_Time.y)

						//t 是时间的余弦值，4个分量分别是 (t/8, t/4, t/2, t)
						_CosTime    float4  Cosine of time: (t/8, t/4, t/2, t).

						//dt 是时间增量，4个分量的值分别是(dt, 1/dt, smoothDt, 1/smoothDt)
						unity_DeltaTime  float4  Delta time: (dt, 1/dt, smoothDt, 1/smoothDt).

					*/

					float	evaluationValue = _Time.r * _AnimSpeed;
			 
					// 顶点索引
					float vertIndex = v.texcoord3.x;
					float meshIndex = v.texcoord3.y;

					/*
						{TextureName}_TexelSize - float4 属性包含纹理大小信息：

						x 包含 1.0/宽度
						y 包含 1.0/高度
						z 包含宽度
						w 包含高度
					*/

					// 当前顶点位置  _AnimMap_TexelSize.x = 1 / _AnimMap.width 
					// (vertIndex + 0.5)   纹理坐标 0 - 1 
					// animUV.x = vertIndex * 1 / _AnimMap.width  + 0.5 * 1 / _AnimMap.width = _AnimMap.u 
					// 0.5 * 1 / _AnimMap.width 0.5个像素 
					// 为什么+ 0.5  保证采样到像素点中心
					float2 animUV = float2((vertIndex + 0.5) * _AnimMap_TexelSize.x , 0.0);

					// 取小数部分  
					animUV.y = frac(evaluationValue);
					// 相对模型坐标本地   tex2Dlod 2D 纹理采样  逐行采样
					float4 localPos = tex2Dlod(_AnimMap, float4(animUV, 0.0, 0.0));

					o.pos = UnityObjectToClipPos(localPos);

					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

	                return o;
	            }


				fixed4 frag (VertexOutput i) : COLOR
				{
					// 颜色贴图采样
					fixed4 col = tex2D(_MainTex, i.uv);
					// 输出
					return col;
				}

				ENDCG
			}
		}
		 
	}
	Fallback Off
}