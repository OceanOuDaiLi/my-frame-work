
Shader "FTX/UvBrush//MeshColor"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1

		[HideInInspector] _WireColor("Wire Color", Color) = (1.0, 0.4, 0.0, 1.0)
		[HideInInspector] _WireSize("Wire Size", Range(0.01, 5.0)) = 2.0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque" 
			"Queue" = "Geometry+0"
			"IsEmissive" = "true" 
		}

		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = i.vertexColor.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}

	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
