/////////////////////////////////////////////////
// SHADER FEATURE CONFIGURATION /////////////////
/////////////////////////////////////////////////
#define TANGENTSPACE
//#define PARALLAX
#define SHADING_METALLIC
#define CUBEMAP_REFLECTION
#if defined(MATCAP_ACCURATE)
    #define MATCAP_ACCURATE
#endif
#if defined(PLANAR_REFLECTION)
    #define SCREEN_COORD
#endif

#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/Includes.hlsl"
#include "Assets/TechArtist/Shaders/PBR/ShaderLibrary/SamplerSkinSSS.hlsl"

CBUFFER_START(UnityPerMaterial)
    half   _Metallic;
    half   _Glossiness;
    half   _AOIntensity;
    half   _NormalBias;
    half   _NormalScale;
    half4  _AlbedoColor;
    half4  _EmissionColor;

    half   _Cutoff;
    half   _EmissionEnabled;
    half   _UVScale;
    half   _RScale;
    half   _GScale;
    half   _BScale;
    half   _AScale;
    half   _NetLightness;
    //   _ShadowIntensity;
    half   _Low;
    half   _High;
    float3 _Light;
    half   _RimScale;
    half   _RimPower;
    half3  _LightColor;
    half   _BackLightScale;

    half   _YSpeed;
    half   _Width;
    half3   _EdgeColor;
    half   _EdgeWidth;
    half   _MaskTexTilling;
    half   _DownWidth;
    half   _Pos;
    half   _RimEdge;
    half   _RimDis;


    //ƽ����Ӱ
    half _shadowFalloff = 0.17;
    half4 _ShadowDirection = (8.4,26,-14.67,0);
    half _ShadowAlpha = 0.1;

    half4 _ShadowPlane;
    half4 _WorldPos;
    half _ShadowIntensity;


    //������

    half _NumberS;
    half _NumberG;
    half _NumberDis;

    half4 _ZmStrV4 = (0.0, 0.0, 0.0, 0.0);
    half4 _ZmStr2V4 = (0.0, 0.0, 0.0, 0.0);
    half4 _ZmStr3V4 = (0.0, 0.0, 0.0, 0.0);
    //half4 _ZMOffsetV4 = (1, 0.8, 2, 0);
    half _ZMIndexDistance = 1;
    half _ZMGroupOffsetY = 2.5;
    half _ZMRotateAngleStep = 1.5;
    half _ZMExtraOffsetX = 0;
    half _ZMFontSize = 1.0;
    half _ZMDeltaYStep = 1.0;
    half _ZMUVDistanceScale = 8.0;

    half4 _NumFrontOffSet = (0.0, 0.0, 1.0, 1.0);
    half4 _NumBackOffSet = (0.0, 0.0, 1.0, 1.0);
    half4 _NumFrontOffSet2 = (0.0, 0.0, 1.0, 1.0);
    half4 _NumBackOffSet2 = (0.0, 0.0, 1.0, 1.0);

    
    half   _ReflectionAlpha;
    half   _PlaneReflectionIntensity;
    half   _ReflectionNoiseScale;

CBUFFER_END

real4 _Override_SHCoefficients[7];

sampler2D _MatCap;
/////////////////////////////////////////////////
// MATERIAL DEFINITION //////////////////////////
/////////////////////////////////////////////////

struct StandardMaterial
{
    half alpha; // Alpha value
    half3 albedo; // Albedo color
    half3 normal; // Tangent or Object space
    half3 emission; // Emission color (HDR)
    half metallic; // Metallic value
    half smoothness; // Smoothness value
    half transmission; // Tranmission multiplier
    half ao; // Ambient occlusion
    
    
    //������
    half4 numTex;
    half4 numTex2;
    half4 zmTex;
};


