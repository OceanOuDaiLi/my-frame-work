Shader "NBA/Effect/CartoonExplode" 
{
    Properties 
	{
        _Noise ("Noise", 2D) = "white" {}
        _Strength ("Strength", Range(0, 1)) = 1
        _Color ("Color", Color) = (0.5294118,0.03892734,0.03892734,1)
        _U_Speed ("U_Speed", Range(0, 1)) = 0
        _V_Speed ("V_Speed", Range(0, 1)) = 0
        _DistortionPower ("DistortionPower", Range(0.5, 5)) = 4.41886
        [MaterialToggle] _IsHardEdge ("IsHardEdge", Float ) = 0
        _Step ("Step", Range(0, 1)) = 0.5
        _AddRange ("AddRange", Range(-5, 5)) = 0.1
        _Clip ("Clip", Range(-0.001, 1)) = -0.001
        [MaterialToggle] _IsVertexAlpha ("IsVertexAlpha", Float ) = 0.0
        _CYF ("CYF", Range(0, 2)) = 1
        _AddBrightness ("AddBrightness", Range(1, 3)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader 
	{
        Tags 
		{
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Strength;
            uniform float4 _Color;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float _DistortionPower;
            uniform fixed _IsHardEdge;
            uniform float _Step;
            uniform float _AddRange;
            uniform float _Clip;
            uniform fixed _IsVertexAlpha;
            uniform float _CYF;
            uniform float _AddBrightness;
            struct VertexInput 
			{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
                o.uv = v.uv;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
				
                half2 noiseUV = half2(_U_Speed*_Time.r+o.uv.r,o.uv.g+_Time.g*_V_Speed);//uv动画
                fixed4 _Noise_var = tex2Dlod(_Noise,float4(TRANSFORM_TEX(noiseUV, _Noise),0.0,0));//获取人家所谓的diffuse
                float distorPow = pow(_Noise_var.r,_DistortionPower);
                v.vertex.xyz += (distorPow*v.normal*_Strength*pow(o.uv.b,_CYF));//进行顶点挤出
				
				
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR 
			{
                //float3 normalDirection = normalize(i.normalDir);
                half2 noiseUV = half2(_U_Speed*_Time.r+i.uv.r,i.uv.g+_Time.g*_V_Speed);
                fixed4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(noiseUV, _Noise));
				
                clip((lerp( _Clip, (i.uv.b*-1.001+1.0), _IsVertexAlpha )/(1.0 - _Noise_var.g)) - 0.5);//通过_IsVertexAlpha进行a或b的clip方式
				//clip(_Clip/(1.0-_Noise_var.y) - 0.5);//a通过_Clip参数数值进行剪裁
				//clip((-1.001*i.uv.z+1.0)/(1.0-_Noise_var.y) - 0.5);//通过uv的z通道进行剪裁
				
                fixed colorStep = saturate(_Noise_var.r+i.uv.b*_AddRange);//获取颜色的Lerp值
				fixed4 finalColor = fixed4(0,0,0,1);
                finalColor.rgb = lerp(i.vertexColor.rgb,_Color.rgb,lerp( colorStep, floor(saturate(colorStep+_Step)), _IsHardEdge ))*(1.0+i.uv.a*_AddBrightness);
				return finalColor;
            }
            ENDCG
        }
    }
}
