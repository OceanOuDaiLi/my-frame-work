
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ShaderContentData.cs
	Author:		DaiLi.Ou
	Descriptions: Shader Content Data For GUI Drawing.
*********************************************************************/

using UnityEngine;

namespace TechArtist.Editor
{
    public class ShaderContentData
    {
        // Shader keywords.
        public const string useVertexColor = "Vertex Color";

        #region PBR Default

        // PBR Texture Slots.
        public GUIContent albedoMap = new GUIContent("Albedo Map");
        public GUIContent normalMap = new GUIContent("Normal Map");
        public GUIContent emissionMap = new GUIContent("Emission Map");
        public GUIContent metallicMap = new GUIContent("Metallic Map");

        // Advanced Rendering Paramters.
        public GUIContent cutOff = new GUIContent("CutOff");
        public GUIContent cullMode = new GUIContent("Cull Mode");
        public GUIContent queueOffset = new GUIContent("QueueOffset");

        public GUIContent depthTest = new GUIContent("Depth Test");
        public GUIContent depthWrite = new GUIContent("Depth Write");
        public GUIContent renderQueue = new GUIContent("Render Queue");
        public GUIContent gpuInstancing = new GUIContent("GPU Instancing");

        public GUIContent blendingMode = new GUIContent("Blending Mode",
            "Determineing how URP calculates the color of each pixel of the transparent Material by Blending the material with the background pixels");

        public GUIContent alphaClipText = new GUIContent("Alpha Clipping",
            "Makes your Material act like a Cutout Shader.Use this to create a transparent effect with hard edges between the opaque and transparent areas.");

        public GUIContent alphaClipThresholdText = new GUIContent("Threshold",
            "Threshold,Which appears when you enable Alpha Clipping.All values above your threshold are fully opaque,and all values below your threshold are invisible.");

        public GUIContent receviceShadowText = new GUIContent("Receive Shadow",
            "If enabled,Other GameObjects can cast shadows onto this GameObject.");

        #endregion

        #region Character Skin SSS

        // SSSLut
        public GUIContent sssLutMap = new GUIContent("SSSLut Map");
        // Thickness
        public GUIContent thickness = new GUIContent("Thickness");
        public GUIContent thicknessContrast = new GUIContent("Thickness Contrast");
        public GUIContent thicknessBrightness = new GUIContent("Thickness Brichtness");
        // Curvature
        public GUIContent curvature = new GUIContent("Curvature");
        public GUIContent curvatureContrast = new GUIContent("Curvature Contrast");
        public GUIContent curvatureBrightness = new GUIContent("Curvature Brightness");
        // Thickness & Curvature
        public GUIContent thicknessAndCurvarture = new GUIContent("Thickness And Curvature");
        // Sweat
        public GUIContent sweatMap = new GUIContent("Sweat Map");
        public GUIContent sweatBump = new GUIContent("Sweat Bump Scale");
        public GUIContent sweatSwitch = new GUIContent("Sweat Switch");
        public GUIContent sweatSmoothness = new GUIContent("Sweat Smoothness Scale");

        #endregion

        #region Common String values for drawing gui.
        // content title values.
        public string advanced = "Advanced";

        // content tips values.
        public string[] enableFlagTips = new string[] { "On", "Off" };

        #endregion

        #region Character String values for drawing gui.

        // content surface options title values.
        public string blendMode = "Blend Mode";
        public string renderFace = "Render Face";
        public string surfaceType = "Surface Type";

        #endregion
    }
}
