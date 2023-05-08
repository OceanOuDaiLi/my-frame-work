/////////////////////////////////////////////////
// SHADER FEATURE CONFIGURATION /////////////////
/////////////////////////////////////////////////
#define TANGENTSPACE
#define SHADING_METALLIC
#define CUBEMAP_REFLECTION

// DEPENDENCIES
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/SamplerSkinSSS.hlsl"

// MATERIAL PARAMETERS
CBUFFER_START(UnityPerMaterial)
half _Glossiness;
half _AOIntensity;
half _NormalBias;
half _NormalScale;
half4 _AlbedoColor;
half4 _EmissionColor;
//half   _LitAttenContrast;
//half   _LitAttenInt;
//half   _LitAttenSub;
half _Cutoff;
half _DetailBumpScale;
half _UVScale;
half3 _TranslucencyColor;
half4 _ThicknessAndCurvature;

half _ThicknessPower;
half _ThicknessScale;
half _SSSPower;
half _SSSFactor;
half4 _SSSColor;

half _RScale;
half _GScale;
half _BScale;
half _AScale;
half _SweatBumpScale;
half _SweatSmoothnessScale;
half _SweatSwitch;
half4 _SkinShadowColor;
half4 _DetailBump_ST;
half _Low;
half _High;
half3 _Light;
half _RimScale;
half _RimPower;
half3 _LightColor;
half _LightOn;
half _BackLightScale;

half _YSpeed;
half _Width;
half3 _EdgeColor;
half _EdgeWidth;
half _MaskTexTilling;
half _DownWidth;
half _Pos;
half _RimEdge;
half _RimDis;

half _Test3;
CBUFFER_END
uniform sampler2D _MatCap;

/////////////////////////////////////////////////
/// MATERIAL INPUT PROPERTIES ///////////////////
/////////////////////////////////////////////////

struct SSSMaterial
{
    half alpha; // Alpha value
    half3 albedo; // Albedo color
    half3 normal; // Tangent or Object space
    half3 emission; // Emission color (HDR)
    //half  metallic;      // Metallic value
    half smoothness; // Smoothness value
    half translucency;
    half curvature;
    half ao; // Ambient occlusion

    half4 numTex;
    half4 numTex2;
    half4 zmTex;
    half4 zmTex2;
};

/////////////////////////////////////////////////
// MATERIAL DEFINITION //////////////////////////
/////////////////////////////////////////////////

inline SSSMaterial GetMaterial(half2 uv)
{
    half4 albedoMap = SampleAlbedoMap(uv);
    half4 normalMap = SampleNormalMap(uv, _NormalBias);
    half4 metallicMap = SampleMetallicMap(uv);

    SSSMaterial material;
    material.alpha = Alpha(albedoMap.a, _AlbedoColor, _Cutoff);
    material.normal = UnpackNormalRGB(normalMap, _NormalScale);
    material.albedo = albedoMap.rgb;

    //half2 a = pow(metallicMap.ra, _ThicknessAndCurvature.xy) * _ThicknessAndCurvature.zw;
    //material.translucency = a.x;
#if defined(MATCAP_ACCURATE)
    // material.metallic =0;
    material.ao = 1;
    material.smoothness = _Glossiness;
#else
    material.smoothness = metallicMap.g * _Glossiness;
#endif
    material.ao = 1 - _AOIntensity + _AOIntensity * metallicMap.b;
    material.translucency = pow(abs(metallicMap.r), _ThicknessPower) * _ThicknessScale; //metallicMap.r=0��������һ����������ֵ��0


    material.emission = 0.0h;
    //material.transmission = 0.0h;


#if defined (DETAIL_MICRO)
    half2 detailUv = uv * _DetailBump_ST.xy + _DetailBump_ST.zw;
    half4 microNormalMap = SampleDetailNormalMap(detailUv);
    material.normal = BlendNormal(material.normal, UnpackNormalRGB(microNormalMap, _DetailBumpScale));
#endif

#if defined (SWEAT_DETAIL)
    half4 sweatMap = SampleSweatMap(uv);
    material.normal = BlendNormal(material.normal, UnpackNormalRG(sweatMap, _SweatBumpScale * _SweatSwitch));
    material.smoothness += sweatMap.b * _SweatSmoothnessScale * _SweatSwitch;
#endif

    material.normal = normalize(material.normal);
    material.albedo *= _AlbedoColor.rgb;

    return material;
}

