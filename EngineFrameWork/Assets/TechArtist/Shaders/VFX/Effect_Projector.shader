// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NBA/Effect/Projector" {
  Properties {
     _ShadowTex ("Cookie", 2D) = "" { }
	 _Color ("Color", Color) = (1.0,1.0,1.0,1.0)
  }
  Subshader {
     pass {
        ZWrite off
        Blend DstColor One
       CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        uniform sampler2D _ShadowTex;
        uniform float4x4 unity_Projector;
		uniform fixed4 _Color;

        struct v2f {
            float4 pos:SV_POSITION;
            float4 texc:TEXCOORD0;
        };
        v2f vert(appdata_base v)
        {
            v2f o;
            o.pos=UnityObjectToClipPos(v.vertex);
            o.texc=mul(unity_Projector,v.vertex);
            return o;
        }
        float4 frag(v2f i):COLOR
        {
            float4 c=tex2Dproj(_ShadowTex,i.texc);
			c *= _Color;
            return c;
        }
        ENDCG
    }//endpass
  }
}