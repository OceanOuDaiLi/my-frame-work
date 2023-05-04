Shader "NBA2/URP/IndoorScene"
{
    Properties
    {
        _AlbedoMap                  ("Albedo Map", 2D) = "white" {}
        [HDR]_AlbedoColor                ("Albedo Color", Color) = (1,1,1,1)
        _UVScale				    ("UV Scale", Float) = 1.0

        _NormalMap                  ("Normal Map", 2D) = "bump"{}
        _NormalBias                 ("Normal Bias", Float) = 0
        _NormalScale			    ("Normal Scale", Float) = 1.0

        _MetallicMap                ("Metallic Map", 2D) = "white" {}
        _Metallic                   ("Metallic", Range(0.0, 1.0)) = 0

        _Glossiness                 ("Glossiness", Range(0.0, 2.0)) = 0.5
        _AOIntensity                ("AO Intensity", Range(0,1)) = 1

        _EmissionEnabled            ("Emission Enabled", Float) = 0.0
        _NetLightness               ("NetLightness", range(0, 1)) = 0
        _EmissionMap                ("Emission Map", 2D) = "white" {}
        [HDR]
        _EmissionColor			    ("Emission Color", Color) = (0,0,0,0)

        _QueueOffset                ("Queue Offset", Float) = 0.0
        _Cutoff                     ("Cut off", Range(0.0, 1.0)) = 0.5
        _Transparency               ("Transparency",Float)  = 0

        GSAA_ON                     ("GSAA_ON",Float)  = 0
        _CURV_MOD                   ("CURV_MOD",Float)  = 1
        _MAX_POW                    ("MAX_POW",Float)  = 0.18
        VIR_LIT                     ("VIR_LIT", Float) = 0

        _FlagWaveSpeed                  ("Flag Wave Speed",Float)  = 1
        _FlagWaveIntensity              ("Flag Wave Intensity",Float)  = 1
        _FlagShadowIntensity            ("Flag Shadow Intensity",Float)  = 1
        _FlagWaveRotation               ("Flag Wave Rotation",Float)  = 1

        FADE_TO_BLACK_ON            ("FADE_TO_BLACK_ON",Float)  = 1
        _gentle                     ("gentle", range(0,1)) = 0.05
        _ind                        ("ind", range(0,10)) = 1
        _xVar                       ("_xVar", Float) = 1.65
        _yVar                       ("_yVar", Float) = 1.5
        _radius                     ("_radius", Float) = 0.2
        _titling                    ("titling", Float) = 10
        _ellipseX                    ("ellipseX", Float) = 0.5
        _ellipseY                    ("ellipseY", Float) = 0.32

        _Low                        ("Low",Float) = 1
        _High                       ("High",Float) = 1
        _Light                      ("Light",Vector) = (0,0,0,0.1)
        _RimScale                   ("Rim Scale",Float) = 1
        _RimPower                   ("Rim Power",Float) = 5
        _LightColor                 ("Light Color",Color) = (1,1,1,1)
        _BackLightScale             ("Back Light Scale",Float) = 0.1

        [HideInInspector] _Surface          ("__surface", Float) = 0.0
        [HideInInspector] _Blend            ("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip        ("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend         ("__src", Float) = 1.0
        [HideInInspector] _DstBlend         ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite           ("__zw", Float) = 1.0
        [HideInInspector] _Cull             ("__cull", Float) = 2.0

        _ShadowIntensity            ("Shadow Intensity", range(0, 1)) = 0
        _ReceiveShadows             ("Receive Shadows", Float) = 1.0

        _ReflectionUvCube   ("Reflection Uv Cube",Cube) = "black"{}
        _ReflectionInstensity   ("Reflection Instensity",Range(0,5)) = 1
        _ReflectionRotation   ("Reflection Rotation",Range(0,360)) = 1
        _ReflectionColor ("Reflection Color",Color) = (1,1,1,1)

        _AOTex ("AO Tex",2D) = "white"{}
        _LogoTex ("Logo Tex",2D) = "black"{}
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

            Blend  [_SrcBlend][_DstBlend]
            Cull   [_Cull]
            //ZTest  [_ZTest]
            ZWrite [_ZWrite]

            HLSLPROGRAM
                #pragma target 3.5
                #pragma instancing_options procedural:setup
                #pragma skip_variants INSTANCING_ON

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_instancing
                //#pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

                // -------------------------------------
                // Universal Pipeline keywords
                #pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH

                #pragma shader_feature_local RIM_MASK
                #pragma shader_feature_local SHADING_EMISSION
                #pragma shader_feature_local STIPPLE_ON
                #pragma shader_feature_local VERTEX_COLOR
                #pragma shader_feature_local GSAA_ON
                #pragma shader_feature_local FADE_TO_BLACK_ON
                #pragma shader_feature_local IS_FLAG
                #pragma shader_feature_local USE_PROBES

                #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
                #pragma shader_feature_local_fragment ALPHATEST_ON

                #define LIGHTMAP_SHADOW_MIXING
                //#define NORMAL_BLEND_WHITEOUT 1

                #pragma vertex   LitVert
                #pragma fragment LitFrag
                #pragma skip_variants FOG_EXP FOG_EXP2 SHADOWS_CUBE
                #include "IndoorScene.hlsl"
                #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/StaticInstancing.hlsl"
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

            #include "IndoorScene.hlsl"
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

            #pragma vertex   DepthOnlyVert
            #pragma fragment DepthOnlyFrag
            #pragma shader_feature_local_fragment ALPHATEST_ON
            #include "IndoorScene.hlsl"
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

            #pragma shader_feature_local_fragment ALPHATEST_ON
            #include "IndoorScene.hlsl"
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

            #pragma vertex   ShadowVert
            #pragma fragment ShadowFrag

            #pragma multi_compile_shadowcaster
            #pragma shader_feature_local_fragment ALPHATEST_ON
            #include "IndoorScene.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
    CustomEditor "FTX.Rendering.ShaderGUIIndoorScene"
}