/////////////////////////////////////////////////
/// BTDF SHADING DATA ///////////////////////////
/////////////////////////////////////////////////

inline BTDFData GetBTDFData(SSSMaterial material, DotProducts dotProducts, half3 translucencyColor)//translucencyColor���ǲ������ϵ�transmissionColor
{
    BTDFData btdfData;

    half Opacity = 1 - material.translucency; //translucency=0 Opacity:��͸����
    half InScatter = pow(dotProducts.NoH, 12) * Opacity; //lerp(3, 0.1f, Opacity);H=viewdir+LightDir
    half NormalContribution = saturate(dotProducts.RawNoH * Opacity + 1 - Opacity);
    half BackScatter = NormalContribution / (3.14 * 2);
    half3 lightScattering = saturate(1 - lerp(BackScatter, 1, InScatter)) * material.translucency * translucencyColor; //lerp(BackScatter, 1, InScatter)=BackScatter+InScatter*(1-BackScatter)
    btdfData.transmission = lightScattering;

    half smoothness = material.smoothness;
    btdfData.specular = kDieletricSpec.rgb * 0.7;

    half reflectivity = ReflectivitySpecular(btdfData.specular);
    half oneMinusReflectivity = 1.0 - reflectivity;
    //btdfData.diffuse = material.albedo * (half3(1.0h, 1.0h, 1.0h) - btdfData.specular);
    btdfData.diffuse = material.albedo * oneMinusReflectivity; //UNITY_CONSERVE_ENERGY_MONOCHROME
    //btdfData.diffuse = EnergyConservationBetweenDiffuseAndSpecular(albedo, o.specColor, /*out*/ o.oneMinusReflectivity); //specular work flow may be better for skin?
    btdfData.grazingTerm = saturate(smoothness + reflectivity);
    //btdfData.transmission *= oneMinusReflectivity;

    //btdfData.perceptualRoughness = material.roughness;
    btdfData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness); //1.0 - Smoothness
    btdfData.roughness = max(PerceptualRoughnessToRoughness(btdfData.perceptualRoughness), HALF_MIN);
    btdfData.roughness2 = btdfData.roughness * btdfData.roughness;
    btdfData.roughness2MinusOne = btdfData.roughness2 - 1.0h;
    btdfData.normalizationTerm = btdfData.roughness * 4.0h + 2.0h;

    return btdfData;
}

/////////////////////////////////////////////////
/// PBR LIGHTING CALCULATIONS ///////////////////
/////////////////////////////////////////////////

half3 LightingDirectBRDF1(BTDFData btdfData, DotProducts dotProducts, Light light, half3 diffuseTerm, half3 skinShadowColor, half3 reflectDir)
{
    
    half atten = light.distanceAttenuation * light.shadowAttenuation; //����˥������Ӱ˥����ƽ�й����˥����?1����û��˥����
    //half atten = light.color * light.distanceAttenuation * light.shadowAttenuation * dotProducts.NoL;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten ;
    //half3 diffuseTerm = SampleBSSRDFMap(half2((dotProducts.RawNoL * 0.5 + 0.5), curvature));
    //btdfData.diffuse * diffuseTerm * light.color + btdfData.diffuse * (1- atten * diffuseTerm) * _SkinShadowColor

    half V = V_SmithJointGGXApprox(dotProducts.NoL, dotProducts.NoV, btdfData.roughness);
    half D = D_GGXNoPI(dotProducts.NoH, btdfData.roughness);
    half3 F = F_Schlick(btdfData.specular, dotProducts.LoH);

    half specularTerm = V * D;
    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * dotProducts.NoL);
    // return half4 (lightAtten,1);

    return diffuseTerm * btdfData.diffuse * radiance + specularTerm * F * lightAtten + btdfData.transmission + btdfData.diffuse * (1 - atten * diffuseTerm) * skinShadowColor;

}

