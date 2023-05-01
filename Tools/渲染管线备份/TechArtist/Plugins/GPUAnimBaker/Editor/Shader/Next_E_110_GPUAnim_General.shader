Shader "Next_E/11_GPUAnim/0_General"
{
	Properties
	{
		_MainTex ("颜色贴图", 2D) = "white" {}
		_Opacity ("不透明度", Range(0.0, 1.0)) = 1.0
		_AnimMap ("动画缓存图", 2D) = "black" {}
		_AnimProgress ("播放进度", Range(0.0, 1.0)) = 0.0
		_AnimSpeed ("播放速度", Range(-100.0, 100.0)) = 10.0
		_AnimStep ("延迟步幅", Range(0.0, 1.0)) = 0.1
		_AlphaStep ("消隐步幅", Range(0.0, 1.0)) = 0.1
		_OffsetStep ("位移步幅", Vector) = (0.0, 0.0, 0.0, 0.0)
		_SamplerParams ("采样参数", Vector) = (-1.917,  -2.518, 0,3.83)

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
				//开启gpu instancing
				#pragma multi_compile_instancing

				#pragma shader_feature __ _BLEND_AT _BLEND_AB _BLEND_AD
				#pragma shader_feature __ _COLORMASK_RGBA
				#pragma shader_feature __ _GA_CYCLE
				#pragma shader_feature __ _GA_GHOST _GA_AFTERIMAGE
	

            	#include "UnityCG.cginc"
				#pragma target 3.0

				uniform sampler2D _MainTex; 
				uniform float4 _MainTex_ST;

				// 非不透明模式 _Opacity为不透明度
				#if _BLEND_AT || _BLEND_AB || _BLEND_AD
					uniform fixed _Opacity;
				#endif

				// 动画缓存图及其尺寸参数
				uniform sampler2D _AnimMap;	
				uniform float4 _AnimMap_TexelSize;

				// 循环模式 _AnimSpeed为循环速率
				#if _GA_CYCLE		
					uniform half _AnimSpeed;
				// 进度模式 _AnimProgress为进度值
				#else
					uniform half _AnimProgress;
				#endif

				// 幽灵和残影模式 _AnimStep _AlphaStep分别为动作和透明度延迟步幅
				#if _GA_GHOST || _GA_AFTERIMAGE
					uniform half _AnimStep;
					uniform half _AlphaStep;
					uniform half4 _OffsetStep;
				#endif

				// 采样参数
				uniform half4 _SamplerParams;

				// AnimMap采样UV计算方法
				//frac 返回标量或矢量的小数
				float2 GetAnimUV(float2 indexUV, float animMapTexelSizeX, float evaluationValue)
				{
					float vertIndex = indexUV.x; // 0- vertexCount
					float meshIndex = indexUV.y;


					//float2 animUV = float2((vertIndex + 0.5) * _AnimMap_TexelSize.x , 0.0);
					// (vertIndex + 0.5 ) 顶点索引 + 0.5    vertIndex * _AnimMap_TexelSize.x  + .5*  _AnimMap_TexelSize.x
					//https://blog.csdn.net/u013412391/article/details/108503124
					//对于n个依次排列的顶点，希望其对应纹理上依次排列的n个像素。那么对于X方向（或者说贴图上的U方向），
					// 第0个顶点不是0.0，最后一个顶点也不是1.0。而是比0.0~1.0的这个范围“缩小”一圈。即，第x（0到1-n）个顶点的U值是：
					// (x + 0.5)/n
					// 采样的是像素的中心
					//https://zhuanlan.zhihu.com/p/143377682
					//https://docs.microsoft.com/en-us/windows/win32/direct3d9/directly-mapping-texels-to-pixels 从像素到纹理
					float2 animUV = float2((vertIndex + 0.5 ) * _AnimMap_TexelSize.x , 0.0);

					#if _GA_GHOST
						animUV.y = frac(evaluationValue - _AnimStep * meshIndex);

					#elif _GA_AFTERIMAGE
						
						float animV = frac(evaluationValue - _AnimStep * meshIndex);
						animUV.y = lerp(animV, ceil(animV / _AnimStep) * _AnimStep, min(1.0, meshIndex));

					#else

						animUV.y = frac(evaluationValue);

					#endif
					return animUV;
				}

				struct VertexInput {
	                float2 texcoord : TEXCOORD0;
					// uv4为定点序号信息 X-顶点序号 Y-SubMesh序号
					float2 texcoord3 : TEXCOORD3;
					UNITY_VERTEX_INPUT_INSTANCE_ID
	            };
				
				//SV_前缀的变量代表system value
				// SV_POSTION一旦被作为vertex shader的输出语义，那么这个最终的顶点位置就被固定了,直接进入光栅化处理
				//如果作为fragment shader的输入语义那么和POSITION是一样的
	            struct VertexOutput {
	                float4 pos : SV_POSITION;
	                float2 uv : TEXCOORD0;
					 
					// 幽灵和残影模式 透明度
					#if _GA_GHOST || _GA_AFTERIMAGE
						float alpha : TEXCOORD1;

					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
	            };

	            VertexOutput vert (VertexInput v) {

					UNITY_SETUP_INSTANCE_ID(v);
					 
	                VertexOutput o = (VertexOutput)0;
					// 从AnimMap中采样顶点位置
					float evaluationValue = 0.0;

					#if _GA_CYCLE
						evaluationValue = _Time.r * _AnimSpeed;
					#else
						evaluationValue = _AnimProgress;
						//evaluationValue = _Time.r * _AnimSpeed;
					#endif

					float2 animUV = GetAnimUV(v.texcoord3, _AnimMap_TexelSize.x, evaluationValue);

					float4 localPos = tex2Dlod(_AnimMap, float4(animUV, 0.0, 0.0));

					localPos.xyz = localPos.xyz * _SamplerParams.w + _SamplerParams.xyz;

					// 幽灵和残影模式 残影偏移
					#if _GA_GHOST || _GA_AFTERIMAGE
						localPos.xyz -= _OffsetStep * v.texcoord3.y;
					#endif

					o.pos = UnityObjectToClipPos(localPos);
					// UV
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

					// 幽灵和残影模式 透明度
					#if _GA_GHOST || _GA_AFTERIMAGE
						o.alpha = 1.0f - _AlphaStep * v.texcoord3.y;
					#endif

	                return o;
	            }

				fixed4 frag (VertexOutput i) : COLOR
				{
					// 颜色贴图采样
					fixed4 finalRGBA = tex2D(_MainTex, i.uv);

					
					// 输出
					return finalRGBA;
				}

				ENDCG
			}
		}
		
		// sm3.0以下备用方法 无动画
		// Shader Model 3.0 （DirectX9.0c） 以下
		SubShader
		{
			pass
			{
				CGPROGRAM
            	#pragma vertex vert
            	#pragma fragment frag
				#pragma shader_feature __ _BLEND_AT _BLEND_AB _BLEND_AD
            	#include "UnityCG.cginc"
				#pragma target 2.0

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;

				struct VertexInput {
					float4 vertex : POSITION;
	                float2 texcoord : TEXCOORD0;
	            }; 

	            struct VertexOutput {
	                float4 pos : SV_POSITION;
	                float2 uv : TEXCOORD0;
	            };

	            VertexOutput vert (VertexInput v) {
	                VertexOutput o = (VertexOutput)0;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	                return o;
	            }

				fixed4 frag (VertexOutput i) : COLOR
				{
					fixed4 finalRGBA = tex2D(_MainTex, i.uv);
					// 处理透明方式
					#if _BLEND_AT
						clip(finalRGBA.a - 0.5);
						#if _COLORMASK_RGBA
							finalRGBA.a = ceil(finalRGBA.a - 0.5);
						#endif
					#elif _BLEND_AB
						// Do Nothing
					#elif _BLEND_AD
						finalRGBA.rgb *= finalRGBA.a;
						#if _COLORMASK_RGBA
							finalRGBA.a = finalRGBA.r * 0.30 + finalRGBA.g * 0.59 + finalRGBA.b * 0.11;
						#endif
					#endif
					// 输出
					return finalRGBA;
				}

				ENDCG
			}
		}
	}
	Fallback Off
	CustomEditor "GPUAnimMatEditor"
}