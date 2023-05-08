// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility
// UNITY_SHADER_NO_UPGRADE

#ifndef _VOLUMETRIC_LIGHT_BEAM_SHARED_INCLUDED_
#define _VOLUMETRIC_LIGHT_BEAM_SHARED_INCLUDED_

#include "UnityCG.cginc"


//#define DEBUG_SHOW_DEPTH 1
//#define DEBUG_SHOW_NOISE3D 1
//#define DEBUG_BLEND_INSIDE_OUTSIDE 1

#if DEBUG_SHOW_DEPTH && !VLB_DEPTH_BLEND
#define VLB_DEPTH_BLEND 1
#endif

#if DEBUG_SHOW_NOISE3D && !VLB_NOISE_3D
#define VLB_NOISE_3D 1
#endif


#if UNITY_VERSION < 540
#define matWorldToObject _World2Object
#define matObjectToWorld _Object2World
inline float4 UnityObjectToClipPos(in float3 pos) { return mul(UNITY_MATRIX_MVP, float4(pos, 1.0)); }
inline float3 UnityObjectToViewPos(in float3 pos) { return mul(UNITY_MATRIX_MV, float4(pos, 1.0)).xyz; }
#else
#define matWorldToObject unity_WorldToObject
#define matObjectToWorld unity_ObjectToWorld
#endif

inline float3 UnityWorldToObjectPos(in float3 pos) { return mul(matWorldToObject, float4(pos, 1.0)).xyz; }
inline float3 UnityObjectToWorldPos(in float3 pos) { return mul(matObjectToWorld, float4(pos, 1.0)).xyz; }

#if VLB_DEPTH_BLEND
#ifndef UNITY_DECLARE_DEPTH_TEXTURE // handle Unity pre 5.6.0
#define UNITY_DECLARE_DEPTH_TEXTURE(tex) sampler2D_float tex
#endif
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

inline half SampleSceneZ(half4 projPos)
{
    return LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(projPos)));
}

half4 DepthFade_VS_ComputeProjPos(half4 vertex_in, half4 vertex_out)
{
    half4 projPos = ComputeScreenPos(vertex_out);
    projPos.z = -UnityObjectToViewPos(vertex_in).z; // = COMPUTE_EYEDEPTH
    return projPos;
}

half DepthFade_PS_BlendDistance(half4 projPos, half distance)
{
    half sceneZ = max(0, SampleSceneZ(projPos) - _ProjectionParams.g);
    half partZ = max(0, projPos.z - _ProjectionParams.g);
    return saturate((sceneZ - partZ) / distance);
}
#endif

inline half lerpClamped(half a, half b, half t) { return lerp(a, b, saturate(t)); }
inline half invLerp(half a, half b, half t) { return (t - a) / (b - a); }
inline half invLerpClamped(half a, half b, half t) { return saturate(invLerp(a, b, t)); }
inline half fromABtoCD_Clamped(half valueAB, half A, half B, half C, half D)  { return lerpClamped(C, D, invLerpClamped(A, B, valueAB)); }


struct v2f
{
    float4 posClipSpace : SV_POSITION;
    float4 posObjectSpace : TEXCOORD0;
    float4 posWorldSpace : TEXCOORD1;
    float4 posViewSpaceAndIsCap : TEXCOORD2;
    UNITY_FOG_COORDS(3)
#if VLB_DEPTH_BLEND
    float4 projPos : TEXCOORD4;
#endif
#if VLB_NOISE_3D
    float4 uvgrab : TEXCOORD5;
#endif
};

CBUFFER_START(UnityPerMaterial)
uniform half4 _Color;
uniform half _AlphaInside;
uniform half _AlphaOutside;
uniform half2 _ConeSlopeCosSin; // between -1 and +1
uniform half2 _ConeRadius; // x = start radius ; y = end radius
uniform half _ConeApexOffsetZ; // > 0
uniform half _AttenuationLerpLinearQuad;
uniform half _DistanceFadeStart;
uniform half _DistanceFadeEnd;
uniform half _DistanceCamClipping;
uniform half _FresnelPow;
uniform half _GlareFrontal;
uniform half _GlareBehind;
uniform half4 _CameraParams; // xyz: object space forward vector ; w: cameraIsInsideBeamFactor (-1 : +1)

