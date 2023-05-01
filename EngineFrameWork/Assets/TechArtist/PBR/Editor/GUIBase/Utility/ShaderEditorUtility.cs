using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ShaderEditorUtility.cs
	Author:		DaiLi.Ou
	Descriptions:  Shader GUI Utility Methods & Variables.
*********************************************************************/

namespace TechArtist.Editor
{
    public class ShaderEditorUtility : IShaderEditorUtility
    {
        private Material m_material;
        private readonly MaterialEditor m_MaterialEditor;
        private Dictionary<string, MaterialProperty> m_Properties;

        // GUI Variables
        private static readonly int[] s_EnabledFlag = new int[] { 1, 0 };
        private static readonly GUIStyle s_TitleStyle = EditorStyles.boldLabel;
        private static readonly Color s_TitleColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        private static readonly float s_FieldShort = EditorGUIUtility.singleLineHeight * 4.0f;
        private static readonly string s_MissingParameter = "Shader parameter {0} not found!";

        public ShaderEditorUtility(MaterialEditor materialEditor)
        {
            m_MaterialEditor = materialEditor;
            m_material = materialEditor.target as Material;
            m_Properties = new Dictionary<string, MaterialProperty>();
        }

        public void FetchProperties(MaterialProperty[] properties)
        {
            m_Properties.Clear();
            foreach (MaterialProperty mP in properties)
            {
                m_Properties.Add(mP.name, mP);
            }
        }

        #region Methods For Draw Below Content -> Advanced Options.
        public void DrawCullMode(string name, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            Rect rect = DrawLabel(label);
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                float value = (float)(CullMode)EditorGUI.EnumPopup(rect, (CullMode)(int)property.floatValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = value;
                }

                EditorGUI.showMixedValue = false;
            }
        }