inline StandardMaterial GetMaterial01(float2 uv, float2 uv1, half2 uv2)
{
    //float2 uv = input.uv0.xy;
    half4 albedoMap = SampleAlbedoMap(uv);
    half4 normalMap = SampleNormalMap(uv, _NormalBias);
    half4 metallicMap = SampleMetallicMap(uv);

    StandardMaterial material;
    material.alpha = Alpha(albedoMap.a, _AlbedoColor, _Cutoff);
    material.normal = UnpackNormalRGB(normalMap, _NormalScale);
    material.albedo = albedoMap.rgb;
    material.metallic = metallicMap.r;

    material.emission = 0.0h;

    material.transmission = 0.0h;

#if defined (SHADING_EMISSION)
    material.emission = SampleEmissionMap(uv).rgb * _EmissionColor.rgb;
#endif
#if defined(MATCAP_ACCURATE)
    material.smoothness = _Glossiness;
    material.metallic =0;
    material.ao = 1;
#else
    material.ao = 1 - _AOIntensity + _AOIntensity * metallicMap.b;
    material.smoothness = metallicMap.g * _Glossiness;
    material.metallic *= _Metallic;
#endif

    material.normal = normalize(material.normal);
    material.albedo *= _AlbedoColor.rgb;

    material.numTex = SampleNumberTexture(uv1);
    material.numTex2 = SampleNumberTexture2(uv1);

    // material.smoothness * _Glossiness;
    material.zmTex = SampleZMTexture(uv2);

    return material;
}


inline BTDFData GetBTDFData02(StandardMaterial material, DotProducts dotProducts, inout half alpha)
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



void ReplaceSH()
{
#if OVERRIDE_SH
    unity_SHAr = _Override_SHCoefficients[0];
    unity_SHAg = _Override_SHCoefficients[1];
    unity_SHAb = _Override_SHCoefficients[2];
    unity_SHBr = _Override_SHCoefficients[3];
    unity_SHBg = _Override_SHCoefficients[4];
    unity_SHBb = _Override_SHCoefficients[5];
    unity_SHC = _Override_SHCoefficients[6];
#endif
}

/////////////////////////////////////////////////
// FORWARD PASS /////////////////////////////////
/////////////////////////////////////////////////
Varyings LitVertForward(Attributes input)
{
    Varyings output = (Varyings)0;
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

    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float4 positionCS = TransformWorldToHClip(positionWS);
    half3  normalWS   = TransformObjectToWorldNormal(input.normalOS);
    half3 viewDirWS   = GetCameraPositionWS() - positionWS;
    //tangent
    half3 tanOS = TransformObjectToWorldDir(input.tangentOS.xyz);
    output.tangentWS   = half4(tanOS, viewDirWS.y);
    // OUTPUT TEXCOORDS
    OUTPUT_TEXCOORD0(input, output, _UVScale);
    OUTPUT_TEXCOORD2(input, output);
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
    
    
    //�������õ�
    output.posOSNormalY.x = input.positionOS.x;
    output.posOSNormalY.y = input.normalOS.y;
    output.posOSNormalY.zw = 0;
    
    //half3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
    half3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
    half3 worldBinormal = cross(normalWS, worldTangent) * input.tangentOS.w;
    output.tSpace0 = half3(worldTangent.x, worldBinormal.x, normalWS.x);
    output.tSpace1 = half3(worldTangent.y, worldBinormal.y, normalWS.y);
    output.tSpace2 = half3(worldTangent.z, worldBinormal.z, normalWS.z);
    
    
    output.uv0.xy = input.texcoord0.xy;
    output.uv0.zw = input.uv3.xy;

    return output;
}

