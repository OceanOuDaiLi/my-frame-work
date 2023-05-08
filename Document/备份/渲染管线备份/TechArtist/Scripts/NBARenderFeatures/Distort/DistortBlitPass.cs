using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechArtist.DistortBlit
{
    public class DistortBlitPass : ScriptableRenderPass
    {
        private DistortBlitSettings settings;
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetIdentifier destination { get; set; }

        private string m_ProfilerTag;
        private RenderTargetHandle m_TemporaryColorTexture;
        private RenderTargetHandle m_DestinationTexture;

        public Material DistortMaterial = null;
        public FilterMode filterMode { get; set; }

        public Material overrideMaterial { get; set; }
        public int overrideMaterialPassIndex { get; set; }

        public DistortBlitPass(RenderPassEvent renderPassEvent, DistortBlitSettings settings, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            this.settings = settings;
            DistortMaterial = settings.distortMaterial;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");

            if (settings.dstType == Target.TextureID)
            {
                m_DestinationTexture.Init(settings.dstTextureId);
            }
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            if (settings.setInverseViewMatrix)
            {
                Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
            }

            if (settings.dstType == Target.TextureID)
            {
                cmd.GetTemporaryRT(m_DestinationTexture.id, opaqueDesc, filterMode);
            }

            if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
            {
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                Blit(cmd, source, m_TemporaryColorTexture.Identifier(), DistortMaterial, settings.blitMaterialPassIndex);
                Blit(cmd, m_TemporaryColorTexture.Identifier(), destination);
            }
            else
            {

                Blit(cmd, source, destination, DistortMaterial, settings.blitMaterialPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (settings.dstType == Target.TextureID)
            {
                cmd.ReleaseTemporaryRT(m_DestinationTexture.id);
            }
            if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
            {
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
            }
        }
    }
}