Shader "LLLLLLL/Complex_FX" {
    Properties {
        //////////////特效固有/////////////
        [Enum(UnityEngine.Rendering.BlendMode)]_SourceBlend("Source Blend mode",float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_DestBlend("Dest Blend mode",float) = 10
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", float) = 2.0

        //////////////////////////////////
        
        _TotalAlpha("Total Alpha",Range(0,1)) = 1

        [HDR]_Tex_color ("Tex_Color", Color) = (0.5019608,0.5019608,0.5019608,0)
        _Texture ("Texture", 2D) = "white" {}
        [MaterialToggle]_TexStep("Texture Step",float) = 0
        _TexStepCount("Texture Step Count",Range(2,20)) = 7
        _Intensity ("Intensity", float ) = 1
        _Tex_U_Speed ("Tex_U_Speed", float ) = 0
        _Tex_V_Speed ("Tex_V_Speed", float ) = 0
        [MaterialToggle] _Tex_UV_flow ("Tex_UV_Flow", float ) = 0
        [MaterialToggle] _Show_undertone ("Show_Undertone", float ) = 0


        [MaterialToggle] _UseColor ("Use Color", float ) = 0
        [HDR]_TexColor1("Color1", Color) = (1,0,0,0)
        [HDR]_TexColor2("Color2", Color) = (0,0.3408313,1,0)
        _TexColor3("Color3", Color) = (0,1,0.3755169,0)
        _TexUV_01("UV_01", Float) = 1
        _TexUV_02("UV_02", Float) = 1
        _TexUV_03("UV_03", Float) = 1
        _TexUV_Intensity("UV_Intensity", float) = 1


        _Maks01 ("Maks01", 2D) = "white" {}
        _Maks01_U ("Maks01_U", float ) = 0
        _Maks01_V ("Maks01_V", float ) = 0
        _Maks01Noise("Masks01 noise", 2D) = "white" {}
        _Maks01NoiseEmission ("Emission", float ) = 0
        _Maks01Noise_U ("Maks01Noise_U", float ) = 0
        _Maks01Noise_V ("Maks01Noise_V", float ) = 0
        
        _Maks02 ("Maks02", 2D) = "white" {}
        _Mask02_U ("Mask02_U", float ) = 0
        _Mask02_V ("Mask02_V", float ) = 0
        _Tex_PolarCoordinates ("Tex_PolarCoordinates", 2D) = "white" {}
        [HDR]_PolarCoordinates_Color ("PolarCoordinates_Color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _PolarCoordinates_on ("PolarCoordinates_on", float ) = 1
        _Intensity_PolarCoordinates ("Intensity_PolarCoordinates", float ) = 0
        _Orientation ("Orientation", Range(-1, 1)) = 0
        [MaterialToggle] _Soft_Dissolution ("Soft_Dissolution", float ) = 1
        _Soft_Dissolution_Intensity ("Soft_Dissolution_Intensity", float ) = 1
        _Tex_Dissolve ("Tex_Dissolve", 2D) = "white" {}
        _Dissolve_U ("Dissolve_U", float ) = 0
        _Dissolve_V ("Dissolve_V", float ) = 0
        _Tex_DissolveDir("Tex_Dissolve_Mask", 2D) = "white" {}
        _Tortuosity ("Tortuosity", 2D) = "white" {}
        _TortuosityIntensity ("TortuosityIntensity", float ) = 0
        _Tortuosity_U ("Tortuosity_U", float ) = 0
        _Tortuosity_V ("Tortuosity_V", float ) = 0
        _Vertex_Move ("Vertex_Move", 2D) = "white" {}
        _Vertex_U ("Vertex_U", float ) = 0
        _Vertex_V ("Vertex_V", float ) = 0
        _Vertex_XYZW ("Vertex_XYZW", Vector) = (0,0,0,0)
        _Edgerange ("Edge range", Range(0, 5)) = 0
        _Endgebrightness ("Endge brightness", Range(0, 1)) = 0
        [HDR]_Edgelight_Color ("Edgelight_Color", Color) = (1,1,1,1)
        _NackFace ("NackFace", Range(0, 1)) = 1
        [HDR]_Doublesided_Color01 ("Double-sided_Color01", Color) = (1,1,1,1)
        [HDR]_Doublesided_Color02 ("Double-sided_Color02", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        CGINCLUDE
        #pragma target 3.5

        

        sampler2D _Texture; 
        sampler2D _Maks01; 
        sampler2D _Maks01Noise;
        sampler2D _Tex_PolarCoordinates;
        sampler2D _Tortuosity; 
        sampler2D _Maks02; 
        sampler2D _Vertex_Move; 
        sampler2D _Tex_Dissolve; 
        sampler2D _Tex_DissolveDir; 

        float4 _Texture_ST;
        float4 _Maks01_ST;
        float4 _Maks01Noise_ST;
        float4 _Tex_PolarCoordinates_ST;
        float4 _Tortuosity_ST;
        float4 _Maks02_ST;
        float4 _Vertex_Move_ST;
        float4 _Tex_Dissolve_ST;
        float4 _Tex_DissolveDir_ST;
        half4 _Doublesided_Color02;
        half4 _Doublesided_Color01;
        half4 _Vertex_XYZW;
        half4 _Tex_color;
        half4 _TexColor1;
        half4 _TexColor2;
        half4 _TexColor3;
        half4 _PolarCoordinates_Color;
        half4 _Edgelight_Color;

        float _TexStepCount;
        half _TexStep;
        half _UseColor;
        half _TexUV_01;
        half _TexUV_02;
        half _TexUV_03;
        half _TexUV_Intensity;
        half _Tex_U_Speed;
        half _Tex_V_Speed;
        half _Show_undertone;
        half _Maks01_V;
        half _Maks01_U;
        half _Maks01Noise_V;
        half _Maks01Noise_U;
        half _Maks01NoiseEmission;
        half _Intensity; 
        half _PolarCoordinates_on;
        half _Orientation;
        half _Intensity_PolarCoordinates;
        half _Soft_Dissolution;
        half _Soft_Dissolution_Intensity;
        half _Tex_UV_flow;
        half _Tortuosity_U;
        half _Tortuosity_V;
        half _Mask02_U;
        half _Mask02_V;
        half _Vertex_U;
        half _Vertex_V;
        half _TortuosityIntensity;
        half _Edgerange;
        half _Endgebrightness;
        half _NackFace;
        half _Dissolve_U;
        half _Dissolve_V;
        half _TotalAlpha;
        half _MixFogIntensity;



        ENDCG

        Pass {
            Name "FORWARD"

            Blend [_SourceBlend] [_DestBlend]
            Cull [_CullMode]
            ZTest[_ZTest]
            ZWrite Off
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                half4 vertexColor : COLOR;

            };
            struct v2f {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                half4 vertexColor : COLOR;


            };
            v2f vert (a2v v) {
                v2f o = (v2f)0;

                o.uv = v.uv;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                o.vertexColor = v.vertexColor;
                half4 node_7503 = _Time;
                float2 node_1157 = (float2((_Vertex_U*node_7503.g),(node_7503.g*_Vertex_V))+o.uv);
                half4 _Vertex_Move_var = tex2Dlod(_Vertex_Move,half4(TRANSFORM_TEX(node_1157, _Vertex_Move),0.0,0));
                v.vertex.xyz += (_Vertex_Move_var.rgb*_Vertex_XYZW.rgb*v.normal);
                o.positionWS = mul( unity_ObjectToWorld, v.vertex ).xyz;
                o.positionCS = UnityObjectToClipPos( v.vertex.xyz);

                return o;
            }
            half4 frag(v2f i, half facing : VFACE) : COLOR {
                half isFrontFace = ( facing >= 0 ? 1 : 0 );
                half node_4438 = saturate((1.0-i.uv1.g));
                half4 node_8184 = _Time;
                half2 node_3560 = (i.uv*2.0+-1.0);
                half2 node_9755 = node_3560.rg;
                float2 _PolarCoordinates_on_var = lerp( (node_4438+i.uv), half2(((_Orientation*node_8184.g)+length(node_3560)),atan2(node_9755.r,node_9755.g)), _PolarCoordinates_on );
                half4 _Tex_PolarCoordinates_var = tex2D(_Tex_PolarCoordinates,TRANSFORM_TEX(_PolarCoordinates_on_var, _Tex_PolarCoordinates));
                half node_9269 = 0.0;
                half node_370 = 1.0;
                half4 node_9762 = _Time;
                half2 node_345 = (i.uv+half2((node_9762.g*_Tortuosity_U),(node_9762.g*_Tortuosity_V)));
                half4 _Tortuosity_var = tex2D(_Tortuosity,TRANSFORM_TEX(node_345, _Tortuosity));
                half4 node_6611 = _Time;
                float2 node_8870 = ((half2(_Tortuosity_var.r,_Tortuosity_var.r)*_TortuosityIntensity)+lerp( lerp(float2(_Tortuosity_var.g,i.uv.r),(i.uv+float2(i.uv2.b,i.uv2.a)),i.uv2.g), (half2((_Tex_U_Speed*node_6611.g),(node_6611.g*_Tex_V_Speed))+i.uv), _Tex_UV_flow ));
                half4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_8870, _Texture));
                _TexStepCount = floor(_TexStepCount);
                _Texture_var.r *= _TexStepCount;
                _Texture_var.r = floor(_Texture_var.r) * _TexStep  + _Texture_var.r * (1 - _TexStep);
                _Texture_var.r *= rcp(_TexStepCount);
                //return float4(_TexStep.rrr,1);
                //return _Texture_var;
                
                half4 _Tex_R = tex2D(_Texture,TRANSFORM_TEX(node_8870*float2(_TexUV_01,_TexUV_01), _Texture));
                half4 _Tex_G = tex2D(_Texture,TRANSFORM_TEX(node_8870*float2(_TexUV_02,_TexUV_02), _Texture));
                half4 _Tex_B = tex2D(_Texture,TRANSFORM_TEX(node_8870*float2(_TexUV_03,_TexUV_03), _Texture));
                _Tex_R.r *= _TexStepCount;
                _Tex_R.r = floor(_Tex_R.r) * _TexStep  + _Tex_R.r * (1 - _TexStep);
                _Tex_R.r *= rcp(_TexStepCount);

                _Tex_G.g *= _TexStepCount;
                _Tex_G.g = floor(_Tex_G.g) * _TexStep  + _Tex_G.g * (1 - _TexStep);
                _Tex_G.g *= rcp(_TexStepCount);

                _Tex_B.b *= _TexStepCount;
                _Tex_B.b = floor(_Tex_B.b) * _TexStep  + _Tex_B.b * (1 - _TexStep);
                _Tex_B.b *= rcp(_TexStepCount);
                _Tex_R = _Tex_R * 0.73;
                float4 TexColor1 = _Tex_R.r * _TexColor1 *i.vertexColor;

                float4 TexColor2 = _Tex_G.g * _TexColor2 *i.vertexColor;
                float4 TexColor3 = _Tex_B.b * _TexColor3 *i.vertexColor;
                float4 finalTexColor123 = TexColor1+ TexColor2+TexColor3;
                finalTexColor123 *=_TexUV_Intensity;
                float finalTexColor123Alpha = TexColor1.a + TexColor2.a + TexColor3.a;
                finalTexColor123.a = finalTexColor123Alpha;
                _Texture_var = finalTexColor123 *_UseColor + (1 - _UseColor) * _Texture_var;
                //return finalTexColor123Alpha;
                
                half3 emissive = ((((_PolarCoordinates_Color.rgb*_Intensity_PolarCoordinates)*_Tex_PolarCoordinates_var.rgb)+((_NackFace + ( ((1.0 - isFrontFace) - node_9269) * (node_370 - _NackFace) ) * rcp(node_370 - node_9269))*_Texture_var.rgb*lerp(_Doublesided_Color01.rgb,_Doublesided_Color02.rgb,isFrontFace)))*_Tex_color.rgb*i.vertexColor.rgb*_Intensity);
                half4 node_2534 = _Time;
                float2 node_6861 = (half2((_Dissolve_U*node_2534.g),(node_2534.g*_Dissolve_V))+i.uv);
                half4 _Tex_Dissolve_var = tex2D(_Tex_Dissolve,TRANSFORM_TEX(node_6861, _Tex_Dissolve));
                half2 disDirUV = i.uv.xy * _Tex_DissolveDir_ST.xy + _Tex_DissolveDir_ST.zw;
                half4 _Tex_DissolveDir_var = tex2D(_Tex_DissolveDir,disDirUV);
                _Tex_Dissolve_var *=_Tex_DissolveDir_var;
                half node_1230 = (saturate(((_Tex_Dissolve_var.r-i.uv1.r)*_Soft_Dissolution_Intensity))*node_4438);
                half DissolveRim = node_1230;
                half node_7342_if_leA = step(_Edgerange,DissolveRim);
                half node_7342_if_leB = step(DissolveRim,_Edgerange);
                half3 finalColor = emissive + (_Endgebrightness*lerp((node_7342_if_leA*0.0)+(node_7342_if_leB*1.0),1.0,node_7342_if_leA*node_7342_if_leB)*_Edgelight_Color.rgb);
                half4 node_8324 = _Time;
                float2 node_3497 = (half2((_Maks01_U*node_8324.g),(node_8324.g*_Maks01_V))+i.uv);
                //half4 _Maks01_var = tex2D(_Maks01,TRANSFORM_TEX(node_3497, _Maks01));
                ///////////////////
                float2 _Maks01noiseUV = (half2((_Maks01Noise_U*node_8324.g *1.5),(node_8324.g*_Maks01Noise_V * 1.5))+i.uv);
                half2 _Maks01noise_tex = tex2D(_Maks01Noise,TRANSFORM_TEX(_Maks01noiseUV, _Maks01Noise)).xy;
                _Maks01noise_tex = ((_Maks01noise_tex*2)-1) *_Maks01NoiseEmission;
                half4 _Maks01_var = tex2D(_Maks01,TRANSFORM_TEX(node_3497 + _Maks01noise_tex, _Maks01));
                ///////////////////

                half4 node_1505 = _Time;
                float2 node_3922 = (i.uv+half2((_Mask02_U*node_1505.g),(node_1505.g*_Mask02_V)));
                half4 _Maks02_var = tex2D(_Maks02,TRANSFORM_TEX(node_3922, _Maks02));
                half _Soft_Dissolution_var = lerp( step(i.uv1.r,_Tex_Dissolve_var.r), node_1230, _Soft_Dissolution );
                half4 finalRGBA = half4(finalColor,((lerp( _Texture_var.r, _Texture_var.a, _Show_undertone )*i.vertexColor.a*(_Maks01_var.r*_Maks02_var.r)*_Soft_Dissolution_var*step(i.uv2.r,_Tex_Dissolve_var.r))*_Soft_Dissolution_var));
                finalRGBA.a *= _TotalAlpha;


                return finalRGBA;
            }
            ENDCG
        }
    }
}
