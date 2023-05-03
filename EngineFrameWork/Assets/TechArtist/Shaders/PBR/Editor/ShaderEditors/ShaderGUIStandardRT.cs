namespace FTX.Rendering
{
	public sealed class ShaderGUIStandardRT : ShaderEditorBase
	{
		protected override void DrawGUIParameters()
		{
			m_ShaderGUI.Section(m_Content.standard, before: -7.0f);

            m_ShaderGUI.Space();
            //m_ShaderGUI.TextureSlot(ShaderParameter.mainTexture, m_Content.NumberMainTexture, m_Content.channelsAlbedoTransparent);
            //m_ShaderGUI.VectorRange(ShaderParameter.offset, m_Content.NumberOffset);
            //m_ShaderGUI.VectorRange(ShaderParameter.tilling, m_Content.NumberTilling);


            m_ShaderGUI.TextureSlotWithColor(ShaderParameter.albedoMap, ShaderParameter.albedoColor, m_Content.albedoMap, m_Content.channelsAlbedoOnly, true);
            //m_ShaderGUI.TextureSlot(ShaderParameter.normalMap, m_Content.normalMap, m_Content.channelsNormal);


            //m_ShaderGUI.Space();
            //m_ShaderGUI.FloatRange(ShaderParameter.numberS, -100, 100, m_Content.NumberS);
            //m_ShaderGUI.FloatRange(ShaderParameter.numberG, -100, 100, m_Content.NumberG);
            //m_ShaderGUI.FloatRange(ShaderParameter.numberDis, -100, 100, m_Content.NumberDis);
            //m_ShaderGUI.VectorRange(ShaderParameter.numBackOffSet, m_Content.NumBackOffSet);
            //m_ShaderGUI.VectorRange(ShaderParameter.numFrontOffSet, m_Content.NumFrontOffSet);

            //m_ShaderGUI.Space();
            //m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture, m_Content.NumberTexture, m_Content.channelsAlbedoTransparent);
            //         m_ShaderGUI.TextureSlot(ShaderParameter.numberTexture2, m_Content.NumberTexture2, m_Content.channelsAlbedoTransparent);

            //m_ShaderGUI.Space(10);
            //m_ShaderGUI.FloatRange(ShaderParameter.aoIntensity, 0f, 1f, m_Content.ao);
            //m_ShaderGUI.FloatRange(ShaderParameter.metallic, 0f, 1f, m_Content.metallic);
            //m_ShaderGUI.FloatRange(ShaderParameter.uvScale, 0.1f, 10.0f, m_Content.uvScale);
            //m_ShaderGUI.FloatRange(ShaderParameter.glossiness, 0f, 2f, m_Content.smoothness);

            if (m_ShaderGUI.Section(m_Content.useEmission, ShaderParameter.useEmission))
			{
				m_ShaderGUI.TextureSlotWithColor(ShaderParameter.emissionMap, ShaderParameter.emissionColor, m_Content.emissionMap, m_Content.emissionMapChannels, true, true);
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