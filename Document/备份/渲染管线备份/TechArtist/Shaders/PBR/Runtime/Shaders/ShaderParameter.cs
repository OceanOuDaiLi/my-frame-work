using UnityEngine;

namespace FTX
{
	public static class ShaderParameter
	{
		#region Keywords
		public static readonly string useIsFlag = "IS_FLAG";
		public static readonly string useProbes = "USE_PROBES";
		public static readonly string useStandard = "STANDARD_SHADER";
		public static readonly string useVertexColor = "VERTEX_COLOR";
		public static readonly string useEmission = "SHADING_EMISSION";
		public static readonly string useRotationMask = "ROTATION_MASK";
		public static readonly string useNumberPlat = "USE_NUMBER_PLAT";
		public static readonly string useReceiveShadow = "_RECEIVE_SHADOWS_OFF";
		public static readonly string useNumtex = "NUMTEX_ON";
		public static readonly string useNumtex02 = "TEX2_ON";
		public static readonly string useProjectShadow = "_USE_PROJECT_SHADOW";

		#endregion

		#region Material Property Names
		//Common
		public static readonly string uvScale = "_UVScale";
		public static readonly string albedoMap = "_AlbedoMap";
		public static readonly string albedoColor = "_AlbedoColor";

		public static readonly string normalMap = "_NormalMap";
		public static readonly string normalBias = "_NormalBias";
		public static readonly string normalScale = "_NormalScale";

		public static readonly string detailBump = "_DetailBump";
		public static readonly string detailBumpScale = "_DetailBumpScale";

		public static readonly string metallicMap = "_MetallicMap";
		public static readonly string metallic = "_Metallic";
		public static readonly string glossiness = "_Glossiness";
		public static readonly string aoIntensity = "_AOIntensity";
		public static readonly string reflectionNoiseScale = "_ReflectionNoiseScale";

		//Project Shadow∆Ω√Ê“ı”∞
		public static readonly string shadowDirection = "_ShadowDirection";
		public static readonly string shadowAlpha = "_ShadowAlpha";
		public static readonly string shadowFallOff = "_ShadowFalloff";
		public static readonly string shadowPlane = "_ShadowPlane";
		public static readonly string worldPos = "_WorldPos";
		public static readonly string ShadowIntensity01 = "_ShadowIntensity";

		//Number Plat
		public static readonly string stencil = "_Stencil";
		public static readonly string numberS = "_NumberS";
		public static readonly string numberG = "_NumberG";
		public static readonly string numberDis = "_NumberDis";
		public static readonly string numberTexture = "_NumTex";
		public static readonly string numberTexture2 = "_NumTex2";
		public static readonly string numBackOffSet = "_NumBackOffSet";
		public static readonly string numFrontOffSet = "_NumFrontOffSet";
		public static readonly string numBackOffSet2 = "_NumBackOffSet2";
		public static readonly string numFrontOffSet2 = "_NumFrontOffSet2";

		//ShadowIntensity
		public static readonly string shadowIntensity = "_ShadowIntensity";
		public static readonly string skinShadowColor = "_SkinShadowColor";
		public static readonly string lightAttenuation = "_LightAttenuation";

		public static readonly string thicknessMap = "_MetallicMap";
		public static readonly string transparency = "_Transparency";

		//SkinSSS
		public static readonly string BSSRDFLutMap = "_BSSRDFMap";
		public static readonly string thicknessContrast = "_Thicknecurvature ssPower";
		public static readonly string SSSPower = "_SSSPower";
		public static readonly string SSSFactor = "_SSSFactor";
		public static readonly string SSSColor = "_SSSColor";
		public static readonly string thicknessBrightness = "_ThicknessScale";
		public static readonly string translucencyColor = "_TranslucencyColor";
		public static readonly string thicknessAndCurvature = "_ThicknessAndCurvature";

		public static readonly string emissionMap = "_EmissionMap";
		public static readonly string netLightness = "_NetLightness";
		public static readonly string emissionColor = "_EmissionColor";
		public static readonly string emissionEnabled = "_EmissionEnabled";

		//∫≈¬Î≈∆
		//public static readonly string NumberTexture = "_NumTex";
		//public static readonly string NumberTexture2 = "_NumTex2";
		//public static readonly string NumberS = "_NumberS";
		//public static readonly string NumberG = "_NumberG";
		//public static readonly string NumberDis = "_NumberDis";
		


		public static readonly string vertexColorR = "_RColor";
		public static readonly string vertexColorG = "_GColor";
		public static readonly string vertexColorB = "_BColor";
		public static readonly string vertexColorA = "_AColor";

		//Muscles Control
		public static readonly string rScale = "_RScale";
		public static readonly string gScale = "_GScale";
		public static readonly string bScale = "_BScale";
		public static readonly string aScale = "_AScale";

		public static readonly string lodLevel = "_Quality";

		public static readonly string cullMode = "_CullMode";
		public static readonly string blendMode = "_Blend";
		public static readonly string queueOffset = "_QueueOffset";
		#endregion

		#region Material Property IDs
		public static readonly int uvScaleID = Shader.PropertyToID (uvScale);
		#endregion

	}
}