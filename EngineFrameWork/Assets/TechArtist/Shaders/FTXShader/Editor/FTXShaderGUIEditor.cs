using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngineInternal;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace FTXUnityEditor
{
    public class FTXShaderGUIEditor : Editor
    {
        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        public enum BlendMode
        {
            Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
            Additive,
            Multiply
        }

        public enum SmoothnessSource
        {
            BaseAlpha,
            SpecularAlpha
        }

        public enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        protected class Styles
        {
            // Catergories
            public static readonly GUIContent SurfaceOptions =
                new GUIContent("Surface Options", "Controls how Universal RP renders the Material on a screen.");

            public static readonly GUIContent SurfaceInputs = new GUIContent("Surface Inputs",
                "These settings describe the look and feel of the surface itself.");

            public static readonly GUIContent AdvancedLabel = new GUIContent("Advanced",
                "These settings affect behind-the-scenes rendering and underlying calculations.");

            public static readonly GUIContent surfaceType = new GUIContent("Surface Type",
                "Select a surface type for your texture. Choose between Opaque or Transparent.");

            public static readonly GUIContent blendingMode = new GUIContent("Blending Mode",
                "Controls how the color of the Transparent surface blends with the Material color in the background.");

            public static readonly GUIContent cullingText = new GUIContent("Render Face",
                "Specifies which faces to cull from your geometry. Front culls front faces. Back culls backfaces. None means that both sides are rendered.");

            public static readonly GUIContent alphaClipText = new GUIContent("Alpha Clipping",
                "Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

            public static readonly GUIContent alphaClipThresholdText = new GUIContent("Threshold",
                "Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

            public static readonly GUIContent receiveShadowText = new GUIContent("Receive Shadows",
                "When enabled, other GameObjects can cast shadows onto this GameObject.");

            public static readonly GUIContent baseMap = new GUIContent("Base Map",
                "Specifies the base Material and/or Color of the surface. If you¡¯ve selected Transparent or Alpha Clipping under Surface Options, your Material uses the Texture¡¯s alpha channel or color.");

            public static readonly GUIContent emissionMap = new GUIContent("Emission Map",
                "Sets a Texture map to use for emission. You can also select a color with the color picker. Colors are multiplied over the Texture.");

            public static readonly GUIContent normalMapText =
                new GUIContent("Normal Map", "Assigns a tangent-space normal map.");

            public static readonly GUIContent bumpScaleNotSupported =
                new GUIContent("Bump scale is not supported on mobile platforms");

            public static readonly GUIContent fixNormalNow = new GUIContent("Fix now",
                "Converts the assigned texture to be a normal map format.");

            public static readonly GUIContent queueSlider = new GUIContent("Priority",
                "Determines the chronological rendering order for a Material. High values are rendered first.");
        }

        public struct FTXLitProperties
        {
            // Surface Option Props
            public MaterialProperty workflowMode;

            // Surface Input Props
            public MaterialProperty metallic;
            public MaterialProperty specColor;
            public MaterialProperty metallicGlossMap;
            public MaterialProperty specGlossMap;
            public MaterialProperty smoothness;
            public MaterialProperty smoothnessMapChannel;
            public MaterialProperty bumpMapProp;
            public MaterialProperty bumpScaleProp;
            public MaterialProperty parallaxMapProp;
            public MaterialProperty parallaxScaleProp;
            public MaterialProperty occlusionStrength;
            public MaterialProperty occlusionMap;

            // Advanced Props
            public MaterialProperty highlights;
            public MaterialProperty reflections;

            public MaterialProperty playerReflections;

            public MaterialProperty clearCoat;  // Enable/Disable dummy property
            public MaterialProperty clearCoatMap;
            public MaterialProperty clearCoatMask;
            public MaterialProperty clearCoatSmoothness;

            public MaterialProperty emissionMapProp;
            public MaterialProperty emissionColorProp;

            public MaterialProperty surfaceTypeProp;
            public MaterialProperty blendModeProp;
            public MaterialProperty cullingProp;
            public MaterialProperty alphaClipProp;
            public MaterialProperty alphaCutoffProp;
            public MaterialProperty receiveShadowsProp;

            // Common Surface Input properties
            public MaterialProperty baseMapProp;
            public MaterialProperty baseColorProp;
            public MaterialProperty queueOffsetProp;

            public MaterialProperty base2MapProp;
            public MaterialProperty base2ColorProp;

            public FTXLitProperties(MaterialProperty[] properties)
            {
                // Surface Option Props
                workflowMode = BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
                // Surface Input Props
                metallic = BaseShaderGUI.FindProperty("_Metallic", properties, false);
                specColor = BaseShaderGUI.FindProperty("_SpecColor", properties, false);
                metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties, false);
                specGlossMap = BaseShaderGUI.FindProperty("_SpecGlossMap", properties, false);
                smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
                smoothnessMapChannel = BaseShaderGUI.FindProperty("_SmoothnessTextureChannel", properties, false);
                bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
                bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);
                parallaxMapProp = BaseShaderGUI.FindProperty("_ParallaxMap", properties, false);
                parallaxScaleProp = BaseShaderGUI.FindProperty("_Parallax", properties, false);
                occlusionStrength = BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
                occlusionMap = BaseShaderGUI.FindProperty("_OcclusionMap", properties, false);
                // Advanced Props
                highlights = BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
                reflections = BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);

                playerReflections = BaseShaderGUI.FindProperty("_PlayerReflections", properties, false);

                clearCoat = BaseShaderGUI.FindProperty("_ClearCoat", properties, false);
                clearCoatMap = BaseShaderGUI.FindProperty("_ClearCoatMap", properties, false);
                clearCoatMask = BaseShaderGUI.FindProperty("_ClearCoatMask", properties, false);
                clearCoatSmoothness = BaseShaderGUI.FindProperty("_ClearCoatSmoothness", properties, false);

                emissionMapProp = BaseShaderGUI.FindProperty("_EmissionMap", properties, false);
                emissionColorProp = BaseShaderGUI.FindProperty("_EmissionColor", properties, false);

                surfaceTypeProp = BaseShaderGUI.FindProperty("_Surface", properties, false);
                blendModeProp = BaseShaderGUI.FindProperty("_Blend", properties, false);
                cullingProp = BaseShaderGUI.FindProperty("_Cull", properties, false);
                alphaClipProp = BaseShaderGUI.FindProperty("_AlphaClip", properties, false);
                alphaCutoffProp = BaseShaderGUI.FindProperty("_Cutoff", properties, false);
                receiveShadowsProp = BaseShaderGUI.FindProperty("_ReceiveShadows", properties, false);
                baseMapProp = BaseShaderGUI.FindProperty("_BaseMap", properties, false);
                baseColorProp = BaseShaderGUI.FindProperty("_BaseColor", properties, false);
                queueOffsetProp = BaseShaderGUI.FindProperty("_QueueOffset", properties, false);

                base2MapProp = BaseShaderGUI.FindProperty("_Base2Map", properties, false);
                base2ColorProp = BaseShaderGUI.FindProperty("_Base2Color", properties, false);
            }
        }

        private const int queueOffsetRange = 50;

        public static void FTXColorProperty(MaterialProperty materialProperty , string label , MaterialEditor materialEditor)
        {
            Rect controlRectForSingleLine = EditorGUILayout.GetControlRect(true, 20f, EditorStyles.layerMaskField);
            FTXShaderGUIEditor.GetRectsForMiniThumbnailField(controlRectForSingleLine, out var thumbRect, out var labelRect);
            EditorGUI.HandlePrefixLabel(controlRectForSingleLine, labelRect, new GUIContent(label), 0, EditorStyles.label);
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            ExtraPropertyAfterTexture(MaterialEditor.GetLeftAlignedFieldRect(controlRectForSingleLine), materialProperty , materialEditor);
        }

        public static void ExtraPropertyAfterTexture(Rect r, MaterialProperty property, MaterialEditor materialEditor)
        {
            if ((property.type == MaterialProperty.PropType.Float || property.type == MaterialProperty.PropType.Color) && r.width > EditorGUIUtility.fieldWidth)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = r.width - EditorGUIUtility.fieldWidth;
                materialEditor.ShaderProperty(r, property, " ");
                EditorGUIUtility.labelWidth = labelWidth;
            }
            else
            {
                materialEditor.ShaderProperty(r, property, string.Empty);
            }
        }
        public static void GetRectsForMiniThumbnailField(Rect position, out Rect thumbRect, out Rect labelRect)
        {
            thumbRect = EditorGUI.IndentedRect(position);
            thumbRect.y -= 0f;
            thumbRect.height = 18f;
            thumbRect.width = 32f;
            float num = thumbRect.x + 30f;
            labelRect = new Rect(num, position.y, thumbRect.x + EditorGUIUtility.labelWidth - num, position.height);
        }

        public static void DoMetallicSpecularArea(FTXLitProperties properties, MaterialEditor materialEditor, Material material)
        {
            string[] smoothnessChannelNames;
            bool hasGlossMap = false;
            if (properties.workflowMode == null ||
                (WorkflowMode)properties.workflowMode.floatValue == WorkflowMode.Metallic)
            {
                hasGlossMap = properties.metallicGlossMap.textureValue != null;
                materialEditor.TexturePropertySingleLine(new GUIContent("Metallic Map", "Sets and configures the map for the Metallic workflow."), properties.metallicGlossMap,
                    hasGlossMap ? null : properties.metallic);

            }
            if (properties.metallicGlossMap.textureValue != null)
            {
                material.EnableKeyword("_METALLICSPECGLOSSMAP");
            }
            else
            {
                material.DisableKeyword("_METALLICSPECGLOSSMAP");
            }
            EditorGUI.indentLevel++;
            DoSmoothness(properties, material);
            EditorGUI.indentLevel--;
        }

        public static void DoSmoothness(FTXLitProperties properties, Material material)
        {
            var opaque = ((BaseShaderGUI.SurfaceType)material.GetFloat("_Surface") ==
                          BaseShaderGUI.SurfaceType.Opaque);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = properties.smoothness.hasMixedValue;
            var smoothness = EditorGUILayout.Slider(new GUIContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface."), properties.smoothness.floatValue, 0f, 1f);
            var occlusionStrength = EditorGUILayout.Slider(new GUIContent("Occlusion"), properties.occlusionStrength.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                properties.smoothness.floatValue = smoothness;
                properties.occlusionStrength.floatValue = occlusionStrength;
            }
            EditorGUI.showMixedValue = false;

            if (properties.smoothnessMapChannel != null) // smoothness channel
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!opaque);
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = properties.smoothnessMapChannel.hasMixedValue;
                var smoothnessSource = (int)properties.smoothnessMapChannel.floatValue;
                //if (opaque)
                //    smoothnessSource = EditorGUILayout.Popup(Styles.smoothnessMapChannelText, smoothnessSource,
                //        smoothnessChannelNames);
                //else
                //    EditorGUILayout.Popup(Styles.smoothnessMapChannelText, 0, smoothnessChannelNames);
                if (EditorGUI.EndChangeCheck())
                    properties.smoothnessMapChannel.floatValue = smoothnessSource;
                EditorGUI.showMixedValue = false;
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        public static void DrawEmissionProperties(FTXLitProperties properties, MaterialEditor materialEditor , Material material, bool keyword)
        {
            var emissive = true;
            var hadEmissionTexture = properties.emissionMapProp.textureValue != null;

            if (!keyword)
            {
                materialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission Map"), properties.emissionMapProp, properties.emissionColorProp, false);
            }
            else
            {
                // Emission for GI?
                emissive = materialEditor.EmissionEnabledProperty();

                EditorGUI.BeginDisabledGroup(!emissive);
                {
                    // Texture and HDR color controls
                    materialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission Map"), properties.emissionMapProp, properties.emissionColorProp, false);
                }
                EditorGUI.EndDisabledGroup();
            }

            // If texture was assigned and color was black set color to white
            var brightness = properties.emissionColorProp.colorValue.maxColorComponent;
            if (properties.emissionMapProp.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                properties.emissionColorProp.colorValue = Color.white;

            // UniversalRP does not support RealtimeEmissive. We set it to bake emissive and handle the emissive is black right.
            if (emissive)
            {
                var oldFlags = material.globalIlluminationFlags;
                var newFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

                if (brightness <= 0f)
                    newFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                if (newFlags != oldFlags)
                    material.globalIlluminationFlags = newFlags;

                if (properties.emissionMapProp.textureValue == null)
                {
                    material.DisableKeyword("_EMISSION");
                }
                else
                {
                    material.EnableKeyword("_EMISSION");
                }
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }

        public static void DrawSurfaceInputs(Material material, FTXLitProperties litProperties, MaterialEditor materialEditor)
        {
            if (litProperties.baseMapProp != null && litProperties.baseColorProp != null) // Draw the baseMap, most shader will have at least a baseMap
            {
                materialEditor.TexturePropertySingleLine(Styles.baseMap, litProperties.baseMapProp, litProperties.baseColorProp);
                // TODO Temporary fix for lightmapping, to be replaced with attribute tag.
                if (material.HasProperty("_MainTex"))
                {
                    material.SetTexture("_MainTex", litProperties.baseMapProp.textureValue);
                    var baseMapTiling = litProperties.baseMapProp.textureScaleAndOffset;
                    material.SetTextureScale("_MainTex", new Vector2(baseMapTiling.x, baseMapTiling.y));
                    material.SetTextureOffset("_MainTex", new Vector2(baseMapTiling.z, baseMapTiling.w));
                }
            }
        }

        public static void DrawSurfaceOptions(Material material, FTXLitProperties litProperties, MaterialEditor materialEditor)
        {
            BaseShaderGUI.DoPopup(Styles.surfaceType, litProperties.surfaceTypeProp, Enum.GetNames(typeof(SurfaceType)), materialEditor);
            if ((SurfaceType)material.GetFloat("_Surface") == SurfaceType.Transparent)
                BaseShaderGUI.DoPopup(Styles.blendingMode, litProperties.blendModeProp, Enum.GetNames(typeof(BlendMode)), materialEditor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = litProperties.cullingProp.hasMixedValue;
            var culling = (RenderFace)litProperties.cullingProp.floatValue;
            culling = (RenderFace)EditorGUILayout.EnumPopup(Styles.cullingText, culling);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.cullingText.text);
                litProperties.cullingProp.floatValue = (float)culling;
                material.doubleSidedGI = (RenderFace)litProperties.cullingProp.floatValue != RenderFace.Front;
            }

            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = litProperties.alphaClipProp.hasMixedValue;
            //var alphaClipEnabled = EditorGUILayout.Toggle(Styles.alphaClipText, litProperties.alphaClipProp.floatValue == 1);
            //if (EditorGUI.EndChangeCheck())
            //    litProperties.alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
            EditorGUI.showMixedValue = false;

            //if (litProperties.alphaClipProp.floatValue == 1)
            //    materialEditor.ShaderProperty(litProperties.alphaCutoffProp, Styles.alphaClipThresholdText, 1);

            if (litProperties.receiveShadowsProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = litProperties.receiveShadowsProp.hasMixedValue;
                var receiveShadows =
                    EditorGUILayout.Toggle(Styles.receiveShadowText, litProperties.receiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                    litProperties.receiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                EditorGUI.showMixedValue = false;
            }
        }

        public static void DrawAdvancedOptions(Material material, FTXLitProperties litProperties, MaterialEditor materialEditor)
        {
            materialEditor.EnableInstancingField();
            DrawQueueOffsetField(litProperties);
        }

        public static void DrawQueueOffsetField(FTXLitProperties litProperties)
        {
            if (litProperties.queueOffsetProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = litProperties.queueOffsetProp.hasMixedValue;
                var queue = EditorGUILayout.IntSlider(Styles.queueSlider, (int)litProperties.queueOffsetProp.floatValue, -queueOffsetRange, queueOffsetRange);
                if (EditorGUI.EndChangeCheck())
                    litProperties.queueOffsetProp.floatValue = queue;
                EditorGUI.showMixedValue = false;
            }
        }
    }
}