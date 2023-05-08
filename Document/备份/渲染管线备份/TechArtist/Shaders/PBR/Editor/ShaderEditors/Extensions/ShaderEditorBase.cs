using UnityEditor;

namespace FTX.Rendering
{
	public abstract class ShaderEditorBase : ShaderGUI
	{
		protected ContentShader m_Content;
		protected ShaderEditorGUI m_ShaderGUI;

		protected bool m_UseTransmission;
		protected bool m_UseBoxProjection;

		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty [] properties)
		{
			if (m_ShaderGUI == null)
			{
				m_ShaderGUI = new ShaderEditorGUI (materialEditor);
				m_Content = new ContentShader ();
			}
			m_ShaderGUI.FetchProperties (properties);

			DrawGUIParameters ();

			m_ShaderGUI.Space();
			m_ShaderGUI.Section(m_Content.receiveshadow, ShaderParameter.useReceiveShadow);

			m_ShaderGUI.Section(m_Content.numtex_ON, ShaderParameter.useNumtex);

			m_ShaderGUI.Section (m_Content.advanced);

			m_ShaderGUI.CullMode (ShaderParameter.cullMode, m_Content.cullMode);
			m_ShaderGUI.RenderQueue (m_Content.renderQueue, m_Content.renderQueueOptions);
			m_ShaderGUI.GPUInstancing (m_Content.gpuInstancing, m_Content.enableFlagOptions);
			m_ShaderGUI.QueueOffset(m_Content.queueOffset);
			materialEditor.DoubleSidedGIField();
		}

		protected virtual void DrawGUIParameters (){ }
	}
}