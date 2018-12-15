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
        public MinIntParameter sampleCount = new MinIntParameter(32, 2);


        // Physical settings
        public MinFloatParameter intensity = new MinFloatParameter(20.0f, 0.0f);
        private MinIntParameter tileSize = new MinIntParameter(16, 1);

        public MinFloatParameter maxVelocity = new MinFloatParameter(64.0f, 1.0f);

        // Advanced settings
        public MinFloatParameter minVelSqInPixels = new MinFloatParameter(0.5f, 0.0f);
        public MinFloatParameter tileMinMaxVelSqRatioForHighQuality = new MinFloatParameter(0.5f, 0.0f);

        public BoolParameter highQuality = new BoolParameter(true);

        public bool IsActive()
        {
            return intensity > 0.0f;
        }
    }
}
