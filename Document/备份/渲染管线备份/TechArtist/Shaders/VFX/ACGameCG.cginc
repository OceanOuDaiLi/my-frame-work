#ifndef ACGAME_CG_INCLUDED
#define ACGAME_CG_INCLUDED
#include "UnityStandardUtils.cginc"
#include "AutoLight.cginc"
#include "UnityInstancing.cginc"


//nature
sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _NormalTex;
float4 _NormalTex_ST;
sampler2D _HeightTex;
float4 _HeightTex_ST;
sampler2D _RampTex;
float4 _RampTex_ST;

//fixed4 _BackColor;
fixed4 _GradientColor;
fixed _HalfLambertValue;
float _NormalFactor;
fixed _HeightFactor;
fixed4 _FogStartColor;
fixed4 _FogMiddleColor;
fixed4 _FogEndColor;
fixed4 _FogTopColor;
float _FogStart;
float _FogMiddle;
float _FogEnd;
float _FogBottom;
float _FogTop;
fixed4 _LightColor0;
fixed _BackBrightness;
fixed _Strength;


//effect
sampler2D _DissolveTex;
float4 _DissolveTex_ST;
sampler2D _MaskTex;
float4 _MaskTex_ST;


//GPU Instancing Properties.
UNITY_INSTANCING_BUFFER_START(ACGAME)

	//nature.
	UNITY_DEFINE_INSTANCED_PROP(fixed4,_BackColor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_GradientColor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_HalfLambertValue)
	//UNITY_DEFINE_INSTANCED_PROP(float,_NormalFactor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_HeightFactor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_FogStartColor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_FogMiddleColor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_FogEndColor)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_FogTopColor)
	//UNITY_DEFINE_INSTANCED_PROP(float,_FogStart)
	//UNITY_DEFINE_INSTANCED_PROP(float,_FogMiddle)
	//UNITY_DEFINE_INSTANCED_PROP(float,_FogEnd)
	//UNITY_DEFINE_INSTANCED_PROP(float,_FogBottom)
	//UNITY_DEFINE_INSTANCED_PROP(float,_FogTop)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_BackBrightness)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_Strength)

	//effect.
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_Cutoff)
	//UNITY_DEFINE_INSTANCED_PROP(half,_Dissolve)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_DissolveToggle)
	//UNITY_DEFINE_INSTANCED_PROP(half,_Mask)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_MaskToggle)
	//UNITY_DEFINE_INSTANCED_PROP(float,_Power)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_SpeedToggle)
	//UNITY_DEFINE_INSTANCED_PROP(float,_SpeedX)
	//UNITY_DEFINE_INSTANCED_PROP(float,_SpeedY)
	//UNITY_DEFINE_INSTANCED_PROP(float,_SpeedX2)
	//UNITY_DEFINE_INSTANCED_PROP(float,_SpeedY2)
	//UNITY_DEFINE_INSTANCED_PROP(half,_UVRotate)
	//UNITY_DEFINE_INSTANCED_PROP(fixed4,_MaskSecUVOffset)
	//UNITY_DEFINE_INSTANCED_PROP(float,_Rotate)
	UNITY_DEFINE_INSTANCED_PROP(fixed4,_Color)
	UNITY_DEFINE_INSTANCED_PROP(fixed4,_Color2)
	UNITY_DEFINE_INSTANCED_PROP(fixed4,_CeilColorA)
	UNITY_DEFINE_INSTANCED_PROP(fixed4,_CeilColorB)
	//UNITY_DEFINE_INSTANCED_PROP(fixed,_GradientValue)

UNITY_INSTANCING_BUFFER_END(ACGAME)

fixed _Cutoff;
half _Dissolve;
fixed _DissolveToggle;
half _Mask;
fixed _MaskToggle;
float _Power;
fixed _SpeedToggle;
float _SpeedX;
float _SpeedY;
float _SpeedX2;
float _SpeedY2;
half _UVRotate;
fixed4 _MaskSecUVOffset;
float _Rotate;
//fixed4 _Color;
//fixed4 _Color2;
//fixed4 _CeilColorA;
//fixed4 _CeilColorB;
fixed _GradientValue;


