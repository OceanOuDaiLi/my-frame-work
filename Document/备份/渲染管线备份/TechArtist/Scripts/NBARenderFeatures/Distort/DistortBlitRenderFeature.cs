using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechArtist.DistortBlit
{
    [System.Serializable]
    public class DistortBlitSettings
    {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

        public Material distortMaterial = null;
        public int blitMaterialPassIndex = 0;
        public bool setInverseViewMatrix = false;

        public Target srcType = Target.CameraColor;
        public string srcTextureId = "_CameraColorTexture";
        public RenderTexture srcTextureObject;

        public Target dstType = Target.CameraColor;
        public string dstTextureId = "_BlitPassTexture";
        public RenderTexture dstTextureObject;

        public RenderQueueType RenderQueueType;
        public LayerMask LayerMask;
        public string[] PassNames;

        public DistortBlitSettings()
        {
            RenderQueueType = RenderQueueType.Opaque;
            LayerMask = 0;
        }
    }

    public enum RenderQueueType
    {
        Opaque,
        Transparent,
    }

    public enum Target
    {
        CameraColor,
        TextureID,
        RenderTextureObject
    }

    public class DistortBlitRenderFeature : ScriptableRendererFeature
    {
        public DistortBlitSettings settings = new DistortBlitSettings();

        DistortBlitPass blitPass;

        private RenderTargetIdentifier srcIdentifier, dstIdentifier;

        public override void Create()
        {
            var passIndex = settings.distortMaterial != null ? settings.distortMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new DistortBlitPass(settings.Event, settings, name);

            if (settings.Event == RenderPassEvent.AfterRenderingPostProcessing)
            {
                Debug.LogWarning("Note that the \"After Rendering Post Processing\"'s Color target doesn't seem to work? (or might work, but doesn't contain the post processing) :( -- Use \"After Rendering\" instead!");
            }

            UpdateSrcIdentifier();
            UpdateDstIdentifier();
        }

        private void UpdateSrcIdentifier()
        {
            srcIdentifier = UpdateIdentifier(settings.srcType, settings.srcTextureId, settings.srcTextureObject);
        }

        private void UpdateDstIdentifier()
        {
            dstIdentifier = UpdateIdentifier(settings.dstType, settings.dstTextureId, settings.dstTextureObject);
        }

        private RenderTargetIdentifier UpdateIdentifier(Target type, string s, RenderTexture obj)
        {
            if (type == Target.RenderTextureObject)
            {
                return obj;
            }
            else if (type == Target.TextureID)
            {
                //RenderTargetHandle m_RTHandle = new RenderTargetHandle();
                //m_RTHandle.Init(s);
                //return m_RTHandle.Identifier();
                return s;
            }
            return new RenderTargetIdentifier();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {

            if (settings.distortMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            if (settings.Event == RenderPassEvent.AfterRenderingPostProcessing)
            {
            }
            else if (settings.Event == RenderPassEvent.AfterRendering && renderingData.postProcessingEnabled)
            {
                // If event is AfterRendering, and src/dst is using CameraColor, switch to _AfterPostProcessTexture instead.
                if (settings.srcType == Target.CameraColor)
                {
                    settings.srcType = Target.TextureID;
                    settings.srcTextureId = "_AfterPostProcessTexture";
                    UpdateSrcIdentifier();
                }
                if (settings.dstType == Target.CameraColor)
                {
                    settings.dstType = Target.TextureID;
                    settings.dstTextureId = "_AfterPostProcessTexture";
                    UpdateDstIdentifier();
                }
            }
            else
            {
                // If src/dst is using _AfterPostProcessTexture, switch back to CameraColor
                if (settings.srcType == Target.TextureID && settings.srcTextureId == "_AfterPostProcessTexture")
                {
                    settings.srcType = Target.CameraColor;
                    settings.srcTextureId = "";
                    UpdateSrcIdentifier();
                }
                if (settings.dstType == Target.TextureID && settings.dstTextureId == "_AfterPostProcessTexture")
                {
                    settings.dstType = Target.CameraColor;
                    settings.dstTextureId = "";
                    UpdateDstIdentifier();
                }
            }

            var src = (settings.srcType == Target.CameraColor) ? renderer.cameraColorTarget : srcIdentifier;
            var dest = (settings.dstType == Target.CameraColor) ? renderer.cameraColorTarget : dstIdentifier;

            blitPass.Setup(src, dest);
            //Debug.Log(src);
            renderer.EnqueuePass(blitPass);
        }
    }
}