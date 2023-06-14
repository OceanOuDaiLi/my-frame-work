#ifndef UNIVERSAL_FTX_BAKE_INDIRECT_INPUT_INCLUDED
#define UNIVERSAL_FTX_BAKE_INDIRECT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"


// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseColor;

half _Cutoff;

half _Surface;

half _Metallic;
half _Smoothness;
half4 _EmissionColor;
CBUFFER_END

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
