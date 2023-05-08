using UnityEngine;

namespace FTX
{
	public class ContentShader
	{
		// Shader keywords
		public string useEmission = "Emission";
		public string useVertexColor = "Vertex Color";
		public string useVertexColorAlbedo = "Vertex Color Albedo";
		public string useVertexColorScale = "Vertex Color Scale";

		public string useProbes = "Use Probes";
		public string useOrNormal = "Origin Normal";
		public string useRotationMask = "RotationMask";
		public string userNumberPlat = "Use Number Plat";
		public string useProjectShadow = "Use Project Shadow";

		//Project Shadow
		public GUIContent projectShadowDirection = new GUIContent("Shadow Direction");
		public GUIContent projectShadowLengh = new GUIContent("Shadow Alpha");
		public GUIContent projectShadowFallOff = new GUIContent("Shadow FallOff");

		//Number Plat
		public GUIContent NumberS = new GUIContent("Number S");
		public GUIContent NumberG = new GUIContent("Number G");
		public GUIContent NumberStencil = new GUIContent("Stencil");
		public GUIContent NumberDis = new GUIContent("Number Distance");
		public GUIContent NumberTexture = new GUIContent("Number Texture");
		public GUIContent NumberTexture2 = new GUIContent("Number Texture2");
		public GUIContent NumBackOffSet = new GUIContent("Number Back Offset");
		public GUIContent NumFrontOffSet = new GUIContent("Number Front Offset");
		public GUIContent NumBackOffSet2 = new GUIContent("Number Back Offset2");
		public GUIContent NumFrontOffSet2 = new GUIContent("Number Front Offset2");

		//MatCap
		public string useMatCapAccurate = "MatCap Accurate";
		public GUIContent matCap = new GUIContent ("MatCap");


		// Texture slots
		public GUIContent albedoMap = new GUIContent ("Albedo Map");
		public GUIContent normalMap = new GUIContent ("Normal Map");
		public GUIContent emissionMap = new GUIContent ("Emission Map");
		public GUIContent SSSLutMap = new GUIContent("SSSLut Map");
		public GUIContent SweatMap = new GUIContent("Sweat Map");
		public GUIContent metallicMap = new GUIContent("Metallic Map");

		// Shader parameters
		public GUIContent blank = GUIContent.none;
		public GUIContent uvScale = new GUIContent ("UV Scale");
		public GUIContent metallic = new GUIContent ("Metallic");
		public GUIContent smoothness = new GUIContent ("Glossiness");
		public GUIContent ao = new GUIContent("AO Intensity");
		//public GUIContent specularID = new GUIContent ("Specular ID");
		//public GUIContent specularLUT = new GUIContent ("Specular LUT");
		public GUIContent albedoColor = new GUIContent ("Albedo Color");
		public GUIContent normalScale = new GUIContent ("Normal Scale");
        public GUIContent transmission = new GUIContent("Transmission");
        public GUIContent emissionColor = new GUIContent ("Emission Color");
		public GUIContent netLightness = new GUIContent ("Net Lightness");
		public GUIContent emissionScale = new GUIContent ("Emission Scale");
		public GUIContent shadowIntensity = new GUIContent("Shadow Intensity");
		public GUIContent lightAttenuation = new GUIContent("Light Attenuation");
		public GUIContent skinShadowColor = new GUIContent("Skin Shadow Color");
		//public GUIContent transmissionFactor = new GUIContent ("Transmission Factor");
		//public GUIContent transmissionExponent = new GUIContent ("Transmission Exponent");
		//public GUIContent transmissionDistortion = new GUIContent ("Transmission Distortion");
		public GUIContent transmissionColor = new GUIContent("Transmission Color");
		public GUIContent reflectionNoiseScale = new GUIContent("Reflection Noise Scale");

		

		public GUIContent lodLevel = new GUIContent("LodLevel");
		//SkinSSS
		public GUIContent thickness = new GUIContent("Thickness");
		public GUIContent curvature = new GUIContent("Curvature");
		public GUIContent thicknessContrast = new GUIContent("Thickness Contrast");
		public GUIContent thicknessBrightness = new GUIContent("Thickness Brightness");
		public GUIContent SSSPower = new GUIContent("SSS Power");
		public GUIContent SSSFactor = new GUIContent("SSS Factor");
		public GUIContent SSSColor = new GUIContent("SSS Color");
		public GUIContent sweatBump = new GUIContent("Sweat Bump Scale");
		public GUIContent sweatSmoothness = new GUIContent("Sweat Smoothness Scale");
		public GUIContent sweatSwitch = new GUIContent("Sweat Switch");

		//∫≈¬Î≈∆
		//public GUIContent NumTex = new GUIContent("NumTex");
		//public GUIContent NumTex2 = new GUIContent("NumTex2");