half4 LitFragForward (Varyings input) : SV_TARGET
{
    ReplaceSH();

    UNITY_SETUP_INSTANCE_ID(input);
    //Normalize viewDirWS
    SETUP_SHADING_DATA(input);

    
    half3 baseColor;
            //������
    half4 numFrontOffset_var = _NumFrontOffSet;
    half4 numBackOffset_var = _NumBackOffSet;
    
#if defined(TEX2_ON)
    _NumFrontOffSet=_NumFrontOffSet2;
    _NumBackOffSet=_NumBackOffSet2;
#else
    _NumFrontOffSet = numFrontOffset_var;
    _NumBackOffSet = numBackOffset_var;
#endif	

	//���º���uv��������
    half IDUs = _NumberS; //ȡNumberʮλ��Ϊ����ID1
    half IDVg = _NumberG; //ȡNumber��λ��Ϊ����ID2
    half MaskNormal = step(0.1, -input.posOSNormalY.y);//ǰ�����ֹ�ϵ
    half FBscale = lerp(1 / max(0.01, _NumFrontOffSet.z), 1 / max(0.01, _NumBackOffSet.z), MaskNormal); // ǰ�������Ų���  FBscale =1.0
    half UvXMove = lerp((-_NumFrontOffSet.x), _NumBackOffSet.x, MaskNormal); // ǰ����X�ƶ�
    half LeftRightMask = step((input.uv0.z + (UvXMove * (1.0 / max(0.01, FBscale)) * 0.5)), 0.5); // ����UVx���� �ּ��
    half sgSwitch = saturate(IDUs); //ʮλ��λ�����л�����
    half numDistance = sgSwitch * lerp(-0.4 - _NumberDis, 0.4 + _NumberDis, LeftRightMask); //���ּ�����
    half numScaleX = (input.uv0.z - 0.5) * (2.0 * FBscale) + 0.5; //X����ǰ������				
    half numMoveY = lerp(0.25 * _NumFrontOffSet.y, 0.25 * _NumBackOffSet.y, MaskNormal); //Y����ǰ��Y�ƶ�
    half numScaleY = (input.uv0.w + numMoveY - 0.5) * FBscale + 0.5; //ǰ������
    half2 NumUV = float2(numScaleX + numDistance + UvXMove, numScaleY) * float2(0.1, 1.0);
    //half2 finalNumUV = float2((lerp(IDVg, IDUs, (LeftRightMask * sgSwitch)) * 0.1), 0) + NumUV;
    half2 finalNumUV = float2(((IDVg + ((LeftRightMask * sgSwitch) * (IDUs - IDVg))) * 0.1), 0) + NumUV;
    //half2 finalNumUV = float2(IDUs * 0.1, 0) + NumUV;
    
     //��ĸ�ߴ�
    half ScaleSetting = 4;
    //��ֱƫ��
    half ZMPositionOffsetY = -0.35;
    //�ַ�����
    half zmNum = 31 + 1;
      // //0.032258 = 1/31 31���ַ�
    half zmItemWidth = 0.032258;
    
    half level1 = zmNum;
    half level2 = level1 * zmNum;
    
    int W3 = floor(_ZmStr3V4.w);
    float index3W2 = step(level2, W3) * 3;
    float index3W1 = step(index3W2, 2) * step(level1, W3) * 2;
    int index3NumW = max(index3W2, index3W1);
    index3NumW = max(index3NumW, step(0.1, W3));
    
    int Z3 = floor(_ZmStr3V4.z);
    float index3Z2 = step(level2, Z3) * 3;
    float index3Z1 = step(index3Z2, 2) * step(level1, Z3) * 2;
    int index3NumZ = max(index3Z2, index3Z1);
    index3NumZ = max(index3NumZ, step(0.1, Z3));
    
    int Y3 = floor(_ZmStr3V4.y);
    float index3Y2 = step(level2, Y3) * 3;
    float index3Y1 = step(index3Y2, 2) * step(level1, Y3) * 2;
    int index3NumY = max(index3Y2, index3Y1);
    index3NumY = max(index3NumY, step(0.1, Y3));
    
    int X3 = floor(_ZmStr3V4.x);
    float index3X2 = step(level2, X3) * 3;
    float index3X1 = step(index3X2, 2) * step(level1, X3) * 2;
    int index3NumX = max(index3X2, index3X1);
    index3NumX = max(index3NumX, step(0.1, X3));
    


    int W2 = floor(_ZmStr2V4.w);
    float index2W2 = step(level2, W2) * 3;
    float index2W1 = step(index2W2, 2) * step(level1, W2) * 2;
    int index2NumW = max(index2W2, index2W1);
    index2NumW = max(index2NumW, step(0.1, W2));
    
    int Z2 = floor(_ZmStr2V4.z);
    float index2Z2 = step(level2, Z2) * 3;
    float index2Z1 = step(index2Z2, 2) * step(level1, Z2) * 2;
    int index2NumZ = max(index2Z2, index2Z1);
    index2NumZ = max(index2NumZ, step(0.1, Z2));
    
    int Y2 = floor(_ZmStr2V4.y);
    float index2Y2 = step(level2, Y2) * 3;
    float index2Y1 = step(index2Y2, 2) * step(level1, Y2) * 2;
    int index2NumY = max(index2Y2, index2Y1);
    index2NumY = max(index2NumY, step(0.1, Y2));
    
    int X2 = floor(_ZmStr2V4.x);
    float index2X2 = step(level2, X2) * 3;
    float index2X1 = step(index2X2, 2) * step(level1, X2) * 2;
    int index2NumX = max(index2X2, index2X1);
    index2NumX = max(index2NumX, step(0.1, X2));

    

    int X = floor(_ZmStrV4.x);
    float indexX2 = step(level2, X) * 3;
    float indexX1 = step(indexX2, 2) * step(level1, X) * 2;
    int indexNumX = max(indexX2, indexX1);
    indexNumX = max(indexNumX, step(0.1, X));

    int Y = floor(_ZmStrV4.y);
    float indexY2 = step(level2, Y) * 3;
    float indexY1 = step(indexY2, 2) * step(level1, Y) * 2;
    int indexNumY = max(indexY2, indexY1);
    indexNumY = max(indexNumY, step(0.1, Y));
    
    int Z = floor(_ZmStrV4.z);
    float indexZ2 = step(level2, Z) * 3;
    float indexZ1 = step(indexZ2, 2) * step(level1, Z) * 2;
    int indexNumZ = max(indexZ2, indexZ1);
    indexNumZ = max(indexNumZ, step(0.1, Z));
    
    int W = floor(_ZmStrV4.w);
    float indexW2 = step(level2, W) * 3;
    float indexW1 = step(indexW2, 2) * step(level1, W) * 2;
    int indexNumW = max(indexW2, indexW1);
    indexNumW = max(indexNumW, 1);

    //�ܹ���λ��
    int indexNum = indexNumX + indexNumY + indexNumZ + indexNumW
    + index2NumX + index2NumY + index2NumZ + index2NumW
    + index3NumX + index3NumY + index3NumZ + index3NumW;
    //return half4(indexNumW, indexNum, 1, 1) * 0.1;
    

    //half showZMCount = min(8, indexNum);
    //showZMCount = max(4, indexNum);
    
    //half zmNeedHalfWidth = (indexNum / 2) * 0.1;
    half indexNumCompare = step(indexNum, 4);
    //�����ַ��ߴ��С
    half zmUVScaleSize = indexNumCompare * 1.2 + (1 - indexNumCompare) * min((1.2 + (indexNum - 4) * 0.2), 2.2);
    
    zmUVScaleSize = zmUVScaleSize * _ZMFontSize;
    
    //�����ַ���ת�ĽǶȣ���������
    half zmRotateAngle = _ZMRotateAngleStep;
    //ȫ����ĸ��Y���ϵ�ƫ��
    half zmGroupOffsetY = _ZMGroupOffsetY;
    //������ĸƽ��  ���ȵ�UV
    half numFlag = step(9, indexNum);
    half allLen = numFlag * 0.95 + (1 - numFlag) * 0.8;
    half zmItemSize = allLen / indexNum;
    half zmExtraOffsetX = _ZMExtraOffsetX;
    half zmDeltaYStep = _ZMDeltaYStep;
    
    zmItemSize = _ZMIndexDistance * zmItemSize;
    
    
    
    //�볤��
    half halfNum = (indexNum / 2);
    halfNum = max(1, halfNum);
    half zmNeedHalfWidth = zmItemSize * halfNum;
    zmNeedHalfWidth = min(0.45, zmNeedHalfWidth);
    //ģ������λ��
    half midPosX = 0.55;
    //���õ�����ĸƫ��λ��
    half zmItemUVOffsetX = zmUVScaleSize * (-0.5 + indexNum * 0.0185);
    half maxX = midPosX + zmNeedHalfWidth;
    half minX = midPosX - zmNeedHalfWidth;

    half x = input.uv0.z;
    half y = input.uv0.w;
    
    half stepX = (maxX - minX) / indexNum;
    
    half midFloatIndex = (indexNum - 1) / 2.0;
    half midIndex = floor(indexNum / 2);

    int zmItemIndex = floor((maxX - x) / (stepX));
    
    half isLeft = step(x, midPosX);
    half absVal = abs(midPosX - x);
    
    //return step(absVal, 0.1);
    
    int absDeltaIndex = floor(absVal / stepX);
    int realDelteIndex = isLeft * (absDeltaIndex) + (1 - isLeft) * (-1 * absDeltaIndex);
    //zmItemIndex = realDelteIndex + midIndex;
    
    //return step(abs(zmItemIndex - midFloatIndex), 1);
    
    //return zmItemIndex * 0.1;

    

    //half tanVal = y / x;
    //half upY = step(circleOrigin.y, y);
    //half tagFlag = step(length(tanVal - tan(angleDeg * angleCur)), 0.01);
    //return circleOut * circleIn * tagFlag * upY; //sinFlag * cosFlag;
    
    
    //zmItemIndex = zmItemIndex - 1;
    //int zmItemIndex = floor((x - (indexNum - 1) * 0.05 - 0.5) / stepX);
    //return zmItemIndex * 0.1;

    //half midIndex = floor(midFloatIndex);
    midIndex = max(1, midIndex);
    
        //����ж�
    //half isClamp1 = step(x, maxX);
    //half isClamp2 = step(minX, x);
    //half isClamp = isClamp1 * isClamp2;
    //half isOver = 1 - isClamp;
    
    half isClamp1 = step(zmItemIndex, max(indexNum - 1, 1));
    half isClamp2 = step(0, zmItemIndex);
    half isClamp = isClamp1 * isClamp2;
    half isOver = 1 - isClamp;
    
     //ÿ��half �������ĸ����
    int singleZMCount = 2; //4
    
    half switchW = step(zmItemIndex, singleZMCount - 1);
    half switchZ = (1 - switchW) * step(zmItemIndex, singleZMCount * 2 - 1);
    half switchY = (1 - switchZ) * step(zmItemIndex, singleZMCount * 3 - 1);
    half switchX = (1 - switchY) * step(zmItemIndex, singleZMCount * 4 - 1);
    
    half switchW2 = (1 - switchX) * step(zmItemIndex, singleZMCount * 5 - 1);
    half switchZ2 = (1 - switchW2) * step(zmItemIndex, singleZMCount * 6 - 1);
    half switchY2 = (1 - switchZ2) * step(zmItemIndex, singleZMCount * 7 - 1);
    half switchX2 = (1 - switchY2) * step(zmItemIndex, singleZMCount * 8 - 1);
    
    half switchW3 = (1 - switchX2) * step(zmItemIndex, singleZMCount * 9 - 1);
    half switchZ3 = (1 - switchW3) * step(zmItemIndex, singleZMCount * 10 - 1);
    half switchY3 = (1 - switchZ3) * step(zmItemIndex, singleZMCount * 11 - 1);
    half switchX3 = (1 - switchY3) * step(singleZMCount * 11 - 1 + 0.1, zmItemIndex);
    
    float maxValW = floor(pow(zmNum, zmItemIndex + 1)); //26
    float minValW = floor(pow(zmNum, zmItemIndex)); //1
    
    float maxValZ = floor(pow(zmNum, zmItemIndex + 1 - (1 * singleZMCount))); //26
    float minValZ = floor(pow(zmNum, zmItemIndex - (1 * singleZMCount))); //1
    
    float maxValY = floor(pow(zmNum, zmItemIndex + 1 - (2 * singleZMCount))); //26
    float minValY = floor(pow(zmNum, zmItemIndex - (2 * singleZMCount))); //1
    
    float maxValX = floor(pow(zmNum, zmItemIndex + 1 - (3 * singleZMCount))); //26
    float minValX = floor(pow(zmNum, zmItemIndex - (3 * singleZMCount))); //1
    
    
    float maxValW2 = floor(pow(zmNum, zmItemIndex + 1 - (4 * singleZMCount)));
    float minValW2 = floor(pow(zmNum, zmItemIndex - (4 * singleZMCount)));
    
    float maxValZ2 = floor(pow(zmNum, zmItemIndex + 1 - (5 * singleZMCount)));
    float minValZ2 = floor(pow(zmNum, zmItemIndex - (5 * singleZMCount)));
    
    float maxValY2 = floor(pow(zmNum, zmItemIndex + 1 - (6 * singleZMCount)));
    float minValY2 = floor(pow(zmNum, zmItemIndex - (6 * singleZMCount)));
    
    float maxValX2 = floor(pow(zmNum, zmItemIndex + 1 - (7 * singleZMCount)));
    float minValX2 = floor(pow(zmNum, zmItemIndex - (7 * singleZMCount)));
    
    
    float maxValW3 = floor(pow(zmNum, zmItemIndex + 1 - (8 * singleZMCount)));
    float minValW3 = floor(pow(zmNum, zmItemIndex - (8 * singleZMCount)));
    
    float maxValZ3 = floor(pow(zmNum, zmItemIndex + 1 - (9 * singleZMCount)));
    float minValZ3 = floor(pow(zmNum, zmItemIndex - (9 * singleZMCount)));
    
    float maxValY3 = floor(pow(zmNum, zmItemIndex + 1 - (10 * singleZMCount)));
    float minValY3 = floor(pow(zmNum, zmItemIndex - (10 * singleZMCount)));
    
    float maxValX3 = floor(pow(zmNum, zmItemIndex + 1 - (11 * singleZMCount)));
    float minValX3 = floor(pow(zmNum, zmItemIndex - (11 * singleZMCount)));
    
    
    half valueW3 = switchW3 * floor((W3 % maxValW3) / minValW3);
    half valueZ3 = switchZ3 * floor((Z3 % maxValZ3) / minValZ3);
    half valueY3 = switchY3 * floor((Y3 % maxValY3) / minValY3);
    half valueX3 = switchX3 * floor((X3 % maxValX3) / minValX3);
    valueW3 = max(0, valueW3);
    valueZ3 = max(0, valueZ3);
    valueY3 = max(0, valueY3);
    valueX3 = max(0, valueX3);
    
    half valueW2 = switchW2 * floor((W2 % maxValW2) / minValW2);
    half valueZ2 = switchZ2 * floor((Z2 % maxValZ2) / minValZ2);
    half valueY2 = switchY2 * floor((Y2 % maxValY2) / minValY2);
    half valueX2 = switchX2 * floor((X2 % maxValX2) / minValX2);
    valueW2 = max(0, valueW2);
    valueZ2 = max(0, valueZ2);
    valueY2 = max(0, valueY2);
    valueX2 = max(0, valueX2);
    
    half valueW = switchW * floor((W % maxValW) / minValW);
    half valueZ = switchZ * floor((Z % maxValZ) / minValZ);
    half valueY = switchY * floor((Y % maxValY) / minValY);
    half valueX = switchX * floor((X % maxValX) / minValX);
    valueW = max(0, valueW);
    valueZ = max(0, valueZ);
    valueY = max(0, valueY);
    valueX = max(0, valueX);
    
    //return half4(valueX, valueY, valueZ, 1) * 0.01;
    
    half zmValue = valueW + valueZ + valueY + valueX
    + valueW2 + valueZ2 + valueY2 + valueX2
    + valueW3 + valueZ3 + valueY3 + valueX3;
    
    //return zmValue * 0.1;
    //half _clampVal = Z % maxVal;
    //_clampVal = floor(_clampVal / minVal);
    //27����
    half rVal = zmValue - 1;
    
    //return rVal * 0.03;
        

    int zmStep = floor(midIndex - zmItemIndex);
    //x �����ϵ�ƫ��
    //half zmUvXMove = zmItemUVOffsetX; /** ScaleSetting */ /*+ _ZMOffsetV4.w*/; // * 5 * zmStep;
    
    //half zmDis = 0.1;
    
    half zmStaticNumScaleX = (input.uv0.z - 0.5) * (2 * ScaleSetting) + 0.5 /*+ _ZMOffsetV4.w*/;
    
    //half zmDistance = zmStep * 0.5 * stepX;
    
    //return zmDistance+0.5;

    //Y����λ�ã���
    half zm_numScaleY = (input.uv0.w + ZMPositionOffsetY - 0.5) * ScaleSetting + 0.5;
  
    //���Ŵ�С
    //zm_numScaleX = zm_numScaleX * _ZMOffsetV4.x;

    half special_1 = step(indexNum - 1, 0.1);
    
    half floatDis = zmItemIndex - midFloatIndex;
    //return half4(zmItemIndex, midFloatIndex, 1, 1) * 0.1;
    half zmDistance = /*(1 - special_1) **/floatDis * /*8*/_ZMUVDistanceScale * stepX;
    
    //return floatDis;
    
  
    half2 circleOrigin = half2(midPosX, 0.5);
    half2 curPosition = half2(x, y);
    half _distance = length(curPosition - circleOrigin);
    half cirleLine = step(abs(_distance - 0.5), 0.05);
   
    half upY = step(circleOrigin.y, y);
    half angle2Deg = 3.14159 / 180;
    half deg2Angle = 180 / 3.14159;
    half angleStep = zmRotateAngle;
    half angleAll = indexNum * angleStep;
    //half angleStep = angleAll / indexNum;
    
    //return abs(zmItemIndex - midFloatIndex) / indexNum;

    //half angleStart = 90 - ((indexNum / 2) * angleStep);
    half angleCur = angleStep * /*floor*/(midFloatIndex - zmItemIndex);
    
    //return half4(zmItemIndex, midFloatIndex, 1, 1) * 0.1;
    
    //angleCur = -30;
    //half zFlag = (0, angleCur);
    //return (zFlag * angleCur + (1 - zFlag) * (-1 * angleCur)) * 0.001;
    
    half atanVal = atan2(y - circleOrigin.y, x - circleOrigin.x);
    half angleVal = atanVal * deg2Angle;
    
    //return zmDistance;
    //zmUvXMove = 0;
    half2 calculUV = float2(zmStaticNumScaleX + zmDistance + zmItemUVOffsetX + zmExtraOffsetX, zm_numScaleY + zmGroupOffsetY);
    
    //������ת
    float2 rotateUv = 0;
    //float deltaAngle =/* -1 * */((90 - angleCur) * angle2Deg); // -1*(curAngle * angle2Deg / 4) ;
    half deltaAngle = (angleCur) * angle2Deg; // -1*(curAngle * angle2Deg / 4) ;
    half2 cleanUV = calculUV - circleOrigin;
    rotateUv.x = cleanUV.x * cos(deltaAngle) - cleanUV.y * sin(deltaAngle);
    rotateUv.y = cleanUV.x * sin(deltaAngle) + cleanUV.y * cos(deltaAngle);
    calculUV = rotateUv + circleOrigin;
    
    
      //����ƽ��
    //half angleIndex = angleVal / angleStep; //floor(angleVal / 60);
    half angleIndex = floor(angleVal / angleStep);
    
    //return half4(angleStart, angleStep, curAngle, 1) * 0.01;
    
    //return circleIn * circleOut * angleIndex * 0.1 /** upY*/;
    half curAngleDeg = angleCur * angle2Deg;
    //(90 - abs(angleCur))*angle2Deg;
    //deltaAngle; //20 + (angleStep * zmItemIndex /*+ angleStart*/);
    
    //return half4(angleStart, angleStep, curAngle, 1) * 0.01;
    //return half4(angleStart * 0.01, 0.9, curAngle * 0.01, 1);
    
    //return (curAngle + 180) * 0.001;
    //half leftAngle = step(curAngle, 0);
    //curAngle = leftAngle * (-1 * curAngle + 90) + (1 - leftAngle) * curAngle;
    
    //half curCosValue = cos(curAngleDeg);
    half zmDistanceX = /*circleOrigin.x + */sin(curAngleDeg);
    //zmDistance = 0;
    //half curSinValue = sin(curAngleDeg);
    //half degFlag = step(0, angleCur);
    half zmDistanceY = -1 * cos(curAngleDeg); // -1*degFlag * (cos(curAngleDeg)) + (1 - degFlag) * (cos(curAngleDeg));
    ///*circleOrigin.y +*/-1*cos(curAngleDeg);
    //half flag = step(3, zmItemIndex);
    //zmDistance = flag * (floatDis * 10 * stepX) + (1 - flag) * (floatDis * 8 * stepX);
    
    half isRight = step(zmItemIndex, midFloatIndex - 0.1);
    half rightVal = abs(zmItemIndex - midFloatIndex);
    //zmUvXMove = 0;
    //half2 worldUV = float2(zm_numScaleX + zmDistance + zmUvXMove + _ZMOffsetV4.w, zm_numScaleY + zmDistanceY + _ZMOffsetV4.y);
    //TODO: ����λ�ƴ�������
    half highDelta = abs(zmItemIndex - midFloatIndex);
    half highVal = highDelta * highDelta * 0.2 + 0.1 * highDelta;
    half2 worldUV = calculUV + half2(zmDistanceX /**0.2*/ * 3.5, zmDistanceY * 3.5 + highVal * 0.07 * zmDeltaYStep /*+ isRight * rightVal * 0.15*/);

    
    half2 zmUV = worldUV * float2(zmItemWidth, 1.0) * zmUVScaleSize;
    
    half zmSwitch = rVal * zmItemWidth;
    
    

   

    half2 finalZMUV = float2(zmSwitch, 0) + zmUV;
    
    
    half numAMask = ceil(frac(saturate(NumUV.r * 10.0))) * ceil(frac(saturate(NumUV.g)));
    
        //return numAMask;

    //��ĸ������
    half zmMask = ceil(frac(saturate(zmUV.r * (zmNum - 1) * 1.0))) * ceil(frac(saturate(zmUV.g)));
    
    //zmMask = cirleLine + zmMask;
    
    zmMask = zmMask * (1 - isOver) * MaskNormal; /** (zmItemIndex + 1) * 0.1*/;
    //zmMask = 1;
    //return zmMask;

    StandardMaterial material = GetMaterial01(input.uv0.xy, finalNumUV, finalZMUV);
    
    half4 Numtex = material.numTex;
    half4 zmTex = material.zmTex;
    
 //   #if defined(TEX2_ON)
	//half4 Numtex = material.numTex2;

 //   #else
 //   half4 Numtex = material.numTex;
	//#endif
    
    // = Numtex * Numtex.a * step(0.1, 1 - ((saturate(dot(input.posOSNormalY.x, input.posOSNormalY.y)) - 0.5) * 2.0 + 0.1)) * numAMask;
    //clip(Numtex.a * numAMask - _NumFrontOffSet.w);
    
   // baseColor = lerp(half4(material.albedo, 1.0), Numtex, Numtex.a * step(0.1, 1 - ((saturate(dot(input.posOSNormalY.x, input.posOSNormalY.y)) - 0.5) * 2.0 + 0.1)) * numAMask);
    half clampVal = saturate(input.posOSNormalY.x * input.posOSNormalY.y);
    
   // return clampVal;
    half compareVal = 1 - ((clampVal - 0.6) * 2.0 + 0.1);
    half isNum = Numtex.a * step(0.1, compareVal) * numAMask;
    half isZM = zmTex.a * step(0.1, compareVal) * zmMask;
    
    baseColor = isZM * zmTex + (1 - isZM) * (isNum * Numtex + (1 - isNum) * half4(material.albedo, 1.0));
    
    material.albedo = baseColor.rgb;
    // Get material properties from texture inputs, convert to world normals
    //StandardMaterial material = GetMaterial(input.uv0.xy);

    half3 normalWS = GetWorldNormal(input, material.normal);
    half3 reflectDir = GetReflectDir(input.normalWS.xyz, viewDirWS, input.positionWS.xyz);
    half3 bakedGI = GetBakedGI(input, normalWS);
    // half3 bakedGI = input.bakeGISH;
    //float3 positionOS = TransformWorldToObject(input.positionWS);
    half4 reflection = 0;

    // Get dot products and shading data
    DotProducts dotProducts = GetDotProducts(mainLight, normalWS, viewDirWS);
    BTDFData btdfData = GetBTDFData02(material, dotProducts, material.alpha);


    // Calculate lighting model
    half3 color;
#if defined(MATCAP_ACCURATE)
    color = LightingIndirect2(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao, reflection);
#else
    color = LightingIndirect(btdfData, bakedGI, dotProducts, normalWS, reflectDir, material.ao, reflection);
#endif

    if (mainLight.distanceAttenuation > 0)
        color += LIGHTING_BRDF(btdfData, dotProducts, mainLight, _ShadowIntensity, _NetLightness, reflectDir) ;
    color += LightingEmissive(material.emission);

#if defined(RIM_MASK)
    half rim = 1 - saturate(dot(viewDirWS, normalWS));
    //trick rim
    float3 lightDir1 = TransformObjectToTangent(float3(0, -1, 0), half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    float3 lightDir2 = TransformWorldToTangent(_Light, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    float resValue1 = RimMask(lightDir1, input.sh_1, float2(_Low, _High) / 2) * _BackLightScale;
    float resValue2 = RimMask(lightDir2, input.sh_1, float2(_Low, _High));
#if defined(VIR_LIT)
    half3 rim2 = _LightColor * btdfData.diffuse * virAtten;
#else
    half3 rim2 = _LightColor;
#endif
    half3 rimColor = (pow(rim, _RimPower) * max(0.0001,_RimScale) * ((resValue1 + resValue2) * material.ao) * rim2);
    color += rimColor;
#endif
#if defined(MATCAP_ACCURATE)
    float3 worldNorm;
    worldNorm.x = dot(input.tSpace0.xyz, material.normal);
    worldNorm.y = dot(input.tSpace1.xyz, material.normal);
    worldNorm.z = dot(input.tSpace2.xyz, material.normal);
    worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
    color += tex2D(_MatCap, worldNorm.xy * 0.5 + 0.5) * material.smoothness;// * half4(4.59479380, 4.59479380, 4.59479380, 2.0);
#endif
    ADDITIONAL_LIGHTS(input, color, btdfData, material.ao, reflectDir);

    color.rgb = GradientAlpha(half4(_YSpeed, _Width, _EdgeWidth, _MaskTexTilling),
        _EdgeColor, input, color, float4(_DownWidth, _Pos, _RimEdge, _RimDis)).rgb;

    color.rgb = FogApply(color, input);
    return float4(color, material.alpha);
}

/////////////////////////////////////////////////
// Unlit PASS //////////////////////////////////
/////////////////////////////////////////////////

VaryingsSimple LitVertUnlit(AttributesSimple input)
{
    OUTPUT_VERTSIMPLE(input)
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
    Varyings LitVert(Attributes input) { return LitVertForward(input); }
    half4 LitFrag (Varyings input) : SV_Target { return LitFragForward(input); }
#endif