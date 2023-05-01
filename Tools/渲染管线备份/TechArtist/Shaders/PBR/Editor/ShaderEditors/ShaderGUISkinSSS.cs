#if UNITY_EDITOR
namespace FTX.Rendering
{
    public sealed class ShaderGUISkinSSS : NBAShaderEditorBase
    {
        protected override void DrawGUIParameters()
        {
            m_ShaderGUI.Section(m_Content.skinSSS, before: -7.0f);


            m_ShaderGUI.TextureSlotWithColor(ShaderParameter.albedoMap, ShaderParameter.albedoColor, m_Content.albedoMap, m_Content.channelsAlbedoOnly);
            m_ShaderGUI.TextureSlot(ShaderParameter.thicknessMap, m_Content.thickness, m_Content.channelsThickness);

            m_ShaderGUI.FloatRange(ShaderParameter.glossiness, 0f, 2f, m_Content.smoothness);
            m_ShaderGUI.ColorField(ShaderParameter.translucencyColor, m_Content.transmissionColor, false, false);

            float x = m_ShaderGUI.FloatRange(ShaderParameter.thicknessContrast, 0f, 5f, m_Content.thicknessContrast);
            float y = m_ShaderGUI.FloatRange(ShaderParameter.thicknessBrightness, 0f, 2f, m_Content.thicknessBrightness);
           
            m_ShaderGUI.FloatRange(ShaderParameter.aoIntensity, 0f, 1f, m_Content.ao);


            m_ShaderGUI.TextureSlot(ShaderParameter.normalMap, m_Content.normalMap, m_Content.channelsNormal);
            m_ShaderGUI.TextureSlot(ShaderParameter.BSSRDFLutMap, m_Content.SSSLutMap, m_Content.channelsSSSLut);
            float w = m_ShaderGUI.FloatRange(ShaderParameter.SSSFactor, 0.01f, 1f, m_Content.SSSFactor);
            float z = m_ShaderGUI.FloatRange(ShaderParameter.SSSPower, 0.01f, 5f, m_Content.SSSPower);
            m_ShaderGUI.ColorField(ShaderParameter.SSSColor, m_Content.SSSColor, false, false);

            //∫≈¬Î≈∆
            m_ShaderGUI.Section("Use Numtex", ShaderParameter.useNumtex);

            m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture, m_Content.NumberTexture, m_Content.channelsNumTex);

            m_ShaderGUI.Section("TEX2_ON", ShaderParameter.useNumtex02);
            m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture2, m_Content.NumberTexture2, m_Content.channelsNumTex2);
            //m_ShaderGUI.FloatField(ShaderParameter.numberS,m_Content.NumberS);
            //m_ShaderGUI.FloatField(ShaderParameter.numberG, m_Content.NumberG);
            //m_ShaderGUI.FloatField(ShaderParameter.numberDis, m_Content.NumberDis);

            //Project Shadow∆Ω√Ê“ı”∞
            m_ShaderGUI.VectorRange(ShaderParameter.shadowDirection, m_Content.projectShadowDirection);
            m_ShaderGUI.FloatRange(ShaderParameter.shadowAlpha, 0f, 10f, m_Content.projectShadowLengh);
            m_ShaderGUI.FloatRange(ShaderParameter.shadowFallOff, -1.0f, 1.0f, m_Content.projectShadowFallOff);


            m_ShaderGUI.FloatRange(ShaderParameter.stencil, 0f, 10f, m_Content.NumberStencil);

            m_ShaderGUI.FloatRange(ShaderParameter.numberS, 0, 9.9f, m_Content.NumberS);
            m_ShaderGUI.FloatRange(ShaderParameter.numberG, 0, 9.9f, m_Content.NumberG);
            m_ShaderGUI.FloatRange(ShaderParameter.numberDis, -1.0f, 1.0f, m_Content.NumberDis);

            m_ShaderGUI.VectorRange(ShaderParameter.numBackOffSet, m_Content.NumBackOffSet);
            m_ShaderGUI.VectorRange(ShaderParameter.numFrontOffSet, m_Content.NumFrontOffSet);

            m_ShaderGUI.VectorRange(ShaderParameter.numBackOffSet2, m_Content.NumBackOffSet2);
            m_ShaderGUI.VectorRange(ShaderParameter.numFrontOffSet2, m_Content.NumFrontOffSet2);

            m_ShaderGUI.Space();

            m_ShaderGUI.FloatRange(ShaderParameter.uvScale, 0.1f, 10.0f, m_Content.uvScale);
            m_ShaderGUI.FloatRange(ShaderParameter.normalScale, 0.1f, 5.0f, m_Content.normalScale);
            m_ShaderGUI.ColorField(ShaderParameter.skinShadowColor, m_Content.skinShadowColor, false, false);

            //if (m_ShaderGUI.Section(m_Content.userNumberPlat, ShaderParameter.useNumberPlat))
            //{
            //    m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture, m_Content.NumberTexture, m_Content.channelsAlbedoTransparent);
            //    m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture2, m_Content.NumberTexture2, m_Content.channelsAlbedoTransparent);

            //    m_ShaderGUI.FloatRange(ShaderParameter.stencil, 0f, 10f, m_Content.NumberStencil);

            //    m_ShaderGUI.FloatRange(ShaderParameter.numberS, 1.0f, 9.9f, m_Content.NumberS);
            //    m_ShaderGUI.FloatRange(ShaderParameter.numberG, 1.0f, 9.9f, m_Content.NumberG);
            //    m_ShaderGUI.FloatRange(ShaderParameter.numberDis, -1.0f, 1.0f, m_Content.NumberDis);

            //    m_ShaderGUI.VectorRange(ShaderParameter.numBackOffSet, m_Content.NumBackOffSet);
            //    m_ShaderGUI.VectorRange(ShaderParameter.numFrontOffSet, m_Content.NumFrontOffSet);

            //    m_ShaderGUI.VectorRange(ShaderParameter.numBackOffSet2, m_Content.NumBackOffSet2);
            //    m_ShaderGUI.VectorRange(ShaderParameter.numFrontOffSet2, m_Content.NumFrontOffSet2);
            //}

            if (m_ShaderGUI.Section(m_Content.useVertexColor, ShaderParameter.useVertexColor))
            {
                m_ShaderGUI.FloatField(ShaderParameter.rScale, m_Content.rScale);
                m_ShaderGUI.FloatField(ShaderParameter.gScale, m_Content.gScale);
                m_ShaderGUI.FloatField(ShaderParameter.bScale, m_Content.bScale);
                m_ShaderGUI.FloatField(ShaderParameter.aScale, m_Content.aScale);
            }


        }
    }
}
#endif