#if VLB_CLIPPING_PLANE
uniform half4 _ClippingPlaneWS;
#endif

#if VLB_DEPTH_BLEND
uniform half _DepthBlendDistance;
#endif

#if VLB_NOISE_3D
uniform sampler3D _VLB_NoiseTex3D;
uniform half4 _VLB_NoiseGlobal;
uniform half4 _NoiseLocal;
uniform half3 _NoiseParam;
#endif

CBUFFER_END


#define CONST_PI 3.14159

v2f vert(appdata_base v)
{
    v2f o;

    // compute the proper cone shape
    float4 vertex = v.vertex;
    vertex.xy *= lerp(_ConeRadius.x, _ConeRadius.y, vertex.z);
    vertex.z *= _DistanceFadeEnd;

    o.posClipSpace = UnityObjectToClipPos(vertex);
    o.posWorldSpace = mul(matObjectToWorld, vertex);

    o.posObjectSpace = vertex;

#if VLB_DEPTH_BLEND
    o.projPos = DepthFade_VS_ComputeProjPos(vertex, o.posClipSpace);
#endif

    float3 posViewSpace = UnityObjectToViewPos(vertex);
    float isCap = 0;//v.texcoord.x > 0.5;
    o.posViewSpaceAndIsCap = float4(posViewSpace, isCap);

#if VLB_NOISE_3D
#if UNITY_UV_STARTS_AT_TOP
    half scaleY = -1.0;
#else
    half scaleY = 1.0;
#endif
    o.uvgrab.xy = (float2(o.posClipSpace.x, o.posClipSpace.y * scaleY) + o.posClipSpace.w) * 0.5;
    o.uvgrab.zw = o.posClipSpace.zw;
#endif

    UNITY_TRANSFER_FOG(o, o.posClipSpace);
    return o;
}


half GetNoise3DFactor(half3 wpos)
{
#if VLB_NOISE_3D
    half intensity = _NoiseParam.x;
    half3 velocity = lerp(_NoiseLocal.xyz, _VLB_NoiseGlobal.xyz, _NoiseParam.y);
    half scale = lerp(_NoiseLocal.w, _VLB_NoiseGlobal.w, _NoiseParam.z);
    half noise = tex3D(_VLB_NoiseTex3D, frac(wpos * scale + (_Time.y * velocity))).a;
    return lerp(1, noise, intensity);
#else
    return 1;
#endif
}

// Get signed distance of pos from the plane (normal ; d).
// Normal should be normalized.
// If we want to disable this feature, we could set normal and d to 0 (no discard in this case).
inline half DistanceToPlane(half3 pos, half3 normal, half d) { return dot(normal, pos) + d; }

