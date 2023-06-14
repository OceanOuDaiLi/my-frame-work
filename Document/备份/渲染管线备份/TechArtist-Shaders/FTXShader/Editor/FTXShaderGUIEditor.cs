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
            }
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

    }
}