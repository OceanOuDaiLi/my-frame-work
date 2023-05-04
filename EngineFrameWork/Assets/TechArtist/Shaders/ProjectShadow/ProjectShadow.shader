Shader "My/ProjectShadow"
{
                Properties
    {
                // 平面阴影
        //_Stencil("Stencil", Float) = 5  
        _ShadowDirection("ShadowDirection",Vector) = (0.6, 1, 0.5, 0.05)
        _ShadowAlpha("ShadowAlpha",Range(0.0,10.0)) = 0.5
        _ShadowFalloff("ShadowFalloff",Range(-1.0,1.0)) = 1.0

    }

    SubShader
    {


        //角色平面阴影
        Pass
        {

            Tags
            {

                //"LightMode" = "SRPDefaultUnlit"

                "LightMode" = "ProjectShadow"

                "Queue" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            //Blend[_SorcBlend][_DestBlend]
            Offset -1 , 0
            ZWrite Off
            //ZWrite On

            Cull Back
            ColorMask RGBA

            Stencil
            {
                Ref 1
                Comp NotEqual
                WriteMask 255
                ReadMask 255

                Pass Replace
                Fail Keep
                //ZFail Keep
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            //#include "StandardCharacter.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma shader_feature _USE_PROJECT_SHADOW

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half4 color :TEXCOORD5;
            };

            CBUFFER_START(UnityPerMaterial)

            half _ShadowFalloff=1;
            half4 _ShadowDirection = (0.6, 1, 0.5, 0.05);
            half _ShadowAlpha=0.5;

            CBUFFER_END

            float3 ShadowProjectPos(float4 vertPos)
            {
                float3 shadowPos;
                float3 worldPos = TransformObjectToWorld(vertPos).xyz;
                half3 lightDir = normalize(_ShadowDirection.xyz);

                shadowPos.xz = worldPos.xz - lightDir.xz * max(0.0 , worldPos.y - _ShadowDirection.w ) / lightDir.y;
                //shadowPos.y = min(worldPos.y , _ShadowDirection.w);
                shadowPos.y = _ShadowDirection.w;

                return shadowPos;
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 shadowPos = ShadowProjectPos(v.vertex);
                o.pos = TransformWorldToHClip(shadowPos);

                float3 center = float3(unity_ObjectToWorld[0].w , _ShadowDirection.w , unity_ObjectToWorld[2].w);
                half falloff = 1-saturate(pow(length(shadowPos - center) * _ShadowAlpha,_ShadowFalloff));

                o.color = half4(0.01f, 0.02f, 0.1f, 0.99f);
                o.color.a *= saturate(falloff);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 finalCol = i.color;
                //return i.pos.z;
                return finalCol;
            }

            ENDHLSL
        }

    }
}
