using FTXUnityEditor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static FTXUnityEditor.FTXShaderGUIEditor;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class FTXLitShader : BaseShaderGUI
    {
        private FTXLitProperties litProperties;

        private MaterialProperty useUV3Prop = new MaterialProperty();

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
            litProperties = new FTXLitProperties(properties);

            useUV3Prop = FindProperty("_UseUV3", properties, false);
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
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            FTXShaderGUIEditor.DrawSurfaceOptions(material, litProperties, materialEditor);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            FTXShaderGUIEditor.DrawSurfaceInputs(material, litProperties, materialEditor);


            FTXShaderGUIEditor.DoMetallicSpecularArea(litProperties, materialEditor, material);
            DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);

            FTXShaderGUIEditor.DrawEmissionProperties(litProperties, materialEditor, material, true);
            DrawTileOffset(materialEditor, litProperties.baseMapProp);
       
            DrawUseUV3(material, litProperties, materialEditor);
            
            SetNormalKeywords(material, litProperties.bumpMapProp, emissionMapProp);
        }

        private void DrawUseUV3(Material material, FTXLitProperties litProperties, MaterialEditor materialEditor)
        {
            EditorGUILayout.Space();
            materialEditor.ShaderProperty(useUV3Prop, new GUIContent("Use UV 3"));
            if(useUV3Prop.floatValue == 1)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("UV3 Map"), litProperties.base2MapProp, litProperties.base2ColorProp);
                if(litProperties.base2MapProp.textureValue != null)
                {
                    material.DisableKeyword("_USEUV3_OFF");
                }
                else
                {
                    material.EnableKeyword("_USEUV3_OFF");
                }
            }
            else
            {
                material.EnableKeyword("_USEUV3_OFF");
            }
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
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if (EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                }
            }

            FTXShaderGUIEditor.DrawAdvancedOptions(material, litProperties, materialEditor);
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
