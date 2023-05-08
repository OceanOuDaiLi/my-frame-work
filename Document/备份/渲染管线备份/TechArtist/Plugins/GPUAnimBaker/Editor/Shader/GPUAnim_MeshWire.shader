// 2018.01.09 Code by Lookinglu
// Useage: Editor用 网格显示
// 参考: http://www.cnblogs.com/Esfog/p/UnityGeometryShader_WireFrame.html

Shader "GPUAnim/MeshWire"
{
	Properties 
	{
		[HideInInspector] _WireColor("Wire Color", Color) = (1.0, 0.4, 0.0, 1.0)
		[HideInInspector] _WireSize ("Wire Size", Range(0.01, 5.0)) = 2.0
	}

	Category
    {
		Tags
        {
        	"Queue"="Transparent"
			"IgnoreProjector"="True"
	        "RenderType"="Transparent"
	    }

		Blend Off
	    ColorMask RGB
	    Lighting Off 
        Cull Back

		SubShader
        {
        	Pass
        	{
				CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag
				#pragma target 5.0

				uniform fixed4 _WireColor;
				uniform half _WireSize;

				struct VertexInput {
	                float4 vertex : POSITION;
	                float2 texcoord : TEXCOORD0;
	            };

				struct VertexToGeometry {
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
				};

				struct GeometryOutput {
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
					float3 dist : TEXCOORD1;
				};

	            VertexToGeometry vert (VertexInput v) {
	                VertexToGeometry vo = (VertexToGeometry)0;
	                vo.pos = UnityObjectToClipPos(v.vertex);
	                vo.uv = v.texcoord;
	                return vo;
	            }

				// 限制图元的最大顶点数 小于3的图元将被舍弃 大于3的超过部分的信息将被舍弃
				[maxvertexcount(3)]
				// triangle定义图元以三角形传入 数据结构为VertexToGeometry的数组 长度为3
				// inout TriangleStream定义图元以三角形输出 数据结构为GeometryOutput的数据流
				void geom (triangle VertexToGeometry vo[3], inout TriangleStream<GeometryOutput> triStream) {

					// 取三角形各点在屏幕空间的位置
					float2 screenSize = _ScreenParams.xy;
					float2 p0 = screenSize * vo[0].pos.xy / vo[0].pos.w;
					float2 p1 = screenSize * vo[1].pos.xy / vo[1].pos.w;
					float2 p2 = screenSize * vo[2].pos.xy / vo[2].pos.w;
					// 取各点间的差 可视为将三角形的p2点平移到了原点
					float2 v0 = p2-p1;
					float2 v1 = p2-p0;
					float2 v2 = p1-p0;
					// 计算三角形面积
					float areaMul2 = abs(v1.x * v2.y - v1.y * v2.x);
					// 计算各顶点到对边距离
					float d0 = areaMul2 / length(v0);
					float d1 = areaMul2 / length(v1);
					float d2 = areaMul2 / length(v2);
					// 输出图元
					GeometryOutput go = (GeometryOutput)0;
					go.pos = vo[0].pos;
					go.uv = vo[0].uv;
					go.dist = float3(d0, 0.0, 0.0);
					triStream.Append(go);

					go.pos = vo[1].pos;
					go.uv = vo[1].uv;
					go.dist = float3(0.0, d1, 0.0);
					triStream.Append(go);

					go.pos = vo[2].pos;
					go.uv = vo[2].uv;
					go.dist = float3(0.0, 0.0, d2);
					triStream.Append(go);
				}

	            fixed4 frag(GeometryOutput i) : COLOR {
					// 提取到任意边距离为0的像素 取3边距离值最小值即可
					float dist = min(i.dist.x, min(i.dist.y, i.dist.z));
					// 用指数函数x<0段的图形对dist做映射 得到线框的Mask
 					float lineMask = exp2(-dist * dist * (1.0 / _WireSize));
 					// 输出
					fixed baseColor = fixed4(0.0, 0.0, 0.0, 1.0);
 					return lerp(baseColor, _WireColor, lineMask);		
	            }
				ENDCG
			}
		}
	}
	FallBack Off
}