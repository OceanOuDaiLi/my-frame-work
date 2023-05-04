

/////////////////////////////////////////////////
// FORWARD PASS DATA ////////////////////////////
/////////////////////////////////////////////////
struct Attributes
{
    float3 normalOS     : NORMAL;
    float4 positionOS   : POSITION;
    float4 texcoord0    : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 uv2 : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID

#if defined(VERTEX_COLOR)
    half4 color         : COLOR;
#endif

//#if defined(LIGHTMAP_ON)
//    float2 uv3    : TEXCOORD3;
//#endif

    ////�������õ�
    float2 uv3 : TEXCOORD3;

    
    float2 texcoord2    : TEXCOORD4;

#if defined(TANGENTSPACE)
    float4 tangentOS    : TANGENT;
#endif

#if defined(WATER)
    float4 waterCoord   : TEXCOORD5;
#endif
    
    ////�������õ�
    //float2 uv1 : TEXCOORD5;
    
#if defined(_ZWriteOn)
    float4 ShadowCoord : TEXCOORD6;
#endif
    float2 lightmapUV   : TEXCOORD7;


};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float4 positionWS   : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID

#if defined(VERTEX_COLOR)
    half4  color0       : COLOR0;
#endif

    float2 uvAO         : COLOR1;
#if defined(LIGHTMAP_ON)
    float4 uv0          : TEXCOORD1;
#else
    float4 uv0          : TEXCOORD1;
#endif

#if defined(TANGENTSPACE)
    half4 normalWS      : TEXCOORD2;    // w = viewDirWS.x
    half4 tangentWS     : TEXCOORD3;    // w = viewDirWS.y
    half4 bitangentWS   : TEXCOORD4;    // w = viewDirWS.z
#else
    half3 normalWS      : TEXCOORD2;
    half3 viewDirWS     : TEXCOORD3;
#endif
#if defined(_ADDITIONAL_LIGHTS_VERTEX)
    half3 vertexLight   : TEXCOORD5;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord  : TEXCOORD6;
#endif

#if defined(SCREEN_COORD)
    float4 screenCoord  : TEXCOORD7;
#endif

#if defined(PARALLAX)
    half3 viewDirTS    : TEXCOORD8;
#endif

//#if defined(WATER)
//    float4 waterCoord  : TEXCOORD9;
//#endif
    

//�������õ�
    float4 posOSNormalY : TEXCOORD9;


    float2 uv2          : TEXCOORD10;
    //float2 lightmapUV   : TEXCOORD11;
#if defined(RIM_MASK)
    half4 sh_1         : TEXCOORD11;
#endif
    half3 bakeGISH     : TEXCOORD12;

//#if defined(MATCAP_ACCURATE)
//    half3 tSpace0 : TEXCOORD13;
//    half3 tSpace1 : TEXCOORD14;
//    half3 tSpace2 : TEXCOORD15;
//#endif

    //������Ҫ��
    half3 tSpace0 : TEXCOORD13;
    half3 tSpace1 : TEXCOORD14;
    half3 tSpace2 : TEXCOORD15;

    
    float2 positionOSXY   : TEXCOORD16;
};

/////////////////////////////////////////////////
// SHADOW PASS DATA /////////////////////////////
/////////////////////////////////////////////////
struct AttributesShadow
{
    float3 normalOS     : NORMAL;
    float4 positionOS   : POSITION;
    float2 texcoord     : TEXCOORD0;
#if defined(VERTEX_COLOR)
    half4 color         : COLOR;
#endif
    
    UNITY_VERTEX_INPUT_INSTANCE_ID

};

struct VaryingsShadow
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/////////////////////////////////////////////////
// DEPTH ONLY PASS DATA /////////////////////////
/////////////////////////////////////////////////
struct AttributesDepthOnly
{
    float4 positionOS   : POSITION;
#if defined(VERTEX_COLOR)
    half4 color         : COLOR;
    float3 normalOS     : NORMAL;
#endif
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VaryingsDepthOnly
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/////////////////////////////////////////////////
// DEPTH NORMAL PASS DATA /////////////////////////
/////////////////////////////////////////////////
struct AttributesDepthNormal
{
    float4 positionOS     : POSITION;
    float4 tangentOS      : TANGENT;
    float2 texcoord       : TEXCOORD0;
    float3 normal         : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VaryingsDepthNormal
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD1;
    float3 normalWS     : TEXCOORD2;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/////////////////////////////////////////////////
// UNLIT PASS DATA //////////////////////////////
/////////////////////////////////////////////////
struct AttributesSimple
{
    float4 positionOS       : POSITION;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VaryingsSimple
{
    float2 uv       : TEXCOORD0;
    float fogCoord  : TEXCOORD1;
    float4 vertex : SV_POSITION;
        //matcap
    half3 normalWS  : TEXCOORD2;
    half3 tSpace1 : TEXCOORD3;
    half3 tSpace2 : TEXCOORD4;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};