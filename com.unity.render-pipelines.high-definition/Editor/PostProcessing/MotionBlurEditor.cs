using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;


 // All params need renaming...

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(MotionBlur))]
    sealed class MotionBlurEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_MinVelSqInPixels;
        SerializedDataParameter m_TileMinMaxVelSqRatioForHighQuality;

        public override bool hasAdvancedMode => true;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<MotionBlur>(serializedObject);

            m_MinVelSqInPixels = Unpack(o.Find(x => x.minVelSqInPixels));

            m_TileMinMaxVelSqRatioForHighQuality = Unpack(o.Find(x => x.tileMinMaxVelSqRatioForHighQuality));
        }

        public override void OnInspectorGUI()
        {

            bool advanced = isInAdvancedMode;

            if (advanced)
            {
                PropertyField(m_MinVelSqInPixels);
                PropertyField(m_TileMinMaxVelSqRatioForHighQuality);

                // Advanced stuff
            }
        }
    }
}
