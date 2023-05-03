Shader "NBA2/URP/StandardInstanced"
{
    Properties
    {
        _AlbedoMap                  ("", 2D) = "white" {}
        _AlbedoColor                ("", Color) = (1,1,1,1)
        _UVScale				    ("", Float) = 1.0

        _NormalMap                  ("", 2D) = "bump"{}
        _NormalBias                 ("", Float) = 0
        _NormalScale			    ("", Float) = 1.0

        _MetallicMap                ("", 2D) = "white" {}
	    _Metallic                   ("", Range(0.0, 1.0)) = 0

        _Glossiness                 ("", Range(0.0, 2.0)) = 0.5
        //_GlossMapScale              ("", Range(0.0, 2.0)) = 1.0

        _AOIntensity                ("", Range(0,1)) = 1

        _EmissionEnabled            ("", Float) = 0.0
        _EmissionMap                ("", 2D) = "white" {}
        _EmissionColor			    ("", Color) = (0,0,0,0)


        _LitAttenContrast           ("", Float) = 1.0
        _LitAttenInt                ("", Float) = 14.0
        _LitAttenSub                ("", Float) = 0.9

        _RColor                     ("", Color) = (1,1,1,1)
        _GColor                     ("", Color) = (1,1,1,1)
        _BColor                     ("", Color) = (1,1,1,1)
        _AColor                     ("", Color) = (1,1,1,1)


        _DetailBump                 ("", 2D) = "bump"{}
        _DetailBumpScale            ("", Range(0, 2)) = 1

        _CullMode				    ("", Float) = 2.0
        _ZTest					    ("", Float) = 4.0
        _ZWrite					    ("", Float) = 1.0
        _QueueOffset                ("", Float) = 0.0


        DETAIL_MICRO			    ("", Float) = 0
        VIR_LIT                     ("", Float) = 0
        VERTEX_COLOR			    ("", Float) = 0
        //GPU_SKINNING			    ("", Float) = 0

                // 平面阴影
        //_Stencil("Stencil", Float) = 5  
        _ShadowDirection("ShadowDirection",Vector) = (0.39, 1, 0.3, 0.05)
        _ShadowAlpha("ShadowAlpha",Range(0.0,10.0)) = 0.1
        _ShadowFalloff("ShadowFalloff",Range(-1.0,1.0)) = 0


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

			Cull   [_CullMode]
            ZTest  [_ZTest]
			ZWrite [_ZWrite]

            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nolodfade

            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

            #pragma shader_feature_local DETAIL_MICRO
            #pragma shader_feature_local SHADING_EMISSION
            #pragma shader_feature_local VERTEX_COLOR

            #pragma vertex   LitVert
            #pragma fragment LitFrag

            #include "StandardInstanced.hlsl"
            ENDHLSL
        }

        
        ////角色平面阴影
        //Pass
        //{

        //    Tags
        //    {

        //        "LightMode" = "SRPDefaultUnlit"
        //        //"LightMode" = "SRPTest"

        //        "Queue" = "Transparent"
        //        "RenderPipeline" = "UniversalPipeline"
        //        "IgnoreProjector" = "True"
        //        "RenderType" = "Transparent"
        //    }

        //    Blend SrcAlpha OneMinusSrcAlpha
        //    //Blend[_SorcBlend][_DestBlend]
        //    Offset -1 , 0
        //    ZWrite Off
        //    //ZWrite On

        //    Cull Back
        //    ColorMask RGBA

        //    Stencil
        //    {
        //        Ref 1
        //        Comp NotEqual
        //        WriteMask 255
        //        ReadMask 255

        //        Pass Replace
        //        Fail Keep
        //        //ZFail Keep
        //    }

        //    HLSLPROGRAM

        //    #pragma vertex vert
        //    #pragma fragment frag
        //    //#include "StandardCharacter.hlsl"
        //    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        //    #pragma shader_feature _USE_PROJECT_SHADOW

        //    struct appdata
        //    {
        //        float4 vertex : POSITION;
        //    };

        //    struct v2f
        //    {
        //        float4 pos : SV_POSITION;
        //        half4 color :TEXCOORD5;
        //    };

        //    CBUFFER_START(UnityPerMaterial)

        //    half _ShadowFalloff=0.1;
        //    half4 _ShadowDirection = (0.39, 1, 0.3, 0.05);
        //    half _ShadowAlpha=0.1;

        //    CBUFFER_END

        //    float3 ShadowProjectPos(float4 vertPos)
        //    {
        //        float3 shadowPos;
        //        float3 worldPos = TransformObjectToWorld(vertPos).xyz;
        //        half3 lightDir = normalize(_ShadowDirection.xyz);

        //        shadowPos.xz = worldPos.xz - lightDir.xz * max(0.0 , worldPos.y - _ShadowDirection.w) / lightDir.y;
        //        shadowPos.y = min(worldPos.y , _ShadowDirection.w);

        //        return shadowPos;
        //    }

        //    v2f vert(appdata v)
        //    {
        //        v2f o;
        //        float3 shadowPos = ShadowProjectPos(v.vertex);
        //        o.pos = TransformWorldToHClip(shadowPos);


        //        float3 center = float3(unity_ObjectToWorld[0].w , _ShadowDirection.w , unity_ObjectToWorld[2].w);
        //        half falloff = 1-saturate(pow(length(shadowPos - center) * _ShadowAlpha,_ShadowFalloff));

        //        o.color = half4(0.01f, 0.01f, 0.2f, 0.99f);
        //        o.color.a *= saturate(falloff);

        //        return o;
        //    }

        //    half4 frag(v2f i) : SV_Target
        //    {
        //        half4 finalCol = i.color;
        //        //return i.pos.z;
        //        return finalCol;
        //    }

        //    ENDHLSL
        //}



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
            #pragma multi_compile_instancing
            #include "StandardInstanced.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/DepthOnlyPass.hlsl"

            ENDHLSL
        }

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
            #pragma multi_compile_instancing
            #include "StandardInstanced.hlsl"
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

            #pragma multi_compile_instancing
            #pragma multi_compile_shadowcaster
            #include "StandardInstanced.hlsl"
            #include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/ShadowCasterPass.hlsl"
			ENDHLSL
		}
    }

    Fallback "Hidden/InternalErrorShader"
    CustomEditor "FTX.Rendering.ShaderGUIStandard"
}