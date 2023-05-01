using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;

namespace FTX.Rendering
{
	public class ShaderEditorGUI
	{
		private Material m_Material;
		private readonly MaterialEditor m_MaterialEditor;
		private Dictionary<string, MaterialProperty> m_Properties;

		// Class constants
		private static readonly int [] s_EnabledFlag = new int [] { 1, 0 };
		private static readonly GUIStyle s_TitleStyle = EditorStyles.boldLabel;
		private static readonly Color s_TitleLight = new Color (1.0f, 1.0f, 1.0f, 0.2f);
		private static readonly float s_FieldShort = EditorGUIUtility.singleLineHeight * 4.0f;
		private static readonly string s_MissingParameter = "Shader parameter {0} not found!";
		private const int queueOffsetRange = 100;

		public enum BlendMode
		{
			Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
			Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
			Additive,
			Multiply
		}

		/// <summary>
		/// Creates a new instance of MaterialEditor.
		/// </summary>
		/// <param name="materialEditor">The material editor.</param>
		public ShaderEditorGUI (MaterialEditor materialEditor)
		{
			m_MaterialEditor = materialEditor;
			m_Material = materialEditor.target as Material;
			m_Properties = new Dictionary<string, MaterialProperty> ();
		}

		#region Public Methods
		/// <summary>
		/// Loads all material properties into a dictionary.
		/// </summary>
		/// <param name="properties">The material properties.</param>
		public void FetchProperties (MaterialProperty [] properties)
		{
			m_Properties.Clear ();

			foreach (MaterialProperty property in properties)
			{
				m_Properties.Add (property.name, property);
			}

			SetEmissionFlag ();
		}

		/// <summary>
		/// Adds a vertical space between elements.
		/// </summary>
		/// <param name="pixels">Optional spacing parameter, 5.0 by default.</param>
		public void Space (float pixels = 5.0f)
		{
			GUILayout.Space (pixels);
		}

		/// <summary>
		/// Draws a bold title label, and optional spacing before and after.
		/// </summary>
		/// <param name="label">The section label.</param>
		/// <param name="before">Optional spacing before the section, 3.0 by default.</param>
		/// <param name="after">Optional spacing after the section, 1.0 by default.</param>
		public void Section (string label, float before = 3.0f, float after = 1.0f)
		{
			GUILayout.Space (before);
			Rect rect = EditorGUILayout.GetControlRect ();
			EditorGUI.DrawRect (new Rect (0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleLight);
			EditorGUI.LabelField (rect, label, s_TitleStyle);
			GUILayout.Space (after);
		}

		/// <summary>
		/// Draws a bold title label with a keyword toggle, and optional spacing before and after.
		/// </summary>
		/// <param name="label">The section label.</param>
		/// <param name="keyword"></param>
		/// <param name="before">Optional spacing before the section, 3.0 by default.</param>
		/// <param name="after">Optional spacing after the section, 1.0 by default.</param>
		/// <returns>True, if the keyword is enabled, false otherwise.</returns>
		public bool Section (string label, string keyword, float before = 3.0f, float after = 1.0f)
		{
			GUILayout.Space (before);
			Rect rect = EditorGUILayout.GetControlRect ();
			EditorGUI.DrawRect (new Rect (0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleLight);
			EditorGUI.LabelField (rect, label, s_TitleStyle);
			GUILayout.Space (after);

			bool flag = m_Material.IsKeywordEnabled (keyword);

			EditorGUI.BeginChangeCheck ();
			flag = EditorGUI.Toggle (new Rect (rect.width, rect.y, 12f, rect.height), flag);

			if (EditorGUI.EndChangeCheck ())
			{
				if (flag)
				{
					m_Material.EnableKeyword (keyword);
				}
				else
				{
					m_Material.DisableKeyword (keyword);
				}
			}

			return flag;
		}

		public bool SubSection(string label, string keyword, float before = 3.0f, float after = 1.0f)
		{
			GUILayout.Space(before);
			Rect rect = EditorGUILayout.GetControlRect();
			//EditorGUI.DrawRect(new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleLight);
			EditorGUI.LabelField(rect, label, s_TitleStyle);
			GUILayout.Space(after);

			bool flag = m_Material.IsKeywordEnabled(keyword);

			EditorGUI.BeginChangeCheck();
			flag = EditorGUI.Toggle(new Rect(rect.width, rect.y, 12f, rect.height), flag);

			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					m_Material.EnableKeyword(keyword);
				}
				else
				{
					m_Material.DisableKeyword(keyword);
				}
			}

			return flag;
		}

