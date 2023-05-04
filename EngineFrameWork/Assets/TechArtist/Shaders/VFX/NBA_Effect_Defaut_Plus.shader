Shader "NBA/Effect/UberDefault"
{
    Properties
    {
        //[Toggle]_FowBlend("FOW Blend" , float) = 0	
        _AlphaCtrl("AlphaCtrl",range(0,1)) = 1

        [HideInInspector] _OptionMode("__OptionMode",float) = 0	
        [HideInInspector] _BlendMode ("__BlendMode",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("__src", float) = 1.0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("__dst", float) = 0.0
        [Enum(Off, 0, On, 1)] _ZWriteMode ("__ZWriteMode", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("__CullMode", float) = 2
        [Enum(Less, 2, Greater, 5 ,Always , 8)] _ZTestMode ("__ZTestMode", Float) = 2


        [Toggle]_WrapMode("Custom WrapMode", Float) = 0
        [Toggle]_ScreenUV("Screen Space Mode", Float) = 0

        [Toggle] _ScaleOnCenter("Scale On Center", Float) = 1

        _fresnelScale("fresnel Scale" , range(0,10)) = 0
        _fresnelIndensity("fresnel Indensity" , range(0.1,10)) = 0
        [HDR]_fresnelColor("fresnel Color" , color) = (1,1,1,1)
        
        [HideInInspector]_OffsetColor ("OffsetColor", Color) = (0,0,0,0) 
        [HideInInspector]_OffsetColorLerp ("OffsetColor", Float) = 0

        [hdr]_Color ("Main Color" , color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _MainTexTransform ("MainTexTransform" , vector) = (0,0,0,1)
        [Toggle] _MainTexUvMode("UV Mode", Float) = 0

        [hdr]_TinctColor ("Tinct Color" , color) = (1,1,1,1)
        _AdditiveTex ("Additive Texture", 2D) = "white" {}
        _AdditiveTexTransform ("AdditiveTexTransform" , vector) = (0,0,0,1)
        [Toggle] _AdditiveTexUvMode("UV Mode", Float) = 0
        _AdditiveIntensity("Additive Intensity" , range(0,1)) = 1

        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskTexTransform ("Mask Transform" , vector) = (0,0,0,1)
        [Toggle] _MaskTexUvMode("UV Mode", Float) = 0

        _MainWrapMode ("WrapMode" , vector) = (1,1,1,1)
        _AddWrapMode ("WrapMode" , vector) = (1,1,1,1)

        _NormalMap("",2D) = "bump"{}
        _NormalMapTransform("",Vector) = (1,1,1,1)
        _LightPos("",Vector)=(1,1,1,1)
        _LightColor("",Color)=(1,1,1,1)

        _DissolveTex("Dissolve Texture", 2D) = "white" {}
        _DissolveTexTransform ("_DissolveTex Transform" , vector) = (0,0,0,1)
        [Toggle] _DissolveTexUvMode("UV Mode", Float) = 0
        _DissolveValue("Dissolve", range(0,1)) = 0
        _DissolveRangeSize ("Range Size", range(0.01,0.5)) = 0.1
        [hdr]_DissolveRangeCol ("Range Color" , color) = (1,1,1,1)

        [Toggle]_ContouristColor("Contourist Color", Float) = 0
        [HDR]_ContouristColorA ("Edge First Color", Color) = (1,1,1,1)
        [HDR]_ContouristColorB ("Edge Second Color", Color) = (1,1,1,1)
        _EdgeRange("Edge Range",Range(0.0,1.0)) = 0.2

        _WarpTex("Warp Texture", 2D) = "bump" {}
        _WarpTexTransform ("FlowTex Transform" , Vector) = (0,0,0,1)
        [Toggle] _WarpTexUvMode("UV Mode", Float) = 0
        _WarpIntensity("Warp Intensity" , range(0,1)) = 1

        _SubWrapMode ("WrapMode" , vector) = (1,1,1,1)

        [HDR]_CeilColorA ("Contourist Color A", Color) = (1,1,1,1)
        [HDR]_CeilColorB ("Contourist Color B", Color) = (1,1,1,1)
        _ContouristPower("Contourist Pow",Range(-5,500)) = 1

        _BillboardRotation("Rotation", vector) = (0,0,0,0)
        _BillboardScale("Scale", vector) = (1,1,1,0)
        _BillboardMatrix0("Matrix1", vector) = (0,0,0,0)
        _BillboardMatrix1("Matrix2", vector) = (0,0,0,0)
        _BillboardMatrix2("Matrix3", vector) = (0,0,0,0)


        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
        SubShader
        {
            Tags 
            {
                "DisableBatching" = "False" 
                "LightMode" = "UniversalForward" 
                "Queue" = "Transparent" 
                "RenderType" = "Transparent"  
                "RenderPipeline" = "UniversalPipeline"
                "CanUseSpriteAtlas"="True"
                "IgnoreProjector"="True"
            }
            LOD 100

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWriteMode]
            ZTest[_ZTestMode]
            Cull[_CullMode]
            ColorMask [_ColorMask]

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing

                #pragma shader_feature __ _BILLBOARD_ON
                #pragma shader_feature __ _ADDITIVETEX_ON
                #pragma shader_feature __ _MASK_ON
                #pragma shader_feature __ _DISSOLVE_ON
                #pragma shader_feature __ _DISSOLVECOLOR_ON
                #pragma shader_feature __ _WARP_ON
                #pragma shader_feature __ _WRAPMODE_ON
                #pragma shader_feature __ _SCREENUV_ON
                #pragma shader_feature __ _CONTOURIST_ON
                #pragma shader_feature __ _NORMAL_ON
                
                //#include "CGInclude/LGameEffect.cginc"
                #include "UnityCG.cginc"
			    #include "UnityUI.cginc"
                #pragma multi_compile __ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
                #include "CGInclude/NBAEffect.cginc"  

                ENDCG
            }
        }

            SubShader
        {
            Tags 
            { 
                "Queue" = "Transparent" 
                "LightMode" = "UniversalForward" 
                "IgnoreProjector" = "True" 
                "RenderType" = "Transparent" 
                "RenderPipeline" = "UniversalPipeline"
                "CanUseSpriteAtlas"="True"
            }
            LOD 5
            Blend One One
            ZWrite[_ZWriteMode]
            ZTest[_ZTestMode]
            Cull[_CullMode]
            ColorMask [_ColorMask]

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragtest
                //#include "CGInclude/LGameEffect.cginc" 
                #include "CGInclude/NBAEffect.cginc"  

                half4 fragtest(v2f i) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(i);

                    fixed4 texColor = tex2D(_MainTex, i.uvMain.xy, float2(0, 0), float2(0, 0));

                    return half4(0.15,0.06,0.03, texColor.a < 0.001);
                }
                ENDCG
            }
        }

    SubShader
    {
        Tags {"DisableBatching" = "False" "LightMode"="ForwardBase" "Queue"="Transparent" "RenderType"="Transparent"  }
        LOD 100

        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWriteMode]
        ZTest [_ZTestMode]
        Cull [_CullMode]

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #pragma shader_feature __ _BILLBOARD_ON
            #pragma shader_feature __ _ADDITIVETEX_ON
            #pragma shader_feature __ _MASK_ON
            #pragma shader_feature __ _DISSOLVE_ON
            #pragma shader_feature __ _DISSOLVECOLOR_ON
            #pragma shader_feature __ _WARP_ON
            #pragma shader_feature __ _WRAPMODE_ON
            #pragma shader_feature __ _SCREENUV_ON
            #pragma shader_feature __ _CONTOURIST_ON
            #pragma shader_feature __ _NORMAL_ON
            
            #include "UnityCG.cginc"
			#include "UnityUI.cginc"
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            //#include "CGInclude/LGameEffect.cginc"
            #include "CGInclude/NBAEffect.cginc"  

            ENDCG
        }
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "LightMode" = "ForwardBase" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 5
        Blend One One
        ZWrite[_ZWriteMode]
        ZTest[_ZTestMode]
        Cull[_CullMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragtest
            //#include "CGInclude/LGameEffect.cginc" 
            #include "CGInclude/NBAEffect.cginc"  

            half4 fragtest(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                fixed4 texColor = tex2D(_MainTex, i.uvMain.xy, float2(0, 0), float2(0, 0));

                return half4(0.15,0.06,0.03, texColor.a < 0.001);
            }
            ENDCG
        }
    }
    
    CustomEditor"NBAEffectDefaultGUI"
}
