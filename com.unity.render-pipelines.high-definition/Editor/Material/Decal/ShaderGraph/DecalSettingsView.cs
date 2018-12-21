using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline.Drawing
{
    class DecalSettingsView : VisualElement
    {
        DecalMasterNode m_Node;

        IntegerField m_DrawOrderField;

        Label CreateLabel(string text, int indentLevel)
        {
            string label = "";
            for (var i = 0; i < indentLevel; i++)
            {
                label += "    ";
            }
            return new Label(label + text);
        }

        public DecalSettingsView(DecalMasterNode node)
        {
            m_Node = node;
            PropertySheet ps = new PropertySheet();
            
            int indentLevel = 0;
            ps.Add(new PropertyRow(CreateLabel("Affect Metal", indentLevel)), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.affectsMetal.isOn;
                    toggle.RegisterValueChangedCallback(ChangeAffectsMetal);
                });
            });

            ps.Add(new PropertyRow(CreateLabel("Affect AO", indentLevel)), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.affectsAO.isOn;
                    toggle.RegisterValueChangedCallback(ChangeAffectsAO);
                });
            });

            ps.Add(new PropertyRow(CreateLabel("Affect Smoothness", indentLevel)), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.affectsSmoothness.isOn;
                    toggle.RegisterValueChangedCallback(ChangeAffectsSmoothness);
                });
            });

            m_DrawOrderField = new IntegerField();
            ps.Add(new PropertyRow(CreateLabel("Draw Order", indentLevel)), (row) =>
            {
                row.Add(m_DrawOrderField, (field) =>
                {
                    field.value = m_Node.drawOrder;
                    field.RegisterValueChangedCallback(ChangeDrawOrder);
                });
            });

            Add(ps);
        }

        void ChangeAffectsMetal(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Affects Metal Change");
            ToggleData td = m_Node.affectsMetal;
            td.isOn = evt.newValue;
            m_Node.affectsMetal = td;            
        }

        void ChangeAffectsAO(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Affects AO Change");
            ToggleData td = m_Node.affectsAO;
            td.isOn = evt.newValue;
            m_Node.affectsAO = td;
        }

        void ChangeAffectsSmoothness(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Affects Smoothness Change");
            ToggleData td = m_Node.affectsSmoothness;
            td.isOn = evt.newValue;
            m_Node.affectsSmoothness = td;
        }

        void ChangeDoubleSidedMode(ChangeEvent<Enum> evt)
        {
            //if (Equals(m_Node.doubleSidedMode, evt.newValue))
            //    return;

            //m_Node.owner.owner.RegisterCompleteObjectUndo("Double Sided Mode Change");
            //m_Node.doubleSidedMode = (DoubleSidedMode)evt.newValue;
        }

        void ChangeMaterialType(ChangeEvent<Enum> evt)
        {
            //if (Equals(m_Node.materialType, evt.newValue))
            //    return;

            //m_Node.owner.owner.RegisterCompleteObjectUndo("Material Type Change");
            //m_Node.materialType = (DecalMasterNode.MaterialType)evt.newValue;
        }

        void ChangeTransmission(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Transmission Change");
            //ToggleData td = m_Node.transmission;
            //td.isOn = evt.newValue;
            //m_Node.transmission = td;
        }

        void ChangeSubsurfaceScattering(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("SSS Change");
            //ToggleData td = m_Node.subsurfaceScattering;
            //td.isOn = evt.newValue;
            //m_Node.subsurfaceScattering = td;
        }

        void ChangeBlendMode(ChangeEvent<Enum> evt)
        {
            // Make sure the mapping is correct by handling each case.
            //AlphaMode alphaMode = GetAlphaMode((DecalMasterNode.AlphaModeDecal)evt.newValue);

            //if (Equals(m_Node.alphaMode, alphaMode))
            //    return;

            //m_Node.owner.owner.RegisterCompleteObjectUndo("Alpha Mode Change");
            //m_Node.alphaMode = alphaMode;
        }

        void ChangeBlendPreserveSpecular(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Blend Preserve Specular Change");
            //ToggleData td = m_Node.blendPreserveSpecular;
            //td.isOn = evt.newValue;
            //m_Node.blendPreserveSpecular = td;
        }

        void ChangeTransparencyFog(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Transparency Fog Change");
            //ToggleData td = m_Node.transparencyFog;
            //td.isOn = evt.newValue;
            //m_Node.transparencyFog = td;
        }

        void ChangeBackThenFrontRendering(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Back Then Front Rendering Change");
            //ToggleData td = m_Node.backThenFrontRendering;
            //td.isOn = evt.newValue;
            //m_Node.backThenFrontRendering = td;
        }

        void ChangeDrawOrder(ChangeEvent<int> evt)
        {
            m_Node.drawOrder = evt.newValue;
            m_DrawOrderField.value = m_Node.drawOrder;
            if (Equals(m_Node.drawOrder, evt.newValue))
                return;
            m_Node.owner.owner.RegisterCompleteObjectUndo("Draw Order Change");
        }

        void ChangeAlphaTest(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Alpha Test Change");
            //ToggleData td = m_Node.alphaTest;
            //td.isOn = evt.newValue;
            //m_Node.alphaTest = td;
        }

        void ChangeAlphaTestPrepass(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Alpha Test Depth Prepass Change");
            //ToggleData td = m_Node.alphaTestDepthPrepass;
            //td.isOn = evt.newValue;
            //m_Node.alphaTestDepthPrepass = td;
        }

        void ChangeAlphaTestPostpass(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Alpha Test Depth Postpass Change");
            //ToggleData td = m_Node.alphaTestDepthPostpass;
            //td.isOn = evt.newValue;
            //m_Node.alphaTestDepthPostpass = td;
        }

        void ChangeDecal(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Decal Change");
            //ToggleData td = m_Node.receiveDecals;
            //td.isOn = evt.newValue;
            //m_Node.receiveDecals = td;
        }

        void ChangeSSR(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("SSR Change");
            //ToggleData td = m_Node.receiveSSR;
            //td.isOn = evt.newValue;
            //m_Node.receiveSSR = td;
        }

        void ChangeEnergyConservingSpecular(ChangeEvent<bool> evt)
        {
            //m_Node.owner.owner.RegisterCompleteObjectUndo("Energy Conserving Specular Change");
            //ToggleData td = m_Node.energyConservingSpecular;
            //td.isOn = evt.newValue;
            //m_Node.energyConservingSpecular = td;
        }

        void ChangeSpecularOcclusionMode(ChangeEvent<Enum> evt)
        {
            //if (Equals(m_Node.specularOcclusionMode, evt.newValue))
            //    return;

            //m_Node.owner.owner.RegisterCompleteObjectUndo("Specular Occlusion Mode Change");
            //m_Node.specularOcclusionMode = (SpecularOcclusionMode)evt.newValue;
        }

        //public AlphaMode GetAlphaMode(DecalMasterNode.AlphaModeDecal alphaModeLit)
        //{
        //    switch (alphaModeLit)
        //    {
        //        case DecalMasterNode.AlphaModeDecal.Alpha:
        //            return AlphaMode.Alpha;
        //        case DecalMasterNode.AlphaModeDecal.PremultipliedAlpha:
        //            return AlphaMode.Premultiply;
        //        case DecalMasterNode.AlphaModeDecal.Additive:
        //            return AlphaMode.Additive;
        //        default:
        //            {
        //                Debug.LogWarning("Not supported: " + alphaModeLit);
        //                return AlphaMode.Alpha;
        //            }

        //    }
        //}

        //public DecalMasterNode.AlphaModeDecal GetAlphaModeLit(AlphaMode alphaMode)
        //{
        //    switch (alphaMode)
        //    {
        //        case AlphaMode.Alpha:
        //            return DecalMasterNode.AlphaModeDecal.Alpha;
        //        case AlphaMode.Premultiply:
        //            return DecalMasterNode.AlphaModeDecal.PremultipliedAlpha;
        //        case AlphaMode.Additive:
        //            return DecalMasterNode.AlphaModeDecal.Additive;
        //        default:
        //            {
        //                Debug.LogWarning("Not supported: " + alphaMode);
        //                return DecalMasterNode.AlphaModeDecal.Alpha;
        //            }
        //    }
        //}
    }
}
