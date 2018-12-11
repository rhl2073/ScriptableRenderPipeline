using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(MotionBlur))]
    sealed class MotionBlurEditor : VolumeComponentEditor
    {

        public override bool hasAdvancedMode => true;

        public override void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {

            bool advanced = isInAdvancedMode;

            if (advanced)
            {
                // Advanced stuff
            }
        }
    }
}
