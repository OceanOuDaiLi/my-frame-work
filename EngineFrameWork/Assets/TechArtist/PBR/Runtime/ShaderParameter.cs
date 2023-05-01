/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ShaderParameter.cs
	Author:		DaiLi.Ou
	Descriptions: Shader Parameters.
*********************************************************************/


namespace TechArtist
{
    public static class ShaderParameter
    {
        #region Default material properties name.

        // Albedo.
        public static readonly string albedoMap = "_AlbedoMap";
        public static readonly string albedoColor = "_AlbedoColor";
        // Normal. 
        public static readonly string normalMap = "_NormalMap";
        public static readonly string normalScale = "_NormalScale";
        // Metallic.
        public static readonly string metallicMap = "_MetallicMap";
        public static readonly string metallicIntensity = "_Metallic";
        public static readonly string glossiness = "_Glossiness";                                   // equal with smoothness. 
        public static readonly string aoIntensity = "_AOIntensity";

        // Advanced
        public static readonly string cullMode = "_CullMode";
        public static readonly string blendMode = "_Blend";
        public static readonly string queueOffset = "_QueueOffset";

        #endregion
    }
}