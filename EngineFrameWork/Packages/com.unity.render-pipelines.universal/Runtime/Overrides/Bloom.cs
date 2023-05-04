using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Bloom")]
    public sealed class Bloom : VolumeComponent, IPostProcessComponent
    {
        public MinFloatParameter threshold = new MinFloatParameter(0.9f, 0f);

        public MinFloatParameter intensity = new MinFloatParameter(0f, 0f);

        public ClampedFloatParameter scatter = new ClampedFloatParameter(0.1f, 0f, 1f);

        public ColorParameter tint = new ColorParameter(Color.white, false, false, true);

        public ClampedFloatParameter downscale = new ClampedFloatParameter(0.5f, 0f, 1f);

        public bool IsActive() => intensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
