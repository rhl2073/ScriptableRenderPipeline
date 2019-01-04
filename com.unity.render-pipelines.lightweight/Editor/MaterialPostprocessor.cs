using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

namespace UnityEditor.Rendering.LWRP
{
    class MaterialModificationProcessor : AssetModificationProcessor
    {
        static void OnWillCreateAsset(string asset)
        {
            if (!asset.ToLowerInvariant().EndsWith(".mat"))
            {
                return;
            }
            MaterialPostprocessor.s_CreatedAssets.Add(asset);
        }
    }

    class MaterialPostprocessor : AssetPostprocessor
    {
        static List<string> s_LWRPShaders = new List<string>
        {
            // TODO: Populate this
            // Lit
            "933532a4fcc9baf4fa0491de14d08ed7",
            // SimpleLit
        };

        public static List<string> s_CreatedAssets = new List<string>();

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var upgradeLog = "LWRP Material log:";
            var upgradeCount = 0;
            
            foreach (var asset in importedAssets)
            {
                
                if (!asset.ToLowerInvariant().EndsWith(".mat"))
                {
                    continue;
                }

                var material = (Material)AssetDatabase.LoadAssetAtPath(asset, typeof(Material));
                var shaderGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material.shader));
                if (!s_LWRPShaders.Contains(shaderGuid))
                {
                    continue;
                }

                ShaderPathID id = ShaderUtils.GetEnumFromPath(material.shader.name);
                var wasUpgraded = false;
                var assetVersion = (AssetVersion)AssetDatabase.LoadAssetAtPath(asset, typeof(AssetVersion));
                var debug = "\n" + material.name;

                if (!assetVersion)
                {
                    wasUpgraded = true;
                    assetVersion = ScriptableObject.CreateInstance<AssetVersion>();
                    if (s_CreatedAssets.Contains(asset))
                    {
                        assetVersion.version = k_Upgraders.Length;
                        s_CreatedAssets.Remove(asset);
                        InitializeLatest(material, id);
                    }
                    else
                    {
                        assetVersion.version = 0;
                    }

                    assetVersion.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
                    AssetDatabase.AddObjectToAsset(assetVersion, asset);
                    debug += " initialized.";
                }

                while (assetVersion.version < k_Upgraders.Length)
                {
                    k_Upgraders[assetVersion.version](material, id);
                    debug += $" upgrading:v{assetVersion.version - 1} to v{assetVersion.version}";
                    assetVersion.version++;
                    wasUpgraded = true;
                }

                if (wasUpgraded)
                {
                    upgradeLog += debug;
                    upgradeCount++;
                    EditorUtility.SetDirty(assetVersion);
                }
            }
            if(upgradeCount > 0)
                Debug.Log(upgradeLog);
        }

        static readonly Action<Material, ShaderPathID>[] k_Upgraders = { };

        static void InitializeLatest(Material material, ShaderPathID id)
        {
            
        }

        static void UpgradeV1(Material material, ShaderPathID shaderID)
        {
            var shaderPath = ShaderUtils.GetShaderPath(shaderID);
            var upgradeFlag = MaterialUpgrader.UpgradeFlags.LogMessageWhenNoUpgraderFound;
            
            switch (shaderID)
            {
                case ShaderPathID.Unlit:
                    MaterialUpgrader.Upgrade(material, new LitUpdaterV1(shaderPath), upgradeFlag);
                    break;
                case ShaderPathID.SimpleLit:
                    MaterialUpgrader.Upgrade(material, new LitUpdaterV1(shaderPath), upgradeFlag);
                    break;
                case ShaderPathID.Lit:
                    MaterialUpgrader.Upgrade(material, new LitUpdaterV1(shaderPath), upgradeFlag);
                    break;
            }
        }
    }
    
    // Upgaders v1
    #region UpgradersV1

    internal class LitUpdaterV1 : MaterialUpgrader
    {
        public static void UpdateStandardMaterialKeywords(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");
            
            if(material.GetTexture("_MetallicGlossMap"))
                material.SetFloat("_Smoothness", material.GetFloat("_GlossMapScale"));
            else
                material.SetFloat("_Smoothness", material.GetFloat("_Glossiness"));
            
            material.SetColor("_BaseColor", material.GetColor("_Color"));
        }

        public static void UpdateStandardSpecularMaterialKeywords(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");
            
            if(material.GetTexture("_SpecGlossMap"))
                material.SetFloat("_Smoothness", material.GetFloat("_GlossMapScale"));
            else
                material.SetFloat("_Smoothness", material.GetFloat("_Glossiness"));
            
            material.SetColor("_BaseColor", material.GetColor("_Color"));
        }

        public LitUpdaterV1(string oldShaderName)
        {
            if (oldShaderName == null)
                throw new ArgumentNullException("oldShaderName");

            string standardShaderPath = ShaderUtils.GetShaderPath(ShaderPathID.Lit);

            if (oldShaderName.Contains("Specular"))
            {
                RenameShader(oldShaderName, standardShaderPath, UpdateStandardSpecularMaterialKeywords);
            }
            else
            {
                RenameShader(oldShaderName, standardShaderPath, UpdateStandardMaterialKeywords);
            }

            RenameTexture("_MainTex", "_BaseMap");
            RenameColor("_Color", "_BaseColor");
            RenameFloat("_GlossyReflections", "_EnvironmentReflections");
        }
    }

    internal class UnlitUpdaterV1 : MaterialUpgrader
    {
        static Shader bakedLit = Shader.Find(ShaderUtils.GetShaderPath(ShaderPathID.BakedLit));
        
        public static void UpgradeToUnlit(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            if (material.GetFloat("_SampleGI") != 0)
            {
                material.shader = bakedLit;
                material.EnableKeyword("_NORMALMAP");
            }
        }

        public UnlitUpdaterV1(string oldShaderName)
        {
            if (oldShaderName == null)
                throw new ArgumentNullException("oldShaderName");

            RenameShader(oldShaderName, ShaderUtils.GetShaderPath(ShaderPathID.Unlit), UpgradeToUnlit);

            RenameTexture("_MainTex", "_BaseMap");
            RenameColor("_Color", "_BaseColor");
        }
    }

    internal class SimpleLitUpdaterV1 : MaterialUpgrader
    {
        public SimpleLitUpdaterV1(string oldShaderName)
        {
            if (oldShaderName == null)
                throw new ArgumentNullException("oldShaderName");
            
            RenameShader(oldShaderName, ShaderUtils.GetShaderPath(ShaderPathID.SimpleLit), UpgradeToSimpleLit);
            
            RenameTexture("_MainTex", "_BaseMap");
            RenameColor("_Color", "_BaseColor");
            RenameFloat("_SpecSource", "_SpecularHighlights");
        }
        
        public static void UpgradeToSimpleLit(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            material.SetFloat("_SmoothnessSource" ,1 - material.GetFloat("_GlossinessSource"));
            if (material.GetTexture("_SpecGlossMap") == null)
            {
                var col = material.GetColor("_SpecColor");
                
                col.a = material.GetFloat("_Shininess");
                material.SetColor("_SpecColor", col);
                if (material.GetFloat("_Surface") == 0)
                {
                    var colBase = material.GetColor("_Color");
                    colBase.a = col.a;
                    material.SetColor("_BaseColor", colBase);
                }
            }
        }
    }

    #endregion
    
}
