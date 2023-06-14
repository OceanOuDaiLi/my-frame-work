#ifndef UNIVERSAL_FTX_INPUT_INCLUDED
#define UNIVERSAL_FTX_INPUT_INCLUDED

struct SkinSurfaceData
{
    half3 albedo;
    //half3 specular;
    half  metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;

    half thickness;
};

struct FTXSurfaceData
{
    half3 albedo;
    half  metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;
};

#endif
