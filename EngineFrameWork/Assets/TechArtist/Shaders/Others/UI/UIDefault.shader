// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NBA/UIDefault"

{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Alpha("Alpha",range(0,1)) = 1
		[Enum(Default,0,Mask,1)]_Mode("Mixed",Float) = 0
		_width("width+x,width-x,length+z,length-z", Vector) = (-6.6,-7.54,-13.4,-14.5)
			[Space(5)]
		[KeywordEnum(X,Y)]_XorY("XorY",float) = 0
			//[Toggle]_Dir					("RampDistance",float)=0
			_RampStart("RampStart",Range(0,10)) = 1
			_RampEnd("RampEnd",Range(0,10)) = 1

			[PerRendererData]_StencilComp("Stencil Comparison", Float) = 8
			[PerRendererData]_Stencil("Stencil ID", Float) = 0
			[PerRendererData]_StencilOp("Stencil Operation", Float) = 0
			[PerRendererData]_StencilWriteMask("Stencil Write Mask", Float) = 255
			[PerRendererData]_StencilReadMask("Stencil Read Mask", Float) = 255

			[PerRendererData]_ColorMask("Color Mask", Float) = 15
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent+3"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
			HLSLPROGRAM
			#pragma target 3.0
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"

			#pragma multi_compile_local _XORY_X _XORY_Y
			//#pragma shader_feature _DIR_ON _DIR_OFF
						#pragma vertex UIVertex
						#pragma fragment UIFragment

							struct UIAttributes
							{
								float4 positionOS   : POSITION;
								float4 color    : COLOR;
								float2 texcoord0 : TEXCOORD0;
							};

							struct UIVaryings
							{
								float4 positionCS   : SV_POSITION;
								half4 color : COLOR;
								half2 texcoord0  : TEXCOORD0;
								float3 positionWS : TEXCOORD3;
								float4 screenPos:TEXCOORD1;
							};

							CBUFFER_START(UnityPerMaterial)
								half4 _Color;
								half _Alpha;
								float4 _width;
								half _Mode;
								int _XorY;
								half _RampStart;
								half _RampEnd;
							CBUFFER_END

							UIVaryings UIVertex(UIAttributes input)
							{
								UIVaryings output;
								output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
								output.positionCS = TransformWorldToHClip(output.positionWS);
								output.texcoord0 = input.texcoord0;
				#ifdef UNITY_HALF_TEXEL_OFFSET
								output.positionOS.xy += (_ScreenParams.zw - 1.0) * float2(-1,1);
				#endif
								output.color = input.color * _Color;
								output.color.a = _Alpha;
								output.screenPos = ComputeScreenPos(output.positionCS);
								return output;
							}
							TEXTURE2D(_MainTex);
							SAMPLER(sampler_MainTex);

							half4 UIFragment(UIVaryings input) : SV_Target
							{
								half2 screenPos = input.screenPos.xy / input.screenPos.w;
								_RampStart /= 10;
								_RampEnd /= 10;
								half alpha;
								#if defined(_XORY_Y)
									alpha = smoothstep(_RampStart, _RampEnd, screenPos.y);
								#else
									alpha = smoothstep(_RampStart, _RampEnd, screenPos.x);
								#endif
								half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord0.xy);
								color.a *= alpha;
								if (_Mode == 0)
								{
								return color;
								}
								if (_Mode == 1)
								{
								float3 worldPos = input.positionWS.xyz;
								float4 width = _width;
								float x1 = saturate(worldPos.x + width.x);
								float x2 = saturate((1 - worldPos.x) + width.y);
								float x3 = saturate(worldPos.z + width.z);
								float x4 = saturate(1 - worldPos.z + width.w);
								float a = saturate(x1 + x2);
								float a2 = saturate(x3 + x4);
								float mask = 1 - saturate(a + a2);
								clip(color.a * mask - 0.01);
								return color;
								}
								return color;


							}
						ENDHLSL
						}
		}
}
