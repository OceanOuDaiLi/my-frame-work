#ifndef UNIVERSAL_NUMBER_CLOTH_INPUT_INCLUDED
#define UNIVERSAL_NUMBER_CLOTH_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "HLSL/FTXInput.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
    #define _DETAIL
#endif

// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
    half4 _BaseMap_ST;
    half4 _BaseColor;

    half _Metallic;
    half _Smoothness;
    half _OcclusionStrength;
    half _Surface;

    half _MainLightCompensation;

    //号码的具体控�?
    half _NumberS; //第一个字母的偏移控制
    half _NumberG; //第二个字母的偏移控制
    half _NumberDis; //字母间距

    half4 _ZmStrV4 = (0.0, 0.0, 0.0, 0.0);
    half4 _ZmStr2V4 = (0.0, 0.0, 0.0, 0.0);
    half4 _ZmStr3V4 = (0.0, 0.0, 0.0, 0.0);
    half _ZMHigh = 0;
    half _ZMIndexDistance = 1;
    half _ZMGroupOffsetY = 2.5;
    half _ZMRotateAngleStep = 1.5;
    half _ZMExtraOffsetX = 0;
    half _ZMFontSize = 1.0;
    half _ZMDeltaYStep = 1.0;
    half _ZMUVDistanceScale = 8.0;


    half4 _NumFrontOffSet = (0.0, 0.0, 1.0, 1.0);
    half4 _NumBackOffSet = (0.0, 0.0, 1.0, 1.0);
CBUFFER_END

TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);

TEXTURE2D(_NumTex);             SAMPLER(sampler_NumTex);
TEXTURE2D(_NumTex2);            SAMPLER(sampler_NumTex2);
TEXTURE2D(_ZMTex);              SAMPLER(sampler_ZMTex);

#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;

    #ifdef _METALLICSPECGLOSSMAP
        specGloss = SAMPLE_METALLICSPECULAR(uv);
        specGloss.a *= _Smoothness;
    #else // _METALLICSPECGLOSSMAP
        specGloss.rgb = _Metallic.rrr;
        specGloss.a = _Smoothness;
    #endif

    return specGloss;
}

