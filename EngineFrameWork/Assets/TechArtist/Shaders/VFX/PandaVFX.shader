// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/PandaVFX"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_Cullmode("Cullmode", Float) = 0
		[KeywordEnum(Normal,Polar,Cylinder)] _MaskTexUVS("MaskTexUVS", Float) = 0
		[Toggle]_NormalTexCV("NormalTexCV", Float) = 0
		[Toggle]_NormalTexC("NormalTexC", Float) = 0
		_NormalScale("NormalScale", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_Ztest("Ztest", Float) = 4
		[KeywordEnum(Normal,Polar,Cylinder)] _DissolveTexUVS("DissolveTexUVS", Float) = 0
		_LightScale("LightScale", Range( 0 , 1)) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_Scr("Scr", Float) = 5
		_TexCenter("TexCenter", Vector) = (0,0,0,0)
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[KeywordEnum(Normal,Polar,Cylinder)] _MainTexUVS("MainTexUVS", Float) = 0
		[KeywordEnum(X,Y,Z)] _Face("Face", Float) = 1
		[HideInInspector]_AddTex_ST("AddTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_MainTex_ST("MainTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_NormalTex_ST("NormalTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_VTOTex_ST("VTOTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_DistortTex_ST("DistortTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_DistortMaskTex_ST("DistortMaskTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_DissloveTexPlus_ST("DissloveTexPlus_ST", Vector) = (1,1,0,0)
		[HideInInspector]_DissloveTex_ST("DissloveTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_VTOMaskTex_ST("VTOMaskTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_MaskPlusTex_ST("MaskPlusTex_ST", Vector) = (1,1,0,0)
		[HideInInspector]_MaskTex_ST("MaskTex_ST", Vector) = (1,1,0,0)
		_MainAlpha("MainAlpha", Range( 0 , 100)) = 1
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		_AddTexUspeed("AddTexUspeed", Float) = 0
		_MainTex_Uspeed("MainTex_Uspeed", Float) = 0
		_NormalTex_Uspeed("NormalTex_Uspeed", Float) = 0
		_AddTexVspeed("AddTexVspeed", Float) = 0
		_MainTex_Vspeed("MainTex_Vspeed", Float) = 0
		_NormalTex_Vspeed("NormalTex_Vspeed", Float) = 0
		[Toggle]_DistortAddTex("DistortAddTex", Float) = 1
		[Enum(off,0,on,1)]_DistortNormalTex("DistortNormalTex", Float) = 1
		[Enum(off,0,on,1)]_DistortMainTex("DistortMainTex", Float) = 1
		[Enum(off,0,on,1)]_DistortMask("DistortMask", Float) = 0
		[Enum(off,0,on,1)]_DistortDisTex("DistortDisTex", Float) = 0
		_DistortFactor("DistortFactor", Range( 0 , 1)) = 0
		_DistortTex_Uspeed("DistortTex_Uspeed", Float) = 0
		_DistortTex_Vspeed("DistortTex_Vspeed", Float) = 0
		_DIssloveFactor("DIssloveFactor", Range( 0 , 1)) = 0.5
		_DIssloveWide("DIssloveWide", Range( 0 , 1)) = 0.1
		_DIssloveSoft("DIssloveSoft", Range( 0 , 1)) = 0.5
		[HDR]_DIssloveColor("DIssloveColor", Color) = (1,1,1,1)
		_DisTex_Uspeed("DisTex_Uspeed", Float) = 0
		_DisTex_Vspeed("DisTex_Vspeed", Float) = 0
		_VTOFactor("VTOFactor", Float) = 0
		_VTOTex_Uspeed("VTOTex_Uspeed", Float) = 0
		_VTOTex_Vspeed("VTOTex_Vspeed", Float) = 0
		_VTOMaskTex("VTOMaskTex", 2D) = "white" {}
		_fnl_power("fnl_power", Range( 1 , 10)) = 1
		_fnl_sacle("fnl_sacle", Range( 0 , 1)) = 0
		[HDR]_fnl_color("fnl_color", Color) = (1,1,1,0)
		_softFacotr("softFacotr", Range( 0 , 20)) = 1
		_DepthfadeFactor("DepthfadeFactor", Range( 0 , 10)) = 1
		[Toggle]_CustomdataMaskUV("CustomdataMaskUV", Float) = 0
		_Mask_scale("Mask_scale", Float) = 1
		_NormalTex_Rotat("NormalTex_Rotat", Range( 0 , 360)) = 0
		_MainTex_rotat("MainTex_rotat", Range( 0 , 360)) = 0
		_AddTexRo("AddTexRo", Range( 0 , 360)) = 0
		_VTOR("VTOR", Range( 0 , 360)) = 0
		_VTOMaskR("VTOMaskR", Range( 0 , 360)) = 0
		_DissolvePlusR("DissolvePlusR", Range( 0 , 360)) = 0
		_DIssolve_rotat("DIssolve_rotat", Range( 0 , 360)) = 0
		[Toggle]_FNLfanxiangkaiguan("FNLfanxiangkaiguan", Float) = 0
		[Toggle]_ToggleSwitch0("Toggle Switch0", Float) = 0
		[Toggle]_Depthfadeon("Depthfadeon", Float) = 0
		[Toggle]_screenVTOon("screenVTOon", Float) = 0
		_Mask_Uspeed("Mask_Uspeed", Float) = 0
		_MaskPlusUspeed("MaskPlusUspeed", Float) = 0
		_Mask_rotat("Mask_rotat", Range( 0 , 360)) = 0
		_MaskPlusR("MaskPlusR", Range( 0 , 360)) = 0
		_MaskPlusVspeed("MaskPlusVspeed", Float) = 0
		_Mask_Vspeed("Mask_Vspeed", Float) = 0
		[Toggle]_soft_sting("soft_sting", Float) = 0
		[Toggle]_VTOMaskAR("VTOMaskAR", Float) = 1
		[Toggle]_VTOMaskCV("VTOMaskCV", Float) = 0
		[Toggle]_VTOMaskC("VTOMaskC", Float) = 0
		_qubaohedu("qubaohedu", Range( 0 , 1)) = 0
		[HDR]_DepthColor("DepthColor", Color) = (0,0,0,0)
		_Zwrite("Zwrite", Float) = 0
		[Enum(Option1,0,Option2,1)]_DepthF("DepthF", Float) = 0
		_Dir("Dir", Vector) = (0,0,0,0)
		[HDR]_BackFaceColor("BackFaceColor", Color) = (1,1,1,0)
		[Toggle]_IfMaskColor("IfMaskColor", Float) = 0
		[Enum(Option1,0,Option2,1)]_CustomDistort("CustomDistort", Float) = 0
		[HDR]_AddTexColor("AddTexColor", Color) = (0,0,0,0)
		_AddTexBlend("AddTexBlend", Range( 0 , 1)) = 0
		[Toggle]_IfAddTex("IfAddTex", Float) = 0
		_DissolveTexDivide("DissolveTexDivide", Range( 1 , 10)) = 1
		[Enum(Option1,0,Option2,1)]_CustomdataDisT("CustomdataDisT", Float) = 0
		_DissloveTexPlus("DissloveTexPlus", 2D) = "black" {}
		_CenterU("CenterU", Float) = 0.5
		_CenterV("CenterV", Float) = 0.5
		[Enum(off,0,on,1)]_ScreenAsMain("ScreenAsMain", Float) = 0
		[Toggle]_softback("softback", Float) = 0
		_MainTexRefine("MainTexRefine", Vector) = (1,1,2,0)
		_AddTexRefine("AddTexRefine", Vector) = (1,1,2,0)
		_VTOTexExp("VTOTexExp", Range( 0 , 10)) = 1
		_DissolveTexExp("DissolveTexExp", Range( 0 , 10)) = 1
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[Toggle]_IfCustomLight("IfCustomLight", Float) = 0
		_AddTex("AddTex", 2D) = "white" {}
		_DissloveTex("DissloveTex", 2D) = "white" {}
		_VTOTex("VTOTex", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
		_MaskPlusTex("MaskPlusTex", 2D) = "white" {}
		[Toggle]_AddTexC("AddTexC", Float) = 0
		[Toggle]_MaintexC("MaintexC", Float) = 0
		[Toggle]_VTOC("VTOC", Float) = 0
		[Toggle]_MaskPlusC("MaskPlusC", Float) = 0
		[Toggle]_MaskC("MaskC", Float) = 0
		[Toggle]_DissolveC("DissolveC", Float) = 0
		[Toggle]_MaintexCV("MaintexCV", Float) = 0
		[Toggle]_VTOCV("VTOCV", Float) = 0
		[Toggle]_MaskPlusCV("MaskPlusCV", Float) = 0
		[Toggle]_MaskCV("MaskCV", Float) = 0
		[Toggle]_DissolvePlusC("DissolvePlusC", Float) = 0
		[Toggle]_DissolvePlusAR("DissolvePlusAR", Float) = 1
		[Toggle]_DissolvePlusCV("DissolvePlusCV", Float) = 0
		[Toggle]_DissolveCV("DissolveCV", Float) = 0
		[Toggle]_AddTexCV("AddTexCV", Float) = 0
		[Toggle]_MainTex_ar("MainTex_a/r", Float) = 0
		[Toggle]_VTOAR("VTOAR", Float) = 1
		[Toggle]_MaskPlusAR("MaskPlusAR", Float) = 1
		[Toggle]_MaskAlphaRA("MaskAlphaRA", Float) = 1
		[Toggle]_DissolveAR("DissolveAR", Float) = 1
		_NormalTex("NormalTex", 2D) = "white" {}
		_StaticNormalScale("StaticNormalScale", Float) = 0
		_StaticNormalOffset("StaticNormalOffset", Range( 0 , 1)) = 0
		[Toggle]_IfStaticNormal("IfStaticNormal", Float) = 0
		_CubemapScale("CubemapScale", Range( 0 , 10)) = 0
		[Toggle]_IfCubemap("IfCubemap", Float) = 0
		[Toggle]_IfDissolvePlus("IfDissolvePlus", Float) = 0
		_CubeMap("CubeMap", CUBE) = "white" {}
		[Toggle]_CustomdataDis("CustomdataDis", Float) = 0
		[Toggle]_sot_sting_A("sot_sting_A", Float) = 0
		[Enum(Alpha,0,Add,1,Multiply,2)]_AddTexBlendMode("AddTexBlendMode", Float) = 0
		_Parallax("Parallax", Range( 0 , 1)) = 0
		[Toggle]_IfPara("IfPara", Float) = 0
		_ParaTex("ParaTex", 2D) = "white" {}
		[Toggle]_IfFlowmap("IfFlowmap", Float) = 0
		[Toggle]_IfNormalDistort("IfNormalDistort", Float) = 0
		[Toggle]_DistortTexAR("DistortTexAR", Float) = 1
		_DistortTex("DistortTex", 2D) = "white" {}
		[Toggle]_DistortMaskTexAR("DistortMaskTexAR", Float) = 1
		_DistortMaskTex("DistortMaskTex", 2D) = "white" {}
		_DistortMaskTexR("DistortMaskTexR", Range( 0 , 360)) = 0
		[Toggle]_DistortMaskTexC("DistortMaskTexC", Float) = 0
		[Toggle]_DistortMaskTexCV("DistortMaskTexCV", Float) = 0
		[Toggle]_AddTexAR("AddTexAR", Float) = 0
		[Toggle]_IfAddTexAlpha("IfAddTexAlpha", Float) = 0
		[Toggle]_IfAddTexColor("IfAddTexColor", Float) = 1
		[Toggle]_AlphaAdd("AlphaAdd", Float) = 0
		[Toggle]_DistortRemap("DistortRemap", Float) = 0
		[Enum(Normal,0,Polar,1,Cylinder,2)]_AddTexUVS("AddTexUVS", Float) = 0
		[Toggle]_CustomdataMainTexUV("CustomdataMainTexUV", Float) = 0
		[Toggle]_CAddTexUV("CAddTexUV", Float) = 0
		[Toggle]_CAddTexUVT("CAddTexUVT", Float) = 1
		_Reference("Reference", Range( 0 , 200)) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_Pass("Pass", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_Comparison("Comparison", Float) = 8
		[Enum(UnityEngine.Rendering.StencilOp)]_Fail("Fail", Float) = 0
		_StencilStyle("StencilStyle", Float) = 0
		[Toggle]_IfMaskPlusTex("IfMaskPlusTex", Float) = 0
		[Toggle]_IfDissolveColor("IfDissolveColor", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cullmode]
		ZWrite [_Zwrite]
		ZTest [_Ztest]
		Stencil
		{
			Ref [_Reference]
			Comp [_Comparison]
			Pass [_Pass]
			Fail [_Fail]
		}
		Blend [_Scr] [_Dst]
		
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 5.0
		#pragma multi_compile_local _MAINTEXUVS_NORMAL _MAINTEXUVS_POLAR _MAINTEXUVS_CYLINDER
		#pragma multi_compile_local _FACE_X _FACE_Y _FACE_Z
		#pragma multi_compile_local _MASKTEXUVS_NORMAL _MASKTEXUVS_POLAR _MASKTEXUVS_CYLINDER
		#pragma multi_compile_local _DISSOLVETEXUVS_NORMAL _DISSOLVETEXUVS_POLAR _DISSOLVETEXUVS_CYLINDER
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float4 uv3_texcoord3;
			float4 vertexColor : COLOR;
			float4 uv2_texcoord2;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float4 screenPos;
			float4 screenPosition88;
			half ASEVFace : VFACE;
			float3 worldRefl;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Fail;
		uniform float _Zwrite;
		uniform float _Scr;
		uniform float _Pass;
		uniform float _Ztest;
		uniform float _StencilStyle;
		uniform float _Dst;
		uniform float _Comparison;
		uniform float _Reference;
		uniform float _Cullmode;
		uniform float _screenVTOon;
		uniform float _VTOAR;
		uniform sampler2D _VTOTex;
		uniform float _VTOC;
		uniform float _VTOTex_Uspeed;
		uniform float _VTOTex_Vspeed;
		uniform float4 _VTOTex_ST;
		uniform float _CenterU;
		uniform float _CenterV;
		uniform float4 _TexCenter;
		uniform float _VTOR;
		uniform float _VTOCV;
		uniform float _VTOTexExp;
		uniform float _ToggleSwitch0;
		uniform float _VTOFactor;
		uniform float _VTOMaskAR;
		uniform sampler2D _VTOMaskTex;
		uniform float _VTOMaskC;
		uniform float4 _VTOMaskTex_ST;
		uniform float _VTOMaskR;
		uniform float _VTOMaskCV;
		uniform float _AlphaAdd;
		uniform float _Mask_scale;
		uniform float _MaskAlphaRA;
		uniform sampler2D _MaskTex;
		uniform float _MaskC;
		uniform float _Mask_Uspeed;
		uniform float _Mask_Vspeed;
		uniform float4 _MaskTex_ST;
		uniform float _DistortMask;
		uniform float _IfFlowmap;
		uniform float _IfNormalDistort;
		uniform float _CustomDistort;
		uniform float _DistortFactor;
		uniform float _DistortMaskTexAR;
		uniform sampler2D _DistortMaskTex;
		uniform float _DistortMaskTexC;
		uniform float4 _DistortMaskTex_ST;
		uniform float _DistortMaskTexR;
		uniform float _DistortMaskTexCV;
		uniform float _DistortRemap;
		uniform float _DistortTexAR;
		uniform sampler2D _DistortTex;
		uniform float _DistortTex_Uspeed;
		uniform float _DistortTex_Vspeed;
		uniform float4 _DistortTex_ST;
		uniform float _CustomdataMaskUV;
		uniform float _Mask_rotat;
		uniform float _MaskCV;
		uniform float _IfMaskPlusTex;
		uniform float _MaskPlusAR;
		uniform sampler2D _MaskPlusTex;
		uniform float _MaskPlusC;
		uniform float _MaskPlusUspeed;
		uniform float _MaskPlusVspeed;
		uniform float4 _MaskPlusTex_ST;
		uniform float _MaskPlusR;
		uniform float _MaskPlusCV;
		uniform float _IfAddTexAlpha;
		uniform float _CustomdataDisT;
		uniform float _ScreenAsMain;
		uniform float _MainTex_ar;
		uniform sampler2D _MainTex;
		uniform float _MaintexC;
		uniform float _MainTex_Uspeed;
		uniform float _MainTex_Vspeed;
		uniform float _CustomdataMainTexUV;
		uniform float _IfPara;
		uniform float4 _MainTex_ST;
		uniform sampler2D _ParaTex;
		uniform float _Parallax;
		uniform float4 _ParaTex_ST;
		uniform float _DistortMainTex;
		uniform float _MainTex_rotat;
		uniform float _MaintexCV;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float4 _MainColor;
		uniform float _AddTexBlendMode;
		uniform float _AddTexAR;
		uniform sampler2D _AddTex;
		uniform float _AddTexC;
		uniform float _AddTexUspeed;
		uniform float _AddTexVspeed;
		uniform float _AddTexUVS;
		uniform float4 _AddTex_ST;
		uniform float _DistortAddTex;
		uniform float _CAddTexUV;
		uniform float _CAddTexUVT;
		uniform float _AddTexRo;
		uniform float _AddTexCV;
		uniform float _AddTexBlend;
		uniform float _MainAlpha;
		uniform float _sot_sting_A;
		uniform float _DIssloveSoft;
		uniform float _CustomdataDis;
		uniform float _DIssloveFactor;
		uniform float _DissolveAR;
		uniform sampler2D _DissloveTex;
		uniform float _DissolveC;
		uniform float _DisTex_Uspeed;
		uniform float _DisTex_Vspeed;
		uniform float4 _DissloveTex_ST;
		uniform float _DistortDisTex;
		uniform float _DIssolve_rotat;
		uniform float _DissolveCV;
		uniform float _DissolveTexExp;
		uniform float _DissolveTexDivide;
		uniform float _IfDissolvePlus;
		uniform float _DissolvePlusAR;
		uniform sampler2D _DissloveTexPlus;
		uniform float _DissolvePlusC;
		uniform float4 _DissloveTexPlus_ST;
		uniform float _DissolvePlusR;
		uniform float _DissolvePlusCV;
		uniform float _DIssloveWide;
		uniform float _FNLfanxiangkaiguan;
		uniform float _softback;
		uniform float _softFacotr;
		uniform float _Depthfadeon;
		uniform float _DepthF;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthfadeFactor;
		uniform float4 _DepthColor;
		uniform float3 _Dir;
		uniform float _fnl_sacle;
		uniform float _fnl_power;
		uniform float4 _fnl_color;
		uniform float _IfDissolveColor;
		uniform float _soft_sting;
		uniform float _IfAddTex;
		uniform float4 _MainTexRefine;
		uniform float _IfMaskColor;
		uniform float _IfAddTexColor;
		uniform float4 _AddTexColor;
		uniform float4 _AddTexRefine;
		uniform float4 _DIssloveColor;
		uniform float _IfCubemap;
		uniform samplerCUBE _CubeMap;
		uniform sampler2D _NormalTex;
		uniform float _NormalTexC;
		uniform float _NormalTex_Uspeed;
		uniform float _NormalTex_Vspeed;
		uniform float4 _NormalTex_ST;
		uniform float _DistortNormalTex;
		uniform float _NormalTex_Rotat;
		uniform float _NormalTexCV;
		uniform float _NormalScale;
		uniform float _IfStaticNormal;
		uniform float _StaticNormalOffset;
		uniform float _StaticNormalScale;
		uniform float _CubemapScale;
		uniform float _LightScale;
		uniform float4 _BackFaceColor;
		uniform float _qubaohedu;
		uniform float _IfCustomLight;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs.xy += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
			 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
			 	if ( currHeight > currRayZ )
			 	{
			 	 	stepIndex = numSteps + 1;
			 	}
			 	else
			 	{
			 	 	stepIndex++;
			 	 	prevTexOffset = currTexOffset;
			 	 	prevRayZ = currRayZ;
			 	 	prevHeight = currHeight;
			 	 	currTexOffset += deltaTex;
			 	 	currRayZ -= layerHeight;
			 	}
			}
			int sectionSteps = 8;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
			 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
			 	finalTexOffset = prevTexOffset + intersection * deltaTex;
			 	newZ = prevRayZ - intersection * layerHeight;
			 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
			 	if ( newHeight > newZ )
			 	{
			 	 	currTexOffset = finalTexOffset;
			 	 	currHeight = newHeight;
			 	 	currRayZ = newZ;
			 	 	deltaTex = intersection * deltaTex;
			 	 	layerHeight = intersection * layerHeight;
			 	}
			 	else
			 	{
			 	 	prevTexOffset = finalTexOffset;
			 	 	prevHeight = newHeight;
			 	 	prevRayZ = newZ;
			 	 	deltaTex = ( 1 - intersection ) * deltaTex;
			 	 	layerHeight = ( 1 - intersection ) * layerHeight;
			 	}
			 	sectionIndex++;
			}
			return uvs.xy + finalTexOffset;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 appendResult49_g487 = (float2(_VTOTex_Uspeed , _VTOTex_Vspeed));
			float4 break18_g485 = _VTOTex_ST;
			float2 appendResult1_g485 = (float2(break18_g485.x , break18_g485.y));
			float2 appendResult3_g485 = (float2(break18_g485.z , break18_g485.w));
			float2 appendResult866 = (float2(_CenterU , _CenterV));
			float2 centeruv867 = appendResult866;
			float2 CenteredUV15_g486 = ( v.texcoord.xy - centeruv867 );
			float2 break17_g486 = CenteredUV15_g486;
			float2 appendResult23_g486 = (float2(( length( CenteredUV15_g486 ) * break18_g485.x * 2.0 ) , ( atan2( break17_g486.x , break17_g486.y ) * ( 1.0 / 6.28318548202515 ) * break18_g485.y )));
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 break537 = ( _TexCenter + float4( ase_vertex3Pos , 0.0 ) );
			float2 appendResult554 = (float2((0.0 + (atan( ( break537.x / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.y));
			float2 appendResult553 = (float2((0.0 + (atan( ( break537.y / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.x));
			float2 appendResult555 = (float2((0.0 + (atan( ( break537.x / break537.y ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.z));
			#if defined(_FACE_X)
				float2 staticSwitch556 = appendResult553;
			#elif defined(_FACE_Y)
				float2 staticSwitch556 = appendResult554;
			#elif defined(_FACE_Z)
				float2 staticSwitch556 = appendResult555;
			#else
				float2 staticSwitch556 = appendResult554;
			#endif
			float2 maintongUV557 = staticSwitch556;
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch841 = ( ( v.texcoord.xy * appendResult1_g485 ) + appendResult3_g485 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch841 = ( appendResult23_g486 + appendResult3_g485 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch841 = ( ( maintongUV557 * appendResult1_g485 ) + appendResult3_g485 );
			#else
				float2 staticSwitch841 = ( ( v.texcoord.xy * appendResult1_g485 ) + appendResult3_g485 );
			#endif
			float2 panner44_g487 = ( 1.0 * _Time.y * appendResult49_g487 + staticSwitch841);
			float cos55_g487 = cos( ( ( ( _VTOR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g487 = sin( ( ( ( _VTOR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g487 = mul( panner44_g487 - float2( 0.5,0.5 ) , float2x2( cos55_g487 , -sin55_g487 , sin55_g487 , cos55_g487 )) + float2( 0.5,0.5 );
			float2 break52_g487 = rotator55_g487;
			float2 break54_g487 = rotator55_g487;
			float clampResult60_g487 = clamp( break54_g487.x , 0.0 , 1.0 );
			float clampResult50_g487 = clamp( break54_g487.y , 0.0 , 1.0 );
			float2 appendResult53_g487 = (float2(( (float)(int)_VTOC == 0.0 ? break52_g487.x : clampResult60_g487 ) , ( (float)(int)_VTOCV == 0.0 ? break52_g487.y : clampResult50_g487 )));
			float4 tex2DNode27_g487 = tex2Dlod( _VTOTex, float4( appendResult53_g487, 0, 0.0) );
			float3 ase_vertexNormal = v.normal.xyz;
			float4 break18_g483 = _VTOMaskTex_ST;
			float2 appendResult1_g483 = (float2(break18_g483.x , break18_g483.y));
			float2 appendResult3_g483 = (float2(break18_g483.z , break18_g483.w));
			float2 CenteredUV15_g484 = ( v.texcoord.xy - centeruv867 );
			float2 break17_g484 = CenteredUV15_g484;
			float2 appendResult23_g484 = (float2(( length( CenteredUV15_g484 ) * break18_g483.x * 2.0 ) , ( atan2( break17_g484.x , break17_g484.y ) * ( 1.0 / 6.28318548202515 ) * break18_g483.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch853 = ( ( v.texcoord.xy * appendResult1_g483 ) + appendResult3_g483 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch853 = ( appendResult23_g484 + appendResult3_g483 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch853 = ( ( maintongUV557 * appendResult1_g483 ) + appendResult3_g483 );
			#else
				float2 staticSwitch853 = ( ( v.texcoord.xy * appendResult1_g483 ) + appendResult3_g483 );
			#endif
			float cos263 = cos( ( ( ( _VTOMaskR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin263 = sin( ( ( ( _VTOMaskR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator263 = mul( staticSwitch853 - float2( 0.5,0.5 ) , float2x2( cos263 , -sin263 , sin263 , cos263 )) + float2( 0.5,0.5 );
			float2 break372 = rotator263;
			float2 break371 = rotator263;
			float clampResult257 = clamp( break371.x , 0.0 , 1.0 );
			float clampResult373 = clamp( break371.y , 0.0 , 1.0 );
			float2 appendResult375 = (float2((( _VTOMaskC )?( clampResult257 ):( break372.x )) , (( _VTOMaskCV )?( clampResult373 ):( break372.y ))));
			float4 tex2DNode81 = tex2Dlod( _VTOMaskTex, float4( appendResult375, 0, 0.0) );
			float3 VTO82 = ( saturate( pow( ( (float)(int)_VTOAR == 0.0 ? tex2DNode27_g487.a : tex2DNode27_g487.r ) , _VTOTexExp ) ) * ase_vertexNormal * (( _ToggleSwitch0 )?( v.texcoord1.w ):( _VTOFactor )) * (( _VTOMaskAR )?( tex2DNode81.r ):( tex2DNode81.a )) );
			float3 temp_cast_7 = (0.0).xxx;
			v.vertex.xyz += (( _screenVTOon )?( temp_cast_7 ):( VTO82 ));
			v.vertex.w = 1;
			float3 vertexPos88 = ase_vertex3Pos;
			float4 ase_screenPos88 = ComputeScreenPos( UnityObjectToClipPos( vertexPos88 ) );
			o.screenPosition88 = ase_screenPos88;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 appendResult49_g470 = (float2(_Mask_Uspeed , _Mask_Vspeed));
			float4 break18_g448 = _MaskTex_ST;
			float2 appendResult1_g448 = (float2(break18_g448.x , break18_g448.y));
			float2 appendResult3_g448 = (float2(break18_g448.z , break18_g448.w));
			float2 appendResult866 = (float2(_CenterU , _CenterV));
			float2 centeruv867 = appendResult866;
			float2 CenteredUV15_g449 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g449 = CenteredUV15_g449;
			float2 appendResult23_g449 = (float2(( length( CenteredUV15_g449 ) * break18_g448.x * 2.0 ) , ( atan2( break17_g449.x , break17_g449.y ) * ( 1.0 / 6.28318548202515 ) * break18_g448.y )));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 break537 = ( _TexCenter + float4( ase_vertex3Pos , 0.0 ) );
			float2 appendResult554 = (float2((0.0 + (atan( ( break537.x / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.y));
			float2 appendResult553 = (float2((0.0 + (atan( ( break537.y / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.x));
			float2 appendResult555 = (float2((0.0 + (atan( ( break537.x / break537.y ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.z));
			#if defined(_FACE_X)
				float2 staticSwitch556 = appendResult553;
			#elif defined(_FACE_Y)
				float2 staticSwitch556 = appendResult554;
			#elif defined(_FACE_Z)
				float2 staticSwitch556 = appendResult555;
			#else
				float2 staticSwitch556 = appendResult554;
			#endif
			float2 maintongUV557 = staticSwitch556;
			#if defined(_MASKTEXUVS_NORMAL)
				float2 staticSwitch781 = ( ( i.uv_texcoord * appendResult1_g448 ) + appendResult3_g448 );
			#elif defined(_MASKTEXUVS_POLAR)
				float2 staticSwitch781 = ( appendResult23_g449 + appendResult3_g448 );
			#elif defined(_MASKTEXUVS_CYLINDER)
				float2 staticSwitch781 = ( ( maintongUV557 * appendResult1_g448 ) + appendResult3_g448 );
			#else
				float2 staticSwitch781 = ( ( i.uv_texcoord * appendResult1_g448 ) + appendResult3_g448 );
			#endif
			float3 temp_cast_106 = (0.0).xxx;
			float2 appendResult49_g314 = (float2(0.0 , 0.0));
			float4 break18_g312 = _DistortMaskTex_ST;
			float2 appendResult1_g312 = (float2(break18_g312.x , break18_g312.y));
			float2 appendResult3_g312 = (float2(break18_g312.z , break18_g312.w));
			float2 CenteredUV15_g313 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g313 = CenteredUV15_g313;
			float2 appendResult23_g313 = (float2(( length( CenteredUV15_g313 ) * break18_g312.x * 2.0 ) , ( atan2( break17_g313.x , break17_g313.y ) * ( 1.0 / 6.28318548202515 ) * break18_g312.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch1604 = ( ( i.uv_texcoord * appendResult1_g312 ) + appendResult3_g312 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch1604 = ( appendResult23_g313 + appendResult3_g312 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch1604 = ( ( maintongUV557 * appendResult1_g312 ) + appendResult3_g312 );
			#else
				float2 staticSwitch1604 = ( ( i.uv_texcoord * appendResult1_g312 ) + appendResult3_g312 );
			#endif
			float2 panner44_g314 = ( 1.0 * _Time.y * appendResult49_g314 + staticSwitch1604);
			float cos55_g314 = cos( ( ( ( _DistortMaskTexR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g314 = sin( ( ( ( _DistortMaskTexR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g314 = mul( panner44_g314 - float2( 0.5,0.5 ) , float2x2( cos55_g314 , -sin55_g314 , sin55_g314 , cos55_g314 )) + float2( 0.5,0.5 );
			float2 break52_g314 = rotator55_g314;
			float2 break54_g314 = rotator55_g314;
			float clampResult60_g314 = clamp( break54_g314.x , 0.0 , 1.0 );
			float clampResult50_g314 = clamp( break54_g314.y , 0.0 , 1.0 );
			float2 appendResult53_g314 = (float2(( (float)(int)_DistortMaskTexC == 0.0 ? break52_g314.x : clampResult60_g314 ) , ( (float)(int)_DistortMaskTexCV == 0.0 ? break52_g314.y : clampResult50_g314 )));
			float4 tex2DNode27_g314 = tex2D( _DistortMaskTex, appendResult53_g314 );
			float temp_output_1587_0 = ( ( _CustomDistort == 0.0 ? _DistortFactor : i.uv3_texcoord3.w ) * ( (float)(int)_DistortMaskTexAR == 0.0 ? tex2DNode27_g314.a : tex2DNode27_g314.r ) );
			float2 appendResult58 = (float2(_DistortTex_Uspeed , _DistortTex_Vspeed));
			float4 break18_g239 = _DistortTex_ST;
			float2 appendResult1_g239 = (float2(break18_g239.x , break18_g239.y));
			float2 appendResult3_g239 = (float2(break18_g239.z , break18_g239.w));
			float2 CenteredUV15_g240 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g240 = CenteredUV15_g240;
			float2 appendResult23_g240 = (float2(( length( CenteredUV15_g240 ) * break18_g239.x * 2.0 ) , ( atan2( break17_g240.x , break17_g240.y ) * ( 1.0 / 6.28318548202515 ) * break18_g239.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch829 = ( ( i.uv_texcoord * appendResult1_g239 ) + appendResult3_g239 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch829 = ( appendResult23_g240 + appendResult3_g239 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch829 = ( ( maintongUV557 * appendResult1_g239 ) + appendResult3_g239 );
			#else
				float2 staticSwitch829 = ( ( i.uv_texcoord * appendResult1_g239 ) + appendResult3_g239 );
			#endif
			float2 panner59 = ( 1.0 * _Time.y * appendResult58 + staticSwitch829);
			float4 tex2DNode54 = tex2D( _DistortTex, panner59 );
			float temp_output_1550_0 = ( _DistortTexAR == 0.0 ? tex2DNode54.a : tex2DNode54.r );
			float3 temp_cast_113 = (( temp_output_1587_0 * ( _DistortRemap == 0.0 ? temp_output_1550_0 : (-1.0 + (temp_output_1550_0 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) )).xxx;
			float2 break1571 = panner59;
			float temp_output_1570_0 = ( pow( 1.0 , 3.0 ) * 0.1 );
			float2 appendResult1568 = (float2(( break1571.x + temp_output_1570_0 ) , break1571.y));
			float4 tex2DNode1577 = tex2D( _DistortTex, appendResult1568 );
			float3 appendResult1564 = (float3(1.0 , 0.0 , ( ( ( _DistortTexAR == 0.0 ? tex2DNode1577.a : tex2DNode1577.g ) - temp_output_1550_0 ) * temp_output_1587_0 )));
			float2 appendResult1574 = (float2(break1571.x , ( break1571.y + temp_output_1570_0 )));
			float4 tex2DNode1576 = tex2D( _DistortTex, appendResult1574 );
			float3 appendResult1565 = (float3(0.0 , 1.0 , ( ( ( _DistortTexAR == 0.0 ? tex2DNode1576.a : tex2DNode1576.g ) - temp_output_1550_0 ) * temp_output_1587_0 )));
			float3 normalizeResult1563 = normalize( cross( appendResult1564 , appendResult1565 ) );
			float2 appendResult61 = (float2(tex2DNode54.r , tex2DNode54.g));
			float3 DistortUV60 = ( _IfFlowmap == 0.0 ? ( _IfNormalDistort == 0.0 ? temp_cast_113 : normalizeResult1563 ) : float3( ( temp_output_1587_0 * ( appendResult61 - i.uv_texcoord ) ) ,  0.0 ) );
			float3 temp_output_1066_0 = ( _DistortMask == 0.0 ? temp_cast_106 : DistortUV60 );
			float2 temp_cast_115 = (0.0).xx;
			float2 panner44_g470 = ( 1.0 * _Time.y * appendResult49_g470 + ( ( float3( staticSwitch781 ,  0.0 ) + temp_output_1066_0 ) + float3( (( _CustomdataMaskUV )?( i.uv3_texcoord3.xy ):( temp_cast_115 )) ,  0.0 ) ).xy);
			float cos55_g470 = cos( ( ( ( _Mask_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g470 = sin( ( ( ( _Mask_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g470 = mul( panner44_g470 - float2( 0.5,0.5 ) , float2x2( cos55_g470 , -sin55_g470 , sin55_g470 , cos55_g470 )) + float2( 0.5,0.5 );
			float2 break52_g470 = rotator55_g470;
			float2 break54_g470 = rotator55_g470;
			float clampResult60_g470 = clamp( break54_g470.x , 0.0 , 1.0 );
			float clampResult50_g470 = clamp( break54_g470.y , 0.0 , 1.0 );
			float2 appendResult53_g470 = (float2(( (float)(int)_MaskC == 0.0 ? break52_g470.x : clampResult60_g470 ) , ( (float)(int)_MaskCV == 0.0 ? break52_g470.y : clampResult50_g470 )));
			float4 tex2DNode27_g470 = tex2D( _MaskTex, appendResult53_g470 );
			float2 appendResult49_g482 = (float2(_MaskPlusUspeed , _MaskPlusVspeed));
			float3 temp_cast_124 = (0.0).xxx;
			float4 break18_g479 = _MaskPlusTex_ST;
			float2 appendResult1_g479 = (float2(break18_g479.x , break18_g479.y));
			float2 appendResult3_g479 = (float2(break18_g479.z , break18_g479.w));
			float2 CenteredUV15_g480 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g480 = CenteredUV15_g480;
			float2 appendResult23_g480 = (float2(( length( CenteredUV15_g480 ) * break18_g479.x * 2.0 ) , ( atan2( break17_g480.x , break17_g480.y ) * ( 1.0 / 6.28318548202515 ) * break18_g479.y )));
			#if defined(_MASKTEXUVS_NORMAL)
				float2 staticSwitch1714 = ( ( i.uv_texcoord * appendResult1_g479 ) + appendResult3_g479 );
			#elif defined(_MASKTEXUVS_POLAR)
				float2 staticSwitch1714 = ( appendResult23_g480 + appendResult3_g479 );
			#elif defined(_MASKTEXUVS_CYLINDER)
				float2 staticSwitch1714 = ( ( maintongUV557 * appendResult1_g479 ) + appendResult3_g479 );
			#else
				float2 staticSwitch1714 = ( ( i.uv_texcoord * appendResult1_g479 ) + appendResult3_g479 );
			#endif
			float2 panner44_g482 = ( 1.0 * _Time.y * appendResult49_g482 + ( temp_output_1066_0 + float3( staticSwitch1714 ,  0.0 ) ).xy);
			float cos55_g482 = cos( ( ( ( _MaskPlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g482 = sin( ( ( ( _MaskPlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g482 = mul( panner44_g482 - float2( 0.5,0.5 ) , float2x2( cos55_g482 , -sin55_g482 , sin55_g482 , cos55_g482 )) + float2( 0.5,0.5 );
			float2 break52_g482 = rotator55_g482;
			float2 break54_g482 = rotator55_g482;
			float clampResult60_g482 = clamp( break54_g482.x , 0.0 , 1.0 );
			float clampResult50_g482 = clamp( break54_g482.y , 0.0 , 1.0 );
			float2 appendResult53_g482 = (float2(( (float)(int)_MaskPlusC == 0.0 ? break52_g482.x : clampResult60_g482 ) , ( (float)(int)_MaskPlusCV == 0.0 ? break52_g482.y : clampResult50_g482 )));
			float4 tex2DNode27_g482 = tex2D( _MaskPlusTex, appendResult53_g482 );
			float MaskAlpha136 = ( _Mask_scale * ( ( (float)(int)_MaskAlphaRA == 0.0 ? tex2DNode27_g470.a : tex2DNode27_g470.r ) * ( _IfMaskPlusTex == 0.0 ? 1.0 : ( (float)(int)_MaskPlusAR == 0.0 ? tex2DNode27_g482.a : tex2DNode27_g482.r ) ) ) );
			float2 appendResult49_g467 = (float2(_MainTex_Uspeed , _MainTex_Vspeed));
			float CustomMainUV1692 = _CustomdataMainTexUV;
			float2 temp_cast_133 = (0.0).xx;
			float2 temp_output_1691_0 = ( CustomMainUV1692 == 0.0 ? temp_cast_133 : i.uv2_texcoord2.xy );
			float Ifpara1474 = _IfPara;
			float4 break18_g345 = _MainTex_ST;
			float2 appendResult1_g345 = (float2(break18_g345.x , break18_g345.y));
			float2 appendResult3_g345 = (float2(break18_g345.z , break18_g345.w));
			float2 CenteredUV15_g346 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g346 = CenteredUV15_g346;
			float2 appendResult23_g346 = (float2(( length( CenteredUV15_g346 ) * break18_g345.x * 2.0 ) , ( atan2( break17_g346.x , break17_g346.y ) * ( 1.0 / 6.28318548202515 ) * break18_g345.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch607 = ( ( i.uv_texcoord * appendResult1_g345 ) + appendResult3_g345 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch607 = ( appendResult23_g346 + appendResult3_g345 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch607 = ( ( maintongUV557 * appendResult1_g345 ) + appendResult3_g345 );
			#else
				float2 staticSwitch607 = ( ( i.uv_texcoord * appendResult1_g345 ) + appendResult3_g345 );
			#endif
			float Parallax1492 = ( _Parallax * 0.1 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 OffsetPOM80_g422 = POM( _ParaTex, staticSwitch607, ddx(staticSwitch607), ddy(staticSwitch607), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_136 = (0.0).xxx;
			float3 temp_output_1057_0 = ( _DistortMainTex == 0.0 ? temp_cast_136 : DistortUV60 );
			float2 panner44_g467 = ( 1.0 * _Time.y * appendResult49_g467 + ( float3( temp_output_1691_0 ,  0.0 ) + ( float3( ( Ifpara1474 == 0.0 ? staticSwitch607 : OffsetPOM80_g422 ) ,  0.0 ) + temp_output_1057_0 ) ).xy);
			float cos55_g467 = cos( ( ( ( _MainTex_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g467 = sin( ( ( ( _MainTex_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g467 = mul( panner44_g467 - float2( 0.5,0.5 ) , float2x2( cos55_g467 , -sin55_g467 , sin55_g467 , cos55_g467 )) + float2( 0.5,0.5 );
			float2 break52_g467 = rotator55_g467;
			float2 break54_g467 = rotator55_g467;
			float clampResult60_g467 = clamp( break54_g467.x , 0.0 , 1.0 );
			float clampResult50_g467 = clamp( break54_g467.y , 0.0 , 1.0 );
			float2 appendResult53_g467 = (float2(( (float)(int)_MaintexC == 0.0 ? break52_g467.x : clampResult60_g467 ) , ( (float)(int)_MaintexCV == 0.0 ? break52_g467.y : clampResult50_g467 )));
			float4 tex2DNode27_g467 = tex2D( _MainTex, appendResult53_g467 );
			float temp_output_1123_39 = ( (float)(int)_MainTex_ar == 0.0 ? tex2DNode27_g467.a : tex2DNode27_g467.r );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 temp_cast_140 = (0.0).xxx;
			float4 screenColor917 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( temp_output_1057_0 , 0.0 ) ).xy);
			float temp_output_7_0 = ( ( _CustomdataDisT == 0.0 ? i.vertexColor.a : 1.0 ) * ( _ScreenAsMain == 0.0 ? temp_output_1123_39 : screenColor917.a ) * _MainColor.a );
			float2 appendResult49_g465 = (float2(_AddTexUspeed , _AddTexVspeed));
			float4 break18_g347 = _AddTex_ST;
			float2 appendResult1_g347 = (float2(break18_g347.x , break18_g347.y));
			float2 appendResult3_g347 = (float2(break18_g347.z , break18_g347.w));
			float2 CenteredUV15_g348 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g348 = CenteredUV15_g348;
			float2 appendResult23_g348 = (float2(( length( CenteredUV15_g348 ) * break18_g347.x * 2.0 ) , ( atan2( break17_g348.x , break17_g348.y ) * ( 1.0 / 6.28318548202515 ) * break18_g347.y )));
			float2 ifLocalVar1687 = 0;
			if( _AddTexUVS > 1.0 )
				ifLocalVar1687 = ( ( maintongUV557 * appendResult1_g347 ) + appendResult3_g347 );
			else if( _AddTexUVS == 1.0 )
				ifLocalVar1687 = ( appendResult23_g348 + appendResult3_g347 );
			else if( _AddTexUVS < 1.0 )
				ifLocalVar1687 = ( ( i.uv_texcoord * appendResult1_g347 ) + appendResult3_g347 );
			float2 OffsetPOM80_g447 = POM( _ParaTex, ifLocalVar1687, ddx(ifLocalVar1687), ddy(ifLocalVar1687), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_148 = (0.0).xxx;
			float2 temp_cast_149 = (0.0).xx;
			float2 temp_cast_150 = (1.0).xx;
			float2 appendResult1703 = (float2(( _AddTex_ST.x / _MainTex_ST.x ) , ( _AddTex_ST.y / _MainTex_ST.y )));
			float2 temp_cast_151 = (0.0).xx;
			float2 panner44_g465 = ( 1.0 * _Time.y * appendResult49_g465 + ( float3( ( Ifpara1474 == 0.0 ? ifLocalVar1687 : OffsetPOM80_g447 ) ,  0.0 ) + ( _DistortAddTex == 0.0 ? temp_cast_148 : DistortUV60 ) + float3( ( _CAddTexUV == 0.0 ? temp_cast_149 : ( ( _CAddTexUVT == 0.0 ? temp_cast_150 : appendResult1703 ) * temp_output_1691_0 ) ) ,  0.0 ) ).xy);
			float cos55_g465 = cos( ( ( ( _AddTexRo / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g465 = sin( ( ( ( _AddTexRo / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g465 = mul( panner44_g465 - float2( 0.5,0.5 ) , float2x2( cos55_g465 , -sin55_g465 , sin55_g465 , cos55_g465 )) + float2( 0.5,0.5 );
			float2 break52_g465 = rotator55_g465;
			float2 break54_g465 = rotator55_g465;
			float clampResult60_g465 = clamp( break54_g465.x , 0.0 , 1.0 );
			float clampResult50_g465 = clamp( break54_g465.y , 0.0 , 1.0 );
			float2 appendResult53_g465 = (float2(( (float)(int)_AddTexC == 0.0 ? break52_g465.x : clampResult60_g465 ) , ( (float)(int)_AddTexCV == 0.0 ? break52_g465.y : clampResult50_g465 )));
			float4 tex2DNode27_g465 = tex2D( _AddTex, appendResult53_g465 );
			float temp_output_1121_39 = ( (float)(int)_AddTexAR == 0.0 ? tex2DNode27_g465.a : tex2DNode27_g465.r );
			float AddTexAlpha1655 = temp_output_1121_39;
			float lerpResult1659 = lerp( temp_output_7_0 , AddTexAlpha1655 , _AddTexBlend);
			float ifLocalVar1676 = 0;
			if( _AddTexBlendMode > 1.0 )
				ifLocalVar1676 = ( AddTexAlpha1655 * temp_output_7_0 );
			else if( _AddTexBlendMode == 1.0 )
				ifLocalVar1676 = ( AddTexAlpha1655 + temp_output_7_0 );
			else if( _AddTexBlendMode < 1.0 )
				ifLocalVar1676 = lerpResult1659;
			float MainTexAlpha37 = ( ( _IfAddTexAlpha == 0.0 ? temp_output_7_0 : ifLocalVar1676 ) * _MainAlpha );
			float temp_output_51_0_g468 = _DIssloveSoft;
			float temp_output_53_0_g468 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g468 = ( ( temp_output_51_0_g468 + 1.0 ) * temp_output_53_0_g468 );
			int temp_output_38_0_g453 = (int)_DissolveAR;
			float2 appendResult49_g453 = (float2(_DisTex_Uspeed , _DisTex_Vspeed));
			float4 break18_g420 = _DissloveTex_ST;
			float2 appendResult1_g420 = (float2(break18_g420.x , break18_g420.y));
			float2 appendResult3_g420 = (float2(break18_g420.z , break18_g420.w));
			float2 CenteredUV15_g421 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g421 = CenteredUV15_g421;
			float2 appendResult23_g421 = (float2(( length( CenteredUV15_g421 ) * break18_g420.x * 2.0 ) , ( atan2( break17_g421.x , break17_g421.y ) * ( 1.0 / 6.28318548202515 ) * break18_g420.y )));
			#if defined(_DISSOLVETEXUVS_NORMAL)
				float2 staticSwitch805 = ( ( i.uv_texcoord * appendResult1_g420 ) + appendResult3_g420 );
			#elif defined(_DISSOLVETEXUVS_POLAR)
				float2 staticSwitch805 = ( appendResult23_g421 + appendResult3_g420 );
			#elif defined(_DISSOLVETEXUVS_CYLINDER)
				float2 staticSwitch805 = ( ( maintongUV557 * appendResult1_g420 ) + appendResult3_g420 );
			#else
				float2 staticSwitch805 = ( ( i.uv_texcoord * appendResult1_g420 ) + appendResult3_g420 );
			#endif
			float3 temp_cast_161 = (0.0).xxx;
			float2 panner44_g453 = ( 1.0 * _Time.y * appendResult49_g453 + ( float3( staticSwitch805 ,  0.0 ) + ( _DistortDisTex == 0.0 ? temp_cast_161 : DistortUV60 ) ).xy);
			float cos55_g453 = cos( ( ( ( _DIssolve_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g453 = sin( ( ( ( _DIssolve_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g453 = mul( panner44_g453 - float2( 0.5,0.5 ) , float2x2( cos55_g453 , -sin55_g453 , sin55_g453 , cos55_g453 )) + float2( 0.5,0.5 );
			float2 break52_g453 = rotator55_g453;
			float2 break54_g453 = rotator55_g453;
			float clampResult60_g453 = clamp( break54_g453.x , 0.0 , 1.0 );
			float clampResult50_g453 = clamp( break54_g453.y , 0.0 , 1.0 );
			float2 appendResult53_g453 = (float2(( (float)(int)_DissolveC == 0.0 ? break52_g453.x : clampResult60_g453 ) , ( (float)(int)_DissolveCV == 0.0 ? break52_g453.y : clampResult50_g453 )));
			float4 tex2DNode27_g453 = tex2D( _DissloveTex, appendResult53_g453 );
			float temp_output_57_0_g468 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode27_g453.a : tex2DNode27_g453.r ) , _DissolveTexExp ) );
			int temp_output_38_0_g452 = (int)_DissolvePlusAR;
			float2 appendResult49_g452 = (float2(0.0 , 0.0));
			float4 break18_g450 = _DissloveTexPlus_ST;
			float2 appendResult1_g450 = (float2(break18_g450.x , break18_g450.y));
			float2 appendResult3_g450 = (float2(break18_g450.z , break18_g450.w));
			float2 CenteredUV15_g451 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g451 = CenteredUV15_g451;
			float2 appendResult23_g451 = (float2(( length( CenteredUV15_g451 ) * break18_g450.x * 2.0 ) , ( atan2( break17_g451.x , break17_g451.y ) * ( 1.0 / 6.28318548202515 ) * break18_g450.y )));
			#if defined(_DISSOLVETEXUVS_NORMAL)
				float2 staticSwitch817 = ( ( i.uv_texcoord * appendResult1_g450 ) + appendResult3_g450 );
			#elif defined(_DISSOLVETEXUVS_POLAR)
				float2 staticSwitch817 = ( appendResult23_g451 + appendResult3_g450 );
			#elif defined(_DISSOLVETEXUVS_CYLINDER)
				float2 staticSwitch817 = ( ( maintongUV557 * appendResult1_g450 ) + appendResult3_g450 );
			#else
				float2 staticSwitch817 = ( ( i.uv_texcoord * appendResult1_g450 ) + appendResult3_g450 );
			#endif
			float2 panner44_g452 = ( 1.0 * _Time.y * appendResult49_g452 + staticSwitch817);
			float cos55_g452 = cos( ( ( ( _DissolvePlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g452 = sin( ( ( ( _DissolvePlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g452 = mul( panner44_g452 - float2( 0.5,0.5 ) , float2x2( cos55_g452 , -sin55_g452 , sin55_g452 , cos55_g452 )) + float2( 0.5,0.5 );
			float2 break52_g452 = rotator55_g452;
			float2 break54_g452 = rotator55_g452;
			float clampResult60_g452 = clamp( break54_g452.x , 0.0 , 1.0 );
			float clampResult50_g452 = clamp( break54_g452.y , 0.0 , 1.0 );
			float2 appendResult53_g452 = (float2(( (float)(int)_DissolvePlusC == 0.0 ? break52_g452.x : clampResult60_g452 ) , ( (float)(int)_DissolvePlusCV == 0.0 ? break52_g452.y : clampResult50_g452 )));
			float4 tex2DNode27_g452 = tex2D( _DissloveTexPlus, appendResult53_g452 );
			float temp_output_4_0_g468 = saturate( ( ( ( temp_output_57_0_g468 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g468 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode27_g452.a : tex2DNode27_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g468 = smoothstep( ( temp_output_2_0_g468 - temp_output_51_0_g468 ) , temp_output_2_0_g468 , temp_output_4_0_g468);
			float temp_output_1512_0 = smoothstepResult21_g468;
			float temp_output_44_0_g468 = _DIssloveWide;
			float temp_output_3_0_g468 = ( temp_output_53_0_g468 * ( 1.0 + temp_output_44_0_g468 ) );
			float temp_output_1512_43 = step( ( temp_output_3_0_g468 - temp_output_44_0_g468 ) , temp_output_4_0_g468 );
			float DisAplha42 = ( _sot_sting_A == 0.0 ? temp_output_1512_0 : temp_output_1512_43 );
			float dotResult106 = dot( ase_worldViewDir , ase_worldNormal );
			float softedge111 = pow( saturate( (( _softback )?( dotResult106 ):( abs( dotResult106 ) )) ) , _softFacotr );
			float4 ase_screenPos88 = i.screenPosition88;
			float4 ase_screenPosNorm88 = ase_screenPos88 / ase_screenPos88.w;
			ase_screenPosNorm88.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm88.z : ase_screenPosNorm88.z * 0.5 + 0.5;
			float screenDepth88 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm88.xy ));
			float distanceDepth88 = saturate( ( screenDepth88 - LinearEyeDepth( ase_screenPosNorm88.z ) ) / ( _DepthfadeFactor ) );
			float temp_output_409_0 = ( 1.0 - saturate( distanceDepth88 ) );
			float MainAlpha142 = saturate( ( ( MaskAlpha136 * MainTexAlpha37 * DisAplha42 * (( _FNLfanxiangkaiguan )?( softedge111 ):( 1.0 )) * (( _Depthfadeon )?( ( _DepthF == 0.0 ? distanceDepth88 : 1.0 ) ):( 1.0 )) ) + ( _DepthF == 0.0 ? 0.0 : temp_output_409_0 ) ) );
			float temp_output_143_0 = MainAlpha142;
			float4 color1051 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult1001 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float2 appendResult56_g481 = (float2(_NormalTex_Uspeed , _NormalTex_Vspeed));
			float4 break18_g473 = _NormalTex_ST;
			float2 appendResult1_g473 = (float2(break18_g473.x , break18_g473.y));
			float2 appendResult3_g473 = (float2(break18_g473.z , break18_g473.w));
			float2 CenteredUV15_g474 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g474 = CenteredUV15_g474;
			float2 appendResult23_g474 = (float2(( length( CenteredUV15_g474 ) * break18_g473.x * 2.0 ) , ( atan2( break17_g474.x , break17_g474.y ) * ( 1.0 / 6.28318548202515 ) * break18_g473.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch1031 = ( ( i.uv_texcoord * appendResult1_g473 ) + appendResult3_g473 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch1031 = ( appendResult23_g474 + appendResult3_g473 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch1031 = ( ( maintongUV557 * appendResult1_g473 ) + appendResult3_g473 );
			#else
				float2 staticSwitch1031 = ( ( i.uv_texcoord * appendResult1_g473 ) + appendResult3_g473 );
			#endif
			float2 OffsetPOM80_g478 = POM( _ParaTex, staticSwitch1031, ddx(staticSwitch1031), ddy(staticSwitch1031), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_174 = (0.0).xxx;
			float2 panner51_g481 = ( 1.0 * _Time.y * appendResult56_g481 + ( float3( ( Ifpara1474 == 0.0 ? staticSwitch1031 : OffsetPOM80_g478 ) ,  0.0 ) + ( _DistortNormalTex == 0.0 ? temp_cast_174 : DistortUV60 ) ).xy);
			float cos62_g481 = cos( ( ( ( _NormalTex_Rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin62_g481 = sin( ( ( ( _NormalTex_Rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator62_g481 = mul( panner51_g481 - float2( 0.5,0.5 ) , float2x2( cos62_g481 , -sin62_g481 , sin62_g481 , cos62_g481 )) + float2( 0.5,0.5 );
			float2 break59_g481 = rotator62_g481;
			float2 break61_g481 = rotator62_g481;
			float clampResult67_g481 = clamp( break61_g481.x , 0.0 , 1.0 );
			float clampResult57_g481 = clamp( break61_g481.y , 0.0 , 1.0 );
			float2 appendResult60_g481 = (float2(( (float)(int)_NormalTexC == 0.0 ? break59_g481.x : clampResult67_g481 ) , ( (float)(int)_NormalTexCV == 0.0 ? break59_g481.y : clampResult57_g481 )));
			float temp_output_51_0_g466 = _DIssloveSoft;
			float temp_output_53_0_g466 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g466 = ( ( temp_output_51_0_g466 + 1.0 ) * temp_output_53_0_g466 );
			float2 break68_g453 = appendResult53_g453;
			float temp_output_64_0_g453 = ( pow( _StaticNormalOffset , 3.0 ) * 0.1 );
			float2 appendResult67_g453 = (float2(( break68_g453.x + temp_output_64_0_g453 ) , break68_g453.y));
			float4 tex2DNode70_g453 = tex2D( _DissloveTex, appendResult67_g453 );
			float temp_output_57_0_g466 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode70_g453.a : tex2DNode70_g453.r ) , _DissolveTexExp ) );
			float2 break68_g452 = appendResult53_g452;
			float temp_output_64_0_g452 = ( pow( _StaticNormalOffset , 3.0 ) * 0.1 );
			float2 appendResult67_g452 = (float2(( break68_g452.x + temp_output_64_0_g452 ) , break68_g452.y));
			float4 tex2DNode70_g452 = tex2D( _DissloveTexPlus, appendResult67_g452 );
			float temp_output_4_0_g466 = saturate( ( ( ( temp_output_57_0_g466 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g466 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode70_g452.a : tex2DNode70_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g466 = smoothstep( ( temp_output_2_0_g466 - temp_output_51_0_g466 ) , temp_output_2_0_g466 , temp_output_4_0_g466);
			float temp_output_44_0_g466 = _DIssloveWide;
			float temp_output_3_0_g466 = ( temp_output_53_0_g466 * ( 1.0 + temp_output_44_0_g466 ) );
			float DisAplha11397 = ( _sot_sting_A == 0.0 ? smoothstepResult21_g466 : step( ( temp_output_3_0_g466 - temp_output_44_0_g466 ) , temp_output_4_0_g466 ) );
			float temp_output_1402_0 = ( _StaticNormalScale * 10.0 );
			float3 appendResult1273 = (float3(1.0 , 0.0 , ( ( DisAplha11397 - DisAplha42 ) * temp_output_1402_0 )));
			float temp_output_51_0_g469 = _DIssloveSoft;
			float temp_output_53_0_g469 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g469 = ( ( temp_output_51_0_g469 + 1.0 ) * temp_output_53_0_g469 );
			float2 appendResult63_g453 = (float2(break68_g453.x , ( break68_g453.y + temp_output_64_0_g453 )));
			float4 tex2DNode76_g453 = tex2D( _DissloveTex, appendResult63_g453 );
			float temp_output_57_0_g469 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode76_g453.a : tex2DNode76_g453.r ) , _DissolveTexExp ) );
			float2 appendResult63_g452 = (float2(break68_g452.x , ( break68_g452.y + temp_output_64_0_g452 )));
			float4 tex2DNode76_g452 = tex2D( _DissloveTexPlus, appendResult63_g452 );
			float temp_output_4_0_g469 = saturate( ( ( ( temp_output_57_0_g469 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g469 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode76_g452.a : tex2DNode76_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g469 = smoothstep( ( temp_output_2_0_g469 - temp_output_51_0_g469 ) , temp_output_2_0_g469 , temp_output_4_0_g469);
			float temp_output_44_0_g469 = _DIssloveWide;
			float temp_output_3_0_g469 = ( temp_output_53_0_g469 * ( 1.0 + temp_output_44_0_g469 ) );
			float DisAplha21398 = ( _sot_sting_A == 0.0 ? smoothstepResult21_g469 : step( ( temp_output_3_0_g469 - temp_output_44_0_g469 ) , temp_output_4_0_g469 ) );
			float3 appendResult1268 = (float3(0.0 , 1.0 , ( ( DisAplha21398 - DisAplha42 ) * temp_output_1402_0 )));
			float3 normalizeResult1265 = normalize( cross( appendResult1273 , appendResult1268 ) );
			float3 Normal1046 = BlendNormals( UnpackScaleNormal( tex2D( _NormalTex, appendResult60_g481 ), _NormalScale ) , ( _IfStaticNormal == 0.0 ? float3(0,0,1) : normalizeResult1265 ) );
			float3 normalizeResult998 = normalize( (WorldNormalVector( i , Normal1046 )) );
			float dotResult1006 = dot( normalizeResult1001 , normalizeResult998 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_1007_0 = ( ase_lightColor * ase_lightAtten );
			float dotResult1004 = dot( normalizeResult998 , ase_worldlightDir );
			UnityGI gi1012 = gi;
			float3 diffNorm1012 = normalizeResult998;
			gi1012 = UnityGI_Base( data, 1, diffNorm1012 );
			float3 indirectDiffuse1012 = gi1012.indirect.diffuse + diffNorm1012 * 0.0001;
			float4 light1023 = ( ( pow( max( dotResult1006 , 0.0 ) , ( 0.1 * 128.0 ) ) * temp_output_1007_0 * _LightScale ) + ( ( ( temp_output_1007_0 * max( dotResult1004 , 0.0 ) ) + float4( indirectDiffuse1012 , 0.0 ) ) * _LightScale ) );
			c.rgb = (( _IfCustomLight )?( ( MainAlpha142 * light1023 ) ):( color1051 )).rgb;
			c.a = temp_output_143_0;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 appendResult49_g470 = (float2(_Mask_Uspeed , _Mask_Vspeed));
			float4 break18_g448 = _MaskTex_ST;
			float2 appendResult1_g448 = (float2(break18_g448.x , break18_g448.y));
			float2 appendResult3_g448 = (float2(break18_g448.z , break18_g448.w));
			float2 appendResult866 = (float2(_CenterU , _CenterV));
			float2 centeruv867 = appendResult866;
			float2 CenteredUV15_g449 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g449 = CenteredUV15_g449;
			float2 appendResult23_g449 = (float2(( length( CenteredUV15_g449 ) * break18_g448.x * 2.0 ) , ( atan2( break17_g449.x , break17_g449.y ) * ( 1.0 / 6.28318548202515 ) * break18_g448.y )));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 break537 = ( _TexCenter + float4( ase_vertex3Pos , 0.0 ) );
			float2 appendResult554 = (float2((0.0 + (atan( ( break537.x / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.y));
			float2 appendResult553 = (float2((0.0 + (atan( ( break537.y / break537.z ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.x));
			float2 appendResult555 = (float2((0.0 + (atan( ( break537.x / break537.y ) ) - ( -0.5 * UNITY_PI )) * (1.0 - 0.0) / (( 0.5 * UNITY_PI ) - ( -0.5 * UNITY_PI ))) , break537.z));
			#if defined(_FACE_X)
				float2 staticSwitch556 = appendResult553;
			#elif defined(_FACE_Y)
				float2 staticSwitch556 = appendResult554;
			#elif defined(_FACE_Z)
				float2 staticSwitch556 = appendResult555;
			#else
				float2 staticSwitch556 = appendResult554;
			#endif
			float2 maintongUV557 = staticSwitch556;
			#if defined(_MASKTEXUVS_NORMAL)
				float2 staticSwitch781 = ( ( i.uv_texcoord * appendResult1_g448 ) + appendResult3_g448 );
			#elif defined(_MASKTEXUVS_POLAR)
				float2 staticSwitch781 = ( appendResult23_g449 + appendResult3_g448 );
			#elif defined(_MASKTEXUVS_CYLINDER)
				float2 staticSwitch781 = ( ( maintongUV557 * appendResult1_g448 ) + appendResult3_g448 );
			#else
				float2 staticSwitch781 = ( ( i.uv_texcoord * appendResult1_g448 ) + appendResult3_g448 );
			#endif
			float3 temp_cast_6 = (0.0).xxx;
			float2 appendResult49_g314 = (float2(0.0 , 0.0));
			float4 break18_g312 = _DistortMaskTex_ST;
			float2 appendResult1_g312 = (float2(break18_g312.x , break18_g312.y));
			float2 appendResult3_g312 = (float2(break18_g312.z , break18_g312.w));
			float2 CenteredUV15_g313 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g313 = CenteredUV15_g313;
			float2 appendResult23_g313 = (float2(( length( CenteredUV15_g313 ) * break18_g312.x * 2.0 ) , ( atan2( break17_g313.x , break17_g313.y ) * ( 1.0 / 6.28318548202515 ) * break18_g312.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch1604 = ( ( i.uv_texcoord * appendResult1_g312 ) + appendResult3_g312 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch1604 = ( appendResult23_g313 + appendResult3_g312 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch1604 = ( ( maintongUV557 * appendResult1_g312 ) + appendResult3_g312 );
			#else
				float2 staticSwitch1604 = ( ( i.uv_texcoord * appendResult1_g312 ) + appendResult3_g312 );
			#endif
			float2 panner44_g314 = ( 1.0 * _Time.y * appendResult49_g314 + staticSwitch1604);
			float cos55_g314 = cos( ( ( ( _DistortMaskTexR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g314 = sin( ( ( ( _DistortMaskTexR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g314 = mul( panner44_g314 - float2( 0.5,0.5 ) , float2x2( cos55_g314 , -sin55_g314 , sin55_g314 , cos55_g314 )) + float2( 0.5,0.5 );
			float2 break52_g314 = rotator55_g314;
			float2 break54_g314 = rotator55_g314;
			float clampResult60_g314 = clamp( break54_g314.x , 0.0 , 1.0 );
			float clampResult50_g314 = clamp( break54_g314.y , 0.0 , 1.0 );
			float2 appendResult53_g314 = (float2(( (float)(int)_DistortMaskTexC == 0.0 ? break52_g314.x : clampResult60_g314 ) , ( (float)(int)_DistortMaskTexCV == 0.0 ? break52_g314.y : clampResult50_g314 )));
			float4 tex2DNode27_g314 = tex2D( _DistortMaskTex, appendResult53_g314 );
			float temp_output_1587_0 = ( ( _CustomDistort == 0.0 ? _DistortFactor : i.uv3_texcoord3.w ) * ( (float)(int)_DistortMaskTexAR == 0.0 ? tex2DNode27_g314.a : tex2DNode27_g314.r ) );
			float2 appendResult58 = (float2(_DistortTex_Uspeed , _DistortTex_Vspeed));
			float4 break18_g239 = _DistortTex_ST;
			float2 appendResult1_g239 = (float2(break18_g239.x , break18_g239.y));
			float2 appendResult3_g239 = (float2(break18_g239.z , break18_g239.w));
			float2 CenteredUV15_g240 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g240 = CenteredUV15_g240;
			float2 appendResult23_g240 = (float2(( length( CenteredUV15_g240 ) * break18_g239.x * 2.0 ) , ( atan2( break17_g240.x , break17_g240.y ) * ( 1.0 / 6.28318548202515 ) * break18_g239.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch829 = ( ( i.uv_texcoord * appendResult1_g239 ) + appendResult3_g239 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch829 = ( appendResult23_g240 + appendResult3_g239 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch829 = ( ( maintongUV557 * appendResult1_g239 ) + appendResult3_g239 );
			#else
				float2 staticSwitch829 = ( ( i.uv_texcoord * appendResult1_g239 ) + appendResult3_g239 );
			#endif
			float2 panner59 = ( 1.0 * _Time.y * appendResult58 + staticSwitch829);
			float4 tex2DNode54 = tex2D( _DistortTex, panner59 );
			float temp_output_1550_0 = ( _DistortTexAR == 0.0 ? tex2DNode54.a : tex2DNode54.r );
			float3 temp_cast_13 = (( temp_output_1587_0 * ( _DistortRemap == 0.0 ? temp_output_1550_0 : (-1.0 + (temp_output_1550_0 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) )).xxx;
			float2 break1571 = panner59;
			float temp_output_1570_0 = ( pow( 1.0 , 3.0 ) * 0.1 );
			float2 appendResult1568 = (float2(( break1571.x + temp_output_1570_0 ) , break1571.y));
			float4 tex2DNode1577 = tex2D( _DistortTex, appendResult1568 );
			float3 appendResult1564 = (float3(1.0 , 0.0 , ( ( ( _DistortTexAR == 0.0 ? tex2DNode1577.a : tex2DNode1577.g ) - temp_output_1550_0 ) * temp_output_1587_0 )));
			float2 appendResult1574 = (float2(break1571.x , ( break1571.y + temp_output_1570_0 )));
			float4 tex2DNode1576 = tex2D( _DistortTex, appendResult1574 );
			float3 appendResult1565 = (float3(0.0 , 1.0 , ( ( ( _DistortTexAR == 0.0 ? tex2DNode1576.a : tex2DNode1576.g ) - temp_output_1550_0 ) * temp_output_1587_0 )));
			float3 normalizeResult1563 = normalize( cross( appendResult1564 , appendResult1565 ) );
			float2 appendResult61 = (float2(tex2DNode54.r , tex2DNode54.g));
			float3 DistortUV60 = ( _IfFlowmap == 0.0 ? ( _IfNormalDistort == 0.0 ? temp_cast_13 : normalizeResult1563 ) : float3( ( temp_output_1587_0 * ( appendResult61 - i.uv_texcoord ) ) ,  0.0 ) );
			float3 temp_output_1066_0 = ( _DistortMask == 0.0 ? temp_cast_6 : DistortUV60 );
			float2 temp_cast_15 = (0.0).xx;
			float2 panner44_g470 = ( 1.0 * _Time.y * appendResult49_g470 + ( ( float3( staticSwitch781 ,  0.0 ) + temp_output_1066_0 ) + float3( (( _CustomdataMaskUV )?( i.uv3_texcoord3.xy ):( temp_cast_15 )) ,  0.0 ) ).xy);
			float cos55_g470 = cos( ( ( ( _Mask_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g470 = sin( ( ( ( _Mask_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g470 = mul( panner44_g470 - float2( 0.5,0.5 ) , float2x2( cos55_g470 , -sin55_g470 , sin55_g470 , cos55_g470 )) + float2( 0.5,0.5 );
			float2 break52_g470 = rotator55_g470;
			float2 break54_g470 = rotator55_g470;
			float clampResult60_g470 = clamp( break54_g470.x , 0.0 , 1.0 );
			float clampResult50_g470 = clamp( break54_g470.y , 0.0 , 1.0 );
			float2 appendResult53_g470 = (float2(( (float)(int)_MaskC == 0.0 ? break52_g470.x : clampResult60_g470 ) , ( (float)(int)_MaskCV == 0.0 ? break52_g470.y : clampResult50_g470 )));
			float4 tex2DNode27_g470 = tex2D( _MaskTex, appendResult53_g470 );
			float2 appendResult49_g482 = (float2(_MaskPlusUspeed , _MaskPlusVspeed));
			float3 temp_cast_24 = (0.0).xxx;
			float4 break18_g479 = _MaskPlusTex_ST;
			float2 appendResult1_g479 = (float2(break18_g479.x , break18_g479.y));
			float2 appendResult3_g479 = (float2(break18_g479.z , break18_g479.w));
			float2 CenteredUV15_g480 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g480 = CenteredUV15_g480;
			float2 appendResult23_g480 = (float2(( length( CenteredUV15_g480 ) * break18_g479.x * 2.0 ) , ( atan2( break17_g480.x , break17_g480.y ) * ( 1.0 / 6.28318548202515 ) * break18_g479.y )));
			#if defined(_MASKTEXUVS_NORMAL)
				float2 staticSwitch1714 = ( ( i.uv_texcoord * appendResult1_g479 ) + appendResult3_g479 );
			#elif defined(_MASKTEXUVS_POLAR)
				float2 staticSwitch1714 = ( appendResult23_g480 + appendResult3_g479 );
			#elif defined(_MASKTEXUVS_CYLINDER)
				float2 staticSwitch1714 = ( ( maintongUV557 * appendResult1_g479 ) + appendResult3_g479 );
			#else
				float2 staticSwitch1714 = ( ( i.uv_texcoord * appendResult1_g479 ) + appendResult3_g479 );
			#endif
			float2 panner44_g482 = ( 1.0 * _Time.y * appendResult49_g482 + ( temp_output_1066_0 + float3( staticSwitch1714 ,  0.0 ) ).xy);
			float cos55_g482 = cos( ( ( ( _MaskPlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g482 = sin( ( ( ( _MaskPlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g482 = mul( panner44_g482 - float2( 0.5,0.5 ) , float2x2( cos55_g482 , -sin55_g482 , sin55_g482 , cos55_g482 )) + float2( 0.5,0.5 );
			float2 break52_g482 = rotator55_g482;
			float2 break54_g482 = rotator55_g482;
			float clampResult60_g482 = clamp( break54_g482.x , 0.0 , 1.0 );
			float clampResult50_g482 = clamp( break54_g482.y , 0.0 , 1.0 );
			float2 appendResult53_g482 = (float2(( (float)(int)_MaskPlusC == 0.0 ? break52_g482.x : clampResult60_g482 ) , ( (float)(int)_MaskPlusCV == 0.0 ? break52_g482.y : clampResult50_g482 )));
			float4 tex2DNode27_g482 = tex2D( _MaskPlusTex, appendResult53_g482 );
			float MaskAlpha136 = ( _Mask_scale * ( ( (float)(int)_MaskAlphaRA == 0.0 ? tex2DNode27_g470.a : tex2DNode27_g470.r ) * ( _IfMaskPlusTex == 0.0 ? 1.0 : ( (float)(int)_MaskPlusAR == 0.0 ? tex2DNode27_g482.a : tex2DNode27_g482.r ) ) ) );
			float2 appendResult49_g467 = (float2(_MainTex_Uspeed , _MainTex_Vspeed));
			float CustomMainUV1692 = _CustomdataMainTexUV;
			float2 temp_cast_33 = (0.0).xx;
			float2 temp_output_1691_0 = ( CustomMainUV1692 == 0.0 ? temp_cast_33 : i.uv2_texcoord2.xy );
			float Ifpara1474 = _IfPara;
			float4 break18_g345 = _MainTex_ST;
			float2 appendResult1_g345 = (float2(break18_g345.x , break18_g345.y));
			float2 appendResult3_g345 = (float2(break18_g345.z , break18_g345.w));
			float2 CenteredUV15_g346 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g346 = CenteredUV15_g346;
			float2 appendResult23_g346 = (float2(( length( CenteredUV15_g346 ) * break18_g345.x * 2.0 ) , ( atan2( break17_g346.x , break17_g346.y ) * ( 1.0 / 6.28318548202515 ) * break18_g345.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch607 = ( ( i.uv_texcoord * appendResult1_g345 ) + appendResult3_g345 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch607 = ( appendResult23_g346 + appendResult3_g345 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch607 = ( ( maintongUV557 * appendResult1_g345 ) + appendResult3_g345 );
			#else
				float2 staticSwitch607 = ( ( i.uv_texcoord * appendResult1_g345 ) + appendResult3_g345 );
			#endif
			float Parallax1492 = ( _Parallax * 0.1 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 OffsetPOM80_g422 = POM( _ParaTex, staticSwitch607, ddx(staticSwitch607), ddy(staticSwitch607), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_36 = (0.0).xxx;
			float3 temp_output_1057_0 = ( _DistortMainTex == 0.0 ? temp_cast_36 : DistortUV60 );
			float2 panner44_g467 = ( 1.0 * _Time.y * appendResult49_g467 + ( float3( temp_output_1691_0 ,  0.0 ) + ( float3( ( Ifpara1474 == 0.0 ? staticSwitch607 : OffsetPOM80_g422 ) ,  0.0 ) + temp_output_1057_0 ) ).xy);
			float cos55_g467 = cos( ( ( ( _MainTex_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g467 = sin( ( ( ( _MainTex_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g467 = mul( panner44_g467 - float2( 0.5,0.5 ) , float2x2( cos55_g467 , -sin55_g467 , sin55_g467 , cos55_g467 )) + float2( 0.5,0.5 );
			float2 break52_g467 = rotator55_g467;
			float2 break54_g467 = rotator55_g467;
			float clampResult60_g467 = clamp( break54_g467.x , 0.0 , 1.0 );
			float clampResult50_g467 = clamp( break54_g467.y , 0.0 , 1.0 );
			float2 appendResult53_g467 = (float2(( (float)(int)_MaintexC == 0.0 ? break52_g467.x : clampResult60_g467 ) , ( (float)(int)_MaintexCV == 0.0 ? break52_g467.y : clampResult50_g467 )));
			float4 tex2DNode27_g467 = tex2D( _MainTex, appendResult53_g467 );
			float temp_output_1123_39 = ( (float)(int)_MainTex_ar == 0.0 ? tex2DNode27_g467.a : tex2DNode27_g467.r );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 temp_cast_40 = (0.0).xxx;
			float4 screenColor917 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( temp_output_1057_0 , 0.0 ) ).xy);
			float temp_output_7_0 = ( ( _CustomdataDisT == 0.0 ? i.vertexColor.a : 1.0 ) * ( _ScreenAsMain == 0.0 ? temp_output_1123_39 : screenColor917.a ) * _MainColor.a );
			float2 appendResult49_g465 = (float2(_AddTexUspeed , _AddTexVspeed));
			float4 break18_g347 = _AddTex_ST;
			float2 appendResult1_g347 = (float2(break18_g347.x , break18_g347.y));
			float2 appendResult3_g347 = (float2(break18_g347.z , break18_g347.w));
			float2 CenteredUV15_g348 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g348 = CenteredUV15_g348;
			float2 appendResult23_g348 = (float2(( length( CenteredUV15_g348 ) * break18_g347.x * 2.0 ) , ( atan2( break17_g348.x , break17_g348.y ) * ( 1.0 / 6.28318548202515 ) * break18_g347.y )));
			float2 ifLocalVar1687 = 0;
			if( _AddTexUVS > 1.0 )
				ifLocalVar1687 = ( ( maintongUV557 * appendResult1_g347 ) + appendResult3_g347 );
			else if( _AddTexUVS == 1.0 )
				ifLocalVar1687 = ( appendResult23_g348 + appendResult3_g347 );
			else if( _AddTexUVS < 1.0 )
				ifLocalVar1687 = ( ( i.uv_texcoord * appendResult1_g347 ) + appendResult3_g347 );
			float2 OffsetPOM80_g447 = POM( _ParaTex, ifLocalVar1687, ddx(ifLocalVar1687), ddy(ifLocalVar1687), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_48 = (0.0).xxx;
			float2 temp_cast_49 = (0.0).xx;
			float2 temp_cast_50 = (1.0).xx;
			float2 appendResult1703 = (float2(( _AddTex_ST.x / _MainTex_ST.x ) , ( _AddTex_ST.y / _MainTex_ST.y )));
			float2 temp_cast_51 = (0.0).xx;
			float2 panner44_g465 = ( 1.0 * _Time.y * appendResult49_g465 + ( float3( ( Ifpara1474 == 0.0 ? ifLocalVar1687 : OffsetPOM80_g447 ) ,  0.0 ) + ( _DistortAddTex == 0.0 ? temp_cast_48 : DistortUV60 ) + float3( ( _CAddTexUV == 0.0 ? temp_cast_49 : ( ( _CAddTexUVT == 0.0 ? temp_cast_50 : appendResult1703 ) * temp_output_1691_0 ) ) ,  0.0 ) ).xy);
			float cos55_g465 = cos( ( ( ( _AddTexRo / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g465 = sin( ( ( ( _AddTexRo / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g465 = mul( panner44_g465 - float2( 0.5,0.5 ) , float2x2( cos55_g465 , -sin55_g465 , sin55_g465 , cos55_g465 )) + float2( 0.5,0.5 );
			float2 break52_g465 = rotator55_g465;
			float2 break54_g465 = rotator55_g465;
			float clampResult60_g465 = clamp( break54_g465.x , 0.0 , 1.0 );
			float clampResult50_g465 = clamp( break54_g465.y , 0.0 , 1.0 );
			float2 appendResult53_g465 = (float2(( (float)(int)_AddTexC == 0.0 ? break52_g465.x : clampResult60_g465 ) , ( (float)(int)_AddTexCV == 0.0 ? break52_g465.y : clampResult50_g465 )));
			float4 tex2DNode27_g465 = tex2D( _AddTex, appendResult53_g465 );
			float temp_output_1121_39 = ( (float)(int)_AddTexAR == 0.0 ? tex2DNode27_g465.a : tex2DNode27_g465.r );
			float AddTexAlpha1655 = temp_output_1121_39;
			float lerpResult1659 = lerp( temp_output_7_0 , AddTexAlpha1655 , _AddTexBlend);
			float ifLocalVar1676 = 0;
			if( _AddTexBlendMode > 1.0 )
				ifLocalVar1676 = ( AddTexAlpha1655 * temp_output_7_0 );
			else if( _AddTexBlendMode == 1.0 )
				ifLocalVar1676 = ( AddTexAlpha1655 + temp_output_7_0 );
			else if( _AddTexBlendMode < 1.0 )
				ifLocalVar1676 = lerpResult1659;
			float MainTexAlpha37 = ( ( _IfAddTexAlpha == 0.0 ? temp_output_7_0 : ifLocalVar1676 ) * _MainAlpha );
			float temp_output_51_0_g468 = _DIssloveSoft;
			float temp_output_53_0_g468 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g468 = ( ( temp_output_51_0_g468 + 1.0 ) * temp_output_53_0_g468 );
			int temp_output_38_0_g453 = (int)_DissolveAR;
			float2 appendResult49_g453 = (float2(_DisTex_Uspeed , _DisTex_Vspeed));
			float4 break18_g420 = _DissloveTex_ST;
			float2 appendResult1_g420 = (float2(break18_g420.x , break18_g420.y));
			float2 appendResult3_g420 = (float2(break18_g420.z , break18_g420.w));
			float2 CenteredUV15_g421 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g421 = CenteredUV15_g421;
			float2 appendResult23_g421 = (float2(( length( CenteredUV15_g421 ) * break18_g420.x * 2.0 ) , ( atan2( break17_g421.x , break17_g421.y ) * ( 1.0 / 6.28318548202515 ) * break18_g420.y )));
			#if defined(_DISSOLVETEXUVS_NORMAL)
				float2 staticSwitch805 = ( ( i.uv_texcoord * appendResult1_g420 ) + appendResult3_g420 );
			#elif defined(_DISSOLVETEXUVS_POLAR)
				float2 staticSwitch805 = ( appendResult23_g421 + appendResult3_g420 );
			#elif defined(_DISSOLVETEXUVS_CYLINDER)
				float2 staticSwitch805 = ( ( maintongUV557 * appendResult1_g420 ) + appendResult3_g420 );
			#else
				float2 staticSwitch805 = ( ( i.uv_texcoord * appendResult1_g420 ) + appendResult3_g420 );
			#endif
			float3 temp_cast_61 = (0.0).xxx;
			float2 panner44_g453 = ( 1.0 * _Time.y * appendResult49_g453 + ( float3( staticSwitch805 ,  0.0 ) + ( _DistortDisTex == 0.0 ? temp_cast_61 : DistortUV60 ) ).xy);
			float cos55_g453 = cos( ( ( ( _DIssolve_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g453 = sin( ( ( ( _DIssolve_rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g453 = mul( panner44_g453 - float2( 0.5,0.5 ) , float2x2( cos55_g453 , -sin55_g453 , sin55_g453 , cos55_g453 )) + float2( 0.5,0.5 );
			float2 break52_g453 = rotator55_g453;
			float2 break54_g453 = rotator55_g453;
			float clampResult60_g453 = clamp( break54_g453.x , 0.0 , 1.0 );
			float clampResult50_g453 = clamp( break54_g453.y , 0.0 , 1.0 );
			float2 appendResult53_g453 = (float2(( (float)(int)_DissolveC == 0.0 ? break52_g453.x : clampResult60_g453 ) , ( (float)(int)_DissolveCV == 0.0 ? break52_g453.y : clampResult50_g453 )));
			float4 tex2DNode27_g453 = tex2D( _DissloveTex, appendResult53_g453 );
			float temp_output_57_0_g468 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode27_g453.a : tex2DNode27_g453.r ) , _DissolveTexExp ) );
			int temp_output_38_0_g452 = (int)_DissolvePlusAR;
			float2 appendResult49_g452 = (float2(0.0 , 0.0));
			float4 break18_g450 = _DissloveTexPlus_ST;
			float2 appendResult1_g450 = (float2(break18_g450.x , break18_g450.y));
			float2 appendResult3_g450 = (float2(break18_g450.z , break18_g450.w));
			float2 CenteredUV15_g451 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g451 = CenteredUV15_g451;
			float2 appendResult23_g451 = (float2(( length( CenteredUV15_g451 ) * break18_g450.x * 2.0 ) , ( atan2( break17_g451.x , break17_g451.y ) * ( 1.0 / 6.28318548202515 ) * break18_g450.y )));
			#if defined(_DISSOLVETEXUVS_NORMAL)
				float2 staticSwitch817 = ( ( i.uv_texcoord * appendResult1_g450 ) + appendResult3_g450 );
			#elif defined(_DISSOLVETEXUVS_POLAR)
				float2 staticSwitch817 = ( appendResult23_g451 + appendResult3_g450 );
			#elif defined(_DISSOLVETEXUVS_CYLINDER)
				float2 staticSwitch817 = ( ( maintongUV557 * appendResult1_g450 ) + appendResult3_g450 );
			#else
				float2 staticSwitch817 = ( ( i.uv_texcoord * appendResult1_g450 ) + appendResult3_g450 );
			#endif
			float2 panner44_g452 = ( 1.0 * _Time.y * appendResult49_g452 + staticSwitch817);
			float cos55_g452 = cos( ( ( ( _DissolvePlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin55_g452 = sin( ( ( ( _DissolvePlusR / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator55_g452 = mul( panner44_g452 - float2( 0.5,0.5 ) , float2x2( cos55_g452 , -sin55_g452 , sin55_g452 , cos55_g452 )) + float2( 0.5,0.5 );
			float2 break52_g452 = rotator55_g452;
			float2 break54_g452 = rotator55_g452;
			float clampResult60_g452 = clamp( break54_g452.x , 0.0 , 1.0 );
			float clampResult50_g452 = clamp( break54_g452.y , 0.0 , 1.0 );
			float2 appendResult53_g452 = (float2(( (float)(int)_DissolvePlusC == 0.0 ? break52_g452.x : clampResult60_g452 ) , ( (float)(int)_DissolvePlusCV == 0.0 ? break52_g452.y : clampResult50_g452 )));
			float4 tex2DNode27_g452 = tex2D( _DissloveTexPlus, appendResult53_g452 );
			float temp_output_4_0_g468 = saturate( ( ( ( temp_output_57_0_g468 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g468 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode27_g452.a : tex2DNode27_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g468 = smoothstep( ( temp_output_2_0_g468 - temp_output_51_0_g468 ) , temp_output_2_0_g468 , temp_output_4_0_g468);
			float temp_output_1512_0 = smoothstepResult21_g468;
			float temp_output_44_0_g468 = _DIssloveWide;
			float temp_output_3_0_g468 = ( temp_output_53_0_g468 * ( 1.0 + temp_output_44_0_g468 ) );
			float temp_output_1512_43 = step( ( temp_output_3_0_g468 - temp_output_44_0_g468 ) , temp_output_4_0_g468 );
			float DisAplha42 = ( _sot_sting_A == 0.0 ? temp_output_1512_0 : temp_output_1512_43 );
			float dotResult106 = dot( ase_worldViewDir , ase_worldNormal );
			float softedge111 = pow( saturate( (( _softback )?( dotResult106 ):( abs( dotResult106 ) )) ) , _softFacotr );
			float4 ase_screenPos88 = i.screenPosition88;
			float4 ase_screenPosNorm88 = ase_screenPos88 / ase_screenPos88.w;
			ase_screenPosNorm88.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm88.z : ase_screenPosNorm88.z * 0.5 + 0.5;
			float screenDepth88 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm88.xy ));
			float distanceDepth88 = saturate( ( screenDepth88 - LinearEyeDepth( ase_screenPosNorm88.z ) ) / ( _DepthfadeFactor ) );
			float temp_output_409_0 = ( 1.0 - saturate( distanceDepth88 ) );
			float MainAlpha142 = saturate( ( ( MaskAlpha136 * MainTexAlpha37 * DisAplha42 * (( _FNLfanxiangkaiguan )?( softedge111 ):( 1.0 )) * (( _Depthfadeon )?( ( _DepthF == 0.0 ? distanceDepth88 : 1.0 ) ):( 1.0 )) ) + ( _DepthF == 0.0 ? 0.0 : temp_output_409_0 ) ) );
			float4 DepthColor412 = ( temp_output_409_0 * _DepthColor );
			float3 normalizeResult407 = normalize( ( ase_worldViewDir + _Dir ) );
			float fresnelNdotV91 = dot( ase_worldNormal, normalizeResult407 );
			float fresnelNode91 = ( 0.0 + _fnl_sacle * pow( 1.0 - fresnelNdotV91, _fnl_power ) );
			float switchResult438 = (((i.ASEVFace>0)?(saturate( fresnelNode91 )):(0.0)));
			float4 fnlColor97 = ( switchResult438 * _fnl_color * i.vertexColor );
			float4 temp_cast_71 = (1.0).xxxx;
			float4 temp_output_1123_31 = tex2DNode27_g467;
			float3 temp_output_9_0_g472 = ( _AlphaAdd == 0.0 ? temp_output_1123_31 : ( temp_output_1123_31 * temp_output_1123_39 ) ).rgb;
			float4 break7_g472 = _MainTexRefine;
			float3 temp_cast_73 = (max( break7_g472.z , 0.01 )).xxx;
			float3 lerpResult4_g472 = lerp( ( temp_output_9_0_g472 * break7_g472.x ) , ( pow( temp_output_9_0_g472 , temp_cast_73 ) * break7_g472.y ) , break7_g472.w);
			float4 temp_cast_75 = (1.0).xxxx;
			float4 MaskColor439 = tex2DNode27_g470;
			float4 temp_output_223_0 = ( _MainColor * ( _ScreenAsMain == 0.0 ? float4( lerpResult4_g472 , 0.0 ) : screenColor917 ) * (( _IfMaskColor )?( MaskColor439 ):( temp_cast_75 )) );
			float4 temp_cast_77 = (1.0).xxxx;
			float3 temp_output_9_0_g471 = ( tex2DNode27_g465 * temp_output_1121_39 ).rgb;
			float4 break7_g471 = _AddTexRefine;
			float3 temp_cast_79 = (max( break7_g471.z , 0.01 )).xxx;
			float3 lerpResult4_g471 = lerp( ( temp_output_9_0_g471 * break7_g471.x ) , ( pow( temp_output_9_0_g471 , temp_cast_79 ) * break7_g471.y ) , break7_g471.w);
			float4 AddTexColors479 = ( _AddTexColor * float4( lerpResult4_g471 , 0.0 ) );
			float4 temp_cast_82 = (1.0).xxxx;
			float4 temp_cast_84 = (1.0).xxxx;
			float4 temp_cast_86 = (1.0).xxxx;
			float4 lerpResult488 = lerp( temp_output_223_0 , AddTexColors479 , _AddTexBlend);
			float4 ifLocalVar1675 = 0;
			if( _AddTexBlendMode > 1.0 )
				ifLocalVar1675 = ( AddTexColors479 * temp_output_223_0 );
			else if( _AddTexBlendMode == 1.0 )
				ifLocalVar1675 = ( AddTexColors479 + temp_output_223_0 );
			else if( _AddTexBlendMode < 1.0 )
				ifLocalVar1675 = lerpResult488;
			float4 MainColornoparticle224 = (( _IfAddTex )?( ( _IfAddTexColor == 0.0 ? temp_output_223_0 : ifLocalVar1675 ) ):( temp_output_223_0 ));
			float4 lerpResult230 = lerp( MainColornoparticle224 , _DIssloveColor , _DIssloveColor.a);
			float4 temp_cast_87 = (1.0).xxxx;
			float4 lerpResult33 = lerp( lerpResult230 , ( ( _IfDissolveColor == 0.0 ? i.vertexColor : temp_cast_87 ) * MainColornoparticle224 ) , temp_output_1512_0);
			float temp_output_234_0 = ( temp_output_1512_43 - step( temp_output_3_0_g468 , temp_output_4_0_g468 ) );
			float4 lerpResult244 = lerp( MainColornoparticle224 , ( lerpResult230 * temp_output_234_0 ) , temp_output_234_0);
			float4 DisColor40 = ( ( _IfDissolveColor == 0.0 ? temp_cast_71 : i.vertexColor ) * (( _soft_sting )?( lerpResult244 ):( lerpResult33 )) );
			float2 appendResult56_g481 = (float2(_NormalTex_Uspeed , _NormalTex_Vspeed));
			float4 break18_g473 = _NormalTex_ST;
			float2 appendResult1_g473 = (float2(break18_g473.x , break18_g473.y));
			float2 appendResult3_g473 = (float2(break18_g473.z , break18_g473.w));
			float2 CenteredUV15_g474 = ( i.uv_texcoord - centeruv867 );
			float2 break17_g474 = CenteredUV15_g474;
			float2 appendResult23_g474 = (float2(( length( CenteredUV15_g474 ) * break18_g473.x * 2.0 ) , ( atan2( break17_g474.x , break17_g474.y ) * ( 1.0 / 6.28318548202515 ) * break18_g473.y )));
			#if defined(_MAINTEXUVS_NORMAL)
				float2 staticSwitch1031 = ( ( i.uv_texcoord * appendResult1_g473 ) + appendResult3_g473 );
			#elif defined(_MAINTEXUVS_POLAR)
				float2 staticSwitch1031 = ( appendResult23_g474 + appendResult3_g473 );
			#elif defined(_MAINTEXUVS_CYLINDER)
				float2 staticSwitch1031 = ( ( maintongUV557 * appendResult1_g473 ) + appendResult3_g473 );
			#else
				float2 staticSwitch1031 = ( ( i.uv_texcoord * appendResult1_g473 ) + appendResult3_g473 );
			#endif
			float2 OffsetPOM80_g478 = POM( _ParaTex, staticSwitch1031, ddx(staticSwitch1031), ddy(staticSwitch1031), ase_worldNormal, ase_worldViewDir, i.viewDir, 64, 64, Parallax1492, 0, _ParaTex_ST.xy, float2(0,0), 0 );
			float3 temp_cast_91 = (0.0).xxx;
			float2 panner51_g481 = ( 1.0 * _Time.y * appendResult56_g481 + ( float3( ( Ifpara1474 == 0.0 ? staticSwitch1031 : OffsetPOM80_g478 ) ,  0.0 ) + ( _DistortNormalTex == 0.0 ? temp_cast_91 : DistortUV60 ) ).xy);
			float cos62_g481 = cos( ( ( ( _NormalTex_Rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float sin62_g481 = sin( ( ( ( _NormalTex_Rotat / 360.0 ) * UNITY_PI ) * 2.0 ) );
			float2 rotator62_g481 = mul( panner51_g481 - float2( 0.5,0.5 ) , float2x2( cos62_g481 , -sin62_g481 , sin62_g481 , cos62_g481 )) + float2( 0.5,0.5 );
			float2 break59_g481 = rotator62_g481;
			float2 break61_g481 = rotator62_g481;
			float clampResult67_g481 = clamp( break61_g481.x , 0.0 , 1.0 );
			float clampResult57_g481 = clamp( break61_g481.y , 0.0 , 1.0 );
			float2 appendResult60_g481 = (float2(( (float)(int)_NormalTexC == 0.0 ? break59_g481.x : clampResult67_g481 ) , ( (float)(int)_NormalTexCV == 0.0 ? break59_g481.y : clampResult57_g481 )));
			float temp_output_51_0_g466 = _DIssloveSoft;
			float temp_output_53_0_g466 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g466 = ( ( temp_output_51_0_g466 + 1.0 ) * temp_output_53_0_g466 );
			float2 break68_g453 = appendResult53_g453;
			float temp_output_64_0_g453 = ( pow( _StaticNormalOffset , 3.0 ) * 0.1 );
			float2 appendResult67_g453 = (float2(( break68_g453.x + temp_output_64_0_g453 ) , break68_g453.y));
			float4 tex2DNode70_g453 = tex2D( _DissloveTex, appendResult67_g453 );
			float temp_output_57_0_g466 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode70_g453.a : tex2DNode70_g453.r ) , _DissolveTexExp ) );
			float2 break68_g452 = appendResult53_g452;
			float temp_output_64_0_g452 = ( pow( _StaticNormalOffset , 3.0 ) * 0.1 );
			float2 appendResult67_g452 = (float2(( break68_g452.x + temp_output_64_0_g452 ) , break68_g452.y));
			float4 tex2DNode70_g452 = tex2D( _DissloveTexPlus, appendResult67_g452 );
			float temp_output_4_0_g466 = saturate( ( ( ( temp_output_57_0_g466 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g466 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode70_g452.a : tex2DNode70_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g466 = smoothstep( ( temp_output_2_0_g466 - temp_output_51_0_g466 ) , temp_output_2_0_g466 , temp_output_4_0_g466);
			float temp_output_44_0_g466 = _DIssloveWide;
			float temp_output_3_0_g466 = ( temp_output_53_0_g466 * ( 1.0 + temp_output_44_0_g466 ) );
			float DisAplha11397 = ( _sot_sting_A == 0.0 ? smoothstepResult21_g466 : step( ( temp_output_3_0_g466 - temp_output_44_0_g466 ) , temp_output_4_0_g466 ) );
			float temp_output_1402_0 = ( _StaticNormalScale * 10.0 );
			float3 appendResult1273 = (float3(1.0 , 0.0 , ( ( DisAplha11397 - DisAplha42 ) * temp_output_1402_0 )));
			float temp_output_51_0_g469 = _DIssloveSoft;
			float temp_output_53_0_g469 = ( _CustomdataDis == 0.0 ? ( _DIssloveFactor + 0.001 ) : ( _CustomdataDisT == 0.0 ? i.uv2_texcoord2.z : (1.0 + (i.vertexColor.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float temp_output_2_0_g469 = ( ( temp_output_51_0_g469 + 1.0 ) * temp_output_53_0_g469 );
			float2 appendResult63_g453 = (float2(break68_g453.x , ( break68_g453.y + temp_output_64_0_g453 )));
			float4 tex2DNode76_g453 = tex2D( _DissloveTex, appendResult63_g453 );
			float temp_output_57_0_g469 = saturate( pow( ( (float)temp_output_38_0_g453 == 0.0 ? tex2DNode76_g453.a : tex2DNode76_g453.r ) , _DissolveTexExp ) );
			float2 appendResult63_g452 = (float2(break68_g452.x , ( break68_g452.y + temp_output_64_0_g452 )));
			float4 tex2DNode76_g452 = tex2D( _DissloveTexPlus, appendResult63_g452 );
			float temp_output_4_0_g469 = saturate( ( ( ( temp_output_57_0_g469 / _DissolveTexDivide ) + ( _IfDissolvePlus == 0.0 ? temp_output_57_0_g469 : ( (float)temp_output_38_0_g452 == 0.0 ? tex2DNode76_g452.a : tex2DNode76_g452.r ) ) ) / 2.0 ) );
			float smoothstepResult21_g469 = smoothstep( ( temp_output_2_0_g469 - temp_output_51_0_g469 ) , temp_output_2_0_g469 , temp_output_4_0_g469);
			float temp_output_44_0_g469 = _DIssloveWide;
			float temp_output_3_0_g469 = ( temp_output_53_0_g469 * ( 1.0 + temp_output_44_0_g469 ) );
			float DisAplha21398 = ( _sot_sting_A == 0.0 ? smoothstepResult21_g469 : step( ( temp_output_3_0_g469 - temp_output_44_0_g469 ) , temp_output_4_0_g469 ) );
			float3 appendResult1268 = (float3(0.0 , 1.0 , ( ( DisAplha21398 - DisAplha42 ) * temp_output_1402_0 )));
			float3 normalizeResult1265 = normalize( cross( appendResult1273 , appendResult1268 ) );
			float3 Normal1046 = BlendNormals( UnpackScaleNormal( tex2D( _NormalTex, appendResult60_g481 ), _NormalScale ) , ( _IfStaticNormal == 0.0 ? float3(0,0,1) : normalizeResult1265 ) );
			float4 Cubemap1144 = ( _IfCubemap == 0.0 ? float4( 0,0,0,0 ) : ( texCUBE( _CubeMap, WorldReflectionVector( i , Normal1046 ) ) * _CubemapScale * _LightScale ) );
			float4 temp_output_145_0 = ( ( _AlphaAdd == 0.0 ? 1.0 : MainAlpha142 ) * ( ( _DepthF == 0.0 ? float4( 0,0,0,0 ) : DepthColor412 ) + ( fnlColor97 + DisColor40 + Cubemap1144 ) ) );
			float4 switchResult433 = (((i.ASEVFace>0)?(temp_output_145_0):(( temp_output_145_0 * _BackFaceColor ))));
			float3 desaturateInitialColor299 = switchResult433.rgb;
			float desaturateDot299 = dot( desaturateInitialColor299, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar299 = lerp( desaturateInitialColor299, desaturateDot299.xxx, _qubaohedu );
			o.Emission = desaturateVar299;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows noambient nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 customPack3 : TEXCOORD3;
				float4 customPack4 : TEXCOORD4;
				float4 screenPos : TEXCOORD5;
				float4 tSpace0 : TEXCOORD6;
				float4 tSpace1 : TEXCOORD7;
				float4 tSpace2 : TEXCOORD8;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.uv3_texcoord3;
				o.customPack2.xyzw = v.texcoord2;
				o.customPack3.xyzw = customInputData.uv2_texcoord2;
				o.customPack3.xyzw = v.texcoord1;
				o.customPack4.xyzw = customInputData.screenPosition88;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv3_texcoord3 = IN.customPack2.xyzw;
				surfIN.uv2_texcoord2 = IN.customPack3.xyzw;
				surfIN.screenPosition88 = IN.customPack4.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "PandaVFXGUI"
}
