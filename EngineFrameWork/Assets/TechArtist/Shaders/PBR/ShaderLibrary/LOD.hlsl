////////////////////////////////////////////////
//LOD LEVEL/////////////////////////////
////////////////////////////////////////////////

#if defined (QUALITY_HIGH)
    #define _NORMALMAP 1

    //#undef _MAIN_LIGHT_SHADOWS
    //#undef _MAIN_LIGHT_SHADOWS_CASCADE
    #define _ADDITIONAL_LIGHTS 1
    #undef _ADDITIONAL_LIGHTS_VERTEX
    //#undef _ADDITIONAL_LIGHT_SHADOWS
    #define _SHADOWS_SOFT 1
    //#define LOD_FADE_CROSSFADE 1
    #define LIGHTING_BRDF LightingDirectBRDF1
    #define LIGHTING_ADDITIONAL LightingAdditionalBRDF1
    #undef UNITY_STANDARD_SIMPLE
    #define LightingDirectGSAA LightingDirectGSAA1
    #define  GIVERTEX GetBakedGIH
    #define  GIFRAG GetBakedGI
    #undef MATCAP_ACCURATE

#elif defined (QUALITY_MEDIUM)

    #define _NORMALMAP 1
    //#undef _MAIN_LIGHT_SHADOWS_CASCADE
    #define _ADDITIONAL_LIGHTS 1
    #undef _ADDITIONAL_LIGHTS_VERTEX
   //#undef _ADDITIONAL_LIGHT_SHADOWS
    #undef _SHADOWS_SOFT
    //#undef LOD_FADE_CROSSFADE
    #define LIGHTING_BRDF LightingDirectBRDF2
    #define LIGHTING_ADDITIONAL LightingAdditionalBRDF2
    #undef UNITY_STANDARD_SIMPLE
    #define LightingDirectGSAA LightingDirectGSAA2
    #define GIVERTEX GetBakedGISH
    #define  GIFRAG GetBakedGIL
    #undef MATCAP_ACCURATE

#elif defined (QUALITY_LOW)

    #define _NORMALMAP 1
    //#undef _MAIN_LIGHT_SHADOWS_CASCADE
    #undef _ADDITIONAL_LIGHTS
    #define _ADDITIONAL_LIGHTS_VERTEX 1
    //#undef _ADDITIONAL_LIGHT_SHADOWS
    #undef _SHADOWS_SOFT
    //#undef LOD_FADE_CROSSFADE
    #undef DETAIL_MICRO
    #undef PLANAR_REFLECTION
    #undef GSAA_ON
    #define LIGHTING_BRDF LightingDirectBRDF3
    #define LIGHTING_ADDITIONAL LightingAdditionalBRDF3
    #undef UNITY_STANDARD_SIMPLE
    //#undef GSAA_ON
    #define GIVERTEX GetBakedGISH
    #define  GIFRAG GetBakedGIL

#else //defined (QUALITY_LOWEST)

    #define _NORMALMAP 1
    //#undef _MAIN_LIGHT_SHADOWS_CASCADE
    #undef _ADDITIONAL_LIGHTS
    #define _ADDITIONAL_LIGHTS_VERTEX 1
    //#undef _ADDITIONAL_LIGHT_SHADOWS
    #undef _SHADOWS_SOFT
    #undef LOD_FADE_CROSSFADE
    #undef DETAIL_MICRO
    #undef PLANAR_REFLECTION
    #undef GSAA_ON
    #define LIGHTING_BRDF LightingDirectBRDF3
    #define LIGHTING_ADDITIONAL LightingAdditionalBRDF3
    #define UNITY_STANDARD_SIMPLE 1
    #define GIVERTEX GetBakedGISH
    #define  GIFRAG GetBakedGIL

#endif