half4 fragShared(v2f i, half outsideBeam)
{
#if VLB_CLIPPING_PLANE
    // clipping plane
    half distToClipPlane = DistanceToPlane(i.posWorldSpace.xyz, _ClippingPlaneWS.xyz, _ClippingPlaneWS.w);
    clip(distToClipPlane);
    half clipPlaneAlpha = lerp(1, smoothstep(0, 0.25, distToClipPlane), saturate(sign(distToClipPlane)));
#else
    half clipPlaneAlpha = 1;
#endif

#if DEBUG_SHOW_DEPTH
    return SampleSceneZ(i.projPos) * _ProjectionParams.w;
#endif

#if DEBUG_SHOW_NOISE3D
    return GetNoise3DFactor(i.posWorldSpace);
#endif

    half3 vecCamForwardOSN = _CameraParams.xyz;
    half cameraIsInsideBeamFactor = _CameraParams.w; // (-1 ; 1)

    half cameraIsOrtho = unity_OrthoParams.w; // w is 1.0 when camera is orthographic, 0.0 when perspective
    half3 posViewSpace = i.posViewSpaceAndIsCap.xyz;
    half isCap = 0;// i.posViewSpaceAndIsCap.w;
    half pixDistFromSource = length(i.posObjectSpace.z);

    // Camera Position in Object Space
    half3 camPosObjectSpace = UnityWorldToObjectPos(_WorldSpaceCameraPos);

    // Vector Camera to current Pixel, in object space and normalized
    half3 vecCamToPixOSN = normalize(i.posObjectSpace.xyz - camPosObjectSpace);

    // Deal with ortho camera:
    // With ortho camera, we don't want to change the fresnel according to camera position.
    // So instead of computing the proper vector "Camera to Pixel", we take account of the "Camera Forward" vector (which is not dependant on the pixel position)
    vecCamToPixOSN = lerp(vecCamToPixOSN, vecCamForwardOSN, cameraIsOrtho);

    // Compute normal
    half2 cosSinFlat = normalize(i.posObjectSpace.xy);
    half3 normalObjectSpace = normalize(half3(cosSinFlat.x * _ConeSlopeCosSin.x, cosSinFlat.y * _ConeSlopeCosSin.x, -_ConeSlopeCosSin.y));
    normalObjectSpace *= (outsideBeam * 2 - 1); // = outsideBeam ? 1 : -1;
    normalObjectSpace = lerp(normalObjectSpace, half3(0, 0, -1), isCap);

    // compute Boost factor
    half insideBoostDistance = lerp(0, _DistanceFadeEnd, _GlareFrontal);
    half boostFactor = 1 - smoothstep(0, 0 + insideBoostDistance + 0.001, pixDistFromSource); // 0 = no boost ; 1 = max boost
    boostFactor = lerp(boostFactor, 0, outsideBeam); // no boost for outside pass
    boostFactor = lerp(0, boostFactor, saturate(cameraIsInsideBeamFactor)); // no boost for outside pass
    boostFactor = lerp(boostFactor, 1, isCap); // cap is always at max boost

    // Attenuation
    half distFromSourceNormalized = invLerpClamped(_DistanceFadeStart, _DistanceFadeEnd, pixDistFromSource);
    // Almost simple linear attenuation between Fade Start and Fade End: Use smoothstep for a better fall to zero rendering
    half attLinear = smoothstep(0, 1, 1 - distFromSourceNormalized);
    // Unity's custom quadratic attenuation https://forum.unity.com/threads/light-attentuation-equation.16006/
    half attQuad = 1.0 / (1.0 + 25.0 * distFromSourceNormalized * distFromSourceNormalized);
    const half kAttQuadStartToFallToZero = 0.8;
    attQuad *= saturate(smoothstep(1.0, kAttQuadStartToFallToZero, distFromSourceNormalized)); // Near the light's range (fade end) we fade to 0 (because quadratic formula never falls to 0)
    half attenuation = lerp(attLinear, attQuad, _AttenuationLerpLinearQuad);

    // Noise factor
    half noise3DFactor = GetNoise3DFactor(i.posWorldSpace);
 //   noise3DFactor = lerpClamped(noise3DFactor, 1, attenuation * 0.1);

    // depth blend factor
#if VLB_DEPTH_BLEND
    // we disable blend factor when the pixel is near the light source,
    // to prevent from blending with the light source model geometry (like the flashlight model).
    half depthBlendStartDistFromSource = _DepthBlendDistance;
    half depthBlendDist = _DepthBlendDistance * invLerpClamped(0, depthBlendStartDistFromSource, pixDistFromSource);
    half depthBlendFactor = DepthFade_PS_BlendDistance(i.projPos, depthBlendDist);
    depthBlendFactor = lerp(depthBlendFactor, 1, step(_DepthBlendDistance, 0));
    depthBlendFactor = lerp(depthBlendFactor, 1, cameraIsOrtho); // disable depth BlendState factor with ortho camera (temporary fix)
#else
    half depthBlendFactor = 1;
#endif

    // fade when too close factor
    half distCamClipping = lerp(_DistanceCamClipping, 0, boostFactor); // do not fade according to camera when we are in boost zone, to keep boost effect
    half camFadeDistStart = _ProjectionParams.y; // cam near place
    half camFadeDistEnd = camFadeDistStart + distCamClipping;
    half distCamToPixWS = abs(posViewSpace.z); // only check Z axis (instead of length(posViewSpace.xyz)) to have smoother transition with near plane (which is not curved)
    half fadeWhenTooClose = smoothstep(0, 1, invLerpClamped(camFadeDistStart, camFadeDistEnd, distCamToPixWS));
    fadeWhenTooClose = lerp(fadeWhenTooClose, 1, cameraIsOrtho); // fading according to camera eye position doesn't make sense with ortho camera

    half vecCamToPixDotZ = dot(vecCamToPixOSN, half3(0, 0, 1));
    half factorNearAxisZ = abs(vecCamToPixDotZ);

#if VLB_NOISE_3D
    // disable noise 3D when looking from behind or from inside because it makes the cone shape too much visible
    noise3DFactor = lerp(noise3DFactor, 1, pow(factorNearAxisZ, 10));
#endif

    // fresnel
    half fresnel = 0;
    {
        // real fresnel factor
        half fresnelReal = dot(normalObjectSpace, -vecCamToPixOSN);

        // compute a fresnel factor to support long beams by projecting the viewDir vector
        // on the virtual plane formed by the normal and tangent
        half3 tangentPlaneNormal = normalize(i.posObjectSpace.xyz + half3(0, 0, _ConeApexOffsetZ));
        half distToPlane = dot(-vecCamToPixOSN, tangentPlaneNormal);
        half3 vec2D = normalize(-vecCamToPixOSN - distToPlane * tangentPlaneNormal);
        half fresnelProjOnTangentPlane = dot(normalObjectSpace, vec2D);

        // blend between the 2 fresnels
        fresnel = lerp(fresnelProjOnTangentPlane, fresnelReal, factorNearAxisZ);
    }

    half fresnelPow = _FresnelPow;

    // Lerp the fresnel pow to the glare factor according to how far we are from the axis Z
    const half kMaxGlarePow = 1.5;
    half glareFactor = kMaxGlarePow * (1 - lerp(_GlareFrontal, _GlareBehind, outsideBeam));
    fresnelPow = lerpClamped(fresnelPow, min(fresnelPow, glareFactor), factorNearAxisZ);

    // Pow the fresnel
    fresnel = saturate(fresnel);
    fresnel = smoothstep(0, 1, fresnel);
    fresnel = saturate(pow(fresnel, fresnelPow));

    // Treat Cap a special way
    fresnel = lerp(fresnel, outsideBeam, isCap);
    outsideBeam = lerp(outsideBeam, 1 - outsideBeam, isCap);

    // Boost distance inside
    half boostFresnel = lerpClamped(fresnel, 1 + 0.001, boostFactor);
    fresnel = lerp(boostFresnel, fresnel, outsideBeam); // no boosted fresnel if outside

    // smooth blend between inside and outside geometry depending of View Direction
    //float factorFaceLightSourcePerPixN = saturate(-vecCamToPixDotZ); // transition is too hard, but UnitTest 'Angle' looks better...
    const half kFaceLightSmoothingLimit = 1;
    half factorFaceLightSourcePerPixN = saturate(smoothstep(kFaceLightSmoothingLimit, -kFaceLightSmoothingLimit, vecCamToPixDotZ)); // smoother transition

    half blendInsideWithOutside = lerp(factorFaceLightSourcePerPixN, 1 - factorFaceLightSourcePerPixN, outsideBeam);

#if DEBUG_BLEND_INSIDE_OUTSIDE
    return lerp(half4(1, 0, 0, 1), half4(0, 1, 0, 1), factorFaceLightSourcePerPixN);
#endif

    half intensity = 1
        * clipPlaneAlpha
        * attenuation
        * fadeWhenTooClose
        * depthBlendFactor
        * fresnel
        * blendInsideWithOutside
        * noise3DFactor
    ;

    half4 col = _Color * intensity;
    col.rgb *= _Color.a;
    col.rgb *= lerp(_AlphaInside, _AlphaOutside, outsideBeam);

    UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0, 0, 0, 0)); // since we use this shader with Additive blending, fog color should be treating as black
    return col;

}



#endif
