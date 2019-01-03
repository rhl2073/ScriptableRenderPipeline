using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

namespace UnityEditor.Rendering.LWRP
{
    public static class MaterialUpdater
    {
        [MenuItem("Edit/Render Pipeline/Update project wide Lightweight Render Pipeline Materials")]
        public static void UpdateProjectMaterials()
        {
            List<MaterialUpgrader> upgraders = new List<MaterialUpgrader>();
            GetUpgraders(ref upgraders);

            MaterialUpgrader.UpgradeProjectFolder(upgraders, "Update to LightweightRP Materials", MaterialUpgrader.UpgradeFlags.LogMessageWhenNoUpgraderFound);
        }

        [MenuItem("Edit/Render Pipeline/Update selected Lightweight Render Pipeline Materials")]
        public static void UpdateSelectedMaterials()
        {
            List<MaterialUpgrader> upgraders = new List<MaterialUpgrader>();
            GetUpgraders(ref upgraders);

            MaterialUpgrader.UpgradeSelection(upgraders, "Update LightweightRP Materials",
                MaterialUpgrader.UpgradeFlags.LogMessageWhenNoUpgraderFound);
        }

        private static void GetUpgraders(ref List<MaterialUpgrader> upgraders)
        {
            // Lit updater
            upgraders.Add(new LitUpdaterV1("Lightweight Render Pipeline/Lit"));
            // Simple Lit updater
            upgraders.Add(new SimpleLitUpdaterV1("Lightweight Render Pipeline/Simple Lit"));
            // Unlit updater
            upgraders.Add(new UnlitUpdaterV1("Lightweight Render Pipeline/Unlit"));
            // Particle updaters
            upgraders.Add(new ParticleUpgrader("Lightweight Render Pipeline/Particles/lit"));
            upgraders.Add(new ParticleUpgrader("Lightweight Render Pipeline/Particles/Unlit"));
        }
    }
}