using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    class SerializedFrameSettings
    {
        public SerializedProperty rootData;
        public SerializedProperty rootOverride;
        
        public LitShaderMode litShaderMode
        {
            get => IsEnable(FrameSettingsField.LitShaderMode) ? LitShaderMode.Deferred : LitShaderMode.Forward;
            set => SetEnable(FrameSettingsField.LitShaderMode, value == LitShaderMode.Deferred);
        }

        public bool IsEnable(FrameSettingsField field) => rootData.GetBitArrayAt((uint)field);
        public void SetEnable(FrameSettingsField field, bool value) => rootData.SetBitArrayAt((uint)field, value);

        public bool GetOverrides(FrameSettingsField field) => rootOverride == null ? false : rootOverride.GetBitArrayAt((uint)field);
        public void SetOverrides(FrameSettingsField field, bool value) => rootOverride?.SetBitArrayAt((uint)field, value);

        public SerializedFrameSettings(SerializedProperty rootData, SerializedProperty rootOverride)
        {
            this.rootData = rootData.FindPropertyRelative("bitDatas");
            this.rootOverride = rootOverride?.FindPropertyRelative("mask");  //rootOverride can be null in case of hdrpAsset defaults
        }
    }
}
