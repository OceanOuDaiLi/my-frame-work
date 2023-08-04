/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */
using Live2D.Cubism.Rendering.Masking;
using UnityEngine;

namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Default materials.
    /// </summary>
    public static class CubismBuiltinMaterials
    {
        /// <summary>
        /// Default unlit material.
        /// </summary>
        public static Material Unlit
        {
            get { return LoadUnlitMaterial("Unlit"); }
        }

        /// <summary>
        /// Default unlit, additively blending material.
        /// </summary>
        public static Material UnlitAdditive
        {
            get { return LoadUnlitMaterial("UnlitAdditive"); }
        }

        /// <summary>
        /// Default unlit, multiply blending material.
        /// </summary>
        public static Material UnlitMultiply
        {
            get { return LoadUnlitMaterial("UnlitMultiply"); }
        }


        /// <summary>
        /// Default unlit masked material.
        /// </summary>
        public static Material UnlitMasked
        {
            get { return LoadUnlitMaterial("UnlitMasked"); }
        }

        /// <summary>
        /// Default unlit masked, additively blending material.
        /// </summary>
        public static Material UnlitAdditiveMasked
        {
            get { return LoadUnlitMaterial("UnlitAdditiveMasked"); }
        }

        /// <summary>
        /// Default unlit masked, multiply blending material.
        /// </summary>
        public static Material UnlitMultiplyMasked
        {
            get { return LoadUnlitMaterial("UnlitMultiplyMasked"); }
        }


        /// <summary>
        /// Default unlit masked inverted material.
        /// </summary>
        public static Material UnlitMaskedInverted
        {
            get { return LoadUnlitMaterial("UnlitMaskedInverted"); }
        }

        /// <summary>
        /// Default unlit masked inverted, additively blending material.
        /// </summary>
        public static Material UnlitAdditiveMaskedInverted
        {
            get { return LoadUnlitMaterial("UnlitAdditiveMaskedInverted"); }
        }

        /// <summary>
        /// Default unlit masked inverted, multiply blending material.
        /// </summary>
        public static Material UnlitMultiplyMaskedInverted
        {
            get { return LoadUnlitMaterial("UnlitMultiplyMaskedInverted"); }
        }


        /// <summary>
        /// Default unlit material.
        /// </summary>
        public static Material UnlitCulling
        {
            get { return LoadUnlitMaterial("UnlitCulling"); }
        }

        /// <summary>
        /// Default unlit, additively blending material.
        /// </summary>
        public static Material UnlitAdditiveCulling
        {
            get { return LoadUnlitMaterial("UnlitAdditiveCulling"); }
        }

        /// <summary>
        /// Default unlit, multiply blending material.
        /// </summary>
        public static Material UnlitMultiplyCulling
        {
            get { return LoadUnlitMaterial("UnlitMultiplyCulling"); }
        }


        /// <summary>
        /// Default unlit masked material.
        /// </summary>
        public static Material UnlitMaskedCulling
        {
            get { return LoadUnlitMaterial("UnlitMaskedCulling"); }
        }

        /// <summary>
        /// Default unlit masked, additively blending material.
        /// </summary>
        public static Material UnlitAdditiveMaskedCulling
        {
            get { return LoadUnlitMaterial("UnlitAdditiveMaskedCulling"); }
        }

        /// <summary>
        /// Default unlit masked, multiply blending material.
        /// </summary>
        public static Material UnlitMultiplyMaskedCulling
        {
            get { return LoadUnlitMaterial("UnlitMultiplyMaskedCulling"); }
        }


        /// <summary>
        /// Default unlit masked inverted material.
        /// </summary>
        public static Material UnlitMaskedInvertedCulling
        {
            get { return LoadUnlitMaterial("UnlitMaskedInvertedCulling"); }
        }

        /// <summary>
        /// Default unlit masked inverted, additively blending material.
        /// </summary>
        public static Material UnlitAdditiveMaskedInvertedCulling
        {
            get { return LoadUnlitMaterial("UnlitAdditiveMaskedInvertedCulling"); }
        }

        /// <summary>
        /// Default unlit masked inverted, multiply blending material.
        /// </summary>
        public static Material UnlitMultiplyMaskedInvertedCulling
        {
            get { return LoadUnlitMaterial("UnlitMultiplyMaskedInvertedCulling"); }
        }



        /// <summary>
        /// Default mask material.
        /// </summary>
        public static Material Mask
        {
            get { return LoadMaskMaterial(); }
        }

        /// <summary>
        /// Default culled mask material.
        /// </summary>
        public static Material MaskCulling
        {
            get { return LoadMaskCullingMaterial(); }
        }


        #region Helper Methods

        /// <summary>
        /// Resource directory of builtin <see cref="Material"/>s.
        /// </summary>
        private const string ResourcesDirectory = "Assets/Resources/live2d/Materials";

        /// <summary>
        /// Loads an unlit material.
        /// </summary>
        /// <param name="name">Material name.</param>
        /// <returns>The material.</returns>
        private static Material LoadUnlitMaterial(string name)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(ResourcesDirectory + "/" + name + ".mat");
#else
           return (Material)UnityEngine.Resources.Load("live2d/Materials/" + name + ".mat");
#endif
        }

        /// <summary>
        /// Loads an mask material.
        /// </summary>
        /// <returns>The material.</returns>
        private static Material LoadMaskMaterial()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(ResourcesDirectory + "/Mask.mat");
#else
            return (Material)UnityEngine.Resources.Load("live2d/Materials/Mask.mat");
#endif

        }

        /// <summary>
        /// Loads an mask culling material.
        /// </summary>
        /// <returns>The material.</returns>
        private static Material LoadMaskCullingMaterial()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(ResourcesDirectory + "/MaskCulling.mat");
#else
            return (Material)UnityEngine.Resources.Load("live2d/Materials/MaskCulling.mat");
#endif
        }

        #endregion
    }
}