//nature
struct nature_in_base
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
	float2 uv2 : TEXCOORD1;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct nature_in_normal
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv2 : TEXCOORD1;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct nature_v2f_base
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 worldNormal : TEXCOORD1;
	LIGHTING_COORDS(2,3)
	float4 worldPos : TEXCOORD4;
	half2 uvLM : TEXCOORD5;
	UNITY_FOG_COORDS(6)
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct nature_v2f_normal
{
	float4 pos : SV_POSITION;
	//float2 uv : TEXCOORD0;
	half4 uv : TEXCOORD0;
	float4 worldPos : TEXCOORD1;
	float3 normalDir : TEXCOORD2;
	float3 tangentDir : TEXCOORD3;
	float3 bitangentDir : TEXCOORD4;
	LIGHTING_COORDS(5,6)
	//half2 uvLM : TEXCOORD7;
	UNITY_FOG_COORDS(7)
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
//effect
struct effect_in_base
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
	float4 control : TEXCOORD1;
	fixed4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_in_full
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
	float4 control : TEXCOORD1;
	float3 normal : NORMAL;
	fixed color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_v2f_base
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	half2 uv : TEXCOORD0;
	float4 control : TEXCOORD1;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_v2f_mask
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	half4 uv : TEXCOORD0;
	float4 control : TEXCOORD1;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_v2f_diss
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	half4 uv : TEXCOORD0;//xy:uv;  zw:dissolve or mask
	float4 control : TEXCOORD1;//xy:main's uv curve;  zw:dissolve's or mask's uv curve
	half2 uv1 : TEXCOORD2;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_v2f_dima
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;//xy:uv;  zw:dissolve or mask
	float4 control : TEXCOORD1;//xy:main's uv curve;  zw:dissolve's or mask's uv curve
	half4 uv1 : TEXCOORD2;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct effect_v2f_full
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;//xy:uv;  zw:dissolve and mask
	float4 control : TEXCOORD1;//xy:main's uv curve;  zw:dissolve's or mask's uv curve
	float2 control2 : TEXCOOED2;//x:distortion;  y:unknown
	half4 uv1 : TEXCOORD2;
	fixed3 normalDir : TEXCOORD3;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

//nature
nature_v2f_base natureVertexBase (nature_in_base v)
{
	nature_v2f_base o = (nature_v2f_base)0;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.worldNormal = UnityObjectToWorldNormal(v.normal);
	o.worldPos = mul(unity_ObjectToWorld,v.vertex);
	o.uvLM = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
	UNITY_TRANSFER_FOG(o,o.pos);
	TRANSFER_VERTEX_TO_FRAGMENT(o)
	return o;
}
nature_v2f_normal natureVertexNormal (nature_in_normal v)
{
	nature_v2f_normal o = (nature_v2f_normal)0;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);	//#if FOGGING
	o.worldPos = mul(unity_ObjectToWorld,v.vertex);
	o.normalDir = UnityObjectToWorldNormal(v.normal);
	o.tangentDir = normalize(mul( unity_ObjectToWorld,float4(v.tangent.xyz,0.0)).xyz);
	o.bitangentDir = normalize(cross(o.normalDir,o.tangentDir)*v.tangent.w);
	//o.uvLM = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
	o.uv.zw = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
	TRANSFER_VERTEX_TO_FRAGMENT(o)
	return o;
}
inline half2 CaculateParallaxUV(half2 uv,float3 sourceViewDir)
{
	fixed height = tex2D(_HeightTex,uv).r;
	fixed3 viewDir = normalize(sourceViewDir);
	half2 offset = viewDir.xy/viewDir.z*height*_HeightFactor;
	return offset;
}
inline float GetShadowByLightMap(float3 worldPos,float attenuation,half2 uvLM)
{
	float shadow = 0;
	#if defined (SHADOWS_SHADOWMASK)
		fixed shadowmask = UNITY_SAMPLE_TEX2D(unity_ShadowMask, uvLM.xy).r;
	#else
		fixed shadowmask = 1;
	#endif
	float zDist = dot(_WorldSpaceCameraPos - worldPos, UNITY_MATRIX_V[2].xyz);
	float fadeDist = UnityComputeShadowFadeDistance(worldPos,zDist);
	shadow = UnityMixRealtimeAndBakedShadows(attenuation,shadowmask,UnityComputeShadowFade(fadeDist));
	return shadow;
}
inline fixed3 ACGameDiffuse(fixed3 col,half3 worldNormal,float3 worldPos,float attenuation,half2 uvLM)
{
	fixed3 color = fixed3(0,0,0);
	float atten = GetShadowByLightMap(worldPos,attenuation,uvLM);
	fixed3 lightColor = _LightColor0.rgb * atten;
	fixed3 normalDir = normalize(worldNormal);
	half nl = max(_BackBrightness,dot(normalDir,_WorldSpaceLightPos0.xyz));
	nl = nl * _HalfLambertValue+(1-_HalfLambertValue);
	half backNL = max(0,dot(normalDir,-_WorldSpaceLightPos0.xyz));
	color.rgb = (col.rgb * nl +  UNITY_ACCESS_INSTANCED_PROP(ACGAME,_BackColor).rgb * backNL) * lightColor.rgb;
	//#if defined (LIGHTMAP)
	#ifdef LIGHTMAP_ON
		fixed4 light = UNITY_SAMPLE_TEX2D(unity_Lightmap,uvLM);
		color += col.rgb * DecodeLightmap(light);
	#else
		color += ShadeSH9 (half4(normalDir, 1.0)) * col.rgb;
	#endif
	return color;
}
inline fixed3 ACGameDiffuse(fixed3 col,nature_v2f_normal i)
{
	fixed3 color = fixed3(0,0,0);
	float attenuation = LIGHT_ATTENUATION(i);
	//float atten = GetShadowByLightMap(i.worldPos,attenuation,i.uvLM);
	float atten = GetShadowByLightMap(i.worldPos,attenuation,i.uv.zw);
	fixed3 lightColor = _LightColor0.rgb * atten;
	float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, normalize(i.normalDir));
	float3 tangentNormal = UnpackNormal(tex2D(_NormalTex,i.uv));
	float3 normalFactor = float3(_NormalFactor,_NormalFactor,1);
	fixed3 normalDir = mul(normalize(tangentNormal * normalFactor),tangentTransform);
	//fixed3 normalDir = normalize(mul(tangentNormal,tangentTransform));
	half nl = max(_BackBrightness,dot(normalDir,_WorldSpaceLightPos0.xyz));
	nl = nl * _HalfLambertValue+(1-_HalfLambertValue);
	half backNL = max(0,dot(normalDir,-_WorldSpaceLightPos0.xyz));
	color.rgb = (col.rgb * nl +  UNITY_ACCESS_INSTANCED_PROP(ACGAME,_BackColor).rgb * backNL) * lightColor.rgb;
	//#if defined (LIGHTMAP)
	#ifdef LIGHTMAP_ON
		//fixed4 light = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
		fixed4 light = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uv.zw);
	  color += col.rgb * DecodeLightmap(light);
	#else
		color += ShadeSH9 (half4(normalDir, 1.0)) * col.rgb;
	#endif
	return color;
}
inline fixed3 ACGameDiffuseByRamp(float3 worldNormal)
{
	fixed3 color = fixed3(0,0,0);
	half nl = max(_BackBrightness,dot(worldNormal,_WorldSpaceLightPos0.xyz));
	fixed3 ramp = tex2D(_RampTex,TRANSFORM_TEX(half2(nl,nl),_RampTex)).rgb;
	color.rgb = ramp.rgb * _LightColor0.rgb;
	return color;
}
inline fixed3 ACGameDiffuseByRamp(half2 uv,fixed3 lightDir)
{
	fixed3 color = fixed3(0,0,0);
	float3 normal = UnpackNormal(tex2D(_NormalTex,uv));
	normal = normal.xyz * float3(_NormalFactor,_NormalFactor,1);
	fixed3 normalDir = normalize(normal);
	half nl = max(_BackBrightness,dot(normalDir,lightDir));
	fixed3 ramp = tex2D(_RampTex,TRANSFORM_TEX(half2(nl,nl),_RampTex)).rgb;
	color.rgb = ramp.rgb * _LightColor0.rgb;
	return color;
}
inline fixed3 ACGameFog(float3 worldPos,fixed3 color)
{
	float localDistance = length(worldPos.xyz - _WorldSpaceCameraPos.xyz);
	float fogForeDistance = _FogMiddle - _FogStart;
	float fogBackDistance = _FogEnd - _FogMiddle;
	fixed lerpFore = saturate((_FogMiddle - localDistance)/fogForeDistance);
	fixed lerpBack = saturate((_FogEnd - localDistance)/fogBackDistance);
	fixed3 _FogForeColor = lerp(_FogMiddleColor.rgb,_FogStartColor.rgb,lerpFore) * (localDistance < _FogMiddle);
	fixed3 _FogBackColor = lerp(_FogEndColor.rgb,_FogMiddleColor.rgb,lerpBack) * (localDistance >= _FogMiddle);
	fixed3 _FogColor =  _FogForeColor + _FogBackColor;
	fixed fogCoord = saturate((_FogEnd - localDistance)/(_FogEnd - _FogStart));
	color.rgb = lerp(_FogColor.rgb,color.rgb,fogCoord);
	float altitudeLerp = saturate((worldPos.y - _FogBottom)/(_FogTop - _FogBottom)) ;
	color.rgb = lerp(color.rgb,_FogTopColor,altitudeLerp * (1 - fogCoord));
	return color.rgb;
}

