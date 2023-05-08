Shader "NBA2/URP/SkinSSS"
{
    Properties
    {
        _AlbedoMap                  ("AlbedoMap", 2D) = "white" {}
        _AlbedoColor                ("AlbedoColor", Color) = (1,1,1,1)
        _UVScale				    ("UVScale", Float) = 1.0

        _NormalMap                  ("NormalMap", 2D) = "bump"{}
        _NormalBias                 ("NormalBias", Float) = 0
        _NormalScale			    ("NormalScale", Float) = 1.0

        _BSSRDFMap                  ("BSSRDFMap", 2D) = "white"{}
        _MetallicMap                ("MetallicMap", 2D) = "white" {}    //_ThicknessMap
        _TranslucencyColor          ("TranslucencyColor", Color) = (1, 0, 0, 1)
        _ThicknessAndCurvature      ("ThicknessAndCurvature", Vector) = (2.5, 5, 0.4, 1)

        _ThicknessScale             ("ThicknessScale", Range(0, 2)) = 1
        _ThicknessPower             ("ThicknessPower", Range(0, 5)) = 1
        _SSSPower                   ("SSS Power", Range(0.01, 5)) = 1
        _SSSFactor                  ("SSS Factor", Range(0.01, 1)) = 1
        _SSSColor                   ("SSS Color", Color) = (0, 0, 0, 1)

        _Glossiness                 ("Glossiness", Range(0.0, 2.0)) = 0.5
        _AOIntensity                ("AOIntensity", Range(0,1)) = 1

        //_EmissionEnabled            ("", Float) = 0.0
        _EmissionMap                ("EmissionMap", 2D) = "white" {}
        _EmissionColor			    ("EmissionColor", Color) = (0,0,0,0)


        _LitAttenContrast           ("LitAttenContrast", Float) = 1.0
        _LitAttenInt                ("LitAttenInt", Float) = 14.0
        _LitAttenSub                ("LitAttenSub", Float) = 0.9
        _SkinShadowColor            ("SkinShadowColor", Color) = (0,0,0,1)


        _DetailBump                 ("DetailBump", 2D) = "bump"{}
        _DetailBumpScale            ("DetailBumpScale", Range(0, 2)) = 1

        //_RScale                     ("", Float) = 1
        //_GScale                     ("", Float) = 1
        //_BScale                     ("", Float) = 1
        //_AScale                     ("", Float) = 1

        _SweatMap                   ("SweatMap", 2D) = "Black"{}
        _SweatBumpScale             ("SweatBumpScale", Range(0, 2)) = 1
        _SweatSmoothnessScale       ("SweatSmoothnessScale", Range(0, 2)) = 1
        _SweatSwitch                ("SweatSwitch", Range(0, 1)) = 1

        _QueueOffset                ("QueueOffset", Float) = 0.0
        _Cutoff                     ("Cutoff", Range(0.0, 1.0)) = 0.5


        DETAIL_MICRO			    ("DETAIL_MICRO", Float) = 0
        VIR_LIT                     ("VIR_LIT", Float) = 0
        SWEAT_DETAIL			    ("SWEAT_DETAIL", Float) = 0
        VERTEX_COLOR			    ("VERTEX_COLOR", Float) = 0

        //Rim Mask
        _Low                        ("",Float) = 1
        _High                       ("",Float) = 1
        _Light                      ("",Vector)=(0,0,0,0.1)
        _RimScale                   ("",Float) = 1
        _RimPower                   ("",Float) = 5
        _LightColor                 ("",Color)=(1,1,1,1)
        _BackLightScale             ("",Float)=0.1

        //Matcap
        _MatCapAccurate             ("MatCapAccurate", Int) = 0
        _MatCap                     ("MatCap", 2D) = "black" {}

        //Gradient Alpha
        _EffectMaskTex              ("EffectMaskTex",2D) = "white"{}
        _YSpeed                     ("YSpeed",Float) = 0
        _Width                      ("Width",Float) = 0
        [HDR]
        _EdgeColor                  ("EdgeColor",Color)=(1,1,1,1)
        _EdgeWidth                  ("EdgeWidth",Float)=0
        _MaskTexTilling             ("MaskTexTilling",Float)=0
        _DownWidth                  ("DownWidth",Float)=0
        _Pos                        ("Pos",Float)=0
        _RimEdge                    ("RimEdge",Float)=0
        _RimDis                     ("RimDis",Float)=0


 

        [Header(VertexColor)]
		_RScale                     ("RScale", Float) = 1
        _GScale                     ("GScale", Float) = 1
        _BScale                     ("BScale", Float) = 1
        _AScale                     ("AScale", Float) = 1

        //stencil test
        _Stencil("Stencil", Float) = 5  
        //stencil compare 8:always 7:GEqual
        _StencilComp("StencilComp", Float) = 8  

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0
        _Numtex_ON("NumtexON", Float) = 1.0
        _useNumtex02("UseNum02", Float)= 1.0


        // 平面阴影
        //_Stencil("Stencil", Float) = 5  
        _ShadowDirection("ShadowDirection",Vector) = (0.39, 1, 0.3, 0.05)
        _ShadowAlpha("ShadowAlpha",Range(0.0,10.0)) = 0.1
        _ShadowFalloff("ShadowFalloff",Range(-1.0,1.0)) = 0


        [Enum(UnityEngine.Rendering.BlendMode)]_SorcBlend("_SorcBlend",int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DestBlend("_DestBlend",int) = 10

        _Test3("Test3",Range(0.01,10)) = 1
    }

    SubShader
    {

        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            // Stencil 
            // {  
            //     Ref 2 
            //     Comp always 
            //     Pass replace  
            //     ZFail keep
            // } 

            Blend[_SrcBlend][_DstBlend]
            Cull[_Cull]
            //ZTest  [_ZTest]
            ZWrite[_ZWrite]
            //ColorMask RGBA

            Stencil//往模板缓冲写入
             {
                 Ref  2//[_Stencil]
                 Comp [_StencilComp]
                 Pass Replace
             }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles


            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            //#pragma multi_compile_fwdbase
            //#pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			//#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH

            #pragma shader_feature_local RIM_MASK
            #pragma shader_feature_local DETAIL_MICRO
            #pragma shader_feature_local SWEAT_DETAIL
            #pragma shader_feature_local GRADIENT_ALPHA
            #pragma multi_compile_local VERTEX_COLOR
            #pragma shader_feature_local MATCAP_ACCURATE

            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            //#pragma shader_feature _USE_PROJECT_SHADOW


            #pragma vertex   LitVert
            #pragma fragment LitFrag

            #include "SkinSSS.hlsl"
            ENDHLSL
        }


        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
			#pragma target 3.0
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

            #pragma multi_compile_local VERTEX_COLOR

			#pragma vertex   DepthOnlyVert
			#pragma fragment DepthOnlyFrag

            #include "SkinSSS.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/DepthOnlyPass.hlsl"

            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 3.0
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #include "SkinSSS.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/DepthNormalPass.hlsl"
            ENDHLSL
        }

        Pass
		{
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

			ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma multi_compile_local VERTEX_COLOR

			#pragma vertex   ShadowVert
			#pragma fragment ShadowFrag

            #pragma multi_compile_shadowcaster
            #include "SkinSSS.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/ShadowCasterPass.hlsl"
			ENDHLSL
		}
    }

    Fallback "Hidden/InternalErrorShader"
    CustomEditor "FTX.Rendering.ShaderGUISkinSSS"
}