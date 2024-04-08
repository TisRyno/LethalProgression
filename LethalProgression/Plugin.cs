using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using LethalProgression.Config;

namespace LethalProgression
{
    [BepInDependency("LethalNetworkAPI")]
    [BepInPlugin("Stoneman.LethalProgression", "Lethal Progression", "1.7.0")]
    internal class LethalPlugin : BaseUnityPlugin
    {
        private const string modGUID = "Stoneman.LethalProgression";
        private const string modName = "Lethal Progression";
        private const string modVersion = "1.7.0";
        private const string modAuthor = "Stoneman";
        public static AssetBundle skillBundle;

        internal static ManualLogSource Log;
        internal static bool ReservedSlots;
        internal static bool MikesTweaks;
        public static LethalPlugin Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            var harmony = new Harmony(modGUID);
            harmony.PatchAll();

            skillBundle = AssetBundle.LoadFromMemory(Properties.Resources.skillmenu);

            Log = Logger;

            Log.LogInfo("Lethal Progression loaded.");

            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID.IndexOf("ReservedItem") >= 0)
                {
                    ReservedSlots = true;
                }

                if (plugin.Value.Metadata.GUID.IndexOf("mikestweaks") >= 0)
                {
                    // Get "ExtraItemSlots" config entry from Mike's Tweaks
                    ConfigEntryBase[] mikesEntries = plugin.Value.Instance.Config.GetConfigEntries();

                    MikesTweaks = true;

                    foreach (var entry in mikesEntries)
                    {
                        if (entry.Definition.Key == "ExtraItemSlots")
                        {
                            if (int.Parse(entry.GetSerializedValue()) > 0)
                            {
                                ReservedSlots = true;
                            }
                        }
                    }
                }
            }

            SkillConfig.InitConfig();
        }

        public void BindConfig<T>(string section, string key, T defaultValue, string description = "")
        {
            Config.Bind(section,
                key,
                defaultValue,
                description
            );
        }

        public IDictionary<string, string> GetAllConfigEntries()
        {
            IDictionary<string, string> localConfig = Config.GetConfigEntries().ToDictionary(
                entry => entry.Definition.Key,
                entry => entry.GetSerializedValue()
            );

            return localConfig;
        }
    }
}
