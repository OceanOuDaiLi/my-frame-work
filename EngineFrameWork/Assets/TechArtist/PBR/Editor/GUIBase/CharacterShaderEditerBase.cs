using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	CharacterShaderEditerBase.cs
	Author:		DaiLi.Ou
	Descriptions: Core Shader GUI Base For Character.
*********************************************************************/

namespace TechArtist.Editor
{
    public abstract class CharacterShaderEditerBase : ShaderGUI
    {
        private bool _firstTmRefresh = true;

        private ShaderContentData _contentData;
        private ShaderEditorUtility _editorUtility;

        // Surface Options Private Variables.
        private MaterialProperty cullingProperty { get; set; }
        private MaterialProperty blendModeProperty { get; set; }
        private MaterialProperty alphaClipProperty { get; set; }
        private MaterialProperty surfaceTypeProperty { get; set; }
        private MaterialProperty alphaCutoffProperty { get; set; }
        // Shadows Option.
        private MaterialProperty receiveShadowsProperty { get; set; }


        /// <summary>
        /// To define a custom shader GUI use the methods of materialEditor to render controls
        /// for the properties arry.
        /// </summary>
        /// <param name="materialEditor">The MaterialEditor that are calling this OnGUI(the 'owner')</param>
        /// <param name="properties">Material properties of the current selected shader.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            RefreshPropertiesData(materialEditor, properties);

            OnFirstTimeRefreshGUI(materialEditor);

            // draw top part of gui inspector.
            EditorGUI.BeginChangeCheck();
            {
                DrawSurfaceOptions(materialEditor);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var item in materialEditor.targets)
                        UpdateMaterial(item as Material);
                }
            }

            EditorGUILayout.Space();

            // virtual method for draw middle part of gui inspector.
            DrawGUIParameters();

            // draw below part of gui inspector.
            _editorUtility.Section(_contentData.advanced);
            _editorUtility.DrawGPUInstancing(_contentData.gpuInstancing, _contentData.enableFlagTips);
            _editorUtility.DrawQueueOffset(ShaderParameter.queueOffset, _contentData.queueOffset);
            _editorUtility.DrawRenderQueue(_contentData.renderQueue);

            materialEditor.DoubleSidedGIField();
        }

        private void RefreshPropertiesData(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // init gui-content-data & draw shader gui utility.
            if (_editorUtility == null || _contentData == null)
            {
                _contentData = new ShaderContentData();
                _editorUtility = new ShaderEditorUtility(materialEditor);
            }
            // init surface data.
            cullingProperty = FindProperty("_Cull", properties);
            blendModeProperty = FindProperty("_Blend", properties);
            alphaClipProperty = FindProperty("_AlphaClip", properties);
            surfaceTypeProperty = FindProperty("_Surface", properties);
            alphaCutoffProperty = FindProperty("_Cutoff", properties);

            // init receive shadow data.
            receiveShadowsProperty = FindProperty("_ReceiveShadows", properties, false);
            // init material properties data.
            _editorUtility.FetchProperties(properties);
        }

        private void OnFirstTimeRefreshGUI(MaterialEditor materialEditor)
        {
            if (_firstTmRefresh)
            {
                foreach (var item in materialEditor.targets)
                {
                    UpdateMaterial(item as Material);
                }

                _firstTmRefresh = false;
            }
        }

        private void UpdateMaterial(Material material)
        {
            // Set Blend Mode.
            SetMaterialBlendMode(material);
            // Set Receive Shadows.
            if (material.HasProperty("_ReceiveShadows"))
            {
                CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_ReceiveShadows") == 0.0f);
            }
        }

        #region Material Blend Mode

        private void SetMaterialBlendMode(Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }

            bool alphaClip = SetAlphaClipMode(material);

            // Surface Type.
            SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
            if (surfaceType.Equals(SurfaceType.Opaque))
            {
                SetOpaqueProperty(material, alphaClip);
            }
            else if (surfaceType.Equals(SurfaceType.Transparent))
            {
                SetTransparentProperty(material);
            }
            else
            {
                UnityEngine.Debug.LogError("SurfaceType Error");
            }
        }

        private void SetOpaqueProperty(Material material, bool alphaClip)
        {
            if (alphaClip)
            {
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "TransparentCutout");
            }
            else
            {
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
            }

            material.renderQueue += material.HasProperty("_QueueOffset") ? (int)material.GetFloat("_QueueOffset") : 0;
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_SrcAlphaBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstAlphaBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_ZWrite", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.DisableKeyword("ALPHAPREMULTIPLY_ON");
            //material.SetShaderPassEnabled("ShadowCaster", true);
        }

        private void SetTransparentProperty(Material material)
        {
            BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");

            material.SetInt("_ZWrite", 0);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.SetOverrideTag("RenderType", "Transparent");
            switch (blendMode)
            {
                case BlendMode.Alpha:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.DisableKeyword("ALPHAPREMULTIPLY_ON");
                    break;
                case BlendMode.Premultiply:

                    break;
                case BlendMode.Additive:
                    break;
                case BlendMode.Multiply:
                    break;
                default:
                    break;
            }
        }

        private bool SetAlphaClipMode(Material material)
        {
            bool alphaClip = false;
            if (material.HasProperty("_AlphaClip"))
            {
                alphaClip = material.GetFloat("_AlphaClip") > 0.5f;
            }
            if (alphaClip)
            {
                material.EnableKeyword("ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("ALPHATEST_ON");
            }

            return alphaClip;
        }

        private void SetSurfaceType()
        {

        }

        #endregion

        private void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;
            // draw Surface Type.
            _editorUtility.DrawSurfaceOptionPopup(_contentData.surfaceType, surfaceTypeProperty, Enum.GetNames(typeof(SurfaceType)), materialEditor);
            // draw Blend Mode.
            SurfaceType surfaceType = (SurfaceType)material.GetFloat(_contentData.surfaceType);
            if (surfaceType.Equals(SurfaceType.Transparent))
            {
                _editorUtility.DrawSurfaceOptionPopup(_contentData.blendMode, blendModeProperty, Enum.GetNames(typeof(BlendMode)), materialEditor);
            }
            // draw Render Face.
            _editorUtility.DrawRenderFaceEnumPopup(_contentData.renderFace, cullingProperty, materialEditor);
            // draw Alpha Clip & AlphaClip Cutoff.
            _editorUtility.DrawAlphaClipAndCutoffToggle(_contentData.alphaClipText, _contentData.alphaClipThresholdText, alphaClipProperty, alphaCutoffProperty, materialEditor);
            // draw Receive Shadows.
            _editorUtility.DrawReceiveShadowsToggle(_contentData.receviceShadowText, receiveShadowsProperty);
        }

        protected virtual void DrawGUIParameters() { }

    }
}