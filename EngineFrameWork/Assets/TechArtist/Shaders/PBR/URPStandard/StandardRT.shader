Shader "NBA2/URP/StandardRT"
{
    Properties
    {
        _AlbedoMap                  ("", 2D) = "white" {}
        _AlbedoColor                ("", Color) = (1,1,1,1)
        _UVScale				    ("", Float) = 1.0

        _NormalMap                  ("", 2D) = "bump"{}
	    _Metallic                   ("", Range(0.0, 1.0)) = 0
        _Glossiness                 ("", Range(0.0, 2.0)) = 0.5
        _AOIntensity                ("", Range(0,1)) = 1

        _EmissionEnabled            ("", Float) = 0.0
        _EmissionMap                ("", 2D) = "white" {}
        _EmissionColor			    ("", Color) = (0,0,0,0)


        _LitAttenContrast           ("", Float) = 1.0
        _LitAttenInt                ("", Float) = 14.0
        _LitAttenSub                ("", Float) = 0.9
        _ShadowIntensity            ("", range(0, 2)) = 0

        _RScale                     ("", Float) = 1
        _GScale                     ("", Float) = 1
        _BScale                     ("", Float) = 1
        _AScale                     ("", Float) = 1

        _DetailBump                 ("", 2D) = "bump"{}
        _DetailBumpScale            ("", Range(0, 2)) = 1

        _CullMode                   ("", Float) = 2.0
        _ZTest					    ("", Float) = 4.0
        _QueueOffset                ("", Float) = 0.0


        DETAIL_MICRO			    ("", Float) = 0
        VIR_LIT                     ("", Float) = 0
        VERTEX_COLOR			    ("", Float) = 0
        //GPU_SKINNING			    ("", Float) = 0

        _EffectMaskTex              ("",2D) = "white"{}
        _YSpeed                     ("",Float) = 0
        _Width                      ("",Float) = 0
        _EdgeColor                  ("",Color) = (1,1,1,1)
        _EdgeWidth                  ("",Float) = 0
        _MaskTexTilling             ("",Float) = 0
        _DownWidth                  ("",Float) = 0
        _Pos                        ("",Float) = 0
        _RimEdge                    ("",Float) = 0
        _RimDis                     ("",Float) = 0

     
        _NumberS("NumberS",float) = 0
        _NumberG("NumberG",float) = 0
        _NumTex("NumTex",2D) = "black"{}
        _NumTex2("NumTex2",2D) = "black"{}
        _NumberDis("NumberDis",float) = 0
        _NumBackOffSet("NumBackOffSet:x背左右y背上下z背缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
        _NumFrontOffSet("NumFrontOffSet:x前左右y前上下z前缩放w无用",Vector) = (0.0,0.0,1.0,1.0)

		 _MainTexture("MainTexture", 2D) = "white" {}
		_TIlling("Tilling", Vector) = (1, 1, 0, 0)
		_Offset("Offset", Vector) = (0, 0, 0, 0)
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        _ReceiveShadows("Receive Shadows", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+0"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
			"UniversalMaterialType" = "Unlit"
        }
        
		Pass
		{
			Name "Pass"
			Tags{ }

			// Render State
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off

			// --------------------------------------------------
			// Pass
			HLSLPROGRAM

			// Pragmas
			#pragma target 3.0
			#pragma exclude_renderers gles gles3 glcore
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#pragma vertex vert
			#pragma fragment frag

			// Keywords
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma shader_feature _ _SAMPLE_GI

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _AlphaClip 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define VARYINGS_NEED_TEXCOORD0
			#define FEATURES_GRAPH_VERTEX
			#define SHADERPASS SHADERPASS_UNLIT

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
			};
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0;

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				float4 uv0;
			};
			struct VertexDescriptionInputs
			{
				float3 ObjectSpaceNormal;
				float3 ObjectSpaceTangent;
				float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 interp0 : TEXCOORD0;

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				output.positionCS = input.positionCS;
				output.interp0.xyzw = input.texCoord0;

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}
			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				output.texCoord0 = input.interp0.xyzw;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTexture_TexelSize;
			half2 _TIlling;
			half2 _Offset;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTexture);
			SAMPLER(sampler_MainTexture);

			// Graph Functions
			void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
			{
				Out = UV * Tiling + Offset;
			}

			// Graph Vertex
			struct VertexDescription
			{
				float3 Position;
				float3 Normal;
				float3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Graph Pixel
			struct SurfaceDescription
			{
				float3 BaseColor;
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				UnityTexture2D _MainTex = UnityBuildTexture2DStructNoScale(_MainTexture);

				half2 _TillingValue = _TIlling;
				half2 _OffsetValue = _Offset;
				float2 _TilingAndOffset_Result;
				Unity_TilingAndOffset_float(IN.uv0.xy, _TillingValue, _OffsetValue, _TilingAndOffset_Result);

				float4 SampleTextureResult = SAMPLE_TEXTURE2D(_MainTex.tex, _MainTex.samplerstate, _TilingAndOffset_Result);
				float SampleTextureResult_R = SampleTextureResult.r;
				float SampleTextureResult_G = SampleTextureResult.g;
				float SampleTextureResult_B = SampleTextureResult.b;
				float SampleTextureResult_A = SampleTextureResult.a;

				surface.BaseColor = (SampleTextureResult.xyz);
				surface.Alpha = SampleTextureResult_A;
				surface.AlphaClipThreshold = 0.01;
				return surface;
			}

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal = input.normalOS;
				output.ObjectSpaceTangent = input.tangentOS.xyz;
				output.ObjectSpacePosition = input.positionOS;

				return output;
			}

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				output.uv0 = input.texCoord0;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =  IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				return output;
			}

			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

			ENDHLSL
		}
   //     Pass
   //     {
   //         Name "ForwardLit"
   //         Tags { "LightMode" = "BeforeTransparent" }

			//Blend  SrcAlpha OneMinusSrcAlpha
			//Cull   Back
   //         ZTest  LEqual
			//ZWrite Off

   //         HLSLPROGRAM
   //         #pragma target 3.0
   //         #pragma prefer_hlslcc gles
   //         #pragma exclude_renderers d3d11_9x
   //         #pragma instancing_options assumeuniformscaling nolodfade

   //         // -------------------------------------
   //         // Unity defined keywords
   //         #pragma multi_compile_instancing
   //         #pragma multi_compile_fog
   //         #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

   //         // -------------------------------------
   //         // Universal Pipeline keywords
   //         #pragma multi_compile_local_fragment _ OVERRIDE_SH
   //         #pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH

   //         #pragma shader_feature_local SHADING_EMISSION
   //         #pragma multi_compile_local VERTEX_COLOR
   //         #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

   //         #pragma vertex   LitVert
   //         #pragma fragment LitFrag

   //         #include "StandardRT.hlsl"
   //         ENDHLSL
   //     }

   //     Pass
   //     {
   //         Name "DepthOnly"
   //         Tags{"LightMode" = "DepthOnly"}

   //         ZWrite On
   //         ColorMask 0
   //         Cull[_Cull]

   //         HLSLPROGRAM
			//#pragma target 3.0
			//#pragma prefer_hlslcc gles
			//#pragma exclude_renderers d3d11_9x

			//#pragma vertex   DepthOnlyVert
			//#pragma fragment DepthOnlyFrag

   //         #include "StandardRT.hlsl"
   //         #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/DepthOnlyPass.hlsl"

   //         ENDHLSL
   //     }

   //     Pass
   //     {
   //         Name "DepthNormals"
   //         Tags{"LightMode" = "DepthNormals"}

   //         ZWrite On
   //         Cull[_Cull]

   //         HLSLPROGRAM
   //         #pragma target 3.0
			//#pragma prefer_hlslcc gles
			//#pragma exclude_renderers d3d11_9x

   //         #pragma vertex DepthNormalsVertex
   //         #pragma fragment DepthNormalsFragment

   //         #include "StandardRT.hlsl"
   //         #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/DepthNormalPass.hlsl"
   //         ENDHLSL
   //     }
    }

    Fallback "Hidden/InternalErrorShader"
    CustomEditor "FTX.Rendering.ShaderGUIStandardRT"
}