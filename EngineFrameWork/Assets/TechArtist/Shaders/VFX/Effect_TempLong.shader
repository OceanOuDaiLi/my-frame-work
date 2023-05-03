// Shader created with Shader Forge v1.38
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33469,y:32787,varname:node_4795,prsc:2|emission-3684-OUT,alpha-6813-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31675,y:32449,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8e75ab0c7ab000744a83271b65382325,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Fresnel,id:9025,x:31281,y:33238,varname:node_9025,prsc:2|EXP-1332-OUT;n:type:ShaderForge.SFN_Multiply,id:1275,x:32019,y:33499,varname:node_1275,prsc:2|A-3437-OUT,B-4470-RGB;n:type:ShaderForge.SFN_Color,id:4470,x:31356,y:33400,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:1332,x:30904,y:33270,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_1332,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.7953832,max:5;n:type:ShaderForge.SFN_Lerp,id:9134,x:32656,y:33342,varname:node_9134,prsc:2|A-3563-OUT,B-3437-OUT,T-3437-OUT;n:type:ShaderForge.SFN_Tex2d,id:8943,x:31641,y:32712,ptovrint:False,ptlb:Particle,ptin:_Particle,varname:node_8943,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7e01e782da5ee8f479ae52273fcd2e23,ntxv:0,isnm:False|UVIN-684-OUT;n:type:ShaderForge.SFN_Time,id:6035,x:30713,y:32805,varname:node_6035,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:9758,x:30975,y:32749,varname:node_9758,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Append,id:684,x:31457,y:32729,varname:node_684,prsc:2|A-3648-OUT,B-9091-OUT;n:type:ShaderForge.SFN_Multiply,id:4160,x:31017,y:32916,varname:node_4160,prsc:2|A-6035-T,B-2551-OUT;n:type:ShaderForge.SFN_Add,id:3684,x:32679,y:32714,varname:node_3684,prsc:2|A-4353-OUT,B-1264-OUT;n:type:ShaderForge.SFN_Multiply,id:4353,x:31954,y:32836,varname:node_4353,prsc:2|A-8943-RGB,B-8943-A,C-5380-RGB;n:type:ShaderForge.SFN_Add,id:9091,x:31182,y:32812,varname:node_9091,prsc:2|A-9758-V,B-4160-OUT;n:type:ShaderForge.SFN_Add,id:3648,x:31182,y:32596,varname:node_3648,prsc:2|A-3469-OUT,B-9758-U;n:type:ShaderForge.SFN_Multiply,id:3469,x:30955,y:32596,varname:node_3469,prsc:2|A-8348-OUT,B-6035-T;n:type:ShaderForge.SFN_Slider,id:8348,x:30545,y:32698,ptovrint:False,ptlb:USpeed,ptin:_USpeed,varname:node_8348,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:2551,x:30545,y:32949,ptovrint:False,ptlb:VSpeed,ptin:_VSpeed,varname:_USpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Add,id:1264,x:32473,y:32815,varname:node_1264,prsc:2|A-6074-RGB,B-1450-OUT;n:type:ShaderForge.SFN_Multiply,id:7475,x:31550,y:33199,varname:node_7475,prsc:2|A-291-OUT,B-9025-OUT;n:type:ShaderForge.SFN_Slider,id:291,x:30922,y:33121,ptovrint:False,ptlb:Strength,ptin:_Strength,varname:node_291,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:5;n:type:ShaderForge.SFN_ConstantClamp,id:3437,x:31716,y:33249,varname:node_3437,prsc:2,min:0,max:1|IN-7475-OUT;n:type:ShaderForge.SFN_Multiply,id:1450,x:32314,y:33041,varname:node_1450,prsc:2|A-8101-OUT,B-1275-OUT;n:type:ShaderForge.SFN_Subtract,id:1016,x:31934,y:33039,varname:node_1016,prsc:2|A-6074-A,B-6925-OUT;n:type:ShaderForge.SFN_Slider,id:6925,x:31471,y:33070,ptovrint:False,ptlb:AlphaOffset,ptin:_AlphaOffset,varname:node_6925,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.2649573,max:1;n:type:ShaderForge.SFN_Clamp01,id:3563,x:32202,y:33166,varname:node_3563,prsc:2|IN-1016-OUT;n:type:ShaderForge.SFN_VertexColor,id:5380,x:31641,y:32871,varname:node_5380,prsc:2;n:type:ShaderForge.SFN_Slider,id:6445,x:32079,y:32495,ptovrint:False,ptlb:HeadStrength,ptin:_HeadStrength,varname:node_6445,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:2;n:type:ShaderForge.SFN_Clamp01,id:8101,x:32167,y:32861,varname:node_8101,prsc:2|IN-3222-OUT;n:type:ShaderForge.SFN_Subtract,id:3222,x:31965,y:32692,varname:node_3222,prsc:2|A-6445-OUT,B-6074-A;n:type:ShaderForge.SFN_Add,id:2629,x:32914,y:33208,varname:node_2629,prsc:2|A-8943-A,B-9134-OUT;n:type:ShaderForge.SFN_Clamp01,id:6813,x:33160,y:33179,varname:node_6813,prsc:2|IN-2629-OUT;proporder:6074-4470-1332-8943-8348-2551-291-6925-6445;pass:END;sub:END;*/

Shader "NBA/Effect/TempLong" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Power ("Power", Range(0.01, 8)) = 0.7953832
        _Particle ("Particle", 2D) = "white" {}
        _USpeed ("USpeed", Range(-2, 2)) = 0
        _VSpeed ("VSpeed", Range(-2, 2)) = 0
        _Strength ("Strength", Range(1, 5)) = 1
        _AlphaOffset ("AlphaOffset", Range(-1, 1)) = 0.2649573
        _HeadStrength ("HeadStrength", Range(1, 2)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
		Pass
		{
			COLORMASK 0
		}
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            //#pragma only_renderers d3d9 d3d11 glcore gles
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _Power;
            uniform sampler2D _Particle; uniform float4 _Particle_ST;
            uniform float _USpeed;
            uniform float _VSpeed;
            uniform float _Strength;
            uniform float _AlphaOffset;
            uniform float _HeadStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_6035 = _Time;
                float2 node_684 = float2(((_USpeed*node_6035.g)+i.uv1.r),(i.uv1.g+(node_6035.g*_VSpeed)));
                float4 _Particle_var = tex2D(_Particle,TRANSFORM_TEX(node_684, _Particle));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_3437 = clamp((_Strength*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Power)),0,1);
                float3 emissive = ((_Particle_var.rgb*_Particle_var.r*i.vertexColor.rgb)+(_MainTex_var.rgb+(saturate((_HeadStrength-_MainTex_var.a))*(node_3437*_Color.rgb))));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,saturate((_Particle_var.r+lerp(saturate((_MainTex_var.a-_AlphaOffset)),node_3437,node_3437))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    //CustomEditor "ShaderForgeMaterialInspector"
	FallBack "Diffuse"
}
