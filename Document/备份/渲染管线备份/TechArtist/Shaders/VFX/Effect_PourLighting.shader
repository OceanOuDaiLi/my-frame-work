Shader "NBA/Boss/PourLighting"
{
    Properties {
        _mask ("mask", 2D) = "white" {}
        _sin ("sin", Range(0, 5)) = 1
        _add ("add", Range(0, 2)) = 1
        _uv ("uv", 2D) = "white" {}
        _D ("D", 2D) = "white" {}
        _speed_u ("speed_u", Range(-10, 10)) = 0
        _speed_v ("speed_v", Range(-10, 10)) = 0
        _glow ("glow", Range(0, 20)) = 0
        [HDR]_color ("color", Color) = (0.5,0.5,0.5,1)
        _mask_02 ("mask_02", 2D) = "white" {}
		_RimPower("Fresnel Falloff", Range(0.1, 8)) = 2
		[HDR]_RimColor("Fresnel Color", Color) = (1.0,1.0,1.0,1.0)
		[ToggleOff]_RimToggle("Fresnel Toggle",float) = 1.0
		_Saturation ("ËÀÍö³·É«", Range(0.0,1.0)) = 1.0
		_OutLineMinWidth ("OutLine Min Width", Range(0.0001, 0.005)) = 0.001
		_OutLineMaxWidth ("OutLine Max Width", Range(0.0001, 0.01)) = 0.001
		_SafeDistance ("Safe Distance", Range(0.01,5)) = 0.5
		[HDR]_OutLineColor("OutLine Color", Color) = (1.0,1.0,1.0,1.0)
		[ToggleOff]_UsedByEffect ("Used By Effect",float) = 0.0
		_OutLineWidthMinStrength ("Outine Min Width Strength",Range(1.0,10.0)) = 1.0
		_OutLineWidthMaxStrength ("Outine Max Width Strength",Range(1.0,10.0)) = 1.0
		
    }
    SubShader
	{
        Tags
		{
            "RenderType"="Opaque"
        }

		Pass
		{
		    Name "Outline"
			Cull front
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform fixed4 _OutLineColor;
			uniform float _SafeDistance;
			uniform fixed _OutLineMinWidth;
			uniform fixed _OutLineMaxWidth;
			uniform fixed _UsedByEffect;
			uniform half _OutLineWidthMinStrength;
			uniform half _OutLineWidthMaxStrength;
			
			uniform sampler2D _D;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 texCoord : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o = (vertexOutput)0;

				float4 worldPos = mul(unity_ObjectToWorld,v.vertex);

				float camDis = length(_WorldSpaceCameraPos.xyz - worldPos.xyz);// * 0.01;
					
				float threshold = saturate((camDis - _SafeDistance)/(_SafeDistance+0.001));
				half minStrength = lerp(1,_OutLineWidthMinStrength,_UsedByEffect) * _OutLineMinWidth;
				half maxStrength = lerp(1,_OutLineWidthMaxStrength,_UsedByEffect) * _OutLineMaxWidth;

				float outLineWidth = lerp(minStrength,maxStrength,threshold);
				
				//half effectOutline = lerp(1,_OutLineWidthStrength,_UsedByEffect);
				half outline = outLineWidth;

				float3 offset = v.normal * outline;

				o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + offset,1) );
				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
                //float4 color = tex2Dlod( _D, float4(input.texCoord, 0, 0));
				//color *= _OutlineColor;
				fixed4 color = fixed4(0,0,0,1);
				fixed3 mainDiffuse = tex2D(_D,i.texCoord.xy).rgb;
				mainDiffuse.rgb = lerp(mainDiffuse.rgb,fixed3(1,1,1),_UsedByEffect);
				color.rgb = pow(mainDiffuse.rgb * _OutLineColor.rgb,1.5);
				return color;
			}

			ENDCG
		}
		
        Pass 
		{
            Name "FORWARD"
            Tags 
			{
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0
            uniform fixed4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _speed_u;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _D; uniform float4 _D_ST;
            uniform float _sin;
            uniform float _add;
            uniform sampler2D _uv; uniform float4 _uv_ST;
            uniform float _speed_v;
            uniform float _glow;
            uniform fixed4 _color;
            uniform sampler2D _mask_02; uniform float4 _mask_02_ST;
			uniform float _RimPower;
			uniform fixed4 _RimColor;
			uniform fixed _RimToggle;
			uniform fixed _Saturation;
            
			struct VertexInput 
			{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            
			struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                fixed3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v)
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR
			{
                fixed3 normalDirection = normalize(i.normalDir);
                fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 lightColor = _LightColor0.rgb;
                float attenuation = LIGHT_ATTENUATION(i);
                fixed3 attenColor = attenuation * _LightColor0.xyz;
                fixed NdotL = max(0.0,dot( normalDirection, lightDirection ));
                fixed3 directDiffuse = max( 0.0, NdotL) * attenColor;
                fixed4 _D_var = tex2D(_D,TRANSFORM_TEX(i.uv0, _D));
                fixed3 diffuse = (directDiffuse + UNITY_LIGHTMODEL_AMBIENT.rgb) * _D_var.rgb;
                fixed4 _mask_02_var = tex2D(_mask_02,TRANSFORM_TEX(i.uv0, _mask_02));
                float4 node_4136 = _Time + _TimeEditor;
                float2 node_8217 = ((i.uv0+(node_4136.g*_speed_u)*float2(1,0))+(i.uv0+(node_4136.g*_speed_v)*float2(0,1)));
                fixed4 _uv_var = tex2D(_uv,TRANSFORM_TEX(node_8217, _uv));
                fixed4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float4 node_4803 = _Time + _TimeEditor;
                float3 emissive = (((_mask_02_var.rgb+(_uv_var.r*_mask_var.r))*(saturate((_mask_var.rgb*sin((node_4803.g*_sin))))+_add)*_glow)*_color.rgb);
                fixed3 finalColor = diffuse + emissive;
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);//UnityWorldSpaceViewDir(i.worldPos);
				float rim = 1 - max(0, dot(normalDirection, viewDir));
				rim = pow(saturate(rim), _RimPower);
				fixed gray  = dot(finalColor.rgb, fixed3(0.299,0.587,0.114));
				finalColor.rgb = lerp(gray,finalColor.rgb,_Saturation);//target color
				finalColor.rgb = lerp(finalColor.rgb, rim*_RimColor.rgb, rim * _RimToggle);


                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
