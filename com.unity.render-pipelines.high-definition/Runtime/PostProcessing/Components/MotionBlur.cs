using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public enum MotionBlurMode
    {
        Off,
        UsePhysicalCamera,
        Manual
    }

    [Serializable, VolumeComponentMenu("Post-processing/Motion Blur")]
    public sealed class MotionBlur : VolumeComponent, IPostProcessComponent
    {

        // Physical settings
        public MinFloatParameter intensity = new MinFloatParameter(20.0f, 0.0f);

        private MinIntParameter tileSize = new MinIntParameter(20, 1);

        // Advanced settings
        public BoolParameter highQuality = new BoolParameter(true);

        public bool IsActive()
        {
            return intensity > 0.0f;
        }
    }
}
