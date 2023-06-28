Shader "FTX Game/Character/NumberCloth"
{
    Properties
    {
        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 0.0

        //_BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
        //_OcclusionMap("Occlusion", 2D) = "white" {}

        _MainLightCompensation("MainLightCompensation", Range(0.0, 1.0)) = 0.4

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        _NumTex				("NumTex",2D) = "black"{}
		_NumTex2			("NumTex2",2D) = "black"{}
        _ZMTex              ("ZMTex",2D) = "black"{}

        _ZmStrV4            ("_ZmStrV4",Vector) = (0.0,0.0,0.0,0.0)
        _ZmStr2V4           ("_ZmStr2V4",Vector) = (0.0,0.0,0.0,0.0)
        _ZmStr3V4           ("_ZmStr3V4",Vector) = (0.0,0.0,0.0,0.0)
        
        _ZMHigh             ("_ZMHigh",float) = 0.0
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
        _Numtex_ON          ("NumtexON", Float) = 1.0
        _useNumtex02        ("UseNum02", Float)= 1.0
    }

    SubShader
    {
        // Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
        // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
        // material work with both Universal Render Pipeline and Builtin Unity Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 300

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP

            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "HLSL/NumberClothInput.hlsl"
            #include "HLSL/NumberClothForwardPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            //#pragma multi_compile_instancing

            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "HLSL/NumberClothInput.hlsl"
            #include "HLSL/FTXShadowCasterPass.hlsl"
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
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            //#pragma multi_compile_instancing

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #include "HLSL/NumberClothInput.hlsl"
            #include "HLSL/FTXDepthOnlyPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.PlayerShader"
}
