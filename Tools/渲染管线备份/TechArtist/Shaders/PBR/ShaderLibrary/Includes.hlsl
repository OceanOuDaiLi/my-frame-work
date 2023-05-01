/////////////////////////////////////////////////
// UNITY KEYWORD CONFIGURATION //////////////////
/////////////////////////////////////////////////
// #define _SHADOWS_SOFT
// #define _MAIN_LIGHT_SHADOWS
#include "LOD.hlsl"

/////////////////////////////////////////////////
// SHADER DEPENDENCIES //////////////////////////
/////////////////////////////////////////////////
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

#include "Attributes.hlsl"
#include "Sampler.hlsl"
#include "Common.hlsl"
#include "Shading.hlsl"
#include "Defines.hlsl"
#include "MultiFunction.hlsl"
#include "ColorGrading.hlsl"