using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace PresetParser.Models
{
    internal static class Anno117
    {
        internal class Model
        {
            private readonly XmlDocument assets;
            private readonly XmlDocument templates;

            private readonly XmlDocument assetPools;
            private readonly XmlDocument unlockAssets;

            private readonly List<XmlNode> buildings;
            private readonly Dictionary<string, string> populationLevelByBuilding = new Dictionary<string, string>();
            private readonly Dictionary<string, string[]> populationLevelsByRegion = new Dictionary<string, string[]>();

            public Model(XmlDocument assets, XmlDocument templates)
            {
                this.assets = assets;
                this.templates = templates;
                this.assetPools = assets.SelectDocument(".//AssetPool/ancestor::Asset", "AssetList");
                this.unlockAssets = assets.SelectDocument(".//UnlockAssets/ancestor::Asset", "AssetList");
                this.buildings = assets.SelectNodes("//Values[Building]/parent::Asset")
                    .Cast<XmlNode>()
                    .ToList();

                foreach (XmlNode populationGroup in assets.SelectNodes("//PopulationGroup7"))
                {
                    string region = populationGroup["Regional"].InnerText;
                    string[] levels = populationGroup["PopulationLevels"].SelectNodes(".//Level").Cast<XmlNode>()
                        .Select(level => level.InnerText)
                        .ToArray();
                    XmlNode buildingMenu = assets.SelectSingleNode($".//LinearBuildingsMenu/{region}");
                    string infrastructureCategoryGuid = buildingMenu["InfrastructureCategory"].InnerText;
                    string materialCategoryGuid = buildingMenu["MaterialCategory"].InnerText;
                    populationLevelsByRegion[region] = levels;

                    foreach (string buildingGuid in ResolveBuildings(region, [materialCategoryGuid]))
                    {
                        if (!populationLevelByBuilding.ContainsKey(buildingGuid))
                        {
                            string populationGuid = ResolvePopulationLevelFromUnlock(buildingGuid, region);
                            if (populationGuid != null) populationLevelByBuilding.Add(buildingGuid, populationGuid);
                        }
                    }

                    foreach (string buildingGuid in ResolveBuildings(region, [infrastructureCategoryGuid]))
                    {
                        if (!populationLevelByBuilding.ContainsKey(buildingGuid))
                        {
                            string populationGuid = ResolvePopulationLevelFromUnlock(buildingGuid, region);
                            if (populationGuid != null) populationLevelByBuilding.Add(buildingGuid, populationGuid);
                        }
                    }

                    List<string> tierCategories = buildingMenu.SelectNodes($".//TierCategory").Cast<XmlNode>()
                        .Select(category => category.InnerText)
                        .ToList();

                    if (tierCategories.Count != levels.Length)
                    {
                        throw new InvalidOperationException($"Population level count does not match construction category count for region '{region}'");
                    }

                    for (int i = 0; i < levels.Length; i++)
                    {
                        string populationGuid = levels[i];
                        string categoryGuid = tierCategories[i];

                        foreach (string buildingGuid in ResolveBuildings(region, [categoryGuid]))
                        {
                            if (!populationLevelByBuilding.ContainsKey(buildingGuid))
                            {
                                populationLevelByBuilding[buildingGuid] = populationGuid;
                            }
                        }
                    }
                }
            }

            public int BuildingCount
            {
                get { return buildings.Count; }
            }

            public Asset GetBuilding(int index)
            {
                XmlNode asset = buildings[index];
                return new Asset(asset, assets, templates);
            }

            public Asset FindAsset(string xpath)
            {
                XmlNode asset = assets.SelectSingleNode(xpath);
                return new Asset(asset, assets, templates);
            }

            public Asset FindAssetByGuid(string guid)
            {
                return FindAsset($".//Asset[Values/Standard/GUID[text()='{guid}']]");
            }

            public string GetGroupByRegion(string region)
            {
                if (region == "Roman") return "(1) Roman";
                else if (region == "Celtic") return "(2) Celtic";
                else throw new NotImplementedException($"Region '{region}' not implemented");
            }

            public string GetFactionByPopulationLevel(string populationLevel)
            {
                switch (populationLevel)
                {
                    default: return null;
                }
            }

            public string GetPopulationLevelByIndex(string region, int index)
            {
                return populationLevelsByRegion[region][index];
            }

            public string ResolveFaction(string guid, string region)
            {
                if (guid != null)
                {
                    if (!populationLevelByBuilding.TryGetValue(guid, out string populationLevel))
                    {
                        populationLevel = ResolvePopulationLevelFromUnlock(guid, region);
                        if (populationLevel != null) populationLevelByBuilding.Add(guid, populationLevel);
                    }

                    if (populationLevel != null)
                    {
                        return GetFactionByPopulationLevel(populationLevel);
                    }
                }

                return null;
            }

            private string ResolvePopulationLevelFromUnlock(string guid, string region)
            {
                if (guid == null) return null;
                XmlNode asset = ResolveUnlockAsset(guid);
                if (asset != null) return ResolvePopulationLevelFromUnlockAsset(guid, asset, region);
                return null;
            }

            private string ResolvePopulationLevelFromUnlockAsset(string guid, XmlNode asset, string region)
            {
                if (asset == null) return null;
                string templateName = asset["Template"]?.InnerText;
                XmlNode values = asset["Values"];

                if (values?["Trigger"] != null)
                {
                    XmlNode trigger = values["Trigger"];
                    XmlNode condition = trigger["TriggerCondition"];
                    return ResolvePopulationLevelFromCondition(condition, region);
                }
                else if (values?["Sequence"] != null)
                {
                    string sequenceGuid = values["Standard"]["GUID"].InnerText;
                    foreach (XmlNode condition in assets.SelectNodes($"//Asset/Values[.//Component[text()='{sequenceGuid}']]/PreConditionList/Condition"))
                    {
                        string result = ResolvePopulationLevelFromCondition(condition, region);
                        if (result != null) return result;
                    }

                    return null;
                }
                else
                {
                    Debug.WriteLine($"{nameof(ResolvePopulationLevelFromUnlockAsset)}: Unlock '{templateName}' not implemented");
                    return null;
                }
            }

            private string ResolvePopulationLevelFromCondition(XmlNode condition, string region)
            {
                string templateName = condition["Template"]?.InnerText;
                XmlNode values = condition["Values"];

                if (templateName == "ConditionAlwaysTrue" || values?["ConditionAlwaysTrue"] != null)
                {
                    return GetPopulationLevelByIndex(region, 0);
                }
                else if (templateName == "ConditionPlayerCounter")
                {
                    XmlNode playerCounter = values["ConditionPlayerCounter"];
                    return ResolvePopulationLevelFromPlayerCounter(playerCounter["Context"].InnerText);
                }
                else
                {
                    Debug.WriteLine($"{nameof(ResolvePopulationLevelFromCondition)}: Condition '{templateName}' not implemented");
                    return null;
                }
            }

            private string ResolvePopulationLevelFromPlayerCounter(string context)
            {
                Asset asset = FindAssetByGuid(context);
                string templateName = asset.TemplateName;

                if (templateName == "PopulationLevel")
                {
                    return context;
                }
                else if (templateName == "ResidenceBuilding")
                {
                    return asset.GetValue("Residence7/PopulationLevel");
                }
                else
                {
                    Debug.WriteLine($"{nameof(ResolvePopulationLevelFromPlayerCounter)}: Context '{templateName}' not implemented");
                    return null;
                }
            }

            private List<string> ResolveBuildings(string region, List<string> guids)
            {
                List<string> result = new List<string>();

                foreach (string guid in guids)
                {
                    Asset asset = FindAssetByGuid(guid);
                    string templateName = asset.TemplateName;

                    if (asset.SelectNode("Monument") != null)
                    {
                        string upgradeTarget = asset.GetValue("Monument/UpgradeTarget");
                        result.AddRange(ResolveBuildings(region, [upgradeTarget]));
                    }
                    else if (asset.SelectNode("Building") != null)
                    {
                        string associatedRegion = asset.GetValue("Building/AssociatedRegions");
                        if (associatedRegion == region) result.Add(guid);
                    }
                    else if (templateName == "ConstructionCategory" || templateName == "ProductionChain")
                    {
                        result.AddRange(ResolveBuildings(region, asset.SelectNodes($"./{templateName}//Building")
                            .Select(node => node.InnerText)
                            .ToList()));
                    }
                    else
                    {
                        Debug.WriteLine($"{nameof(ResolveBuildings)}: Template '{templateName}' not implemented");
                    }
                }

                return result;
            }

            private XmlNode ResolveUnlockAsset(string guid)
            {
                if (guid == null) return null;
                XmlNode asset = unlockAssets.SelectSingleNode($".//UnlockAssets[.//Asset[text()='{guid}']]/ancestor::Asset");

                if (asset != null)
                {
                    return asset;
                }

                // given asset has no unlock trigger by itself, try triggers for asset pools containing our asset
                foreach (XmlNode assetPool in assetPools.SelectNodes($".//AssetPool[.//Asset[text()='{guid}']]/ancestor::Asset"))
                {
                    asset = ResolveUnlockAsset(assetPool["Values"]?["Standard"]?["GUID"]?.InnerText);
                    if (asset != null) return asset;
                }

                return null;
            }
        }

        private static XmlDocument SelectDocument(this XmlNode parent, string xpath, string name)
        {
            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement(name);
            document.AppendChild(root);

            foreach (XmlNode node in parent.SelectNodes(xpath))
            {
                root.AppendChild(document.ImportNode(node, true));
            }

            return document;
        }
    }
}