half3 LightingDirectBRDF2(BTDFData btdfData, DotProducts dotProducts, Light light, half3 diffuseTerm, half3 skinShadowColor, half3 reflectDir)
{
    
    half atten = light.distanceAttenuation * light.shadowAttenuation;
    half3 lightAtten = light.color * atten;
    half3 radiance = lightAtten;
    //half3 diffuseTerm = SampleBSSRDFMap(half2((dotProducts.RawNoL * 0.5 + 0.5), curvature));
    //btdfData.diffuse * diffuseTerm * light.color + btdfData.diffuse * (1- atten * diffuseTerm) * _SkinShadowColor
    half d = dotProducts.NoH * dotProducts.NoH * btdfData.roughness2MinusOne + 1.00001f;
    half LoH2 = dotProducts.LoH * dotProducts.LoH;
    half specularTerm = btdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * btdfData.normalizationTerm);

    return diffuseTerm * btdfData.diffuse * radiance + specularTerm * btdfData.specular * radiance * dotProducts.NoL + btdfData.transmission + btdfData.diffuse * (1 - atten * diffuseTerm) * skinShadowColor;

}

half3 LightingDirectBRDF3(BTDFData btdfData, DotProducts dotProducts, Light light, half3 diffuseTerm, half3 skinShadowColor, half3 reflectDir)
{

    half3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation;
    half2 rlPow4AndFresnelTerm = Pow4(half2(dot(reflectDir, light.direction), 1 - dotProducts.NoV)); // use R.L instead of N.H to save couple of instructions
    half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp
    half fresnelTerm = rlPow4AndFresnelTerm.y;

    half LUT_RANGE = 16.0;
    half specular = SAMPLE_TEXTURE2D(unity_NHxRoughness, sampler_unity_NHxRoughness, half2(rlPow4, btdfData.roughness)).r * LUT_RANGE;

    half3 color = diffuseTerm * btdfData.diffuse + specular * btdfData.specular * dotProducts.NoL;
     color *= radiance;
     color += btdfData.transmission;
    return color;

}
/////////////////////////////////////////////////
// FORWARD PASS /////////////////////////////////
/////////////////////////////////////////////////
Varyings LitVertForward(Attributes input)
{
    Varyings output = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

#if defined(VERTEX_COLOR)
    //for muscle
    input.positionOS.xyz += input.normalOS.xyz*(
                (input.color.r)*(_RScale-1)*0.1+
                (input.color.g)*(_GScale-1)*0.1+
                (input.color.b)*(_BScale-1)*0.1+
                (input.color.a)*(_AScale-1)*0.1
                );
#endif

    half3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    //output.positionWS.xyz = positionWS.xyz;
    half4 positionCS = TransformWorldToHClip(positionWS);
    half3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    half3 viewDirWS = GetCameraPositionWS() - positionWS;
    //viewDirWS = normalize(viewDirWS);
    
    //
    output.posOSNormalY.x = input.positionOS.x;
    output.posOSNormalY.y = input.normalOS.y;
    output.posOSNormalY.zw = 0;
    
    //half3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
    half3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
    half3 worldBinormal = cross(normalWS, worldTangent) * input.tangentOS.w;
    output.tSpace0 = half3(worldTangent.x, worldBinormal.x, normalWS.x);
    output.tSpace1 = half3(worldTangent.y, worldBinormal.y, normalWS.y);
    output.tSpace2 = half3(worldTangent.z, worldBinormal.z, normalWS.z);
    // OUTPUT TEXCOORDS
    OUTPUT_TEXCOORD0(input, output, _UVScale);
    
    //
    output.uv0.xy = input.texcoord0.xy;
    //??
    output.uv0.zw = input.uv3.xy;
    
    
#if defined(RIM_MASK)
    output.sh_1 = half4(input.uv1.x, input.uv1.y, input.uv2.x, input.uv2.y);
#endif
#if defined(MATCAP_ACCURATE)
    half3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
    half3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
    half3 worldBinormal = cross(worldNormal, worldTangent) * input.tangentOS.w;
    output.tSpace0 = half3(worldTangent.x, worldBinormal.x, worldNormal.x);
    output.tSpace1 = half3(worldTangent.y, worldBinormal.y, worldNormal.y);
    output.tSpace2 = half3(worldTangent.z, worldBinormal.z, worldNormal.z);
#endif
    // OUTPUT SHADOW, NORMALS, AMBIENT GI, CLIP-, WORLD-POSITION, VIEWDIR
    OUTPUT_DEFAULT(input, output, positionWS, positionCS, normalWS, viewDirWS);
    output.bakeGISH = GIVERTEX(normalWS);
    //UNITY_SHADOW_COORDS(6);
    return output;
}


