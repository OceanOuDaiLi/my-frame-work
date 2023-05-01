
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	IShaderEditorModel.cs
	Author:		DaiLi.Ou
	Descriptions: ShaderEditorUtility Interface.
*********************************************************************/

using UnityEngine;

namespace TechArtist.Editor
{
    public interface IShaderEditorUtility
    {
        //-------------------------------------------------
        //---------- Public Methods For Draw GUI ----------
        //-------------------------------------------------
        /// <summary>
        /// Adds a vertical/horizontal space between elements.
        /// </summary>
        /// <param name="pixels"></param>
        public void Space(float pixels = 5.0f);

        /// <summary>
        /// Drawing rect content with a bold title label,And optional spacing before and after.
        /// </summary>
        /// <param name="label">The section label.</param>
        /// <param name="before">Optional spacing before the section, 3.0 by default.</param>
        /// <param name="after">Optional spacing after the section, 1.0 bu default.</param>
        public void Section(string label, float before = 3.0f, float after = 1.0f);

        /// <summary>
        /// Drawing rect content with a bold title with a keyword toggle,And optional spacing before and after.
        /// </summary>
        /// <param name="label">The Section label.</param>
        /// <param name="keyword">The materials'shader keyword.</param>
        /// <param name="before">Optional spacing before the section.</param>
        /// <param name="after">Optional spacing after the section.</param>
        public bool Section(string label, string keyword, float before = 3.0f, float after = 1.0f);

        /// <summary>
        /// Drawing a texture slot with channel infomation.
        /// </summary>
        /// <param name="name">The name of the texture property.</param>
        /// <param name="label">The texture label.The tooltip is a string of channel names separated by comma (',')</param>
        /// <param name="channelInfo">Additional infomation about the texture channel layout.</param>
        public void TextureSlot(string name, GUIContent label, string[] channelInfo);

        /// <summary>
        /// Drawing a texture slot with channel information.
        /// </summary>
        /// <param name="nameTexture">The name of the texture property.</param>
        /// <param name="nameColor">The name of the color property.</param>
        /// <param name="label">The texture label.The tooltip is a string of channel names separated by comma (',')</param>
        /// <param name="channelInfo">Additional information about the texture channel layout.</param>
        /// <param name="hasAlpha">Optional parameter describleing whether the color has an aloha vaule, false by default.</param>
        /// <param name="isHDR">Optional parameter idicating whether the color is LDR or HDR, false by default.</param>
        public void TextureSlotWithColor(string nameTexture, string nameColor, GUIContent label, string[] channelInfo, bool hasAlpha = false, bool isHDR = false);

        /// <summary>
        /// Drawing a Vector4Filed property.
        /// </summary>
        /// <param name="name">The material vector4 property name</param>
        /// <param name="label">The property label.</param>
        public void VectorRange(string name, GUIContent label);

        /// <summary>
        /// Drawing a float field property.
        /// </summary>
        /// <param name="name">The material float property name.</param>
        /// <param name="label">The property label.</param>
        /// <returns>Curent float value</returns>
        public float FloatField(string name, GUIContent label);

        /// <summary>
        /// Drawing a float range slider property.
        /// </summary>
        /// <param name="name">The material float range property name.</param>
        /// <param name="min">The float min range value.</param>
        /// <param name="max">The float max range value.</param>
        /// <param name="label">The property label.</param>
        /// <returns>Curent float value</returns>
        public float FloatRange(string name, float min, float max, GUIContent label);

        /// <summary>
        /// Drawing a color field property.
        /// </summary>
        /// <param name="name">The color property.</param>
        /// <param name="label">The property lbael.</param>
        /// <param name="showAlpha">Optional parameter defining whether to show the aloha value or not.</param>
        /// <param name="isHDR">Optional parameter defining whether the value is high dynamic rang.true by default.</param>
        public void ColorField(string name, GUIContent label, bool showAlpha = true, bool isHDR = true);

        /// <summary>
        /// Drawing a label and returns the rect for the GUI field elements.
        /// </summary>
        /// <param name="label">The label content.</param>
        /// <returns>A field rect positioned after the label.</returns>
        public Rect DrawLabel(GUIContent label);

    }
}