using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;


 // All params need renaming...

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(MotionBlur))]
    sealed class MotionBlurEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_SampleCount;
        SerializedDataParameter m_MaxVelocityInPixels;

        SerializedDataParameter m_MinVelInPixels;
        SerializedDataParameter m_TileMinMaxVelRatioForHighQuality;

        public override bool hasAdvancedMode => true;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<MotionBlur>(serializedObject);

            m_SampleCount = Unpack(o.Find(x => x.sampleCount));
            m_MinVelInPixels = Unpack(o.Find(x => x.minVelInPixels));
            m_MaxVelocityInPixels = Unpack(o.Find(x => x.maxVelocity));
            m_TileMinMaxVelRatioForHighQuality = Unpack(o.Find(x => x.tileMinMaxVelRatioForHighQuality));
        }

        public override void OnInspectorGUI()
        {

            bool advanced = isInAdvancedMode;

            PropertyField(m_SampleCount);
            PropertyField(m_MaxVelocityInPixels);

            if (advanced)
            {
                PropertyField(m_MinVelInPixels);
                PropertyField(m_TileMinMaxVelRatioForHighQuality);

                // Advanced stuff
            }
        }
    }
}
