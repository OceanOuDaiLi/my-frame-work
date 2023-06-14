using System;
using UnityEngine;
using static FTXUnityEditor.FTXShaderGUIEditor;
using static UnityEditor.Rendering.Universal.ShaderGUI.CourtFloorShader;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    class CourtFloorShader : BaseShaderGUI
    {
        private FTXLitProperties litProperties;
        //private LitDetailGUI.LitProperties litDetailProperties;
        //private SavedBool m_DetailInputsFoldout;
        private MaterialProperty base2MapProp = new MaterialProperty();
        private MaterialProperty base2ColorProp = new MaterialProperty();

        private MaterialProperty darkMapProp = new MaterialProperty();
        private MaterialProperty darkColorProp = new MaterialProperty();
        private MaterialProperty darkIntensityProp = new MaterialProperty();

        private MaterialProperty smoothnessMapProp = new MaterialProperty();
        private MaterialProperty smoothnessProp = new MaterialProperty();

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);
            //m_DetailInputsFoldout = new SavedBool($"{headerStateKey}.DetailInputsFoldout", true);           
        }

        public override void DrawAdditionalFoldouts(Material material)
        {
            //m_DetailInputsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_DetailInputsFoldout.value, LitDetailGUI.Styles.detailInputs);
            //if (m_DetailInputsFoldout.value)
            //{
                //LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor);
                //EditorGUILayout.Space();
            //}
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            base2MapProp = FindProperty("_Base2Map", properties, false);
            base2ColorProp = FindProperty("_Base2Color", properties, false);

            darkMapProp = FindProperty("_DarkMap", properties, false);
            darkColorProp = FindProperty("_DarkColor", properties, false);
            darkIntensityProp = FindProperty("_DarkIntensity", properties, false);

            smoothnessMapProp = FindProperty("_SmoothnessMap", properties, false);
            smoothnessProp = FindProperty("_Smoothness", properties, false);

            litProperties = new FTXLitProperties(properties);
            //litDetailProperties = new LitDetailGUI.LitProperties(properties);
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

            DrawDarkProperty(material);

            DoSmoothnessArea(material);
            SetNormalKeywords(material, litProperties.bumpMapProp);
            DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);

            //DrawEmissionProperties(material, true);
           
            materialEditor.TexturePropertySingleLine(new GUIContent("UV3Œ∆¿Ì"), base2MapProp, base2ColorProp);

            DrawTileOffset(materialEditor, baseMapProp);
        }

        private void SetNormalKeywords(Material material, MaterialProperty normalProp)
        {
            if(normalProp.textureValue == null)
            {
                material.DisableKeyword("_NORMALMAP");
            }
            else
            {
                material.EnableKeyword("_NORMALMAP");
            }
        }

        private void DrawDarkProperty(Material material)
        {
            if (darkMapProp.textureValue == null)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Dark"), darkMapProp);
                darkIntensityProp.floatValue = 0;
                material.EnableKeyword("_DARKFLOOR_OFF");
            }
            else
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Dark"), darkMapProp, darkColorProp);
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(darkIntensityProp, "Dark Intensity");
                materialEditor.TextureScaleOffsetProperty(darkMapProp);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;

                material.DisableKeyword("_DARKFLOOR_OFF");
            }           
        }

        public void DoSmoothnessArea(Material material)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Smoothness"), smoothnessMapProp, smoothnessProp);
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
            //if (material.HasProperty("_Emission"))
            //{
            //    material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            //}

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

            //if (oldShader.name.Equals("Standard (Specular setup)"))
            //{
            //    material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
            //    Texture texture = material.GetTexture("_SpecGlossMap");
            //    if (texture != null)
            //        material.SetTexture("_MetallicSpecGlossMap", texture);
            //}
            //else
            //{
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            //}

            MaterialChanged(material);
        }
    }
}
