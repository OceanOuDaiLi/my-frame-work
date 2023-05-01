// /////////////////////////////////////////////////
// /// REFLECTION VECTORS //////////////////////////
// /////////////////////////////////////////////////
half3 GetReflectDir(half3 normalWS, half3 viewDirWS, half3 positionWS)
{
    half3 reflectDir = reflect(-viewDirWS, normalWS);

#if defined (CUBEMAP_BOXPROJECTION)
    reflectDir = BoxProjectedCubemapDirection(reflectDir, positionWS, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
#endif

    return reflectDir;
}

/////////////////////////////////////////////////
/// MATERIAL INPUT PROPERTIES ///////////////////
/////////////////////////////////////////////////
struct Material
{
    half  alpha;         // Alpha value
    half3 albedo;        // Albedo color
    half3 normal;        // Tangent or Object space
    half3 emission;      // Emission color (HDR)
    half  metallic;      // Metallic value
    half  smoothness;    // Smoothness value
    half  transmission;  // Tranmission multiplier
    half  ao;            // Ambient occlusion
};

/////////////////////////////////////////////////
/// SHADING DOT PRODUCTS ////////////////////////
/////////////////////////////////////////////////
struct DotProducts
{
    half NoL;    // Normal and light vector angle
    half NoH;    // Normal and half  vector angle
    half NoV;    // Inverse (one minus) normal and view vector
    half LoH;    // Light and half vector angle
    //half VoH;   // View  and half vector angle
    half LoV;    //Light and View vector angle
    half RawNoH;  //Normal and half  vector angle
    half RawNoL;
    half3 H;
};

struct DotProductsSimple
{
    half NoL;   // Normal and light vector angle
    half NoH;   // Normal and half  vector angle
    half LoH;   // Light and half vector angle
    half NoV;    // Inverse (one minus) normal and view vector
    half3 H;
};

inline DotProducts GetDotProducts(Light light, half3 normalWS, half3 viewDirWS)
{
    half3 h = SafeNormalize(light.direction + viewDirWS);

    DotProducts dotProducts;
    dotProducts.RawNoL = dot(normalWS, light.direction);
    dotProducts.NoL = saturate(dotProducts.RawNoL);
    dotProducts.RawNoH = dot(normalWS, h);
    dotProducts.NoH = saturate(dotProducts.RawNoH);
    //dotProducts.NoV = DotClamped(normalWS, viewDirWS);
    dotProducts.LoH = DotClamped(light.direction, h);
    //dotProducts.VoH = DotClamped(viewDirWS, halfDir);
    dotProducts.NoV = DotAbs(normalWS, viewDirWS);
    dotProducts.LoV = dot(light.direction, viewDirWS);
    dotProducts.H = h;

    // dotProducts.VoH = 1.0h - dotProducts.VoH;
    // dotProducts.NoV = 1.0h - dotProducts.NoV;

    return dotProducts;
}

inline DotProductsSimple GetDotProductsSimple(Light light, half3 normalWS, half3 viewDirWS)
{
    half3 halfDir = SafeNormalize(light.direction + viewDirWS);

    DotProductsSimple dotProducts;
    dotProducts.NoL = DotClamped(normalWS, light.direction);
    dotProducts.NoH = DotClamped(normalWS, halfDir);
    dotProducts.LoH = DotClamped(light.direction, halfDir);
    dotProducts.NoV = DotAbs(normalWS, viewDirWS);
    dotProducts.H = halfDir;
    return dotProducts;
}

/////////////////////////////////////////////////
/// BTDF SHADING DATA ///////////////////////////
/////////////////////////////////////////////////
struct BTDFData
{
    half3 diffuse;              // Diffuse color, modulated with metallic mask
    half3 specular;             // Specular color, modulated with metallic mask
    half3 transmission;         // Amount of light transmitting through surface
    half roughness;             // Roughness squared
    half roughness2;            // Roughness quadratic
    half grazingTerm;           // Indirect specular term for 90 degree reflections
    half normalizationTerm;     // Energy conservation term
    half roughness2MinusOne;    // One minus roughness
    half perceptualRoughness;   // Input roughness, from 1.0 - smoothness
    //half3 HairEmission;
};

inline BTDFData GetBTDFData(Material material, DotProducts dotProducts, inout half alpha)
{
    BTDFData btdfData;

    half smoothness = material.smoothness;

#if defined(SHADING_METALLIC)
    half oneMinusReflectivity = OneMinusReflectivityMetallic(material.metallic);
    half reflectivity = 1.0h - oneMinusReflectivity;

    btdfData.diffuse = material.albedo * oneMinusReflectivity;
    btdfData.specular = lerp(kDieletricSpec.rgb, material.albedo, material.metallic);
    btdfData.grazingTerm = saturate(smoothness + reflectivity);
    btdfData.transmission *= oneMinusReflectivity;
#elif defined(SHADING_GLASS)
    btdfData.diffuse = 0.0h;
    btdfData.specular = material.albedo;
    btdfData.grazingTerm = saturate(smoothness + 1.0h);
#else
    btdfData.diffuse = material.albedo * kDieletricSpec.a;
    btdfData.specular = kDieletricSpec.rgb;
    btdfData.grazingTerm = saturate(smoothness + kDieletricSpec.r);
#endif

    //btdfData.perceptualRoughness = material.roughness;
    btdfData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
    btdfData.roughness = PerceptualRoughnessToRoughness(btdfData.perceptualRoughness);
    btdfData.roughness2 = btdfData.roughness * btdfData.roughness;
    btdfData.roughness2MinusOne = btdfData.roughness2 - 1.0h;
    btdfData.normalizationTerm = btdfData.roughness * 4.0h + 2.0h;

#if defined(ALPHAPREMULTIPLY_ON)
    btdfData.diffuse *= material.alpha;
    alpha = material.alpha * oneMinusReflectivity + reflectivity; // NOTE: alpha modified and propagated up.
#endif

    return btdfData;
}

/////////////////////////////////////////////////
/// PBR LIGHTING CALCULATIONS ///////////////////
/////////////////////////////////////////////////
half3 LightingIndirect(BTDFData btdfData, half3 bakedGI, DotProducts dotProducts, half3 normalWS, half3 reflectDir, half occlusion,  half4 reflection = 0)
{
    half fresnelTerm = Pow5(1 - dotProducts.NoV);
    half specularOcc = saturate(pow(abs(dotProducts.NoV + occlusion), exp2(-16 * btdfData.roughness - 1.0)) - 1.0 + occlusion);

    #if defined(CUBEMAP_REFLECTION)
        half3 indirectSpecular = GlossyEnvironmentReflection(reflectDir, btdfData.perceptualRoughness, occlusion);
    #else
        half3 indirectSpecular = _GlossyEnvironmentColor.rgb;
    #endif

    half3 indirectDiffuse = bakedGI * btdfData.diffuse * occlusion;
    half surfaceReduction = 1.0f / (btdfData.roughness2 + 1.0f);
    indirectSpecular += reflection.rgb;
    indirectSpecular = surfaceReduction * indirectSpecular * lerp(btdfData.specular, btdfData.grazingTerm, fresnelTerm) * specularOcc;

    return indirectDiffuse + indirectSpecular;
}

//MatCap Indirect Lighting
half3 LightingIndirect2(BTDFData btdfData, half3 bakedGI, DotProducts dotProducts, half3 normalWS, half3 reflectDir, half occlusion,  half4 reflection = 0)
{
    half3 indirectDiffuse = bakedGI * btdfData.diffuse;
    return indirectDiffuse;
}


half3 LightingIndirect3(BTDFData btdfData, half3 bakedGI, DotProducts dotProducts, half3 normalWS, half3 reflectDir, half occlusion,  half4 reflection = 0)
{
    // half fresnelTerm = Pow5(1 - dotProducts.NoV);
    // half specularOcc = saturate(pow(dotProducts.NoV + occlusion, exp2(-16 * btdfData.roughness - 1.0)) - 1.0 + occlusion);
    half3 indirectDiffuse = bakedGI * btdfData.diffuse * occlusion;
    half surfaceReduction = 1.0f / (btdfData.roughness2 + 1.0f);
    half3 indirectSpecular = surfaceReduction * reflection.rgb;// * lerp(btdfData.specular, btdfData.grazingTerm, fresnelTerm) * specularOcc;

    return indirectDiffuse + indirectSpecular;
}

//Impostor Indirect Lighting 
half3 LightingIndirectIm(BTDFData btdfData, half3 bakedGI, DotProducts dotProducts, half3 normalWS, half3 reflectDir, half occlusion,half3 specu)
{
    half fresnelTerm = Pow5(1 - dotProducts.NoV);
    
#if defined(CUBEMAP_REFLECTION)
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectDir, btdfData.perceptualRoughness, occlusion);
#else
    half3 indirectSpecular = _GlossyEnvironmentColor.rgb;
#endif

    half3 indirectDiffuse = bakedGI * btdfData.diffuse;
    half surfaceReduction = 1.0f / (btdfData.roughness2 + 1.0f);
    indirectSpecular = surfaceReduction * indirectSpecular * lerp(btdfData.specular, btdfData.grazingTerm, fresnelTerm);

    return indirectDiffuse + saturate(indirectSpecular);
}

