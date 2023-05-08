#if UNITY_EDITOR
namespace FTX.Rendering
{
    public sealed class ShaderGUIStandardCharacter : NBAShaderEditorBase
    {
        protected override void DrawGUIParameters()
        {
            m_ShaderGUI.Section(m_Content.standard, before: -7.0f);
            m_ShaderGUI.TextureSlotWithColor(ShaderParameter.albedoMap, ShaderParameter.albedoColor, m_Content.albedoMap, m_Content.channelsAlbedoOnly, true);

            //m_ShaderGUI.DoMetallicArea(ShaderParameter.metallicMap, ShaderParameter.metallic, m_Content.metallicMap);

            m_ShaderGUI.TextureSlot(ShaderParameter.metallicMap, m_Content.metallic, m_Content.channelsMetallicSmoothnessAO);
            m_ShaderGUI.TextureSlot(ShaderParameter.normalMap, m_Content.normalMap, m_Content.channelsNormal);

            m_ShaderGUI.Space();
            m_ShaderGUI.FloatRange(ShaderParameter.metallic, 0f, 1f, m_Content.metallic);
            m_ShaderGUI.FloatRange(ShaderParameter.glossiness, 0f, 2f, m_Content.smoothness);
            m_ShaderGUI.FloatRange(ShaderParameter.aoIntensity, 0f, 1f, m_Content.ao);


            //号码牌
            m_ShaderGUI.Section("Use Numtex", ShaderParameter.useNumtex);

            m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture, m_Content.NumberTexture, m_Content.channelsNumTex);

            m_ShaderGUI.Section("TEX2_ON", ShaderParameter.useNumtex02);

            m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture2, m_Content.NumberTexture2, m_Content.channelsNumTex2);
            //m_ShaderGUI.FloatField(ShaderParameter.numberS,m_Content.NumberS);
            //m_ShaderGUI.FloatField(ShaderParameter.numberG, m_Content.NumberG);
            //m_ShaderGUI.FloatField(ShaderParameter.numberDis, m_Content.NumberDis);

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
            m_ShaderGUI.FloatRange(ShaderParameter.shadowIntensity, 0.0f, 1.0f, m_Content.shadowIntensity);
            m_ShaderGUI.FloatRange(ShaderParameter.reflectionNoiseScale, 0.0f, 2.0f, m_Content.reflectionNoiseScale);

            if (m_ShaderGUI.Section(m_Content.useProjectShadow, ShaderParameter.useProjectShadow))
            {
                m_ShaderGUI.VectorRange(ShaderParameter.shadowDirection, m_Content.projectShadowDirection);
                m_ShaderGUI.FloatField(ShaderParameter.shadowAlpha, m_Content.projectShadowLengh);
                m_ShaderGUI.FloatField(ShaderParameter.shadowFallOff, m_Content.projectShadowFallOff);

            }

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