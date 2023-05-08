#if UNITY_EDITOR
namespace FTX.Rendering
{
    public sealed class ShaderGUIIndoorScene : NBAShaderEditorBase
    {
        protected override void DrawGUIParameters()
        {
            m_ShaderGUI.Section(m_Content.standard, before: -7.0f);
            m_ShaderGUI.TextureSlotWithColor(ShaderParameter.albedoMap, ShaderParameter.albedoColor, m_Content.albedoMap, m_Content.channelsAlbedoOnly, true);

            m_ShaderGUI.TextureSlot(ShaderParameter.metallicMap, m_Content.metallic, m_Content.channelsMetallicSmoothnessAO);
            m_ShaderGUI.TextureSlot(ShaderParameter.normalMap, m_Content.normalMap, m_Content.channelsNormal);

            m_ShaderGUI.Space();
            m_ShaderGUI.FloatRange(ShaderParameter.metallic, 0f, 1f, m_Content.metallic);
            m_ShaderGUI.FloatRange(ShaderParameter.glossiness, 0f, 2f, m_Content.smoothness);
            m_ShaderGUI.FloatRange(ShaderParameter.aoIntensity, 0f, 1f, m_Content.ao);


            m_ShaderGUI.FloatRange(ShaderParameter.uvScale, 0.1f, 10.0f, m_Content.uvScale);
            m_ShaderGUI.FloatRange(ShaderParameter.normalScale, 0.1f, 5.0f, m_Content.normalScale);
            m_ShaderGUI.FloatRange(ShaderParameter.shadowIntensity, 0.0f, 1.0f, m_Content.shadowIntensity);
            m_ShaderGUI.FloatRange(ShaderParameter.reflectionNoiseScale, 0.0f, 2.0f, m_Content.reflectionNoiseScale);

            m_ShaderGUI.Section(m_Content.useProbes, ShaderParameter.useProbes);

            if (m_ShaderGUI.Section(m_Content.useEmission, ShaderParameter.useEmission))
            {
                m_ShaderGUI.TextureSlotWithColor(ShaderParameter.emissionMap, ShaderParameter.emissionColor, m_Content.emissionMap, m_Content.emissionMapChannels, true, true);
                m_ShaderGUI.FloatRange(ShaderParameter.netLightness, 0.0f, 1.0f, m_Content.netLightness);
            }

            if (m_ShaderGUI.Section(m_Content.useVertexColor, ShaderParameter.useVertexColor))
            {
                m_ShaderGUI.ColorField(ShaderParameter.vertexColorR, m_Content.vertexColorR, true, false);
                m_ShaderGUI.ColorField(ShaderParameter.vertexColorG, m_Content.vertexColorG, true, false);
                m_ShaderGUI.ColorField(ShaderParameter.vertexColorB, m_Content.vertexColorB, true, false);
                m_ShaderGUI.ColorField(ShaderParameter.vertexColorA, m_Content.vertexColorA, true, false);

            }
        }
    }
}
#endif