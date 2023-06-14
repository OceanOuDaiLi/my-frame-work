using FTXUnityEditor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class SkinSSSShader : BaseShaderGUI
    {
        private LitGUI.LitProperties litProperties;
        private bool skinAddArea = true;

        private MaterialProperty SSSMapProp = new MaterialProperty();
        private MaterialProperty SSSColorProp = new MaterialProperty();
        private MaterialProperty SSSPowerProp = new MaterialProperty();
        private MaterialProperty SSSFactorProp = new MaterialProperty();

        private MaterialProperty rimMaskProp = new MaterialProperty();
        private MaterialProperty rimColorProp = new MaterialProperty();
        private MaterialProperty rimSmothnessProp = new MaterialProperty();
        private MaterialProperty rimWidthProp = new MaterialProperty();

        private MaterialProperty transmissionWidthProp = new MaterialProperty();
        private MaterialProperty transmissionRampProp = new MaterialProperty();
        private MaterialProperty transmissionIndencityProp = new MaterialProperty();
        private MaterialProperty transmissionColorProp = new MaterialProperty();
        //private LitDetailGUI.LitProperties litDetailProperties;
        //private SavedBool m_DetailInputsFoldout;

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);
            
        }

        public override void DrawAdditionalFoldouts(Material material)
        {
            skinAddArea = EditorGUILayout.BeginFoldoutHeaderGroup(skinAddArea, new GUIContent("Skin"));
            if (skinAddArea)
            {
                //SSS
                materialEditor.TexturePropertySingleLine(new GUIContent("SSS LUT Map"), SSSMapProp, SSSColorProp);
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(SSSPowerProp, "SSS Power");
                materialEditor.RangeProperty(SSSFactorProp, "SSS Factor");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                //Rim
                materialEditor.TexturePropertyWithHDRColor(new GUIContent("Rim Mask Mask"), rimMaskProp, rimColorProp,false);
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(rimSmothnessProp, "Rim Smoothnewss");
                materialEditor.RangeProperty(rimWidthProp, "Rim Width");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                FTXShaderGUIEditor.FTXColorProperty(transmissionColorProp, "Transmission Color", materialEditor);
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(transmissionWidthProp, "Transmission Width");
                materialEditor.RangeProperty(transmissionRampProp, "Transmission Ramp");
                materialEditor.RangeProperty(transmissionIndencityProp, "Transmission Indencity");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);

            SSSMapProp = FindProperty("_SSSLutMap", properties, false);
            SSSColorProp = FindProperty("_SSSColor", properties, false);
            SSSPowerProp = FindProperty("_SSSPower", properties, false);
            SSSFactorProp = FindProperty("_SSSFactor", properties, false);

            rimMaskProp = FindProperty("_RimMaskMap", properties, false);
            rimColorProp = FindProperty("_RimColor", properties, false);
            rimSmothnessProp = FindProperty("_RimSmoothness", properties, false);
            rimWidthProp = FindProperty("_RimWidth", properties, false);

            transmissionWidthProp = FindProperty("_TransmissionWidth", properties, false);
            transmissionRampProp = FindProperty("_TransmissionRamp", properties, false);
            transmissionIndencityProp = FindProperty("_TransmissionIndencity", properties, false);
            transmissionColorProp = FindProperty("_TransmissionColor", properties, false);
        }

        // material changed check
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            //SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            //if (litProperties.workflowMode != null)
            //{
            //    DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, Enum.GetNames(typeof(LitGUI.WorkflowMode)));
            //}
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);           

            DoMetallicSpecularArea(litProperties, materialEditor, material);
            DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);

            //DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        public static void DoMetallicSpecularArea(LitProperties properties, MaterialEditor materialEditor, Material material)
        {
            string[] smoothnessChannelNames;
            bool hasGlossMap = false;
            if (properties.workflowMode == null ||
                (WorkflowMode)properties.workflowMode.floatValue == WorkflowMode.Metallic)
            {
                hasGlossMap = properties.metallicGlossMap.textureValue != null;
                materialEditor.TexturePropertySingleLine(new GUIContent("Metallic Map", "Sets and configures the map for the Metallic workflow."), properties.metallicGlossMap,
                    hasGlossMap ? null : properties.metallic);
                GUILayout.Label("         ( R : Thickness   G : AO   B : Curvature   A  : Smoothness )");

            }
            if(properties.metallicGlossMap.textureValue != null)
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

        public static void DoSmoothness(LitProperties properties, Material material)
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


        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if(EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                }
            }

            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            MaterialChanged(material);
        }
    }
}
