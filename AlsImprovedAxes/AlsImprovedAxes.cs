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
        public const string Guid = "deadcliffdivers.AlsImprovedAxes";
        public const string Version = "1.0.0";
    }
    
    public class AlsImprovedAxesConfig
    {
        public static ConfigEntry<float> SlashMultiplier { get; set; }
        public static ConfigEntry<float> FlintSpiritDamage { get; set; }
        public static ConfigEntry<float> BronzeFireDamage { get; set; }
        public static ConfigEntry<float> IronSpiritDamage { get; set; }
        public static ConfigEntry<float> BlackmetalFrostDamage { get; set; }
        public static ConfigEntry<float> JotunBaneSlash { get; set; }
        public static ConfigEntry<float> JotunBanePoison { get; set; }
    }

    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInProcess("valheim.exe")]
    public class AlsImprovedAxes : BaseUnityPlugin
    {
        void Awake()
        {
            // Initialize config
            AlsImprovedAxesConfig.SlashMultiplier = Config.Bind("General", "SlashMultiplier", 1.0f, "Slash damage multiplier for one-handed axes.");
            AlsImprovedAxesConfig.FlintSpiritDamage = Config.Bind("General", "FlintSpiritDamage", 0f, "Spirit damage for flint axes.");
            AlsImprovedAxesConfig.BronzeFireDamage = Config.Bind("General", "BronzeFireDamage", 0f, "Fire damage for bronze axes.");
            AlsImprovedAxesConfig.IronSpiritDamage = Config.Bind("General", "IronSpiritDamage", 0f, "Spirit damage for iron axes.");
            AlsImprovedAxesConfig.BlackmetalFrostDamage = Config.Bind("General", "BlackmetalFrostDamage", 0f, "Frost damage for blackmetal axes.");
            AlsImprovedAxesConfig.JotunBaneSlash = Config.Bind("General", "JotunBaneSlash", 0f, "Additional slash damage for jotun bane axes.");
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

                    // Check if the item is a one-handed axe
                    if (itemDrop.m_itemData.m_shared.m_name.StartsWith("$item_axe"))
                    {
                        Debug.Log($"Modifying weapon data for {itemDrop.m_itemData.m_shared.m_name}");

                        // Modify the weapon's damage values here
                        itemDrop.m_itemData.m_shared.m_damages.m_slash *= AlsImprovedAxesConfig.SlashMultiplier.Value;

                        if (itemDrop.m_itemData.m_shared.m_name.Contains("flint"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_spirit = AlsImprovedAxesConfig.FlintSpiritDamage.Value;
                        }
                        else if (itemDrop.m_itemData.m_shared.m_name.Contains("bronze"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_fire = AlsImprovedAxesConfig.BronzeFireDamage.Value;
                        }
                        else if (itemDrop.m_itemData.m_shared.m_name.Contains("iron"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_spirit = AlsImprovedAxesConfig.IronSpiritDamage.Value;
                        }
                        else if (itemDrop.m_itemData.m_shared.m_name.Contains("blackmetal"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_frost = AlsImprovedAxesConfig.BlackmetalFrostDamage.Value;
                        }
                        else if (itemDrop.m_itemData.m_shared.m_name.Contains("jotunbane"))
                        {
                            itemDrop.m_itemData.m_shared.m_damages.m_slash += AlsImprovedAxesConfig.JotunBaneSlash.Value;
                            itemDrop.m_itemData.m_shared.m_damages.m_poison = AlsImprovedAxesConfig.JotunBanePoison.Value;
                        }
                    }
                }
            }
        }
    }
}
