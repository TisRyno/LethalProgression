using BepInEx;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using LethalProgression.Config;
using LethalProgression.GUI.XPBar;
using LethalProgression.GUI.Skills;
using LethalProgression.GUI.HandSlot;
using LethalProgression.LessShitConfig;
using System;

namespace LethalProgression;

[BepInDependency("LethalNetworkAPI")]
[BepInPlugin(modGUID, modName, modVersion)]
internal class LethalPlugin : BaseUnityPlugin
{
    private const string modGUID = "TisRyno.LethalProgression";
    private const string modName = "Lethal Progression";
    private const string modVersion = "2.0.0";
    public static AssetBundle skillBundle;

    internal static ManualLogSource Log;
    internal static bool ReservedSlots;
    public static LethalPlugin Instance { get; private set; }

    public static XPBarGUI XPBarGUI { get; private set; }

    public static SkillsGUI SkillsGUI { get; private set; }

    public static SlotTemplate SlotTemplate { get; private set; }

    private void Awake()
    {
        Instance = this;

        XPBarGUI = new();
        SkillsGUI = new();
        SlotTemplate = new();

        var harmony = new Harmony(modGUID);
        harmony.PatchAll();

        skillBundle = AssetBundle.LoadFromMemory(Properties.Resources.skillmenu);

        Log = Logger;

        Log.LogInfo("Lethal Progression initialised.");

        LessShitConfigSystem.RegisterSection<IGeneralConfig>();
        LessShitConfigSystem.RegisterSection<IBatteryLifeConfig>();
        LessShitConfigSystem.RegisterSection<IHandSlotsConfig>();
        LessShitConfigSystem.RegisterSection<IHealthRegenConfig>();
        LessShitConfigSystem.RegisterSection<IMaxHealthConfig>();
        LessShitConfigSystem.RegisterSection<IJumpHeightConfig>();
        LessShitConfigSystem.RegisterSection<ILootValueConfig>();
        LessShitConfigSystem.RegisterSection<ISprintSpeedConfig>();
        LessShitConfigSystem.RegisterSection<IStaminaConfig>();
        LessShitConfigSystem.RegisterSection<IStrengthConfig>();
        
        LessShitConfigSystem.Bake(Config);

        LessShitConfigSystem.SerializeLocalConfigs();

        Log.LogInfo("Lethal Progression Config loaded.");

        foreach (var plugin in Chainloader.PluginInfos)
        {
            var id = plugin.Value.Metadata.GUID;
            if (id.Contains("ReservedItem"))
            {
                ReservedSlots = true;
                break; // Break out of the loop. We don't need to check the rest of the plugins.
            }

            if (id.Contains("mikestweaks")) 
            {
                var mikesFound = plugin.Value.Instance.Config.GetConfigEntries()
                    .Where(entry => entry.Definition.Key == "ExtraItemSlots")
                    .Where(entry => int.TryParse(entry.GetSerializedValue(), out int i) && i > 0)
                    .Any();

                if (mikesFound)
                {
                    ReservedSlots = true;
                    break; // Break out of the loop. We don't need to check the rest of the plugins.
                }
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