        public void DrawDepthWrite(string name, GUIContent label, string[] options)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            Rect rect = DrawLabel(label);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                float value = EditorGUI.IntPopup(rect, (int)property.floatValue, options, s_EnabledFlag);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = value;
                }
                EditorGUI.showMixedValue = false;
            }
        }

        public void DrawDepthTest(string name, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            Rect rect = DrawLabel(label);
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                float value = (float)(CompareFunction)EditorGUI.EnumPopup(rect, (CompareFunction)(int)property.floatValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = value;
                }

                EditorGUI.showMixedValue = false;
            }
        }

        public void DrawRenderQueue(GUIContent label)
        {
            Rect rect = DrawLabel(label);

            GUI.enabled = false;
            EditorGUI.IntField(rect, m_material.renderQueue);
            GUI.enabled = true;
        }

        public void DrawQueueOffset(string name, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                var queue = EditorGUILayout.IntSlider(label, (int)property.floatValue, -100, 100);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = queue;
                }
            }

            EditorGUI.showMixedValue = false;
        }

        public void DrawGPUInstancing(GUIContent label, string[] options)
        {
            Rect rect = DrawLabel(label);
            int index = m_material.enableInstancing ? 1 : 0;
            index = EditorGUI.IntPopup(rect, index, options, s_EnabledFlag);

            m_material.enableInstancing = index == 1;
        }
        #endregion

        #region Methods For Draw Middle Content -> Designated Parameters.

        /// <summary>
        /// Adds a vertical/horizontal space between elements.
        /// </summary>
        /// <param name="pixels"></param>
        public void Space(float pixels = 5)
        {
            GUILayout.Space(pixels);
        }

        /// <summary>
        /// Drawing rect content with a bold title label,And optional spacing before and after.
        /// </summary>
        /// <param name="label">The section label.</param>
        /// <param name="before">Optional spacing before the section, 3.0 by default.</param>
        /// <param name="after">Optional spacing after the section, 1.0 bu default.</param>
        public void Section(string label, float before = 3, float after = 1)
        {
            GUILayout.Space(before);
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.DrawRect(new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleColor);
            EditorGUI.LabelField(rect, label, s_TitleStyle);
            GUILayout.Space(after);
        }

        /// <summary>
        /// Drawing rect content with a bold title with a keyword toggle,And optional spacing before and after.
        /// </summary>
        /// <param name="label">The Section label.</param>
        /// <param name="keyword">The materials'shader keyword.</param>
        /// <param name="before">Optional spacing before the section.</param>
        /// <param name="after">Optional spacing after the section.</param>
        public bool Section(string label, string keyword, float before = 3, float after = 1)
        {
            GUILayout.Space(before);
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.DrawRect(new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleColor);
            EditorGUI.LabelField(rect, label, s_TitleStyle);
            GUILayout.Space(after);

            EditorGUI.BeginChangeCheck();
            {
                bool flag = m_material.IsKeywordEnabled(keyword);
                if (EditorGUI.EndChangeCheck())
                {
                    if (flag)
                    {
                        m_material.EnableKeyword(keyword);
                    }
                    else
                    {
                        m_material.DisableKeyword(keyword);
                    }
                }

                return flag;
            }
        }

        /// <summary>
        /// Drawing a texture slot with channel infomation.
        /// </summary>
        /// <param name="name">The name of the texture property.</param>
        /// <param name="label">The texture label.The tooltip is a string of channel names separated by comma (',')</param>
        /// <param name="channelInfo">Additional infomation about the texture channel layout.</param>
        public void TextureSlot(string name, GUIContent label, string[] channelInfo)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            float h = EditorGUIUtility.singleLineHeight;
            float w = EditorGUIUtility.singleLineHeight * 4.0f;
            int count = Mathf.Max(channelInfo.Length + 1, 5);

            Rect rect = EditorGUILayout.GetControlRect(false, h * count + 2.0f);
            rect.height = h;
            float y = rect.y;

            EditorGUI.LabelField(rect, label);
            foreach (string channel in channelInfo)
            {
                rect.y += h;
                EditorGUI.LabelField(rect, channel, EditorStyles.miniLabel);
            }
            rect.Set(rect.x + rect.width - w, y + h + 2.0f, w, w);          // whatever,just to make the menu look better.

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                Texture texture = (Texture)EditorGUI.ObjectField(rect, property.textureValue, typeof(Texture), false);
                if (EditorGUI.EndChangeCheck())
                {
                    property.textureValue = texture;
                }
                EditorGUI.showMixedValue = false;
            }
        }

        /// <summary>
        /// Drawing a texture slot with channel information.
        /// </summary>
        /// <param name="nameTexture">The name of the texture property.</param>
        /// <param name="nameColor">The name of the color property.</param>
        /// <param name="label">The texture label.The tooltip is a string of channel names separated by comma (',')</param>
        /// <param name="channelInfo">Additional information about the texture channel layout.</param>
        /// <param name="hasAlpha">Optional parameter describleing whether the color has an aloha vaule, false by default.</param>
        /// <param name="isHDR">Optional parameter idicating whether the color is LDR or HDR, false by default.</param>
        public void TextureSlotWithColor(string nameTexture, string nameColor, GUIContent label, string[] channelInfo, bool hasAlpha = false, bool isHDR = false)
        {
            if (!m_Properties.TryGetValue(nameTexture, out MaterialProperty propertyTexture))
            {
                Debug.LogErrorFormat(s_MissingParameter, nameTexture);
                return;
            }

            if (m_Properties.TryGetValue(nameColor, out MaterialProperty propertyColor))
            {
                Debug.LogErrorFormat(s_MissingParameter, nameColor);
                return;
            }

            float h = EditorGUIUtility.singleLineHeight;
            float w = EditorGUIUtility.singleLineHeight * 4.0f;
            int count = Mathf.Max(channelInfo.Length + 1, 5);

            Rect rect = EditorGUILayout.GetControlRect(false, h * count + 2.0f);
            rect.height = h;
            float y = rect.y;

            EditorGUI.LabelField(rect, label);
            foreach (var info in channelInfo)
            {
                rect.y += h;
                EditorGUI.LabelField(rect, info, EditorStyles.miniLabel);
            }

            // Drawing Color Field.
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = propertyColor.hasMixedValue;
                Color color = EditorGUI.ColorField(new Rect(rect.x + rect.width - w, y, w, h), GUIContent.none, propertyColor.colorValue, false, hasAlpha, isHDR);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyColor.colorValue = color;
                }
                EditorGUI.showMixedValue = false;
            }
            rect.Set(rect.x + rect.width - w, y + h + 2.0f, w, w);

            // Drawing Texture Field.
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = propertyTexture.hasMixedValue;
                Texture texture = (Texture)EditorGUI.ObjectField(rect, propertyTexture.textureValue, typeof(Texture), false);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyTexture.textureValue = texture;
                }
            }

            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// Drawing a Vector4Filed property.
        /// </summary>
        /// <param name="name">The material vector4 property name</param>
        /// <param name="label">The property label.</param>
        public void VectorRange(string name, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;

                Vector4 value = EditorGUILayout.Vector4Field(label, property.vectorValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.vectorValue = value;
                }

                EditorGUI.showMixedValue = false;
            }
        }

        /// <summary>
        /// Drawing a float field property.
        /// </summary>
        /// <param name="name">The material float property name.</param>
        /// <param name="label">The property label.</param>
        /// <returns>Curent float value</returns>
        public float FloatField(string name, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat($"Shader float parameter {name} not found! return 0f.");
                return 0;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                float value = EditorGUILayout.FloatField(label, property.floatValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = value;
                }

                EditorGUI.showMixedValue = false;

                return value;
            }
        }

        /// <summary>
        /// Drawing a float range slider property.
        /// </summary>
        /// <param name="name">The material property name.</param>
        /// <param name="min">The float min range value.</param>
        /// <param name="max">The float max range value.</param>
        /// <param name="label">The property label.</param>
        /// <returns></returns>
        public float FloatRange(string name, float min, float max, GUIContent label)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat($"Shader float parameter {name} not found! return 0f.");
                return 0f;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                float value = EditorGUILayout.Slider(label, property.floatValue, min, max);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = value;
                }

                EditorGUI.showMixedValue = false;
                return value;
            }
        }

        /// <summary>
        /// Drawing a color field property.
        /// </summary>
        /// <param name="name">The color property.</param>
        /// <param name="label">The property lbael.</param>
        /// <param name="showAlpha">Optional parameter defining whether to show the aloha value or not.</param>
        /// <param name="isHDR">Optional parameter defining whether the value is high dynamic rang.true by default.</param>
        public void ColorField(string name, GUIContent label, bool showAlpha = true, bool isHDR = true)
        {
            if (!m_Properties.TryGetValue(name, out MaterialProperty property))
            {
                Debug.LogErrorFormat(s_MissingParameter, name);
                return;
            }

            Rect rect = DrawLabel(label);
            EditorGUI.BeginChangeCheck();
            {
                Color color = EditorGUI.ColorField(rect, GUIContent.none, property.colorValue, false, showAlpha, isHDR);
                EditorGUI.showMixedValue = property.hasMixedValue;

                if (EditorGUI.EndChangeCheck())
                {
                    property.colorValue = color;
                }
                EditorGUI.showMixedValue = false;
            }
        }

        /// <summary>
        /// Drawing a label and returns the rect for the GUI field elements.
        /// </summary>
        /// <param name="label">The label content.</param>
        /// <returns>A field rect positioned after the label.</returns>
        public Rect DrawLabel(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            rect.width -= s_FieldShort;

            EditorGUI.LabelField(rect, label);
            rect.x += rect.width;
            rect.width = s_FieldShort;

            return rect;
        }
        #endregion

        #region Methods For Draw Top Content -> Surface Options.
        /// <summary>
        /// Draw Material Surface Option with Popup.
        /// </summary>
        /// <param name="label">surface option name</param>
        /// <param name="property">material property</param>
        /// <param name="options">display options</param>
        /// <param name="materialEditor">material editor</param>
        public void DrawSurfaceOptionPopup(string label, MaterialProperty property, string[] options, MaterialEditor materialEditor)
        {
            if (property == null)
            {
                Debug.LogErrorFormat(s_MissingParameter, label);
                return;
            }

            EditorGUI.showMixedValue = property.hasMixedValue;
            float value = property.floatValue;
            EditorGUI.BeginChangeCheck();
            {
                value = EditorGUILayout.Popup(label, (int)value, options);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(label);
                    property.floatValue = value;
                }
            }
            EditorGUI.showMixedValue = false;

        }

        /// <summary>
        ///  Draw Material Surface Option -> AlphaClip & AlphaClip Cutoff By GUI Toggle.
        /// </summary>
        public void DrawAlphaClipAndCutoffToggle(GUIContent alphaClipText, GUIContent alphaClipThresholdText, MaterialProperty alphaClipProperty, MaterialProperty alphaCutoffProperty, MaterialEditor materialEditor)
        {
            if (alphaClipProperty == null)
            {
                Debug.LogErrorFormat(s_MissingParameter, alphaClipText.text);
                return;
            }
            if (alphaCutoffProperty == null)
            {
                Debug.LogErrorFormat(s_MissingParameter, alphaClipThresholdText.text);
                return;
            }

            EditorGUI.showMixedValue = false;

            // draw Alpha Clip.
            bool alphaClipEnabled;
            EditorGUI.showMixedValue = alphaClipProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            {
                alphaClipEnabled = EditorGUILayout.Toggle(alphaClipText, alphaClipProperty.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    alphaClipProperty.floatValue = alphaClipEnabled ? 1 : 0;
                }
            }

            EditorGUI.showMixedValue = false;

            // draw Alpha Clip Cutoff.
            if (alphaClipEnabled)
            {
                materialEditor.ShaderProperty(alphaCutoffProperty, alphaClipThresholdText, 1);
            }
        }

        /// <summary>
        /// Draw Material Surface Option -> RenderFace By GUI EnumPopup.
        /// </summary>
        /// <param name="label">surface option name</param>
        /// <param name="property">material property</param>
        /// <param name="materialEditor">material editor</param>
        public void DrawRenderFaceEnumPopup(string label, MaterialProperty property, MaterialEditor materialEditor)
        {
            if (property == null)
            {
                Debug.LogErrorFormat(s_MissingParameter, label);
                return;
            }

            Material material = materialEditor.target as Material;

            EditorGUI.showMixedValue = property.hasMixedValue;
            RenderFace culling = (RenderFace)property.floatValue;
            EditorGUI.BeginChangeCheck();
            {
                culling = (RenderFace)EditorGUILayout.EnumPopup(label, culling);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(label);
                    property.floatValue = (float)culling;
                    material.doubleSidedGI = (RenderFace)property.floatValue != RenderFace.FrontFace;

                }
            }
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// Draw Material Received Shadows.
        /// </summary>
        public void DrawReceiveShadowsToggle(GUIContent receiveShadowText, MaterialProperty property)
        {
            if (property == null)
            {
                Debug.LogErrorFormat(s_MissingParameter, receiveShadowText.text);
                return;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMixedValue;
                var receiveShadows = EditorGUILayout.Toggle(receiveShadowText, property.floatValue == 1.0f);

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = receiveShadows ? 1.0f : 0.0f;
                }

                EditorGUI.showMixedValue = false;
            }
        }
        #endregion
    }
}