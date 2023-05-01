Shader "NBA2/BGRemoveACES"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Lum("Lum",Range(-1.5,1.5)) = 1
        _Saturation("Saturation",Range(0.5,1.5)) = 1
        _Contrast("Contrast",Range(0.5,1.5)) = 1
        [HDR]_Hue("Hue",Color) = (1,1,1,1)
        [Enum(Off,0,On,1)]_ZWrite("ZWrite",Float) = 0
        [Enum(One,1,SrcAlpha,5)]_SrcBlend("SrcBlend",Float) = 1
        [Enum(Zero,0,OneMiusSrcAlpha,10)]_DstBlend("DsrBlend",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent""Queue" = "Geometry" }
        LOD 100
        Cull Off
        ZWrite [_ZWrite]
        Blend  [_SrcBlend] [_DstBlend]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _IFREMOVEACES_OFF _IFREMOVEACES_ON
            
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half  _Saturation, _Contrast,_Lum;
            fixed4 _Hue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = tex2D(_MainTex, i.uv);
                const float A = 2.51f;
                const float B = 0.03f;
                const float C = 2.43f;
                const float D = 0.59f;
                const float E = 0.14f;
                float3 color = sqrt(E * col * (A - C * col) + ((B - D * col) * (B - D * col) / 4)) - (B - D * col) / 2;
                color = color / (A - C * col);
                fixed gray = 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
                fixed3 grayCol = fixed3(gray, gray, gray);
                color = lerp(grayCol, color, _Saturation);
                color /= _Lum;
                fixed3 temp = fixed3(0.5, 0.5, 0.5);
                color = lerp(temp, color, _Contrast);
                return fixed4(color, col.a); 
            }
            ENDCG
        }
    }
}
