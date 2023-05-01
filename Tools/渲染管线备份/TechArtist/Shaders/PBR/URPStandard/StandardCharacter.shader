Shader "NBA2/URP/StandardCharacter"
{
    Properties
    {
        _AlbedoMap                  ("", 2D) = "white" {}
        _AlbedoColor                ("", Color) = (1,1,1,1)
        _UVScale("", Float) = 1.0
        
        _NormalMap                  ("", 2D) = "bump"{}
        _NormalBias                 ("", Float) = 0
        _NormalScale			    ("", Float) = 1.0

        _MetallicMap                ("", 2D) = "white" {}
        _Metallic                   ("", Range(0.0, 1.0)) = 0

        _Glossiness                 ("", Range(0.0, 2.0)) = 0.5

        _AOIntensity                ("", Range(0,1)) = 1

        _EmissionEnabled            ("", Float) = 0.0
        _NetLightness               ("", Range(0, 1)) = 0
        _EmissionMap                ("", 2D) = "white" {}
        [HDR]
        _EmissionColor			    ("", Color) = (0,0,0,0)
        _ShadowIntensity            ("", Range(0, 1)) = 1

        _RScale                     ("", Float) = 1
        _GScale                     ("", Float) = 1
        _BScale                     ("", Float) = 1
        _AScale                     ("", Float) = 1

        _QueueOffset                ("", Float) = 0.0
        _Cutoff                     ("", Range(0.0, 1.0)) = 0.5

        VIR_LIT                     ("", Float) = 0
        VERTEX_COLOR			    ("", Float) = 0

        _Low                        ("",Float) = 1
        _High                       ("",Float) = 1
        _Light                      ("",Vector) = (0,0,0,0.1)
        _RimScale                   ("",Float) = 1
        _RimPower                   ("",Float) = 5
        _LightColor                 ("",Color) = (1,1,1,1)
        _BackLightScale             ("",Float) = 0.1

        //Matcap
        _MatCapAccurate             ("", Int) = 0
        _MatCap                     ("", 2D) = "black" {}

        _EffectMaskTex              ("",2D) = "white"{}
        _YSpeed                     ("",Float) = 0
        _Width                      ("",Float) = 0
        _EdgeColor                  ("",Color) = (1,1,1,1)
        _EdgeWidth                  ("",Float) = 0
        _MaskTexTilling("",Float) = 0
        _DownWidth("",Float) = 0
        _Pos("",Float) = 0
        _RimEdge("",Float) = 0
        _RimDis("",Float) = 0


        //号码牌
        [Header(NumberTexture)]
		_NumTex				("NumTex",2D) = "black"{}
		_NumTex2			("NumTex2",2D) = "black"{}

        _ZMTex              ("ZMTex",2D) = "white"{}

        _ZmStrV4            ("_ZmStrV4",Vector) = (0.0,0.0,0.0,0.0)
        _ZmStr2V4           ("_ZmStr2V4",Vector) = (0.0,0.0,0.0,0.0)
        _ZmStr3V4           ("_ZmStr3V4",Vector) = (0.0,0.0,0.0,0.0)

        _ZMIndexDistance    ("_ZMIndexDistance",Range(0.0, 1.0)) = 1
        _ZMGroupOffsetY     ("_ZMGroupOffsetY",float) = 2.5
        _ZMRotateAngleStep  ("_ZMRotateAngleStep",Range(0.0, 30.0)) = 1.5
        _ZMExtraOffsetX     ("_ZMExtraOffsetX",Range(-1.0,1.0)) = 0
        _ZMFontSize         ("_ZMFontSize",Range(0.0,2.0)) = 1
        _ZMDeltaYStep       ("_ZMDeltaYStep",Range(0.0,2.0)) = 1
        _ZMUVDistanceScale  ("_ZMUVDistanceScale",Range(0.0,10.0)) = 8.0

		_NumberS			("NumberS",float)=0
		_NumberG			("NumberG",float)=0
		_NumberDis			("NumberDis",float)=0

		_NumFrontOffSet		("NumFrontOffSet:x前左右y前上下z前缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumBackOffSet		("NumBackOffSet:x背左右y背上下z背缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumFrontOffSet2    ("NumFrontOffSet2:x前左右y前上下z前缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumBackOffSet2		("NumBackOffSet2:x背左右y背上下z背缩放w无用",Vector) = (0.0,0.0,1.0,1.0)



        //stencil test
        _Stencil("Stencil", Float) = 5  

        // Blending state
        [HideInInspector] _Surface          ("__surface", Float) = 0.0
        [HideInInspector] _Blend            ("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip        ("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend         ("__src", Float) = 1.0
        [HideInInspector] _DstBlend         ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite           ("__zw", Float) = 1.0
        [HideInInspector] _Cull             ("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0

        // Project Shadow
        _ShadowDirection("ShadowDirection",Vector) = (8.4, 26, -14.67, 0)
        _ShadowAlpha("ShadowAlpha",Range(0.0,10.0)) = 0.1
        _shadowFalloff("ShadowFalloff",float) = 0.17
        [Enum(UnityEngine.Rendering.BlendMode)]_SorcBlend("_SorcBlend",int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DestBlend("_DestBlend",int) = 10

        _Numtex_ON("NumtexON", Float) = 1.0
        _useNumtex02("TEX2_ON", Float) = 1.0
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

            Blend[_SrcBlend][_DstBlend]
            Cull[_Cull]
            ZWrite[_ZWrite]

            Stencil
            {
                Ref [_Stencil]
                Comp GEqual
                Pass Replace
            }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH
            #pragma multi_compile_local_fragment _ OVERRIDE_SH

            #pragma shader_feature_local RIM_MASK
            #pragma multi_compile_local VERTEX_COLOR
            #pragma shader_feature_local MATCAP_ACCURATE
            #pragma shader_feature_local GRADIENT_ALPHA

            //#pragma shader_feature_local NUMTEX_ON
			#pragma multi_compile _ TEX2_ON

            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local_fragment ALPHATEST_ON
            #pragma shader_feature_local_fragment ALPHAPREMULTIPLY_ON

            #pragma vertex   LitVert
            #pragma fragment LitFrag
            #pragma skip_variants FOG_EXP FOG_EXP2 SHADOWS_CUBE
            #include "StandardCharacter.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            Cull   Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x


            #pragma vertex   MetaVert
            #pragma fragment MetaFrag

            #pragma shader_feature_local SHADING_EMISSION

            #include "StandardCharacter.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Meta.hlsl"

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
            #include "StandardCharacter.hlsl"
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

            //--------------------------------------
            // GPU Instancing
            #include "StandardCharacter.hlsl"
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
            #include "StandardCharacter.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/ShadowCasterPass.hlsl"
            ENDHLSL
        }

    }


    Fallback "Hidden/InternalErrorShader"
    //CustomEditor "FTX.Rendering.ShaderGUIStandardCharacter"
}