half3 LightingDirectBRDF1(BTDFData btdfData, DotProducts dotProducts, Light light, half shadowIntensity = 0,half netLightness = 0, half3 reflectDir = 0)
{

    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten * dotProducts.NoL;
    half diffuseTerm = DisneyDiffuseNoPINBA(dotProducts.NoV, dotProducts.NoL, dotProducts.LoH, btdfData.perceptualRoughness);

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);
    half D = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);

    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);
    //half specularTerm = max(0, V * D);
    half specularTerm = V * D;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
    half3 Direct = diffuseTerm * btdfData.diffuse * radiance + specularTerm * F * lightAtten;

    return Direct
    +lerp(btdfData.diffuse * light.color, 0, light.shadowAttenuation * dotProducts.NoL) * shadowIntensity
    + btdfData.diffuse*(1-diffuseTerm * lightAtten)  * netLightness;
    //return light.shadowAttenuation;

}

//Use Mobile BRDF
half3 LightingDirectBRDF2(BTDFData btdfData, DotProducts dotProducts, Light light, half shadowIntensity = 0, half netLightness = 0, half3 reflectDir = 0)
{
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half3 brdf = btdfData.diffuse;
    half d = dotProducts.NoH * dotProducts.NoH * btdfData.roughness2MinusOne + 1.00001f;
    half LoH2 = dotProducts.LoH * dotProducts.LoH;
    half specularTerm = btdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * btdfData.normalizationTerm);
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
    brdf += specularTerm * btdfData.specular;
    return brdf * radiance
    +lerp(brdf * light.color, 0, light.shadowAttenuation * dotProducts.NoL) * shadowIntensity;
    //BRDF2 + shadowIntensity + netLightness
       //return light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
}

