using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering.HDPipeline.Drawing;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Experimental.Rendering.HDPipeline;


namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [Serializable]
    [Title("Master", "Decal")]
    class DecalMasterNode : MasterNode<IDecalSubShader>, IMayRequirePosition, IMayRequireNormal, IMayRequireTangent
    {
        public const string PositionSlotName = "Position";
        public const int PositionSlotId = 0;

        public const string AlbedoSlotName = "Albedo";
        public const string AlbedoDisplaySlotName = "BaseColor";
        public const int AlbedoSlotId = 1;

        public const string NormalSlotName = "Normal";
        public const int NormalSlotId = 2;

        public const string MetallicSlotName = "Metallic";
        public const int MetallicSlotId = 3;

        public const string AmbientOcclusionSlotName = "Occlusion";
        public const string AmbientOcclusionDisplaySlotName = "AmbientOcclusion";
        public const int AmbientOcclusionSlotId = 4;

        public const string SmoothnessSlotName = "Smoothness";
        public const int SmoothnessSlotId = 5;

        public const string OpacitySlotName = "Opacity";
        public const int AlphaSlotId = 6;

        public const string BaseColorOpacitySlotName = "AlphaAlbedo";
        public const string BaseColorOpacityDisplaySlotName = "BaseColor Opacity";
        public const int BaseColorOpacitySlotId = 7;

        public const string NormaOpacitySlotName = "AlphaNormal";
        public const string NormaOpacityDisplaySlotName = "Normal Opacity";
        public const int NormaOpacitySlotId = 8;

        public const string MetallicOpacitySlotName = "AlphaMettalic";
        public const string MetallicOpacityDisplaySlotName = "Metallic Opacity";
        public const int MetallicOpacitySlotId = 9;

        public const string AmbientOcclusionOpacitySlotName = "AlphaAmbientOcclusion";
        public const string AmbientOcclusionOpacityDisplaySlotName = "Ambient Occlusion Opacity";
        public const int AmbientOcclusionOpacitySlotId = 10;

        public const string SmoothnessOpacitySlotName = "AlphaSmoothness";
        public const string SmoothnessOpacityDisplaySlotName = "Smoothness Opacity";
        public const int SmoothnessOpacitySlotId = 11;


        // Just for convenience of doing simple masks. We could run out of bits of course.
        [Flags]
        enum SlotMask
        {
            None = 0,
            Position = 1 << PositionSlotId,
            Albedo = 1 << AlbedoSlotId,
            Normal = 1 << NormalSlotId,
            Metallic = 1 << MetallicSlotId,
            Smoothness = 1 << SmoothnessSlotId,
            Occlusion = 1 << AmbientOcclusionSlotId,
            Alpha = 1 << AlphaSlotId,
            AlphaAlbedo = 1 << BaseColorOpacitySlotId,
            AlphaNormal = 1 << NormaOpacitySlotId,
            AlphaMetallic = 1 << MetallicOpacitySlotId,
            AlphaOcclusion = 1 << AmbientOcclusionOpacitySlotId,
            AlphaSmoothness = 1 << SmoothnessOpacitySlotId,
        }

        const SlotMask regularDecalParameter = SlotMask.Position | SlotMask.Albedo | SlotMask.AlphaAlbedo | SlotMask.Normal | SlotMask.AlphaNormal | SlotMask.Smoothness | SlotMask.AlphaSmoothness | SlotMask.Alpha;
        const SlotMask detailedDecalParameter = regularDecalParameter | SlotMask.Metallic | SlotMask.AlphaMetallic | SlotMask.Occlusion | SlotMask.AlphaOcclusion;

        public enum DecalType
        {
            Regular_3RT,
            Detailed_4RT
        }

        // This could also be a simple array. For now, catch any mismatched data.
        SlotMask GetActiveSlotMask()
        {
            switch (m_DecalType)
            {
                case DecalType.Regular_3RT:
                    return regularDecalParameter;

                case DecalType.Detailed_4RT:
                    return detailedDecalParameter;

                default:
                    return regularDecalParameter;
            }
        }

        bool MaterialTypeUsesSlotMask(SlotMask mask)
        {
            SlotMask activeMask = GetActiveSlotMask();
            return (activeMask & mask) != 0;
        }

        [SerializeField]
        DecalType m_DecalType;

        public DecalType decalType
        {
            get { return m_DecalType; }
            set
            {
                if (m_DecalType == value)
                    return;

                m_DecalType = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        public DecalMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Decal-Master-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            name = "Decal Master";

            List<int> validSlots = new List<int>();

            // Position
            if (MaterialTypeUsesSlotMask(SlotMask.Position))
            {
                AddSlot(new PositionMaterialSlot(PositionSlotId, PositionSlotName, PositionSlotName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
                validSlots.Add(PositionSlotId);
            }

            // Albedo
            if (MaterialTypeUsesSlotMask(SlotMask.Albedo))
            {
                AddSlot(new ColorRGBMaterialSlot(AlbedoSlotId, AlbedoDisplaySlotName, AlbedoSlotName, SlotType.Input, Color.white, ColorMode.Default, ShaderStageCapability.Fragment));
                validSlots.Add(AlbedoSlotId);
            }

            // AlphaAlbedo
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaAlbedo))
            {
                AddSlot(new Vector1MaterialSlot(BaseColorOpacitySlotId, BaseColorOpacitySlotName, BaseColorOpacityDisplaySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(BaseColorOpacitySlotId);
            }

            // Normal
            if (MaterialTypeUsesSlotMask(SlotMask.Normal))
            {
                AddSlot(new NormalMaterialSlot(NormalSlotId, NormalSlotName, NormalSlotName, CoordinateSpace.Tangent, ShaderStageCapability.Fragment));
                validSlots.Add(NormalSlotId);
            }

            // AlphaNormal
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaNormal))
            {
                AddSlot(new Vector1MaterialSlot(NormaOpacitySlotId, NormaOpacitySlotName, NormaOpacityDisplaySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(NormaOpacitySlotId);
            }

            // Metal
            if (MaterialTypeUsesSlotMask(SlotMask.Metallic))
            {
                AddSlot(new ColorRGBMaterialSlot(MetallicSlotId, MetallicSlotName, MetallicSlotName, SlotType.Input, Color.white, ColorMode.Default, ShaderStageCapability.Fragment));
                validSlots.Add(MetallicSlotId);
            }

            // AlphaMetal
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaMetallic))
            {
                AddSlot(new Vector1MaterialSlot(MetallicOpacitySlotId, MetallicOpacitySlotName, MetallicOpacityDisplaySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(MetallicOpacitySlotId);
            }

            // Ambient Occlusion
            if (MaterialTypeUsesSlotMask(SlotMask.Occlusion))
            {
                AddSlot(new Vector1MaterialSlot(AmbientOcclusionSlotId, AmbientOcclusionDisplaySlotName, AmbientOcclusionSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AmbientOcclusionSlotId);
            }

            // AlphaOcclusion
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaOcclusion))
            {
                AddSlot(new Vector1MaterialSlot(AmbientOcclusionOpacitySlotId, AmbientOcclusionOpacitySlotName, AmbientOcclusionOpacityDisplaySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AmbientOcclusionOpacitySlotId);
            }

            // Smoothness
            if (MaterialTypeUsesSlotMask(SlotMask.Smoothness))
            {
                AddSlot(new Vector1MaterialSlot(SmoothnessSlotId, SmoothnessSlotName, SmoothnessSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(SmoothnessSlotId);
            }


            // AlphaSmoothness
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaSmoothness))
            {
                AddSlot(new Vector1MaterialSlot(SmoothnessOpacitySlotId, SmoothnessOpacitySlotName, SmoothnessOpacityDisplaySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(SmoothnessOpacitySlotId);
            }

            // Alpha
            if (MaterialTypeUsesSlotMask(SlotMask.Alpha))
            {
                AddSlot(new Vector1MaterialSlot(AlphaSlotId, OpacitySlotName, OpacitySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AlphaSlotId);
            }

            RemoveSlotsNameNotMatching(validSlots, true);
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new DecalSettingsView(this);
        }

        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }

        public NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireTangent>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresTangent(stageCapability));
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            // Trunk currently relies on checking material property "_EmissionColor" to allow emissive GI. If it doesn't find that property, or it is black, GI is forced off.
            // ShaderGraph doesn't use this property, so currently it inserts a dummy color (white). This dummy color may be removed entirely once the following PR has been merged in trunk: Pull request #74105
            // The user will then need to explicitly disable emissive GI if it is not needed.
            // To be able to automatically disable emission based on the ShaderGraph config when emission is black,
            // we will need a more general way to communicate this to the engine (not directly tied to a material property).
            //collector.AddShaderProperty(new ColorShaderProperty()
            //{
//                overrideReferenceName = "_EmissionColor",
  //              hidden = true,
    //            value = new Color(1.0f, 1.0f, 1.0f, 1.0f)
      //      });

            base.CollectShaderProperties(collector, generationMode);
        }
    }
}
