﻿using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using UnityEngine.SceneManagement;
using BepInEx.Bootstrap;
using LethalProgression.GUI;
using LethalProgression.Skills;
using LethalProgression.Patches;
using LethalProgression.Config;
using Unity.Netcode;
using UnityEngine;

namespace LethalProgression
{
    [BepInPlugin("Stoneman.LethalProgression", "Lethal Progression", "1.6.0")]
    internal class LethalPlugin : BaseUnityPlugin
    {
        private const string modGUID = "Stoneman.LethalProgression";
        private const string modName = "Lethal Progression";
        private const string modVersion = "1.6.0";
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
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            skillBundle = AssetBundle.LoadFromMemory(LethalProgression.Properties.Resources.skillmenu);

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

            // Network patcher!
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
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