TEXTURE2D(unity_NHxRoughness); SAMPLER(sampler_unity_NHxRoughness);
half3 LightingDirectBRDF3(BTDFData btdfData, DotProducts dotProducts, Light light, half shadowIntensity = 0, half netLightness = 0, half3 reflectDir=0)
{
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half2 rlPow4AndFresnelTerm = Pow4(half2(dot(reflectDir, light.direction), 1 - dotProducts.NoV));  // use R.L instead of N.H to save couple of instructions
    half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp
    half fresnelTerm = rlPow4AndFresnelTerm.y;

    half LUT_RANGE = 16.0;
    half specular = SAMPLE_TEXTURE2D(unity_NHxRoughness, sampler_unity_NHxRoughness,half2(rlPow4, btdfData.roughness)).r * LUT_RANGE;

    half3 color = btdfData.diffuse;// + specular * btdfData.specular;
    return color * radiance
    +lerp(color * light.color, 0, light.shadowAttenuation * dotProducts.NoL) * shadowIntensity;
}

//UseGSAA
half3 LightingDirectGSAA1(BTDFData btdfData, DotProducts dotProducts, Light light, half3 normalWS, half CURV_MOD,half MAX_POW, half shadowIntensity = 0,half netLightness = 0)
{

    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten * dotProducts.NoL;
    half diffuseTerm = DisneyDiffuseNoPINBA(dotProducts.NoV, dotProducts.NoL, dotProducts.LoH, btdfData.perceptualRoughness);

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);

    half filteredRoughness2 = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);
    half3 halfvector = dotProducts.H;
    half3 halfvectorTS = mul(normalWS, halfvector);
    half2 halfvector2D = halfvectorTS.xy;
    half2 deltaU = ddx( halfvector2D );
    half2 deltaV = ddy( halfvector2D );
    half2 boundingRectangle = abs( deltaU ) + abs( deltaV );
    half2 variance = 0.25 * ( boundingRectangle * boundingRectangle );
    half2 kernelRoughness2 = min( 2.0 * variance, MAX_POW );
    half2 D = CURV_MOD*saturate(filteredRoughness2 + kernelRoughness2 );


    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);
    //half specularTerm = max(0, V * D);
    half specularTerm = V * (D.x + D.y) / 2;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
    half3 Direct = diffuseTerm * btdfData.diffuse * radiance + specularTerm * F * lightAtten;

    //return light.color * light.distanceAttenuation;
    // return diffuseTerm * btdfData.diffuse * radiance + specularTerm * F * lightAtten
    // return Direct + btdfData.diffuse * (1- atten * diffuseTerm) * shadowIntensity;
    // return radiance + specularTerm * F * lightAtten;
    return Direct
    +lerp(btdfData.diffuse * light.color, 0, Direct) * shadowIntensity
    + btdfData.diffuse*(1-diffuseTerm * lightAtten)  *netLightness;
    //return light.shadowAttenuation;

}

