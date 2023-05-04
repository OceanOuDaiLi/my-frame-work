/////////////////////////////////////////////////
// TEXTURE COORDINATES //////////////////////////
/////////////////////////////////////////////////
#if defined(LIGHTMAP_ON)
    #define OUTPUT_TEXCOORD0(input, output, scale)                                          \
        float2 uv0 = input.texcoord0 * scale;                                               \
        float2 uv1 = input.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;           \
        output.uv0 = float4 (uv0, uv1);
#else
    #define OUTPUT_TEXCOORD0(input, output, scale)                                          \
        output.uv0 = input.texcoord0 * scale;
#endif

//#if defined(IN_GAME)
    #define OUTPUT_TEXCOORD2(input, output)                                                 \
        float2 uv2 = input.texcoord2;                                                       \
        output.uv2 = uv2;
//#else
//    #define OUTPUT_TEXCOORD2(input, output)
//#endif

/////////////////////////////////////////////////
// PROJECTION COORDINATES ///////////////////////
/////////////////////////////////////////////////
//output.projection = ComputeScreenPos(positionCS);

#if defined(SCREEN_COORD)
    #define OUTPUT_SCREENCOORD(input, output, positionCS)                                           \
        output.screenCoord = ComputeScreenPos(positionCS);
#else
    #define OUTPUT_SCREENCOORD(input, output, positionCS)
#endif

/////////////////////////////////////////////////
// ADDITIONAL LIGHT SOURCES /////////////////////
/////////////////////////////////////////////////
#if defined(_ADDITIONAL_LIGHTS)
    #define OUTPUT_VERTEX_LIGHT(output, positionWS, normalWS)
        #define ADDITIONAL_LIGHTS(input, color, btdfData, ao, reflectDir)                               \
            int pixelLightCount = GetAdditionalLightsCount();                                           \
            if(pixelLightCount > 0)                                                                     \
            {                                                                                           \
                for (int i = 0; i < pixelLightCount; i++)                                               \
                {                                                                                       \
                    Light light = GetAdditionalLight(i, input.positionWS.xyz);                          \
                    DotProductsSimple dotProducts = GetDotProductsSimple(light, normalWS, viewDirWS);   \
                    color += LIGHTING_ADDITIONAL(btdfData, dotProducts, light, ao, reflectDir);         \
                }                                                                                       \
            }                                                                                           \
    /*#define HAIR_ADDITIONAL_LIGHTS(input, color, btdfData, ao,anisoD)                               \
         int pixelLightCount = GetAdditionalLightsCount();                                          \
        for(int i = 0; i < pixelLightCount; i++)                                                    \
        {                                                                                           \
            Light light = GetAdditionalLight(i, input.positionWS);                                  \
            DotProductsSimple dotProducts = GetDotProductsSimple(light, normalWS, viewDirWS);       \
            color += LightingAdditional(btdfData, dotProducts, light, ao,anisoD);                   \
        }*/
#elif defined(_ADDITIONAL_LIGHTS_VERTEX)
    #define OUTPUT_VERTEX_LIGHT(output, positionWS, normalWS)                                       \
        output.vertexLight = VertexLighting(positionWS, normalWS);
    #define ADDITIONAL_LIGHTS(input, color, btdfData, ao, reflectDir)                               \
        color += input.vertexLight * btdfData.diffuse;
    #define HAIR_ADDITIONAL_LIGHTS(input, color, btdfData, ao,anisoD)                               \
        color += input.vertexLight * btdfData.diffuse;
#else
    #define OUTPUT_VERTEX_LIGHT(output, positionWS, normalWS)
    #define ADDITIONAL_LIGHTS(input, color, btdfData, ao, reflectDir)
    #define HAIR_ADDITIONAL_LIGHTS(input, color, btdfData, ao,anisoD)
#endif

/////////////////////////////////////////////////
// SHADOW COORDINATES ///////////////////////////
/////////////////////////////////////////////////
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    #define OUTPUT_SHADOWCOORD(output, positionWS)                                  \
        output.shadowCoord = TransformWorldToShadowCoord(positionWS);
    #define CALCULATE_SHADOWCOORD(input)                                            \
        float4 shadowCoord = input.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    #define OUTPUT_SHADOWCOORD(output, positionWS)
    #define CALCULATE_SHADOWCOORD(input)                                            \
        float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#else
    #define OUTPUT_SHADOWCOORD(output, positionWS)
    #define CALCULATE_SHADOWCOORD(input)
        float4 shadowCoord = float4(0, 0, 0, 0);
