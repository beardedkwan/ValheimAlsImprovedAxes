using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AlsImprovedAxes
{
    public class PluginInfo
    {
        public const string Name = "Al's Improved Axes";
        public const string Guid = "beardedkwan.AlsImprovedAxes";
        public const string Version = "1.0.1";
    }
    
    public class AlsImprovedAxesConfig
    {
        public static ConfigEntry<float> OneHSlashMultiplier { get; set; }
        public static ConfigEntry<float> TwoHSlashMultiplier { get; set; }

        public static ConfigEntry<float> CrystalBattleAxeSpirit { get; set; }
        public static ConfigEntry<float> JotunBanePoison { get; set; }
    }

    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInProcess("valheim.exe")]
    public class AlsImprovedAxes : BaseUnityPlugin
    {
        void Awake()
        {
            // Initialize config
            AlsImprovedAxesConfig.OneHSlashMultiplier = Config.Bind("General", "OneHSlashMultiplier", 1.0f, "Slash damage multiplier for one-handed axes.");
            AlsImprovedAxesConfig.TwoHSlashMultiplier = Config.Bind("General", "TwoHSlashMultiplier", 1.0f, "Slash damage multiplier for two-handed axes.");

            AlsImprovedAxesConfig.CrystalBattleAxeSpirit = Config.Bind("General", "CrystalBattleAxeSpirit", 30f, "Override spirit damage for crystal battleaxe.");
            AlsImprovedAxesConfig.JotunBanePoison = Config.Bind("General", "JotunBanePoison", 40f, "Override poison damage for jotun bane axes.");

            Harmony harmony = new Harmony(PluginInfo.Guid);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        static class ObjectDBCopyOtherDBPatch
        {
            static void Postfix()
            {
                // Access the ObjectDB instance
                ObjectDB objectDB = ObjectDB.instance;

                // Loop through all items in the ObjectDB
                foreach (GameObject itemPrefab in objectDB.m_items)
                {
                    if (itemPrefab == null)
                        continue;

                    ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();

                    if (itemDrop == null)
                        continue;

                    // Check if the item is a one-handed axe or battleaxe
                    if (itemDrop.m_itemData.m_shared.m_name.StartsWith("$item_axe") || itemDrop.m_itemData.m_shared.m_name.StartsWith("$item_battleaxe"))
                    {
                        Debug.Log($"Modifying weapon data for {itemDrop.m_itemData.m_shared.m_name}");

                        bool isOneHanded = itemDrop.m_itemData.m_shared.m_name.StartsWith("$item_axe");

                        // Modify the weapon's damage values here
                        if (isOneHanded)
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_slash *= AlsImprovedAxesConfig.OneHSlashMultiplier.Value;
                        }
                        else
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_slash *= AlsImprovedAxesConfig.TwoHSlashMultiplier.Value;
                        }

                        if (itemDrop.m_itemData.m_shared.m_name.Contains("crystal"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_spirit = AlsImprovedAxesConfig.CrystalBattleAxeSpirit.Value;
                        }
                        else if (itemDrop.m_itemData.m_shared.m_name.Contains("jotunbane"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_poison = AlsImprovedAxesConfig.JotunBanePoison.Value;
                        }
                    }
                }
            }
        }
    }
}