inline fixed3 ACGameFogAdd(float3 worldPos,fixed3 color)
{
	float localDistance = length(worldPos.xyz - _WorldSpaceCameraPos.xyz);
	float fogDistance = _FogEnd - _FogStart;
	float fogCoord = saturate((_FogEnd - localDistance)/fogDistance);
	color.rgb *= fogCoord;
	return color.rgb;
}

//when equal
inline fixed When_EQ(half x, half y)
{
	return 1.0 - abs(sign(x - y));
}
//when not equal
inline fixed When_NEQ(half x,half y)
{
	return abs(sign(x - y));
}
//when greater than
inline fixed When_GT(half x,half y)
{
	return max(sign(x - y), 0.0);
}
//when less than
inline fixed When_LT(half x,half y)
{
	return max(sign(y - x), 0.0);
}
//when greater than or equal to
inline fixed When_GE(half x,half y)
{
	return 1.0 - When_LT(x , y);
}
//when less than or equal to
inline fixed When_LE(half x,half y)
{
	return 1.0 - When_GT(x , y);
}
//and
inline fixed And(fixed a, fixed b)
{
	return a * b;
}
//flip texture uv
inline half2 FlipUV(half2 sourceUV)
{
	half2 uv = sourceUV;
	fixed value1 = And(When_LT(_UVRotate,2.0), When_GE(_UVRotate,1.0));
	fixed value2 = And(When_LT(_UVRotate,3.0), When_GE(_UVRotate,2.0));
	fixed value3 = When_GE(_UVRotate,3.0);
	if(value1)
	{
		uv.x = 1.0 - sourceUV.y;
		uv.y = sourceUV.x;
	}
	else if(value2)
	{
		uv.x = 1.0 - sourceUV.x;
		uv.y = 1.0 - sourceUV.y;
	}
	else if(value3)
	{
		uv.x = sourceUV.y;
		uv.y = 1.0 - sourceUV.x;
	}
	return uv;
}
//rotate texture uv
inline half2 RotateUV(half2 uv)
{
	half2 rotateUV = uv.xy - half2(0.5f,0.5f);
	half s =sin(_Rotate/57.2957796);
	half c = cos(_Rotate/57.2957796);
	rotateUV = half2( rotateUV.x *c -rotateUV.y*s, rotateUV.x*s + rotateUV.y*c);
	rotateUV += half2(0.5f,0.5f);
	return rotateUV;
}
inline fixed4 CeilColor(fixed powValue,fixed powControl)
{
	fixed4 color = lerp(UNITY_ACCESS_INSTANCED_PROP(ACGAME,_CeilColorA),UNITY_ACCESS_INSTANCED_PROP(ACGAME,_CeilColorB),saturate(ceil(pow(powValue,_Power * powControl))));
	return color;
}
//set color.a by dissolve texture and control value
inline fixed AlphaByDissolve(half2 uv,fixed control)
{
	fixed dissolve = tex2D(_DissolveTex,uv).r;
	fixed alpha = saturate(dissolve - control);
	return alpha;
}
//set color.a by particle.alpha and control value
inline fixed AlphaByDissolve(fixed source,fixed control)
{
	fixed alpha = saturate(source - control);
	return alpha;
}
//get uv by time and speed
//source UV,custom datas,uniform data.
inline half2 GetAnimationUV(half2 sourceUV,half2 sourceSpeed,half2 baseSpeed)
{
	half2 uv = sourceUV;
	//half2 speed = lerp(half2(_Time.y,_Time.y),sourceSpeed,_SpeedToggle);
	half2 speed = lerp(half2(_Time.y,_Time.y),sourceSpeed,_SpeedToggle);
	uv.x = uv.x + baseSpeed.x * speed.x;
	uv.y = uv.y + baseSpeed.y * speed.y;
	return uv;
}
/*
inline half2 GetAnimationUV(half2 sourceUV,half2 sourceSpeed)
{
	half2 uv = sourceUV;
	half2 speed = lerp(half2(_Time.y,_Time.y),sourceSpeed,_SpeedToggle);
	uv.x = uv.x + _SpeedX * speed.x;
	uv.y = uv.y + _SpeedY * speed.y;
	return uv;
}
inline half2 GetAnimationUV2(half2 sourceUV)
{
	half2 uv = sourceUV;
	uv.x = uv.x + _SpeedX2 * _Time.y;
	uv.y = uv.y + _SpeedY2 * _Time.y;
	return uv;
}
*/
//base vert
effect_v2f_base effectVertexBase(effect_in_base v)
{
	effect_v2f_base o = (effect_v2f_base)0;

	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v,o);

	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
	o.uv.xy = FlipUV(o.uv.xy);
	o.color.rgba = v.color.rgba;
	o.control.xyzw = v.control.xyzw;
	o.vertex.z += _NormalFactor;
	return o;
}
//mask texture
/*
effect_v2f_diss effectVertexMask(effect_in_base v)
{
	effect_v2f_diss o = (effect_v2f_diss)0;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
	o.uv.xy = FlipUV(o.uv.xy);
	o.uv.zw = v.texcoord.zw;
	o.uv1.xy = TRANSFORM_TEX(v.texcoord.xy,_MaskTex);
	o.color.rgba = v.color.rgba;
	o.control.xyzw = v.control.xyzw;
	return o;
}
*/
//mask texture
//dissovle texture
effect_v2f_diss effectVertexDiss(effect_in_base v)
{
	effect_v2f_diss o = (effect_v2f_diss)0;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
	o.uv.xy = FlipUV(o.uv.xy);
	o.uv.zw = v.texcoord.zw;//dissolve and mask
	o.uv1.xy = TRANSFORM_TEX(v.texcoord.xy,_DissolveTex);
	o.color.rgba = v.color.rgba;
	o.control.xyzw = v.control.xyzw;//xy : main texture,zw : dissolve or mask texture
	o.vertex.z += _NormalFactor;
	return o;
}
//dissolve and mask texture
effect_v2f_dima effectVertexDima(effect_in_base v)
{
	effect_v2f_dima o = (effect_v2f_dima)0;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
	o.uv.xy = FlipUV(o.uv.xy);
	o.uv.zw = v.texcoord.zw;
	o.uv1.xy = TRANSFORM_TEX(v.texcoord.xy,_DissolveTex);
	o.uv1.zw = TRANSFORM_TEX(v.texcoord.xy,_MaskTex);
	o.color.rgba = v.color.rgba;
	o.control.xyzw = v.control.xyzw;
	return o;
}
/*
effect_v2f_full effectVertexFull(effect_in_base v)
{
	effect_v2f_full o = (effect_v2f_full)0;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
	o.uv.xy = FlipUV(o.uv.xy);
	o.uv.zw = v.texcoord.zw;
	o.color.rgba = v.color.rgba;
	o.uv1.xy = TRANSFORM_TEX(v.texcoord.xy,_DissolveTex);
	o.uv1.zw = TRANSFORM_TEX(v.texcoord.xy,_MaskTex);
	return o;
}
*/

#endif