		public GUIContent vertexColorR = new GUIContent("RColor");
		public GUIContent vertexColorG = new GUIContent("GColor");
		public GUIContent vertexColorB = new GUIContent("BColor");
		public GUIContent vertexColorA = new GUIContent("AColor");
		public GUIContent transparency = new GUIContent("Transparency");

		//GSAA
		public GUIContent CURV_MOD = new GUIContent("variance");
		public GUIContent MAX_POW = new GUIContent("Clamping Threshold");
		public GUIContent POW_MOD_MAX = new GUIContent("Gloss Boundaries(X:min,Y:max)");

		//Flag
		public GUIContent flagwaveSpeed = new GUIContent("WaveSpeed");
		public GUIContent flagwaveIntensity = new GUIContent("WaveIntensity");
		public GUIContent flagshadowIntensity = new GUIContent("ShadowIntensity");
		public GUIContent flagwaveRotation = new GUIContent("WaveRotation");

		//Hair
		public GUIContent hairSpecTint = new GUIContent("Anisotropic Color 1");
		public GUIContent hairSpecTint2 = new GUIContent("Anisotropic Color 2");
		public GUIContent hairRange1 = new GUIContent("Anisotropic Range 1");
		public GUIContent hairRange2 = new GUIContent("Anisotropic Range 2");
		public GUIContent hairShift1 = new GUIContent("Anisotropic Shift 1");
		public GUIContent hairShift2 = new GUIContent("Anisotropic Shift 2");
		public GUIContent hairTransmissionColor = new GUIContent("Transmission Color");
		public GUIContent hairTransmissionValue = new GUIContent("Transmission Value");
		public GUIContent hairTransmissionRim = new GUIContent("Transmission Rim");
		public GUIContent hairTransmissionPower = new GUIContent("Transmission Power");
		public GUIContent hairColorDif = new GUIContent("Color Difference");
		public GUIContent hairYChange=new GUIContent("YChagne");

		//Foliage
		public GUIContent fresnelDistortion = new GUIContent("Fresnel Distortion");
		public GUIContent fresnelIntensity = new GUIContent("Fresnel Intensity");
		public GUIContent fresnelPower = new GUIContent("Fresnel Power");
		public GUIContent fresnelColor = new GUIContent("Fresnel Color");
		public GUIContent baseColorillumination = new GUIContent("BaseColor illumination");
		public GUIContent lightingSmoothControl = new GUIContent("Lighting Smooth Control");
		public GUIContent lightOffest = new GUIContent("Light Offest");
		public GUIContent shadowCut = new GUIContent("ShadowCut");
		public GUIContent shadowSmooth = new GUIContent("ShadowSmooth");
		public GUIContent shadowTintColor = new GUIContent("ShadowTintColor");
		public GUIContent tintColor = new GUIContent("TintColor");
		public GUIContent tintColorScale = new GUIContent("TintColorScale");
		public GUIContent translucentScale = new GUIContent("TranslucentScale");

		//RimMask
		public GUIContent backlightScale = new GUIContent("Backlight Scale");
		public GUIContent RimScale = new GUIContent("RimScale");
		public GUIContent RimPower = new GUIContent("RimPower");
		public GUIContent lightColor = new GUIContent("LightColor");
		public GUIContent rimlow = new GUIContent("Rim Low");
		public GUIContent rimhigh = new GUIContent("Rim High");
		public GUIContent useRealLight = new GUIContent("Real light");
		//Water
		public GUIContent albedoMap2 = new GUIContent("Albedo Map2");
		public GUIContent albedo2Flow = new GUIContent("Albedo2 Flow");
		public GUIContent albedoIntensity = new GUIContent("Albedo Intensity");
		public GUIContent albedoContrast = new GUIContent("Albedo Contrast");
		public GUIContent normalMap2 = new GUIContent("Normal Map2");
		public GUIContent normalMap2Strength = new GUIContent("Normal Map2 Strength");
		public GUIContent normalMap2Flow = new GUIContent("Normal Map2 Flow");
		public GUIContent microwaveStrength = new GUIContent("Micro Wave Strength");
		public GUIContent parallaxMap0 = new GUIContent("Parallax Map0");
		public GUIContent parallaxAmount = new GUIContent("Parallax Amount");
		public GUIContent parallaxFlow = new GUIContent("Parallax Flow");
		public GUIContent mirrorColor = new GUIContent("Mirror Color");
		public GUIContent mirrorDepthColor = new GUIContent("Mirror Depth Color");
		public GUIContent mirrorFPOW = new GUIContent("Mirror FPOW");
		public GUIContent mirrorR0 = new GUIContent("Mirror R0");