inline half3 NumZM(half4 uv0, float4 posOSNormalY, half3 albedo)
{
    half4 numFrontOffset_var = _NumFrontOffSet;
    half4 numBackOffset_var = _NumBackOffSet;
    
    //第一个号码控置?
    half IDUs = _NumberS;
    //第二个号码?
    half IDVg = _NumberG;
    //判断正反面？?
    half MaskNormal = step(0.1, -posOSNormalY.y) ;
    //缩放尺寸,这个值越大，实际尺寸越小，这里区分了正反面
    half FBscale = lerp(1 / max(0.01, _NumFrontOffSet.z), 1 / max(0.01, _NumBackOffSet.z), MaskNormal);
    //全部字体左右偏移，根据正反面判断
    half UvXMove = lerp((-_NumFrontOffSet.x), _NumBackOffSet.x, MaskNormal);
    //左右判断
    half val = (uv0.z + (UvXMove * (1.0 / max(0.01, FBscale)) * 0.5));
    //判断左右方向
    half LeftRightMask = step(val, 0.5);
    //数字 Clamp，是否需要 两位 数字
    half sgSwitch = saturate(IDUs);
    //数字 间距
    half numDistance = sgSwitch * lerp(-0.4 - _NumberDis, 0.4 + _NumberDis, LeftRightMask);
    //numDistance = numDistance / 2;
    //单个数字 UV 宽度？？
    half numScaleX = (uv0.z - 0.5) * (2.0 * FBscale) + 0.5;
    //垂直偏移
    half numMoveY = lerp(0.25 * _NumFrontOffSet.y, 0.25 * _NumBackOffSet.y, MaskNormal);
    //中心位置？？
    half numScaleY = (uv0.w + numMoveY - 0.5) * FBscale + 0.5;

    //数字? UV
    half2 NumUV = float2(numScaleX + numDistance + UvXMove, numScaleY) * float2(0.1, 1.0);
    //偏移位置
    half numTempX = (IDVg + (LeftRightMask * sgSwitch) * (IDUs - IDVg)) * 0.1;
    //数字UV的最终位置?
    half2 finalNumUV = float2(numTempX, 0) + NumUV;
    
    //字母尺寸
    half ScaleSetting = 4;
    //垂直偏移
    half ZMPositionOffsetY = -0.35;
    //字符进制
    half zmNum = 31 + 1;
    // //0.032258 = 1/31 31个字符
    half zmItemWidth = 0.03229; //0.032258;
    
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

    //总共的位数
    int indexNum = indexNumX + indexNumY + indexNumZ + indexNumW
    + index2NumX + index2NumY + index2NumZ + index2NumW
    + index3NumX + index3NumY + index3NumZ + index3NumW;
    //return half4(indexNumW, indexNum, 1, 1) * 0.1;
    

    //half showZMCount = min(8, indexNum);
    //showZMCount = max(4, indexNum);
    
    //half zmNeedHalfWidth = (indexNum / 2) * 0.1;
    half indexNumCompare = step(indexNum, 4);
    //单个字符尺寸大小
    half zmUVScaleSize = indexNumCompare * 1.2 + (1 - indexNumCompare) * min((1.2 + (indexNum - 4) * 0.2), 2.2);
    
    zmUVScaleSize = zmUVScaleSize * _ZMFontSize;
    
    //单个字符旋转的角度，随距离叠加
    half zmRotateAngle = _ZMRotateAngleStep;
    //全部字母在Y轴上的偏移
    half zmGroupOffsetY = _ZMGroupOffsetY;
    //单个字母平分  长度的UV
    half numFlag = step(9, indexNum);
    half allLen = numFlag * 0.95 + (1 - numFlag) * 0.8;
    half zmItemSize = allLen / indexNum;
    half zmExtraOffsetX = _ZMExtraOffsetX;
    half zmDeltaYStep = _ZMDeltaYStep;
    
    zmItemSize = _ZMIndexDistance * zmItemSize;
    
    
    
    //半长度
    half halfNum = (indexNum / 2);
    halfNum = max(1, halfNum);
    half zmNeedHalfWidth = zmItemSize * halfNum;
    zmNeedHalfWidth = min(0.45, zmNeedHalfWidth);
    //模拟中心位置
    half midPosX = 0.55;
    //设置单个字母偏移位置
    half zmItemUVOffsetX = zmUVScaleSize * (-0.5 + indexNum * 0.0185);
    half maxX = midPosX + zmNeedHalfWidth;
    half minX = midPosX - zmNeedHalfWidth;

    half x = uv0.z;
    half y = uv0.w;
    
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
    
    half isClamp1 = step(zmItemIndex, max(indexNum - 1, 1));
    half isClamp2 = step(0, zmItemIndex);
    half isClamp = isClamp1 * isClamp2;
    half isOver = 1 - isClamp;
    
    //每个half 表达的字母数量
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
    
    
    
    //float maxValH = floor(pow(zmNum, zmItemIndex + 1 - 16)); //26
    //float minValH = floor(pow(zmNum, zmItemIndex - 16)); //1
    
    //half switchW = 1;
    //half value = (switchZ) * floor((Z % maxValZ) / minValZ);
    
    //return (Z % maxValZ) * 0.02;
    //return switchZ*floor((Z % maxValZ) / minValZ) * 0.1;
    
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
    //27进制
    half rVal = zmValue - 1;
    
    //return rVal * 0.03;
    

    int zmStep = floor(midIndex - zmItemIndex);
    //x 方向上的偏移
    //half zmUvXMove = zmItemUVOffsetX; /** ScaleSetting */ /*+ _ZMOffsetV4.w*/; // * 5 * zmStep;
    
    //half zmDis = 0.1;
    
    half zmStaticNumScaleX = (uv0.z - 0.5) * (2 * ScaleSetting) + 0.5 /*+ _ZMOffsetV4.w*/;
    
    //half zmDistance = zmStep * 0.5 * stepX;
    
    //return zmDistance+0.5;

    //Y中心位置？？
    half zm_numScaleY = (uv0.w + ZMPositionOffsetY - 0.5) * ScaleSetting + 0.5;
    
    //缩放大小
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
    
    //计算旋转
    float2 rotateUv = 0;
    //float deltaAngle =/* -1 * */((90 - angleCur) * angle2Deg); // -1*(curAngle * angle2Deg / 4) ;
    half deltaAngle = (angleCur) * angle2Deg; // -1*(curAngle * angle2Deg / 4) ;
    half2 cleanUV = calculUV - circleOrigin;
    rotateUv.x = cleanUV.x * cos(deltaAngle) - cleanUV.y * sin(deltaAngle);
    rotateUv.y = cleanUV.x * sin(deltaAngle) + cleanUV.y * cos(deltaAngle);
    calculUV = rotateUv + circleOrigin;
    
    
    //计算平移
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
    //TODO: 上下位移存在问题
    half highDelta = abs(zmItemIndex - midFloatIndex);
    half highVal = highDelta * highDelta * 0.2 + 0.1 * highDelta;
    half2 worldUV = calculUV + half2(zmDistanceX /**0.2*/ * 3.5, zmDistanceY * 3.5 + highVal * 0.07 * zmDeltaYStep /*+ isRight * rightVal * 0.15*/);

    
    half2 zmUV = worldUV * float2(zmItemWidth, 1.0) * zmUVScaleSize;
    
    half zmSwitch = rVal * zmItemWidth;

    half2 finalZMUV = float2(zmSwitch, 0) + zmUV;

    half numAMask = ceil(frac(saturate(NumUV.r * 10.0))) * ceil(frac(saturate(NumUV.g)));
    
    //return numAMask;

    //字母的遮罩
    half zmMask = ceil(frac(saturate(zmUV.r * (zmNum - 1) * 1.0))) * ceil(frac(saturate(zmUV.g)));
    
    //zmMask = cirleLine + zmMask;
    
    zmMask = zmMask * (1 - isOver) * MaskNormal; /** (zmItemIndex + 1) * 0.1*/;
    //zmMask = 1;
    //return zmMask;
    
    half4 numCol = SAMPLE_TEXTURE2D(_NumTex, sampler_NumTex, finalNumUV);
    half4 numCol2 = SAMPLE_TEXTURE2D(_NumTex2, sampler_NumTex2, finalNumUV);
    half4 Numtex = numCol;//lerp(numCol, numCol2, isFront);
    half4 zmTex = SAMPLE_TEXTURE2D(_ZMTex, sampler_ZMTex, finalZMUV);

    //??
    half clampVal = saturate(posOSNormalY.x * posOSNormalY.y);
    
    // return clampVal;
    half compareVal = 1 - ((clampVal - 0.6) * 2.0 + 0.1);
    
    //return compareVal;

    half isNum = Numtex.a * step(0.1, compareVal) * numAMask;

    half isZM = zmTex.a * step(0.1, compareVal) * zmMask;
    
    //return zmMask;

    //material.albedo 是球衣和皮肤着色结果 ?     NumTex是号码颜色?

    half4 baseColor = isZM * zmTex + (1 - isZM) * (isNum * Numtex + (1 - isNum) * half4(albedo, 1.0));

    //baseColor = lerp(half4(material.albedo, 1.0), Numtex, lerpVal);
    //material.albedo = zmTex.rgb;
    return baseColor.rgb;
}

inline void InitializeStandardLitSurfaceData(half4 uv, out FTXSurfaceData outFTXSurfaceData, float4 posOSNormalY)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outFTXSurfaceData.alpha = albedoAlpha.a * _BaseColor.a;
    half3 albedo = albedoAlpha.rgb;
    outFTXSurfaceData.albedo = NumZM(uv, posOSNormalY, albedo) * _BaseColor.rgb;

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);

    outFTXSurfaceData.metallic = 0.0h;

    outFTXSurfaceData.smoothness = specGloss.a;

    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
    outFTXSurfaceData.normalTS = UnpackNormal(n);
    //outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

    #if defined(SHADER_API_GLES)
        outFTXSurfaceData.occlusion = specGloss.g;
    #else
        outFTXSurfaceData.occlusion = LerpWhiteTo(specGloss.g, _OcclusionStrength);
    #endif

    outFTXSurfaceData.emission = 0.0h;//SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));

}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