#endif

/////////////////////////////////////////////////
// NORMAL AND VIEW VECTORS //////////////////////
/////////////////////////////////////////////////
#if defined(TANGENTSPACE)
    #define OUTPUT_NORMAL(input, output, normalWS, viewDirWS)                       \
        float  sign        = input.tangentOS.w * GetOddNegativeScale();             \
        float3 tangentWS   = TransformObjectToWorldDir(input.tangentOS.xyz);        \
        float3 bitangentWS = cross(normalWS, tangentWS) * sign;                     \
        output.normalWS    = half4(normalWS, viewDirWS.x);                          \
        output.tangentWS   = half4(tangentWS, viewDirWS.y);                         \
        output.bitangentWS = half4(bitangentWS, viewDirWS.z);
#else
    #define OUTPUT_NORMAL(input, output, normalWS, viewDirWS)                       \
        output.normalWS    = normalWS;                                              \
        output.viewDirWS   = viewDirWS;
#endif

#if defined(VIR_LIT)
    #define OUTPUT_VIRATTEN(positionWS)                                             \
        //float3 positionOS = TransformWorldToObject(positionWS);                     \
        //half virAtten = max(0, pow((1 - positionOS.x), _LitAttenContrast) * _LitAttenInt - _LitAttenSub);
#else
    #define OUTPUT_VIRATTEN(positionWS)
        half virAtten = 1;
#endif

#define SETUP_SHADING_DATA(input)                                                   \
    CALCULATE_SHADOWCOORD(input);                                                   \
    OUTPUT_VIRATTEN(input.positionWS);                                              \
    half3 viewDirWS  = GetViewDir(input);                                           \
    viewDirWS = normalize(viewDirWS);                                               \
    half4 shadowMask = SAMPLE_SHADOWMASK(input.uv0.zw);                             \
    Light mainLight = GetMainLight(shadowCoord, input.positionWS.xyz, shadowMask);     \

/////////////////////////////////////////////////
// Vertex Color /////////////////////////////////
/////////////////////////////////////////////////
#if defined(VERTEX_COLOR)
    #define OUTPUT_VERTEXCOLOR(input, output)                                       \
    output.color0 = input.color;
#else
    #define OUTPUT_VERTEXCOLOR(input, output)
#endif

#define OUTPUT_FOGFACTOR(output, positionCS)            \
        output.positionWS.w = ComputeFogFactor(positionCS.z);
/////////////////////////////////////////////////
// DEFAULT ATTRIBUTES AND VARYINGS //////////////
/////////////////////////////////////////////////
#define OUTPUT_DEFAULT(input, output, positionWS, positionCS, normalWS, viewDirWS)  \
    output.positionWS.xyz = positionWS;                                                 \
    output.positionCS = positionCS;                                                 \
    OUTPUT_VERTEX_LIGHT(output, positionWS, normalWS);                              \
    OUTPUT_NORMAL(input, output, normalWS, viewDirWS);                              \
    OUTPUT_SCREENCOORD(input, output, positionCS);                                  \
    OUTPUT_SHADOWCOORD(output, positionWS);                                         \
    OUTPUT_VERTEXCOLOR(input, output);                                              \
    OUTPUT_FOGFACTOR(output, positionCS);


#define OUTPUT_VERTSIMPLE(input)                                                        \
    VaryingsSimple output = (VaryingsSimple)0;                                          \
    UNITY_SETUP_INSTANCE_ID(input);                                                     \
    UNITY_TRANSFER_INSTANCE_ID(input, output);                                          \
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);   \
    output.vertex = vertexInput.positionCS;                                             \
    output.uv = input.uv * _UVScale;                                                    \
    output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

#define OUTPUT_FRAGSIMPLE(input)                                                        \
    UNITY_SETUP_INSTANCE_ID(input);                                                     \
    half2 uv = input.uv;                                                                \
    half4 texColor = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, uv);               \
    half3 color = texColor.rgb * _AlbedoColor.rgb;                                      \
    half alpha = texColor.a * _AlbedoColor.a;                                           \
    AlphaDiscard(alpha, _Cutoff);                                                       \
    AlphaMulitiply(color, alpha);                                                       \
    color = MixFog(color, input.fogCoord);