		public bool SectionParam(string label, string param, float before = 3.0f, float after = 1.0f)
        {
			GUILayout.Space(before);
			Rect rect = EditorGUILayout.GetControlRect();
			EditorGUI.DrawRect(new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleLight);
			EditorGUI.LabelField(rect, label, s_TitleStyle);
			GUILayout.Space(after);

			bool flag = m_Material.GetFloat(param) > 0;

			EditorGUI.BeginChangeCheck();
			flag = EditorGUI.Toggle(new Rect(rect.width, rect.y, 12f, rect.height), flag);

			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					m_Material.SetFloat(param, 1);
				}
				else
				{
					m_Material.SetFloat(param, 0);
				}
			}

			return flag;
		}

		public bool KeywordToggle (string keyword, string label, string[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect ();
			float w = EditorGUIUtility.singleLineHeight * 4.0f;
			int flag = m_Material.IsKeywordEnabled (keyword) ? 1 : 0;

			EditorGUI.LabelField (rect, label);
			rect.x = rect.width - w + EditorGUIUtility.singleLineHeight;
			rect.width = w;

			EditorGUI.BeginChangeCheck ();
			flag = EditorGUI.IntPopup (rect, flag, options, new [] { 1, 0 });

			if (EditorGUI.EndChangeCheck ())
			{
				if (flag == 1)
				{
					m_Material.EnableKeyword (keyword);
				}
				else
				{
					m_Material.DisableKeyword (keyword);
				}
			}

			return flag == 1;
		}

		public bool Toogle(string name, GUIContent label, float before = 3.0f, float after = 1.0f)
		{

			if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, name);
				return false;
			}

			GUILayout.Space(before);
			Rect rect = EditorGUILayout.GetControlRect();
			//EditorGUI.DrawRect(new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, rect.height), s_TitleLight);
			EditorGUI.LabelField(rect, label, s_TitleStyle);
			GUILayout.Space(after);

			bool flag = property.floatValue == 1;

			EditorGUI.BeginChangeCheck();
			flag = EditorGUI.Toggle(new Rect(rect.width, rect.y, 12f, rect.height), flag);

			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = flag ? 1 : 0;
			}

			return flag;
		}


		public void TextureScaleOffset(string name)
        {

			// Check for missing parameters
			if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, name);
				return;
			}
			m_MaterialEditor.TextureScaleOffsetProperty(property);

		}

		/// <summary>
		/// Draws a texture slot with channel information.
		/// </summary>
		/// <param name="name">The name of the texture property.</param>
		/// <param name="label">The texture label. The tooltip is a string of channel names separated by comma.</param>
		/// <param name="channels">Additional information about the texture channel layout.</param>
		public void TextureSlot (string name, GUIContent label, string [] channelInfo)
		{
			// Check for missing parameters
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			float h = EditorGUIUtility.singleLineHeight;
			float w = EditorGUIUtility.singleLineHeight * 4.0f;
			int count = Mathf.Max (channelInfo.Length + 1, 5);

			Rect rect = EditorGUILayout.GetControlRect (false, h * count + 2.0f);
			rect.height = h;
			float y = rect.y;

			EditorGUI.LabelField (rect, label);
			foreach (string info in channelInfo)
			{
				rect.y += h;
				EditorGUI.LabelField (rect, info, EditorStyles.miniLabel);
			}

			rect.Set (rect.x + rect.width - w, y + h + 2.0f, w, w);


			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			Texture texture = (Texture) EditorGUI.ObjectField (rect, property.textureValue, typeof (Texture), false);

			if (EditorGUI.EndChangeCheck ())
			{
				property.textureValue = texture;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws a texture slot with channel information.
		/// </summary>
		/// <param name="nameTexture">The name of the texture property.</param>
		/// <param name="nameColor">The name of the color property.</param>
		/// <param name="label">The texture label. The tooltip is a string of channel names separated by comma.</param>
		/// <param name="channels">Additional information about the texture channel layout.</param>
		/// <param name="hasAlpha">Optional parameter indicating whether the color has an alpha value, false by default.</param>
		/// <param name="isHDR">Optional parameter indicating whether the color is LDR or HDR, false by default.</param>
		public void TextureSlotWithColor (string nameTexture, string nameColor, GUIContent label, string [] channelInfo, bool hasAlpha = false, bool isHDR = false)
		{
			// Check for missing parameters
			if (!m_Properties.TryGetValue (nameTexture, out MaterialProperty propTexture))
			{
				Debug.LogFormat (s_MissingParameter, nameTexture);
				return;
			}

			if (!m_Properties.TryGetValue (nameColor, out MaterialProperty propColor))
			{
				Debug.LogFormat (s_MissingParameter, nameColor);
				return;
			}

			float h = EditorGUIUtility.singleLineHeight;
			float w = EditorGUIUtility.singleLineHeight * 4.0f;
			int count = Mathf.Max (channelInfo.Length + 1, 5);

			Rect rect = EditorGUILayout.GetControlRect (false, h * count + 2.0f);
			rect.height = h;
			float y = rect.y;

			EditorGUI.LabelField (rect, label);
			foreach (string info in channelInfo)
			{
				rect.y += h;
				EditorGUI.LabelField (rect, info, EditorStyles.miniLabel);
			}

			// Draw Color Field
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = propColor.hasMixedValue;
			Color color = EditorGUI.ColorField (new Rect (rect.x + rect.width - w, y, w, h), GUIContent.none, propColor.colorValue, false, hasAlpha, isHDR);

			if (EditorGUI.EndChangeCheck ())
			{
				propColor.colorValue = color;
			}

			EditorGUI.showMixedValue = false;
			rect.Set (rect.x + rect.width - w, y + h + 2.0f, w, w);

			// Draw texture field
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = propTexture.hasMixedValue;
			Texture texture = (Texture) EditorGUI.ObjectField (rect, propTexture.textureValue, typeof (Texture), false);

			if (EditorGUI.EndChangeCheck ())
			{
				propTexture.textureValue = texture;
			}

			EditorGUI.showMixedValue = false;
		}

		public float FloatField(string name, GUIContent label)
		{
			if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, name);
				return 0;
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUILayout.FloatField(label, property.floatValue);

			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
			return value;
		}

		/// <summary>
		/// Draws a float range slider property.
		/// </summary>
		/// <param name="name">The material property name.</param>
		/// <param name="min">The float min range value.</param>
		/// <param name="max">The float max range value.</param>
		/// <param name="label">The propery label.</param>
		public float FloatRange (string name, float min, float max, GUIContent label)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				// Debug.LogFormat (s_MissingParameter, name);
				return 0;
			}

			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUILayout.Slider (label, property.floatValue, min, max);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
			return value;
		}

		public void VectorRange(string name, GUIContent label)
		{
			if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, name);
				return ;
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMixedValue;
			//float value = EditorGUILayout.Slider(label, property.floatValue, min, max);
			Vector4 value = EditorGUILayout.Vector4Field(label, property.vectorValue);

			if (EditorGUI.EndChangeCheck())
			{
				property.vectorValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws a float popup property.
		/// </summary>
		/// <param name="name">The material float property.</param>
		/// <param name="label">The propery label.</param>
		/// <param name="min">The int min range value.</param>
		/// <param name="max">The int max range value.</param>
		public void FloatPopup (string name, GUIContent label, int min, int max)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			GetPopupOptions (min, max, out int [] values, out string [] options);

			int count = max - min;
			float step = 1.0f / count;
			float half = step * 0.5f;

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUI.IntPopup (rect, (int) ((property.floatValue - half) * count), options, values) * step + half;

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws an int range slider property.
		/// </summary>
		/// <param name="name">The material float property.</param>
		/// <param name="label">The property label.</param>
		/// <param name="min">The int min range value.</param>
		/// <param name="max">The intt max rrange value.</param>
		public int IntRange (string name, GUIContent label, int min, int max)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return 0;
			}

			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUILayout.IntSlider (label, (int) property.floatValue, min, max);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
			return (int)value;
		}

		/// <summary>
		/// Draws an int popup property.
		/// </summary>
		/// <param name="name">The material float property.</param>
		/// <param name="label">The property label.</param>
		/// <param name="min">The int min range value.</param>
		/// <param name="max">The int max range value.</param>
		public void IntPopup (string name, GUIContent label, int min, int max)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			GetPopupOptions (min, max, out int [] values, out string [] options);

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUI.IntPopup (rect, (int) property.floatValue, options, values);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		public void DoProjectShadowBlend(/*string name, GUIContent label*/)
		{
			//if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			//{
			//	Debug.LogFormat(s_MissingParameter, name);
			//	return;
			//}
			m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			//m_Material.SetOverrideTag("RenderType", "Transparent");
			//m_Material.SetOverrideTag("LightMode", "SRPDefaultUnlit");
			//m_Material.SetOverrideTag("Queue", "Transparent");
			//m_Material.SetOverrideTag("IgnoreProjector", "True");
		}

		public void DoBlendMode(string name, GUIContent label)
        {
			if (!m_Properties.TryGetValue(name, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, name);
				return;
			}

			Rect rect = DrawLabel(label);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMixedValue;
			var mode = property.floatValue;
			mode = EditorGUILayout.Popup(label, (int)mode, Enum.GetNames(typeof(BlendMode)));

			property.floatValue = mode;
			BlendMode blendMode = (BlendMode)mode;
			// Specific Transparent Mode Settings
			switch (blendMode)
			{
				case BlendMode.Alpha:
					m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					m_Material.DisableKeyword("ALPHAPREMULTIPLY_ON");
					break;
				case BlendMode.Premultiply:
					m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					m_Material.EnableKeyword("ALPHAPREMULTIPLY_ON");
					break;
				case BlendMode.Additive:
					m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
					m_Material.DisableKeyword("ALPHAPREMULTIPLY_ON");
					break;
				case BlendMode.Multiply:
					m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
					m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					m_Material.DisableKeyword("ALPHAPREMULTIPLY_ON");
					m_Material.EnableKeyword("_ALPHAMODULATE_ON");
					break;
			}

			// General Transparent Material Settings
			m_Material.SetOverrideTag("RenderType", "Transparent");
			m_Material.SetInt("_ZWrite", 0);
			//m_Material.renderQueue = (int)RenderQueue.Transparent;
			//m_Material.renderQueue += material.HasProperty("_QueueOffset") ? (int)material.GetFloat("_QueueOffset") : 0;
			m_Material.SetShaderPassEnabled("ShadowCaster", false);

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws a color field property.
		/// </summary>
		/// <param name="name">The color propety.</param>
		/// <param name="label">The property label.</param>
		/// <param name="showAlpha">An optional parameter defining whether to show the alpha value or not.</param>
		/// <param name="isHDR">An optional parameter defining whether the value is high dynamic range. Default value is true.</param>
		public void ColorField (string name, GUIContent label, bool showAlpha = true, bool isHDR = true)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			Color color = EditorGUI.ColorField (rect, GUIContent.none, property.colorValue, false, showAlpha, isHDR);

			if (EditorGUI.EndChangeCheck ())
			{
				property.colorValue = color;
			}

			EditorGUI.showMixedValue = false;
		}

		public void DoMetallicArea(string metallicMap, string metallic, GUIContent label)
        {
			if (!m_Properties.TryGetValue(metallicMap, out MaterialProperty propertyMap))
			{
				Debug.LogFormat(s_MissingParameter, metallicMap);
				return;
			}

			if (!m_Properties.TryGetValue(metallic, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, metallicMap);
				return;
			}

			m_MaterialEditor.TexturePropertySingleLine(label, propertyMap, property);

		}

		#endregion

		#region Advanced Options
		/// <summary>
		/// Draws a cull mode property.
		/// </summary>
		/// <param name="name">The propery name.</param>
		/// <param name="label">The propery UI label.</param>
		public void CullMode (string name, GUIContent label)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = (float) (CullMode) EditorGUI.EnumPopup (rect, (CullMode) (int) property.floatValue);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws a depth test property.
		/// </summary>
		/// <param name="name">The propery name.</param>
		/// <param name="label">The propery UI label.</param>
		public void DepthTest (string name, GUIContent label)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = (float) (CompareFunction) EditorGUI.EnumPopup (rect, (CompareFunction) (int) property.floatValue);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws the depth write rendering property.
		/// </summary>
		/// <param name="name">The propery name.</param>
		/// <param name="label">The propery UI label.</param>
		/// <param name="options">The dropdown UI options.</param>
		public void DepthWrite (string name, GUIContent label, string [] options)
		{
			if (!m_Properties.TryGetValue (name, out MaterialProperty property))
			{
				Debug.LogFormat (s_MissingParameter, name);
				return;
			}

			Rect rect = DrawLabel (label);
			EditorGUI.BeginChangeCheck ();
			EditorGUI.showMixedValue = property.hasMixedValue;
			float value = EditorGUI.IntPopup (rect, (int) property.floatValue, options, s_EnabledFlag);

			if (EditorGUI.EndChangeCheck ())
			{
				property.floatValue = value;
			}

			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws the material render queue field.
		/// </summary>
		/// <param name="label">The propery name.</param>
		/// <param name="options">The dropdown UI options.</param>
		public void RenderQueue (GUIContent label, string [] options)
		{
			Rect rect = DrawLabel (label);
			//m_Material.renderQueue = EditorGUI.IntPopup (rect, m_Material.renderQueue, options, RenderParameter.renderQueue);
			GUI.enabled = false;
			EditorGUI.IntField(rect, m_Material.renderQueue);
			GUI.enabled = true;
			//m_MaterialEditor.RenderQueueField();
		}

		public void QueueOffset(GUIContent label)
		{
			//m_Material.renderQueue += IntRange(ShaderParameter.queueOffset, label, - queueOffsetRange, queueOffsetRange);
			// Check for missing parameters
			if (!m_Properties.TryGetValue(ShaderParameter.queueOffset, out MaterialProperty property))
			{
				Debug.LogFormat(s_MissingParameter, ShaderParameter.queueOffset);
				return;
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMixedValue;
			var queue = EditorGUILayout.IntSlider(label, (int)property.floatValue, -queueOffsetRange, queueOffsetRange);
			if (EditorGUI.EndChangeCheck())
				property.floatValue = queue;
			EditorGUI.showMixedValue = false;
		}

		/// <summary>
		/// Draws the GPU instancing field.
		/// </summary>
		/// <param name="label">The propery UI label.</param>
		/// <param name="options">The dropdown UI options.</param>
		public void GPUInstancing (GUIContent label, string [] options)
		{
			Rect rect = DrawLabel (label);
			int index = m_Material.enableInstancing ? 1 : 0;

			index = EditorGUI.IntPopup (rect, index, options, s_EnabledFlag);
			m_Material.enableInstancing = index == 1;
		}

		#endregion

		#region Helper Methods
		/// <summary>
		/// Sets the material's global illumination emission flag.
		/// </summary>
		private void SetEmissionFlag ()
		{

			if (m_Material.IsKeywordEnabled(ShaderParameter.useEmission))
			{
				m_Material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

				if (m_Properties.ContainsKey (ShaderParameter.emissionColor))
				{
					MaterialProperty color = m_Properties [ShaderParameter.emissionColor];

					if (color.colorValue.maxColorComponent <= 0.0f)
					{
						m_Material.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
					}
				}
			}
			else
			{
				m_Material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
			}
		}

		/// <summary>
		/// Creates the popup options and values for the specified value range.
		/// </summary>
		/// <param name="min">The minimum value range.</param>
		/// <param name="max">The maximum value range.</param>
		/// <param name="values">The value array.</param>
		/// <param name="options">The string options array.</param>
		private void GetPopupOptions (int min, int max, out int [] values, out string [] options)
		{
			values = new int [max - min];
			options = new string [max - min];

			for (int i = min; i < max; i++)
			{
				values [i] = i + min;
				options [i] = values [i].ToString ();
			}
		}

		/// <summary>
		/// Draws a label and returns the rect for the UI field element.
		/// </summary>
		/// <param name="label">The label content.</param>
		/// <returns>A field rect positioned after the label.</returns>
		private Rect DrawLabel (GUIContent label)
		{
			Rect rect = EditorGUILayout.GetControlRect ();
			rect.width -= s_FieldShort;

			EditorGUI.LabelField (rect, label);
			rect.x += rect.width;
			rect.width = s_FieldShort;

			return rect;
		}
		#endregion
	}
}