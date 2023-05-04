Shader "NBA/Scene/BasketStandTransparent"
{
	Properties
	{
		_AlbedoMap("Texture", 2D) = "white" {}
		_Transparency("Transparency",Range(0.2,1)) =1.0
		_RColor("RColor", Color) = (1,1,1,1)
        _GColor("GColor", Color) = (1,1,1,1)
        _BColor("BColor", Color) = (1,1,1,1)
        _AColor("AColor", Color) = (1,1,1,1)
		_ZWrite("Z Write",float) = 0
		_ClipValue("ClipValue",float) =0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Transparent+1"}
		LOD 100
		//Cull Off
		ZWrite [_ZWrite]
		//ZWrite On
		ZTest On
		

		//Blend srcAlpha OneMinusSrcAlpha
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			
			//#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			float _Transparency;
			half4 _RColor;
			half4 _GColor;
			half4 _BColor;
			half4 _AColor;
			float _ClipValue;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 vertexColor : COLOR;
			};

			//sampler2D _AlbedoMap;
			TEXTURE2D(_AlbedoMap);
			SAMPLER(sampler_AlbedoMap);
			float4 _AlbedoMap_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _AlbedoMap);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				o.vertexColor = v.vertexColor;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half4 col = SAMPLE_TEXTURE2D(_AlbedoMap,sampler_AlbedoMap, i.uv);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				half3 Vertr = lerp(col.rgb, _RColor, i.vertexColor.r);
				half3 Vertg = lerp(col.rgb, _GColor, i.vertexColor.g);
				half3 Vertb = lerp(col.rgb, _BColor, i.vertexColor.b);
				half3 Verta = lerp(col.rgb, _AColor, 1 - i.vertexColor.a);
				half3 Vcol = lerp(col.rgb, Vertr + Vertg + Vertb + Verta, i.vertexColor.r + i.vertexColor.g + i.vertexColor.b + 1 - i.vertexColor.a);
				//return half4(Vcol,1);
				col.rgb = Vcol * 0.4;
				clip(col.a-_ClipValue);
				col.a = _Transparency*col.a;
				return col;
			}
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
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA


            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
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
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple

            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"

            ENDHLSL
        }
	}
}