		//VirtualLight
		public GUIContent litAttenContrast = new GUIContent("LitAttenContrast");
		public GUIContent litAttenInt = new GUIContent("LitAttenInt");
		public GUIContent litAttenSub = new GUIContent("LitAttenSub");

		//Muscles Control
		public GUIContent rScale = new GUIContent("RScale");
		public GUIContent gScale = new GUIContent("GScale");
		public GUIContent bScale = new GUIContent("BScale");
		public GUIContent aScale = new GUIContent("AScale");

		public GUIContent cutOff = new GUIContent("CutOff");
		public GUIContent queueOffset = new GUIContent("QueueOffset");

		// Rendering parameters
		public GUIContent cullMode = new GUIContent ("Cull Mode");
		public GUIContent depthTest = new GUIContent ("Depth Test");
		public GUIContent depthWrite = new GUIContent ("Depth Write");
		public GUIContent renderQueue = new GUIContent ("Render Queue");
		public GUIContent gpuInstancing = new GUIContent ("GPU Instancing");
		public GUIContent blendingMode = new GUIContent("Blending Mode",
				"Controls how the color of the Transparent surface blends with the Material color in the background.");

		public GUIContent alphaClipText = new GUIContent("Alpha Clipping",
				"Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

		public GUIContent alphaClipThresholdText = new GUIContent("Threshold",
			"Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

		public GUIContent receiveShadowText = new GUIContent("Receive Shadows",
				"When enabled, other GameObjects can cast shadows onto this GameObject.");

        public GUIContent numtex_ONTEX = new GUIContent("NumtexON");

		public GUIContent useNumtex02Tex = new GUIContent("UseNum02");

		// Texture channel info
		public string [] channelsAlbedoOnly = new[] { "- Albedo (RGB)"};
		public string [] channelsAlbedo = new [] { "- Albedo (RGB)", "- AO (A)" };
		public string [] channelsAlbedoTransparent = new[] { "- Albedo (RGB)", "- Alpha (A)" };
		public string [] channelsNormal = new[] { "- Normal (RGB)"};
		public string [] channelsMetallicSmoothnessAO = new[] { "- Metallic (R)", "- Smoothness (G)", "- AO (B)" };
		public string[] channelsSpecMaskBackLightMaskAO = new[] { "- SpecMask (R)", "- BackLightMask (G)", "- AO(B)" };
		public string [] channelsNormalMetallic = new [] { "- Normal (RG)", "- Metallic (B)", "- Roughness (A)" };
		public string [] channelsNormalScattering = new [] { "- Normal (RG)", "- Scattering (B)", "- Roughness (A)" };
		public string [] emissionMapChannels = new [] { "- Color (RGB)" };
		public string [] channelsThickness = new[] { "- Thickness (R)", "- Smoothness (G)", "- AO (B)", "- Curvature (A)" };
		public string [] channelsSSSLut = new[] { "- BRDF & Color (RGB)" };
		public string[] channelsNumTex = new[] { "Number Texture" };
		public string[] channelsNumTex2 = new[] { "Number Texture2" };
		public string [] channelsSweat = new[] { "- Sweat Bump (RG)", "- Smoothness (B)" };
		public string [] parallax = new[] { "- Height (RGB)" };

		public string [] enableFlagOptions = new [] { "On", "Off" };
		public string [] matcap = new[] { "- Replace Glossiness to MatCap intensity" };
		public string [] renderQueueOptions = new [] {
			"Default",
			"Alpha Test",
			"Transparent",
			"Skybox",
		};


		// Single string values
		public string id = "ID";
		public string edit = "Edit";
		public string standard = "Standard PBR";
		public string transparent = "Standard PBR Transparent";
		public string cutout = "Standard PBR Transparent Cutout";
		public string advanced = "Advanced";
		public string receiveshadow = "No Receive Shadow";

		public string numtex_ON = "Numtex_ON";
		public string useNumtex02 = "NO useNumtex02";

		public string outline = "Outline Hull";
		public string skinSSS = "Pre-integrated Skin";
		public string hair = "Hair";
		public string foliage = "Foliage";
		public string water = "Water";

		//Gradient alpha
		public GUIContent ySpeed = new GUIContent("YSpeed");
		public GUIContent maskTex = new GUIContent("MaskTex");
		public GUIContent maskTexTilling = new GUIContent("MaskTexTilling");
		public GUIContent width = new GUIContent("Width");
		public GUIContent edgeColor = new GUIContent("EdgeColor");
		public GUIContent edgeWidth = new GUIContent("EdgeWidth");
		public GUIContent DownWidth = new GUIContent("DownWidth");
		public GUIContent pos = new GUIContent("Pos");
		public GUIContent rimEdge = new GUIContent("RimWidth");
		public GUIContent rimDis = new GUIContent("RimGraDis");
	}
}