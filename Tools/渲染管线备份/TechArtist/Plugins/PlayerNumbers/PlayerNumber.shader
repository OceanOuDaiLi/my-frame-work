Shader "FTX/PlayerNumber"
{
	Properties
	{
		[Header(MainTexture)]
		[Toggle(TEX2_ON)] _TEX2("Tex1ToTex2", Float) = 0.0
		_TexColor      ("TexColor", Color) = (1.0, 1.0, 1.0, 1.0)

		_Stencil("Stencil", float) = 5

		[Header(VertexColor)]
		_RScale                     ("_RScale", Float) = 1
        _GScale                     ("_GScale", Float) = 1
        _BScale                     ("_BScale", Float) = 1
        _AScale                     ("_AScale", Float) = 1

        [Header(NumberTexture)]
		_NumTex				("NumTex",2D) = "black"{}
		_NumTex2			("NumTex2",2D) = "black"{}

		_NumberS			("NumberS",float)=0
		_NumberG			("NumberG",float)=0
		_NumberDis			("NumberDis",float)=0

		_NumFrontOffSet		("NumFrontOffSet:x前左右y前上下z前缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumBackOffSet		("NumBackOffSet:x背左右y背上下z背缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumFrontOffSet2    ("NumFrontOffSet2:x前左右y前上下z前缩放w无用",Vector) = (0.0,0.0,1.0,1.0)
		_NumBackOffSet2		("NumBackOffSet2:x背左右y背上下z背缩放w无用",Vector) = (0.0,0.0,1.0,1.0)


	}
	
	SubShader
	{
		Tags{ "RenderType" = "player" "Queue" = "Geometry" "LightMode"="ForwardBase"  }
		
		Pass
		{
			Tags 
            {
                "LightMode" = "UniversalForward"
            }



			Stencil
			{
				Ref [_Stencil]
				Comp GEqual
				Pass Replace
				Fail keep
				ZFail keep
			}

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#pragma multi_compile _ TEX2_ON 
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 normal : NORMAL;
				float4 tangent  : TANGENT;   
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float4 TtoW0   : TEXCOORD2;  
                float4 TtoW1   : TEXCOORD3;  
                float4 TtoW2   : TEXCOORD4;
				float4 posOSNormalY   : TEXCOORD5;//x为模型本地空间Z值，y为模型本地空间顶点法线的法线方向,
				float4 color:TEXCOORD6;
				float4 pos2 : TEXCOORD7;
			};

			uniform TEXTURE2D(_NumTex);
            SAMPLER(sampler_NumTex);

			uniform TEXTURE2D(_NumTex2);
            SAMPLER(sampler_NumTex2);

            SAMPLER(sampler_NomalMap);

			//For SRP Batcher
			CBUFFER_START(UnityPerMaterial)
			uniform half4 _MainTex_ST,_NumTex_ST,_NumTex2_ST,_NumFrontOffSet,_NumBackOffSet;
			uniform half4 _TexColor,_NumFrontOffSet2,_NumBackOffSet2;//_lightColor,_RimColor
			uniform half _NumberS,_NumberG,_NumberDis,_SSSclothIns;//_lightIns,_RimIntesity,_RimRange,
			float _Stencil;
			float _RScale;
			float _GScale;
			float _BScale;
			float _AScale;
			CBUFFER_END


			v2f vert (appdata v)
			{
				v2f o;
				o.posOSNormalY.x = v.vertex.x;
				o.posOSNormalY.y = v.normal.y; 
				o.posOSNormalY.zw = 0; 
				o.color = v.color;

				o.pos=v.vertex;

				o.pos.xyz +=  v.normal.xyz *
				(
					(o.color.r)*(_RScale-1)*0.1+
					(o.color.g)*(_GScale-1)*0.1+
					(o.color.b)*(_BScale-1)*0.1+
					(o.color.a)*(_AScale-1)*0.1
				);

				o.pos = TransformObjectToHClip(o.pos);

				//矩阵向量准备
			    float3 posWS = TransformObjectToWorld(v.vertex);

                float3 nDirWS = TransformObjectToWorldNormal(v.normal);  
				float3 tDirWS = normalize(TransformObjectToWorldDir(v.tangent.xyz));
                float3 bDirWS= normalize(cross(nDirWS, tDirWS) * v.tangent.w);
				       o.TtoW0=float4(tDirWS.x,bDirWS.x,nDirWS.x,posWS.x);
					   o.TtoW1=float4(tDirWS.y,bDirWS.y,nDirWS.y,posWS.y);
					   o.TtoW2=float4(tDirWS.z,bDirWS.z,nDirWS.z,posWS.z);
					   o.uv.zw=v.uv1.xy;
					   o.uv.xy =v.uv.xy;

				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
			    //return half4(i.posOSNormal,1.0);
				//方向向量部分
				half4 bump = (0.5, 0.5, 1.0, 1.0);
				half2 nDirTS1= bump.xy * 2-1;
				half3 nDirTS = float3(nDirTS1.xy,sqrt(1.0-saturate(dot(nDirTS1.xy,nDirTS1.xy)))); 
                half3 nDirWS = normalize(half3(dot(i.TtoW0.xyz,nDirTS),dot(i.TtoW1.xyz,nDirTS),dot(i.TtoW2.xyz,nDirTS)));
				half3 posWS = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
				
				half3 vDirWS = normalize(_WorldSpaceCameraPos.xyz - posWS);
				half3 lDirWS =normalize(vDirWS+float3(0,2,0));
				half3 lrDirWS =reflect(-lDirWS, nDirWS);     
				
				//光照中间向量部分
				half NdotL = dot(nDirWS,lDirWS);
				half ndotv =saturate(dot(nDirWS, vDirWS));
                half vdotr =saturate(dot(vDirWS, lrDirWS));
				half halfLambert = NdotL*0.5+0.6;  

				// 上下侧面环境光部分
				half upMask = max(0.0, nDirWS.g);         
                half downMask = max(0.0, -nDirWS.g);       
                half sideMask = 1.0 - upMask - downMask;   
				half4 numFrontOffset_var=_NumFrontOffSet;
				half4 numBackOffset_var=_NumBackOffSet;


				#if defined(TEX2_ON)
					 _NumFrontOffSet=_NumFrontOffSet2;
					 _NumBackOffSet=_NumBackOffSet2;
				#else
					 _NumFrontOffSet=numFrontOffset_var;
					 _NumBackOffSet=numBackOffset_var;		
				#endif	

				//球衣号码uv操作部分
				half IDUs=_NumberS;																						 //取Number十位作为序列ID1
				half IDVg=_NumberG;																						 //取Number个位作为序列ID2
				half MaskNormal=step(0.1,-i.posOSNormalY.y);
				half FBscale = lerp(1/max(0.01,_NumFrontOffSet.z),1/max(0.01,_NumBackOffSet.z),MaskNormal);				 // 前后背面缩放参数
				half UvXMove = lerp((-_NumFrontOffSet.x),_NumBackOffSet.x,MaskNormal);									 // 前后背面X移动
				half LeftRightMask = step((i.uv.z+(UvXMove*(1.0 /max(0.01,FBscale))*0.5)),0.5);							 // 左右UVx遮罩 字间距
				half sgSwitch = saturate(IDUs);																			 //十位个位缩进切换变量
				half numDistance=sgSwitch*lerp(-0.4-_NumberDis,0.4+_NumberDis,LeftRightMask);							 //文字间距控制
				half numScaleX=(i.uv.z-0.5)*(2.0*FBscale)+0.5;															 //X方向前后背缩放				
				half numMoveY=lerp(0.25*_NumFrontOffSet.y,0.25*_NumBackOffSet.y,MaskNormal);							 //Y方向前后Y移动
				half numScaleY=(i.uv.w+numMoveY-0.5)*FBscale+0.5;														 //前后缩放
				half2 NumUV=float2(numScaleX+numDistance+UvXMove,numScaleY)*float2(0.1,1.0);
				half2 finalNumUV =float2((lerp(IDVg,IDUs,(LeftRightMask*sgSwitch))*0.1),0)+NumUV;
				half  numAMask=ceil(frac(saturate(NumUV.r*10.0)))*ceil(frac(saturate(NumUV.g)));

				//采样贴图
				#if defined(TEX2_ON)
					half4 Numtex = SAMPLE_TEXTURE2D(_NumTex2,sampler_NumTex2, finalNumUV);

				#else
					half4 Numtex = SAMPLE_TEXTURE2D(_NumTex,sampler_NumTex, finalNumUV);
				#endif
				

				//光照计算
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
		
				half phong = pow(max(0.0, vdotr), bump.z) *0.2 ;		

				half3 spec =phong *bump.z;
				half3 baseColor=Numtex * Numtex.a * step(0.1,1-((saturate(dot(i.posOSNormalY.x,i.posOSNormalY.y))-0.5) * 2.0+0.1)) * numAMask  * _TexColor + spec + ambient;

				clip(Numtex.a * numAMask - _NumFrontOffSet.w);
				return half4(baseColor, Numtex.a);

			}

			ENDHLSL
		}
	}

	FallBack "Unlit/Texture"
}