//这里相当于像素着色器内部
half4 LitFragForward(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    //Normalize viewDirWS
    SETUP_SHADING_DATA(input);  
    
    half3 baseColor;

    SSSMaterial material = GetMaterial(input.uv0.xy);
    
    half3 normalWS = GetWorldNormal(input, material.normal);
    

    half3 reflectDir = GetReflectDir(normalWS, viewDirWS, input.positionWS.xyz);
    half3 bakedGI = GIFRAG(input, normalWS) + input.bakeGISH;

    DotProducts dotProducts = GetDotProducts(mainLight, normalWS, viewDirWS);
    BTDFData btdfData = GetBTDFData(material, dotProducts, _TranslucencyColor);
  
    // float deltaWorldNormal = length(fwidth(normalWS));
    // float deltaWorldPos = length(fwidth(input.positionWS.xyz));
    // float inverCurve = (deltaWorldNormal / deltaWorldPos) * _SSSFactor;
    half4 diffuseTerm = SampleBSSRDFMap(half2((dotProducts.RawNoL * 0.5 + 0.5), _SSSFactor));
    diffuseTerm.r = saturate(pow(diffuseTerm.r, _SSSPower * (1 - _SSSColor.r)));
    diffuseTerm.g = saturate(pow(diffuseTerm.g, _SSSPower * (1 - _SSSColor.g)));
    diffuseTerm.b = saturate(pow(diffuseTerm.b, _SSSPower * (1 - _SSSColor.b)));
    //return diffuseTerm;

    // half3 normalMap = UnpackNormalScale(SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, input.uv0.xy, _Test3 * inverCurve),1.0);
    // normalMap = UnpackNormalRGB(half4(normalMap,1), _NormalScale);
    // normalWS = GetWorldNormal(input , normalMap);
    half3 color;
    
#if defined(MATCAP_ACCURATE)
    color = LightingIndirect2(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao);
#else
    color = LightingIndirect(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao);
#endif
#if defined(RIM_MASK)
    half rim = 1 - saturate(dot(viewDirWS, normalWS));
    //trick rim
    half3 lightDir1 = TransformObjectToTangent(half3(0,-1,0), half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    half3 lightDir2 = TransformWorldToTangent(_Light, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    half resValue1 = RimMask(lightDir1, input.sh_1,  half2(_Low, _High)/2) * _BackLightScale;
    half resValue2 = RimMask(lightDir2, input.sh_1, half2(_Low, _High));
#if defined(VIR_LIT)
        half3 rim2 = _LightColor * btdfData.diffuse * virAtten;
#else
        half3 rim2 = _LightColor;
#endif
        half3 rimColor = (pow(rim, _RimPower) * max(0.0001, _RimScale) * ((resValue1 + resValue2) * material.ao) * rim2)* material.ao;
        // half3 rimColor = resValue1;
    color += rimColor;
    // return half4(rimColor, 1);
#endif
    //if (mainLight.distanceAttenuation > 0)//distanceAttenuation����˥��
    //    color += LIGHTING_BRDF(btdfData, dotProducts, mainLight, diffuseTerm.rgb, _SkinShadowColor.rgb, reflectDir);
    if (mainLight.distanceAttenuation > 0)//distanceAttenuation����˥��
        color += LIGHTING_BRDF(btdfData, dotProducts, mainLight, diffuseTerm.rgb, _SkinShadowColor.rgb, reflectDir);
        //color = btdfData.diffuse * diffuseTerm.rgb;
        //return half4(color ,1);
    //return half4(mylight.direction, 1.0);
    
#if defined(MATCAP_ACCURATE)
    float3 worldNorm;
    worldNorm.x = dot(input.tSpace0.xyz, material.normal);
    worldNorm.y = dot(input.tSpace1.xyz, material.normal);
    worldNorm.z = dot(input.tSpace2.xyz, material.normal);
    worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
    color += tex2D(_MatCap, worldNorm.xy * 0.5 + 0.5) * material.smoothness;// * half4(4.59479380, 4.59479380, 4.59479380, 2.0);
#endif
    
    int pixelLightCount = GetAdditionalLightsCount();                                          
    if(pixelLightCount > 0)                                                                     
    {                                                                                           
        for (int i = 0; i < pixelLightCount; i++)                                               
        {                                                                                       
            Light lightAdd = GetAdditionalLight(i, input.positionWS.xyz);                                     
            half4 diffuseTermAdd = SampleBSSRDFMap(half2((dot(normalWS,lightAdd.direction) * 0.5 + 0.5), _SSSFactor));
            diffuseTermAdd.r = pow(diffuseTermAdd.r, _SSSPower * (1 - _SSSColor.r));
            diffuseTermAdd.g = pow(diffuseTermAdd.g, _SSSPower * (1 - _SSSColor.g));
            diffuseTermAdd.b = pow(diffuseTermAdd.b, _SSSPower * (1 - _SSSColor.b));                                                   
            color += lightAdd.color * lightAdd.distanceAttenuation * lightAdd.shadowAttenuation * btdfData.diffuse * diffuseTermAdd.rgb;     
        }                                                                                       
    }

    //ADDITIONAL_LIGHTS(input, color, btdfData, material.ao, reflectDir,normalWS);

    color.rgb = GradientAlpha(half4(_YSpeed, _Width, _EdgeWidth, _MaskTexTilling),
        _EdgeColor, input, color, half4(_DownWidth, _Pos, _RimEdge, _RimDis)).rgb;

    color.rgb = FogApply(color, input);
    
    return half4(color, 1);
}

/////////////////////////////////////////////////
// Unlit PASS //////////////////////////////////
/////////////////////////////////////////////////

VaryingsSimple LitVertUnlit(AttributesSimple input)
{
    OUTPUT_VERTSIMPLE(input);
    return output;
}

half4 LitFragUnlit(VaryingsSimple input) : SV_Target
{
    OUTPUT_FRAGSIMPLE(input);
    return half4(color, alpha);
}

/////////////////////////////////////////////////
// MAIN FUNCTION /////////////////////////////////
/////////////////////////////////////////////////

#if defined(UNITY_STANDARD_SIMPLE)
    VaryingsSimple LitVert (AttributesSimple v) { return LitVertUnlit(v); }
    half4 LitFrag (VaryingsSimple i) : SV_Target { return LitFragUnlit(i); }
#else
Varyings LitVert(Attributes input)
{
    return LitVertForward(input);
}
half4 LitFrag(Varyings input) : SV_Target
{
    return LitFragForward(input);
}
#endif