half3 LightingDirectGSAA2(BTDFData btdfData, DotProducts dotProducts, Light light, half3 normalWS, half CURV_MOD, half MAX_POW, half shadowIntensity = 0, half netLightness = 0)
{

    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half3 brdf = btdfData.diffuse;

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);

    half filteredRoughness2 = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);
    half3 halfvector = dotProducts.H;
    half3 halfvectorTS = mul(normalWS, halfvector);
    half2 halfvector2D = halfvectorTS.xy;
    half2 deltaU = ddx(halfvector2D);
    half2 deltaV = ddy(halfvector2D);
    half2 boundingRectangle = abs(deltaU) + abs(deltaV);
    half2 variance = 0.25 * (boundingRectangle * boundingRectangle);
    half2 kernelRoughness2 = min(2.0 * variance, MAX_POW);
    half2 D = CURV_MOD * saturate(filteredRoughness2 + kernelRoughness2);


    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);
    //half specularTerm = max(0, V * D);
    half specularTerm = V * (D.x + D.y) / 2;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);

    brdf += specularTerm * btdfData.specular;
    return brdf * radiance
        + lerp(brdf * light.color, 0, light.shadowAttenuation * dotProducts.NoL) * shadowIntensity;
    //return light.shadowAttenuation;

}


half CalculateSHOcculation(Varyings input, Light light, half3 LowHight)
{
#if defined(_RIM_MASK)
    half3 lightDir = TransformObjectToTangent(light.direction, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    half occulation = saturate(RimMask(lightDir, input.sh_1, LowHight.xy)+LowHight.z);
    return occulation;
#endif
    return 1;
}


half3 LightingAdditional(BTDFData btdfData, DotProductsSimple dotProducts, Light light, half ao)
{
//     half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten * dotProducts.NoL;
    //half diffuseTerm = DisneyDiffuseNoPINBA(dotProducts.NoV, dotProducts.NoL, dotProducts.LoH, btdfData.perceptualRoughness);

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);

    half D = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);
    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);
    //half specularTerm = max(0, V * D);
    half specularTerm = V * D;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
#if _HAIR

#else
    ao = 1;
#endif
    return btdfData.diffuse * radiance + specularTerm * F * lightAtten*ao;

}

half3 LightingAdditionalBRDF1(BTDFData btdfData, DotProductsSimple dotProducts, Light light, half ao, half3 reflectDir = 0)
{
    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten * dotProducts.NoL;

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);
    half D = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);
    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);

    half specularTerm = V * D;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 20.0); // Prevent FP16 overflow on mobiles
#endif
#if _HAIR

#else
    ao = 1;
#endif
    return btdfData.diffuse * radiance + specularTerm * F * lightAtten * ao;
}

