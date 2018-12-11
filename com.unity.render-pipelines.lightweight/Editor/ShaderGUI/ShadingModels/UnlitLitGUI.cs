using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.LWRP.ShaderGUI
{
    public static class BakedLitGUI
    {
        public static class Styles
        {
            public static GUIContent sampleGILabel = new GUIContent("Global Illumination",
                "If enabled Global Illumination will be sampled from Ambient lighting, Lightprobes or Lightmap.");

        }

        public struct BakedLitProperties
        {
            // Surface Input Props
            public MaterialProperty bumpMapProp;

            public BakedLitProperties(MaterialProperty[] properties)
            {
                // Surface Input Props
                bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
            }
        }

        public static void Inputs(BakedLitProperties properties, MaterialEditor materialEditor)
        {
            BaseShaderGUI.DoNormalArea(materialEditor, properties.bumpMapProp);
        }

        public static void SetMaterialKeywords(Material material)
        {
            bool normalMap = material.GetTexture("_BumpMap");
            CoreUtils.SetKeyword(material, "_NORMALMAP", normalMap);
        }
    }
}
