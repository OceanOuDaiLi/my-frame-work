#ifndef NBA_CHARACTER_INCLUDE
    #define NBA_CHARACTER_INCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
    struct appdata
    {
        float4 vertex	: POSITION;
        float4 color		: COLOR;
        float2 uv		: TEXCOORD0;
        float3 normal : NORMAL;
        float2 uvAdd	    : TEXCOORD2;
    #if _NORMAL_ON
        float4 tangent:TANGENT;
    #endif
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct v2f
    {
        float4 vertexCol	: COLOR;
        float4 pos		: SV_POSITION;
        float4 uvMain	: TEXCOORD0;
        float4 uvSub		: TEXCOORD1;
        float2 uvAdd	    : TEXCOORD2;
        float3 L        : TEXCOORD3;
		float3 N        : TEXCOORD4;
		float3 V        : TEXCOORD5;
        float4 worldPosition : TEXCOORD6;
    #if _NORMAL_ON
        half2 uvNor:TEXCOORD7;
        half3 tangentWS     : TEXCOORD8;
        half3 bitangentWS   : TEXCOORD9;
    #endif
        /*#if defined(LGAME_USEFOW) && _FOW_ON
            float2 fowuv	: TEXCOORD2;
        #endif*/
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    //
    /*UNITY_INSTANCING_BUFFER_START(EffetcFow)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _FogCol)
    #   define B_FogCol UNITY_ACCESS_INSTANCED_PROP(Fow,_FogCol)
        UNITY_DEFINE_INSTANCED_PROP(float, _FowBlend)
    #   define B_FowBlend UNITY_ACCESS_INSTANCED_PROP(Fow,_FowBlend)
    UNITY_INSTANCING_BUFFER_END(Fow)*/
    //

    UNITY_INSTANCING_BUFFER_START(EffetcOption)
        UNITY_DEFINE_INSTANCED_PROP(float, _AlphaCtrl)
    #   define B_AlphaCtrl UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_AlphaCtrl)
            UNITY_DEFINE_INSTANCED_PROP(float, _fresnelScale)
    #   define B_fresnelScale UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_fresnelScale)
            UNITY_DEFINE_INSTANCED_PROP(float, _fresnelIndensity)
    #   define B_fresnelIndensity UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_fresnelIndensity)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _fresnelColor)
    #   define B_fresnelColor UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_fresnelColor)

        UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
    #   define B_Color UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_Color)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _OffsetColor)
    /*#   define B_OffsetColor UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_OffsetColor)
        UNITY_DEFINE_INSTANCED_PROP(float, _OffsetColorLerp)
    #   define B_OffsetColorLerp UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_OffsetColorLerp)*/
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _MainWrapMode)
    #	define B_MainWrapMode UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_MainWrapMode)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _AddWrapMode)
    #	define B_AddWrapMode UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_AddWrapMode)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _SubWrapMode)
    #	define B_SubWrapMode UNITY_ACCESS_INSTANCED_PROP(EffetcOption,_SubWrapMode)
    UNITY_INSTANCING_BUFFER_END(EffetcOption)
    //
    UNITY_INSTANCING_BUFFER_START(EffectMain)
        UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
    #   define B_MainTex_ST UNITY_ACCESS_INSTANCED_PROP(EffectMain,_MainTex_ST)
        UNITY_DEFINE_INSTANCED_PROP(float4, _MainTexTransform)
    #	define B_MainTexTransform UNITY_ACCESS_INSTANCED_PROP(EffectMain,_MainTexTransform)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _MainTexUvMode)
    #	define B_MainTexUvMode UNITY_ACCESS_INSTANCED_PROP(EffectMain,_MainTexUvMode)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _ScaleOnCenter)
    #	define B_ScaleOnCenter UNITY_ACCESS_INSTANCED_PROP(EffectMain,_ScaleOnCenter)
    UNITY_INSTANCING_BUFFER_END(EffectMain)
        //
   UNITY_INSTANCING_BUFFER_START(EffectNormal)
        UNITY_DEFINE_INSTANCED_PROP(float4, _NormalMap_ST)
    #   define B_NormalMap_ST UNITY_ACCESS_INSTANCED_PROP(EffectNormal,_NormalMap_ST)
        UNITY_DEFINE_INSTANCED_PROP(float4, _NormalMapTransform)
    #	define B_NormalMapTransform UNITY_ACCESS_INSTANCED_PROP(EffectNormal,_NormalMapTransform)
    UNITY_INSTANCING_BUFFER_END(EffectNormal)
    //
    UNITY_INSTANCING_BUFFER_START(EffectAdd)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _TinctColor)
    #   define B_TinctColor UNITY_ACCESS_INSTANCED_PROP(EffectAdd,_TinctColor)
        UNITY_DEFINE_INSTANCED_PROP(float4, _AdditiveTex_ST)
    #   define B_AdditiveTex_ST UNITY_ACCESS_INSTANCED_PROP(EffectAdd,_AdditiveTex_ST)
        UNITY_DEFINE_INSTANCED_PROP(float4, _AdditiveTexTransform)
    #   define B_AdditiveTexTransform UNITY_ACCESS_INSTANCED_PROP(EffectAdd,_AdditiveTexTransform)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _AdditiveTexUvMode)
    #	define B_AdditiveTexUvMode UNITY_ACCESS_INSTANCED_PROP(EffectAdd,_AdditiveTexUvMode)
        UNITY_DEFINE_INSTANCED_PROP(float, _AdditiveIntensity)
    #	define B_AdditiveIntensity UNITY_ACCESS_INSTANCED_PROP(EffectAdd,_AdditiveIntensity)
    UNITY_INSTANCING_BUFFER_END(EffectAdd)
    //
    UNITY_INSTANCING_BUFFER_START(EffectMask)
        UNITY_DEFINE_INSTANCED_PROP(float4, _MaskTex_ST)
    #	define B_MaskTex_ST UNITY_ACCESS_INSTANCED_PROP(EffectMask,_MaskTex_ST)
        UNITY_DEFINE_INSTANCED_PROP(float4, _MaskTexTransform)
    #	define B_MaskTexTransform UNITY_ACCESS_INSTANCED_PROP(EffectMask,_MaskTexTransform)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _MaskTexUvMode)
    #	define B_MaskTexUvMode UNITY_ACCESS_INSTANCED_PROP(EffectMask,_MaskTexUvMode)
    UNITY_INSTANCING_BUFFER_END(EffectMask)
    //
    UNITY_INSTANCING_BUFFER_START(EffectDissolve)
        UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveTex_ST)
    #	define B_DissolveTex_ST UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveTex_ST)
        UNITY_DEFINE_INSTANCED_PROP(float, _DissolveValue)
    #	define B_DissolveValue UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveValue)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _DissolveTexUvMode)
    #	define B_DissolveTexUvMode UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveTexUvMode)
        UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveTexTransform)
    #	define B_DissolveTexTransform UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveTexTransform)
        UNITY_DEFINE_INSTANCED_PROP(float, _DissolveRangeSize)
    #	define B_DissolveRangeSize UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveRangeSize)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _DissolveRangeCol)
    #	define B_DissolveRangeCol UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_DissolveRangeCol)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _ContouristColorA)
    #	define B_ContouristColorA UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_ContouristColorA)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _ContouristColorB)
    #	define B_ContouristColorB UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_ContouristColorB)
            UNITY_DEFINE_INSTANCED_PROP(float, _EdgeRange)
    #	define B_EdgeRange UNITY_ACCESS_INSTANCED_PROP(EffectDissolve,_EdgeRange)
    UNITY_INSTANCING_BUFFER_END(EffectDissolve)
    //
    UNITY_INSTANCING_BUFFER_START(EffectWarp)
        UNITY_DEFINE_INSTANCED_PROP(float4, _WarpTex_ST)
    #	define B_WarpTex_ST UNITY_ACCESS_INSTANCED_PROP(EffectWarp,_WarpTex_ST)
        UNITY_DEFINE_INSTANCED_PROP(float4, _WarpTexTransform)
    #	define B_WarpTexTransform UNITY_ACCESS_INSTANCED_PROP(EffectWarp,_WarpTexTransform)
        UNITY_DEFINE_INSTANCED_PROP(float, _WarpIntensity)
    #	define B_WarpIntensity UNITY_ACCESS_INSTANCED_PROP(EffectWarp,_WarpIntensity)
        UNITY_DEFINE_INSTANCED_PROP(fixed, _WarpTexUvMode)
    #	define B_WarpTexUvMode UNITY_ACCESS_INSTANCED_PROP(EffectWarp,_WarpTexUvMode)
    UNITY_INSTANCING_BUFFER_END(EffectWarp)

    UNITY_INSTANCING_BUFFER_START(EffectContourist)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _CeilColorA)
    #	define B__CeilColorA UNITY_ACCESS_INSTANCED_PROP(EffectContourist,_CeilColorA)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _CeilColorB)
    #	define B__CeilColorB UNITY_ACCESS_INSTANCED_PROP(EffectContourist,_CeilColorB)
        UNITY_DEFINE_INSTANCED_PROP(float, _ContouristPower)
    #	define B_ContouristPower UNITY_ACCESS_INSTANCED_PROP(EffectContourist,_ContouristPower)
    UNITY_INSTANCING_BUFFER_END(EffectContourist)

    //
    sampler2D   _MainTex;
    sampler2D   _NormalMap;
    sampler2D   _AdditiveTex;
    sampler2D	_MaskTex;
    sampler2D	_DissolveTex;
    sampler2D	_WarpTex;
    float4		_BillboardMatrix0;
    float4		_BillboardMatrix1;
    float4		_BillboardMatrix2;
    float4      _ClipRect;
    float4      _UnscaledTime;
    float4      _LightPos;
    half4       _LightColor;
    /*sampler2D	_FOWTexture;
    sampler2D	_FOWLastTexture;
    float4		_FOWParam;	*/
    inline float2 RotateUV(float2 uv,float2 uvRotate)
    {
        float2 outUV;
        outUV = uv - 0.5 * B_ScaleOnCenter;
        outUV = float2(	outUV.x * uvRotate.y - outUV.y * uvRotate.x ,
                        outUV.x * uvRotate.x + outUV.y * uvRotate.y );
        return outUV + 0.5 * B_ScaleOnCenter;
    }
    inline float2 TransFormUV(float2 argUV,float4 argST , float4 trans)
    {
        float2 result =  RotateUV(argUV , trans.zw)  * argST.xy + argST.zw;
        result += B_ScaleOnCenter * (1 - argST.xy)*0.5;
        return result + frac(trans.xy * _UnscaledTime.y);
    }
    inline float2 ScreenUV(float4 pos)
    {
        float4 srcPos = ComputeScreenPos(pos);
        return srcPos.xy /srcPos.w;
    }
    inline fixed4 CeilColor(fixed powValue,fixed powControl)
    {
	    fixed4 color = lerp(B__CeilColorA,B__CeilColorB,saturate(ceil(pow(powValue,B_ContouristPower * powControl))));
	    return color;
    }
    v2f vert (appdata v)
    {
        v2f o;

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_TRANSFER_INSTANCE_ID(v, o);

        #if _BILLBOARD_ON
            float3 center =  UnityWorldToViewPos(unity_ObjectToWorld._14_24_34) ;
            float3x3 m ;
            m[0] = _BillboardMatrix0.xyz;
            m[1] = _BillboardMatrix1.xyz;
            m[2] = _BillboardMatrix2.xyz;
            o.pos = mul(UNITY_MATRIX_P , float4(mul(m,v.vertex.xyz) + center, 1) );
        #else
            o.pos = UnityObjectToClipPos(v.vertex);
            //将法线转到世界坐标
			o.N = mul(v.normal, (float3x3)unity_WorldToObject);
            //获取世界坐标的光向量
			o.L = WorldSpaceLightDir(v.vertex);
			//获取世界坐标的视角向量
			o.V = WorldSpaceViewDir(v.vertex);

        #endif
        float2 uvScr = ScreenUV(o.pos);
        //mainTex UV Transfrom
        #if _SCREENUV_ON
             o.uvMain.xy = any(B_MainTexUvMode) ? uvScr : v.uv;
             o.uvAdd.xy = any(B_AdditiveTexUvMode) ? uvScr : v.uv;
             o.uvMain.zw = any(B_MaskTexUvMode) ? uvScr : v.uv;
             o.uvSub.xy	 = any(B_DissolveTexUvMode) ? uvScr : v.uv;
             o.uvSub.zw	 = any(B_WarpTexUvMode) ? uvScr : v.uv;
        #else
             o.uvMain = v.uv.xyxy;
             o.uvSub = v.uv.xyxy;
             o.uvAdd = v.uv.xyxy;
        #endif
        o.uvMain.xy = TransFormUV(o.uvMain.xy, B_MainTex_ST, B_MainTexTransform);
        #if _ADDITIVETEX_ON
        o.uvAdd.xy = TransFormUV(o.uvAdd.xy, B_AdditiveTex_ST, B_AdditiveTexTransform);
        #endif
        #if _MASK_ON
            o.uvMain.zw = TransFormUV(o.uvMain.zw, B_MaskTex_ST, B_MaskTexTransform);
        #endif
        #if _DISSOLVE_ON
            o.uvSub.xy	= TransFormUV(o.uvSub.xy , B_DissolveTex_ST , B_DissolveTexTransform);
        #endif
        #if	_WARP_ON
            o.uvSub.zw	= TransFormUV(o.uvSub.zw, B_WarpTex_ST, B_WarpTexTransform);
        #endif

        #if _NORMAL_ON
            o.uvNor = TransFormUV(o.uvMain.xy, B_NormalMap_ST, B_NormalMapTransform);
            o.tangentWS = mul(v.tangent.xyz, (float3x3)unity_WorldToObject);
            o.bitangentWS = cross(normalize(o.N), normalize(o.tangentWS)) * v.tangent.w; 
        #endif

        o.vertexCol.rgb = v.color.rgb;
        o.vertexCol.a = v.color.a;
        o.worldPosition = v.vertex;
        return o;
    }
    fixed4 frag (v2f i) : SV_Target
    {
        	float3 N = normalize(i.N);
			float3 L = normalize(i.L);
			float3 V = normalize(i.V);
            float4 col;
        UNITY_SETUP_INSTANCE_ID(i);
        #if	_WRAPMODE_ON
            float4 uvSub = lerp(saturate(i.uvSub) , frac(i.uvSub) , B_SubWrapMode);
            float2 uvdissolveTex = uvSub.xy;
            float2 uvWarpTex = uvSub.zw;
        #   if	_WARP_ON
                fixed2 warpTex = UnpackNormal(tex2D(_WarpTex, uvWarpTex, float2(0, 0), float2(0, 0))).xy;
                i.uvMain.xy -= warpTex * B_WarpIntensity;
        #   endif
            float4 uvMain = lerp(saturate(i.uvMain) , frac(i.uvMain) , B_MainWrapMode);
            float2 uvAddTex = lerp(saturate(i.uvAdd.xy) , frac(i.uvAdd.xy) , B_AddWrapMode);
            float2 uvMainTex = uvMain.xy;
            float2 uvMask = uvMain.zw;
        #else
            float2 uvMainTex = i.uvMain.xy;
            float2 uvAddTex = i.uvAdd.xy;
            float2 uvMask = i.uvMain.zw;
            float2 uvdissolveTex = i.uvSub.xy;
            float2 uvWarpTex = i.uvSub.zw;
            #if	_WARP_ON
                fixed2 warpTex = UnpackNormal(tex2D(_WarpTex, uvWarpTex, float2(0, 0), float2(0, 0))).xy;
                uvMainTex -= warpTex * B_WarpIntensity;
            #endif
        #endif
        col = tex2D(_MainTex, uvMainTex, float2(0, 0), float2(0, 0)) * B_Color;
        #if _NORMAL_ON
             half3 normal = UnpackNormal(tex2D(_NormalMap, i.uvNor, float2(0, 0), float2(0, 0)));
             normal = mul(normalize(normal), float3x3(i.tangentWS, i.bitangentWS, i.N));
             float NdotL = max(0, dot(normalize(_LightPos.xyz / 5), normal));
            col += NdotL * _LightColor / 10 * _LightPos.w;
        #endif
        #if _ADDITIVETEX_ON
        fixed4 tinct_var = tex2D(_AdditiveTex, uvAddTex) * B_TinctColor * B_AdditiveIntensity;
        col.rgb = col.rgb + tinct_var.rgb;
        col.a = col.a + tinct_var.a;

        #endif

        #if _MASK_ON
            fixed mask = tex2D(_MaskTex, uvMask, float2(0, 0), float2(0, 0)).r ;
            col.a *= mask;
        #endif

        #if	_DISSOLVE_ON
            fixed dissolveTex = tex2D(_DissolveTex, uvdissolveTex, float2(0, 0), float2(0, 0)).r;
            float disValue = B_DissolveValue * 2 -0.5;
            #if _DISSOLVECOLOR_ON
            fixed dissolve =  smoothstep(disValue - B_DissolveRangeSize,disValue + B_DissolveRangeCol, dissolveTex);
            fixed ClipValue = dissolveTex + B_DissolveValue * (-1.2) + 0.1;
            clip(ClipValue);
            fixed degree = saturate(ClipValue/B_EdgeRange);
            fixed4 edgeColor = lerp(B_ContouristColorA, B_ContouristColorB, degree);

            fixed4 color = lerp(edgeColor ,col ,degree);
            col.rgb = color.rgb;
            col.a *= dissolve;

            #else
            fixed dissolve =  smoothstep(disValue - B_DissolveRangeSize,disValue + B_DissolveRangeCol, dissolveTex);
            fixed4 rangeCol	= (1- dissolve) * B_DissolveRangeCol * dissolve ;
            col.rgb = lerp(B_DissolveRangeCol.rgb ,col.rgb ,dissolve) * dissolve;
            col.a *= dissolve;
            #endif
        #endif

        //菲涅尔
        float fresnel = B_fresnelScale * pow(1-dot(N, V) , max(B_fresnelIndensity, 1e-5));

        col*= i.vertexCol;
        col.rgb *= col.a;
        col.rgb += lerp(col.rgb, _fresnelColor, col.a*fresnel);

        #if _CONTOURIST_ON
            fixed posValue = col.r;
		    fixed4 main_var = CeilColor(posValue,1);
            fixed test = i.uvMain.w;
            fixed4 ContouristColor = main_var;
            col.rgb = ContouristColor.rgb;
            //col.a = color.a;
        #endif

        #ifdef UNITY_UI_CLIP_RECT
        float rectAlpha = UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
        return col * B_AlphaCtrl * rectAlpha;
        #endif

        #ifdef UNITY_UI_ALPHACLIP
        clip (col.a - 0.001);
        #endif

        return col * B_AlphaCtrl;

        /*#if defined(LGAME_USEFOW) && _FOW_ON
            fixed fowTex = fixed(tex2D(_FOWTexture, i.fowuv).r);
            fixed fowLast = fixed(tex2D(_FOWLastTexture, i.fowuv).r);
            col *= 1 - lerp(fowLast, fowTex, B_FowBlend);
        #endif*/

    }
#endif