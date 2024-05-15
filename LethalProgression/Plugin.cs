using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using LethalProgression.Config;
using LethalProgression.GUI.XPBar;
using LethalProgression.GUI.Skills;
using LethalProgression.GUI.HandSlot;

namespace LethalProgression;

[BepInDependency("LethalNetworkAPI")]
[BepInPlugin(modGUID, modName, modVersion)]
internal class LethalPlugin : BaseUnityPlugin
{
    private const string modGUID = "TisRyno.LethalProgression";
    private const string modName = "Lethal Progression";
    private const string modVersion = "1.7.4";
    public static AssetBundle skillBundle;

    internal static ManualLogSource Log;
    internal static bool ReservedSlots;
    public static LethalPlugin Instance { get; private set; }

    public static ModConfig ModConfig { get; private set; }

    public static XPBarGUI XPBarGUI { get; private set; }

    public static SkillsGUI SkillsGUI { get; private set; }

    public static SlotTemplate SlotTemplate { get; private set; }

    private void Awake()
    {
        Instance = this;

        XPBarGUI = new();
        SkillsGUI = new();
        SlotTemplate = new();

        ModConfig = new(Config);

        var harmony = new Harmony(modGUID);
        harmony.PatchAll();

        skillBundle = AssetBundle.LoadFromMemory(Properties.Resources.skillmenu);

        Log = Logger;

        Log.LogInfo("Lethal Progression loaded.");

        foreach (var plugin in Chainloader.PluginInfos)
        {
            if (plugin.Value.Metadata.GUID.IndexOf("ReservedItem") >= 0)
                ReservedSlots = true;

            if (plugin.Value.Metadata.GUID.IndexOf("mikestweaks") >= 0)
            {
                // Get "ExtraItemSlots" config entry from Mike's Tweaks
                ConfigEntryBase[] mikesEntries = plugin.Value.Instance.Config.GetConfigEntries();

                foreach (var entry in mikesEntries)
                    if (entry.Definition.Key == "ExtraItemSlots")
                        if (int.Parse(entry.GetSerializedValue()) > 0)
                            ReservedSlots = true;
            }
        }
    }

    public IDictionary<string, object> GetAllConfigEntries()
    {
        return Config.ToDictionary(
            entry => entry.Value.Definition.Key,
            entry => entry.Value.BoxedValue
        );
    }
}
