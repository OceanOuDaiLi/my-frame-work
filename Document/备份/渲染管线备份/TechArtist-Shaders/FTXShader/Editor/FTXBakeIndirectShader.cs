using FTXUnityEditor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static FTXUnityEditor.FTXShaderGUIEditor;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class FTXBakeIndirectShader : BaseShaderGUI
    {
        private FTXLitProperties litProperties;
        //private LitDetailGUI.LitProperties litDetailProperties;
        //private SavedBool m_DetailInputsFoldout;

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);

        }

        public override void DrawAdditionalFoldouts(Material material)
        {
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new FTXLitProperties(properties);;
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

            DrawSmoothness(material, litProperties);
            DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);

            FTXShaderGUIEditor.DrawEmissionProperties(litProperties, materialEditor, material, true);
            DrawTileOffset(materialEditor, baseMapProp);

            SetNormalKeywords(material, litProperties.bumpMapProp, emissionMapProp);
        }

        public void DrawSmoothness(Material material, FTXLitProperties litProperties)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;        
            materialEditor.RangeProperty(litProperties.metallic, "Metallic");
            materialEditor.RangeProperty(litProperties.smoothness, "Smoothness");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
                
        }

        private void SetNormalKeywords(Material material, MaterialProperty normalProp, MaterialProperty emissionMapProp)
        {
            if (normalProp.textureValue == null)
            {
                material.DisableKeyword("_NORMALMAP");
            }
            else
            {
                material.EnableKeyword("_NORMALMAP");
            }          
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if (EditorGUI.EndChangeCheck())
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
