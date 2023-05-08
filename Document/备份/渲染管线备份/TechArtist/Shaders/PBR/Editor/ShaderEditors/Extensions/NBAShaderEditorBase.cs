#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FTX.Rendering
{
    public abstract class NBAShaderEditorBase : ShaderGUI
    {

        #region Shader Enum

        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        public enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        public enum BlendMode
        {
            Alpha,                                  // 旧版Alpha 混合模式，菲涅尔不影响透明度
            Premultiply,                            // 透明度乘算
            Additive,
            Multiply
        }

        #endregion

        public bool m_FirstTimeApply = true;

        protected ContentShader m_Content;
        protected ShaderEditorGUI m_ShaderGUI;

        protected MaterialProperty surfaceTypeProp { get; set; }
        protected MaterialProperty blendModeProp { get; set; }
        protected MaterialProperty cullingProp { get; set; }
        protected MaterialProperty alphaClipProp { get; set; }
        protected MaterialProperty alphaCutoffProp { get; set; }

        protected MaterialProperty receiveShadowsProp { get; set; }

        protected MaterialProperty useNumtexProp { get; set; }


        protected MaterialProperty useNumtex02Prop { get; set; }
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (m_ShaderGUI == null)
            {
                m_ShaderGUI = new ShaderEditorGUI(materialEditor);
                m_Content = new ContentShader();
            }

            surfaceTypeProp = FindProperty("_Surface", properties);
            blendModeProp = FindProperty("_Blend", properties);
            cullingProp = FindProperty("_Cull", properties);
            alphaClipProp = FindProperty("_AlphaClip", properties);
            alphaCutoffProp = FindProperty("_Cutoff", properties);
            receiveShadowsProp = FindProperty("_ReceiveShadows", properties, false);

            useNumtex02Prop = FindProperty("_useNumtex02", properties, false);

            useNumtexProp = FindProperty("_Numtex_ON", properties, false);

            m_ShaderGUI.FetchProperties(properties);

            if (m_FirstTimeApply)
            {
                foreach (var obj in materialEditor.targets)
                    MaterialChanged((Material)obj);
                m_FirstTimeApply = false;
            }
            EditorGUI.BeginChangeCheck();
            DrawSurfaceOptions(materialEditor);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in materialEditor.targets)
                    MaterialChanged((Material)obj);
            }
            EditorGUILayout.Space();
            DrawGUIParameters();

            m_ShaderGUI.Section(m_Content.advanced);


            m_ShaderGUI.GPUInstancing(m_Content.gpuInstancing, m_Content.enableFlagOptions);
            m_ShaderGUI.QueueOffset(m_Content.queueOffset);
            m_ShaderGUI.RenderQueue(m_Content.renderQueue, m_Content.renderQueueOptions);
            materialEditor.DoubleSidedGIField();
        }

        public void MaterialChanged(Material material)
        {
            SetMaterialKeywords(material);
        }

        public static void SetMaterialKeywords(Material material, Action<Material> shadingModelFunc = null, Action<Material> shaderFunc = null)
        {
            SetupMaterialBlendMode(material);

            if (material.HasProperty("_ReceiveShadows"))
                CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_ReceiveShadows") == 0.0f);

            if (material.HasProperty("_useNumtex02"))
                CoreUtils.SetKeyword(material, "TEX2_ON", material.GetFloat("_useNumtex02") == 0.0f);

        }


        public static void SetupMaterialBlendMode(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");


            bool alphaClip = false;
            if (material.HasProperty("_AlphaClip"))
                alphaClip = material.GetFloat("_AlphaClip") >= 0.5;

            if (alphaClip)
            {
                material.EnableKeyword("ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("ALPHATEST_ON");
            }

            SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
            if (surfaceType == SurfaceType.Opaque)
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
                material.SetInt("_DstAlphaBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("ALPHAPREMULTIPLY_ON");
                material.SetShaderPassEnabled("ShadowCaster", true);
            }
            else
            {
                BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");

                // Specific Transparent Mode Settings
                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        material.SetInt("_ZWrite", 0);
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.DisableKeyword("ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Premultiply:
                        material.SetInt("_ZWrite", 0);
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.EnableKeyword("ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Additive:
                        material.SetInt("_ZWrite", 0);
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.DisableKeyword("ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Multiply:
                        material.SetInt("_ZWrite", 0);
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.DisableKeyword("ALPHAPREMULTIPLY_ON");
                        break;
                }
                material.renderQueue += material.HasProperty("_QueueOffset") ? (int)material.GetFloat("_QueueOffset") : 0;
                material.SetShaderPassEnabled("ShadowCaster", false);
            }
        }

        public void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;
            DoPopup("Surface Type", surfaceTypeProp, Enum.GetNames(typeof(SurfaceType)), materialEditor);
            if ((SurfaceType)material.GetFloat("_Surface") == SurfaceType.Transparent)
                DoPopup("Blend Mode", blendModeProp, Enum.GetNames(typeof(BlendMode)), materialEditor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = cullingProp.hasMixedValue;
            var culling = (RenderFace)cullingProp.floatValue;
            culling = (RenderFace)EditorGUILayout.EnumPopup("Render Face", culling);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo("Render Face");
                cullingProp.floatValue = (float)culling;
                material.doubleSidedGI = (RenderFace)cullingProp.floatValue != RenderFace.Front;
            }

            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = alphaClipProp.hasMixedValue;
            var alphaClipEnabled = EditorGUILayout.Toggle(m_Content.alphaClipText, alphaClipProp.floatValue == 1);
            if (EditorGUI.EndChangeCheck())
                alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
            EditorGUI.showMixedValue = false;

            if (alphaClipProp.floatValue == 1)
                materialEditor.ShaderProperty(alphaCutoffProp, m_Content.alphaClipThresholdText, 1);

            if (receiveShadowsProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = receiveShadowsProp.hasMixedValue;
                var receiveShadows =
                    EditorGUILayout.Toggle(m_Content.receiveShadowText, receiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                    receiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                EditorGUI.showMixedValue = false;
            }

            //if (useNumtex02Prop != null)
            //{
            //    EditorGUI.BeginChangeCheck();
            //    EditorGUI.showMixedValue = useNumtex02Prop.hasMixedValue;
            //    var useNumtex02 =
            //        EditorGUILayout.Toggle(m_Content.useNumtex02, useNumtex02Prop.floatValue == 1.0f);
            //    if (EditorGUI.EndChangeCheck())
            //        useNumtex02Prop.floatValue = useNumtex02 ? 1.0f : 0.0f;
            //    EditorGUI.showMixedValue = false;
            //}

        }

        public static void DoPopup(string label, MaterialProperty property, string[] options, MaterialEditor materialEditor)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            EditorGUI.showMixedValue = property.hasMixedValue;

            var mode = property.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(label, (int)mode, options);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(label);
                property.floatValue = mode;
            }

            EditorGUI.showMixedValue = false;
        }

        protected virtual void DrawGUIParameters() { }
    }
}
#endif