half3 LightingAdditionalBRDF2(BTDFData btdfData, DotProductsSimple dotProducts, Light light, half ao, half3 reflectDir = 0)
{
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half3 brdf = btdfData.diffuse;
    half d = dotProducts.NoH * dotProducts.NoH * btdfData.roughness2MinusOne + 1.00001f;
    half LoH2 = dotProducts.LoH * dotProducts.LoH;
    half specularTerm = btdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * btdfData.normalizationTerm);
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 20.0); // Prevent FP16 overflow on mobiles
#endif

    brdf += specularTerm * btdfData.specular;
#if _HAIR

#else
    ao = 1;
#endif
    return brdf * radiance * ao;
}

half3 LightingAdditionalBRDF3(BTDFData btdfData, DotProductsSimple dotProducts, Light light, half ao , half3 reflectDir = 0)
{
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half2 rlPow4AndFresnelTerm = Pow4(half2(dot(reflectDir, light.direction), 1 - dotProducts.NoV));  // use R.L instead of N.H to save couple of instructions
    half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp

    half LUT_RANGE = 16.0;
    half specular = SAMPLE_TEXTURE2D(unity_NHxRoughness, sampler_unity_NHxRoughness, half2(rlPow4, btdfData.roughness)).r * LUT_RANGE;

    half3 color = btdfData.diffuse + specular * btdfData.specular;
    color *= radiance;
#if _HAIR

#else
    ao = 1;
#endif
    return color * ao;
}


/*
half3 LightingAdditional(BTDFData btdfData, DotProductsSimple dotProducts, Light light, half ao, half4 anisoD)
{
    //     half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten * dotProducts.NoL;
    //half diffuseTerm = DisneyDiffuseNoPINBA(dotProducts.NoV, dotProducts.NoL, dotProducts.LoH, btdfData.perceptualRoughness);

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);
    half D = HairStrandSpecular(anisoD.x, dotProducts.H, anisoD.z) + HairStrandSpecular(anisoD.y, dotProducts.H, anisoD.w);
    D *= radiance;
    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);
    //half specularTerm = max(0, V * D);
    half specularTerm = V * D;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 20.0); // Prevent FP16 overflow on mobiles
#endif
    return btdfData.diffuse * radiance + specularTerm * F * lightAtten * ao;

}
*/

half3 LightingLambert(half3 albedo, half NoL, Light light)
{
    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation * NoL;
    return albedo * radiance;
}

half3 LightingVirtual(BTDFData btdfData, half virAtten, half3 normalWS,half3 light)
{
#if defined(VIR_LIT)
    //half3 positionOS = TransformWorldToObject(positionWS);
    //half virAtten = max(0, pow((1 - positionOS.x), _LitAttenContrast) * _LitAttenInt - _LitAttenSub);
    half NoL = saturate(dot(light, normalWS));
    return NoL * virAtten * btdfData.diffuse;
#else
#if defined(RIM_MASK)
    return 1.0h;
#else
    return 0.0h;
#endif
#endif
}

half3 LightingEmissive(half3 emission)
{
#if defined(SHADING_EMISSION)
    return emission;
#else
    return 0.0h;
#endif
}



half3 FogApply(half3 color, Varyings input)
{
    InputData inputData;
    inputData.fogCoord = input.positionWS.w;
    return MixFog(color.rgb, inputData.fogCoord);
}
half Y0(half3 v)
{
    return 0.2820947917f;
}

half Y1(half3 v)
{
    return 0.4886025119f * v.y;
}

half Y2(half3 v)
{
    return 0.4886025119f * v.z;
}

half Y3(half3 v)
{
    return 0.4886025119f * v.x;
}
half remap(half value, half low1, half high1, half low2, half high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

half RimMask(half3 lightDir,half4 sh_1,half2 rimPara)
{
#if defined(RIM_MASK)
    half3 V = normalize(lightDir);
    half4 sh_0 = half4(Y0(V), Y1(V), Y2(V), Y3(V));
    half rimMask = dot(sh_0,sh_1);
    return saturate(remap(rimMask, rimPara.x, rimPara.y, 0, 1));
#else
    return 1;
#endif
}
