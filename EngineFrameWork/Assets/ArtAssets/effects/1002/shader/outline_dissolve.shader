// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/outline"
{
	Properties
	{
		_Dissolve("Dissolve", 2D) = "white" {}
		[Toggle]_Custom_U_Dissolve("Custom_U_Dissolve", Float) = 0
		_Outline_Width("Outline_Width", Range( 0 , 1)) = 0
		_MaskUV("MaskUV", Float) = 1
		_Outline_Dissolve("Outline_Dissolve", Range( 0 , 1)) = 0.8821928
		_Dissolve_Haft("Dissolve_Haft", Range( 0 , 0.5)) = 0
		_Float0("Float 0", Float) = 0
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" }
		ZWrite Off
		ZTest LEqual
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog alpha:premul  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _Outline_Width;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 color26 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float lerpResult14 = lerp( _Outline_Dissolve , i.uv2_tex4coord2.x , _Custom_U_Dissolve);
			float smoothstepResult44 = smoothstep( _Dissolve_Haft , ( 1.0 - _Dissolve_Haft ) , saturate( ( tex2D( _Dissolve, ( ase_screenPosNorm * _MaskUV ).xy ).r + (1.0 + (lerpResult14 - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0)) ) ));
			o.Emission = color26.rgb;
			o.Alpha = smoothstepResult44;
		}
		ENDCG
		

		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest LEqual
		Blend One One
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 screenPos;
			float4 uv2_tex4coord2;
		};

		uniform float _Float0;
		uniform float _Dissolve_Haft;
		uniform sampler2D _Dissolve;
		uniform float _MaskUV;
		uniform float _Outline_Dissolve;
		uniform float _Custom_U_Dissolve;
		uniform float _Outline_Width;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float lerpResult14 = lerp( _Outline_Dissolve , i.uv2_tex4coord2.x , _Custom_U_Dissolve);
			float smoothstepResult44 = smoothstep( _Dissolve_Haft , ( 1.0 - _Dissolve_Haft ) , saturate( ( tex2D( _Dissolve, ( ase_screenPosNorm * _MaskUV ).xy ).r + (1.0 + (lerpResult14 - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0)) ) ));
			float3 temp_cast_1 = (( _Float0 * smoothstepResult44 )).xxx;
			o.Emission = temp_cast_1;
			o.Alpha = smoothstepResult44;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred noambient novertexlights nolightmap  nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
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
				o.customPack1.xyzw = customInputData.uv2_tex4coord2;
				o.customPack1.xyzw = v.texcoord1;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.uv2_tex4coord2 = IN.customPack1.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
-1913;387;1920;995;2445.726;248.5239;1.135142;True;False
Node;AmplifyShaderEditor.RangedFloatNode;38;-1550.765,225.5591;Inherit;False;Property;_MaskUV;MaskUV;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1769.411,430.5125;Inherit;False;Property;_Outline_Dissolve;Outline_Dissolve;6;0;Create;True;0;0;0;False;0;False;0.8821928;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;36;-1654.549,2.234776;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-1757.307,791.7826;Inherit;False;Property;_Custom_U_Dissolve;Custom_U_Dissolve;3;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;62;-1760.636,521.2954;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;14;-1415.746,548.8132;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1337.765,188.5591;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;39;-1216.109,447.8854;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-1067.347,165.019;Inherit;True;Property;_Dissolve;Dissolve;0;0;Create;True;0;0;0;False;0;False;-1;9523ade04f9640d44a4238de35529299;9523ade04f9640d44a4238de35529299;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-556.6552,125.0597;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-733.98,525.3356;Inherit;False;Property;_Dissolve_Haft;Dissolve_Haft;7;0;Create;True;0;0;0;False;0;False;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;46;-394.0737,440.8489;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-241.7092,156.2853;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-24.75549,121.053;Inherit;False;Property;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-26.56087,1097.06;Inherit;False;Property;_Outline_Width;Outline_Width;4;0;Create;True;0;0;0;False;0;False;0;0.306;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;-253.9334,1021.026;Inherit;False;Constant;_Outline_Color;Outline_Color;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;44;-129.1397,407.7321;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;244.2443,228.0189;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1473.741,908.2736;Inherit;False;Constant;_2;2;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;63;-586.1587,-202.2096;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1412.741,1069.274;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1947.775,702.6575;Half;False;Property;_Dissolve_Scale;Dissolve_Scale;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1574.067,1033.317;Inherit;False;Constant;_1;1;1;0;Create;True;0;0;0;False;0;False;-3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;25;420.29,864.0098;Inherit;False;0;True;AlphaPremultiplied;2;3;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1238.5,840.3427;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;58;569.0716,49.83128;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Custom/outline;False;False;False;False;True;True;True;False;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Custom;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;40;0
WireConnection;14;1;62;1
WireConnection;14;2;18;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;39;0;14;0
WireConnection;10;1;37;0
WireConnection;41;0;10;1
WireConnection;41;1;39;0
WireConnection;46;0;45;0
WireConnection;43;0;41;0
WireConnection;44;0;43;0
WireConnection;44;1;45;0
WireConnection;44;2;46;0
WireConnection;66;0;65;0
WireConnection;66;1;44;0
WireConnection;13;1;15;0
WireConnection;25;0;26;0
WireConnection;25;2;44;0
WireConnection;25;1;29;0
WireConnection;11;1;12;0
WireConnection;11;2;13;0
WireConnection;58;2;66;0
WireConnection;58;9;44;0
WireConnection;58;11;25;0
ASEEND*/
//CHKSM=87FCA72A8931CDA5755E22EC93A642E932D4EF2C