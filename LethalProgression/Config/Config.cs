using System.Collections.Generic;
using BepInEx.Configuration;

namespace LethalProgression.Config;

internal class ModConfig
{
    public IDictionary<string, string> hostConfig = new Dictionary<string, string>();

    public ModConfig(ConfigFile config)
    {
        InitGeneralSettings(config);
        InitHealthSettings(config);
        InitStaminaSettings(config);
        InitBatterySettings(config);
        InitHandSlotsSettings(config);
        InitLootValueSettings(config);
        InitJumpHeightSettings(config);
        InitSprintSpeedSettings(config);
        InitStrengthSettings(config);
    }

    private void InitGeneralSettings(ConfigFile config)
    {
        config.Bind(
            "General",
            "Person Multiplier",
            35,
            "How much does XP cost to level up go up per person?"
        );
        config.Bind(
            "General",
            "Quota Multiplier",
            30,
            "How much more XP does it cost to level up go up per quota? (Percent)"
        );
        config.Bind(
            "General",
            "XP Minimum",
            40,
            "Minimum XP to level up."
        );
        config.Bind(
            "General",
            "XP Maximum",
            750,
            "Maximum XP to level up."
        );
        config.Bind(
            "General",
            "Unspec in Ship Only",
            false,
            "Disallows unspecing stats if you're not currently on the ship."
        );
        config.Bind(
            "General",
            "Unspec in Orbit Only",
            true,
            "Disallows unspecing stats if you're not currently in orbit."
        );
        config.Bind(
            "General",
            "Disable Unspec",
            false,
            "Disallows unspecing altogether."
        );
        config.Bind(
            "General",
            "Keep progress",
            false,
            "Keep your progress after being fired"
        );
    }

    private void InitHealthSettings(ConfigFile config)
    {
        config.Bind(
            "Health",
            "Health Regen Enabled",
            true,
            "Enable the Health Regen skill?"
        );

        config.Bind(
            "Health",
            "Health Regen Max Level",
            20,
            "Maximum level for the health regen."
        );

        config.Bind(
            "Health",
            "Health Regen Multiplier",
            0.05f,
            "How much does the health regen skill increase per level?"
        );
    }

    private void InitStaminaSettings(ConfigFile config)
    {
        config.Bind(
            "Stamina",
            "Stamina Enabled",
            true,
            "Enable the Stamina skill?"
        );

        config.Bind(
            "Stamina",
            "Stamina Max Level",
            99999,
            "Maximum level for the stamina."
        );

        config.Bind(
            "Stamina",
            "Stamina Multiplier",
            2,
            "How much does the stamina skill increase per level?"
        );
    }

    private void InitBatterySettings(ConfigFile config)
    {
        config.Bind(
            "Battery",
            "Battery Life Enabled",
            true,
            "Enable the Battery Life skill?"
        );

        config.Bind(
            "Battery",
            "Battery Life Max Level",
            99999,
            "Maximum level for the battery life."
        );

        config.Bind(
            "Battery",
            "Battery Life Multiplier",
            5,
            "How much does the battery life skill increase per level?"
        );
    }

    private void InitHandSlotsSettings(ConfigFile config)
    {
        config.Bind(
            "Hand Slots",
            "Hand Slots Enabled",
            true,
            "Enable the Hand Slots skill?"
        );

        config.Bind(
            "Hand Slots",
            "Hand Slots Max Level",
            30,
            "Maximum level for the hand slots."
        );

        config.Bind(
            "Hand Slots",
            "Hand Slots Multiplier",
            10,
            "How much does the hand slots skill increase per level?"
        );
    }

    private void InitLootValueSettings(ConfigFile config)
    {
        config.Bind(
            "Loot Value",
            "Loot Value Enabled",
            true,
            "Enable the Loot Value skill?"
        );

        config.Bind(
            "Loot Value",
            "Loot Value Max Level",
            250,
            "Maximum level for the loot value."
        );

        config.Bind(
            "Loot Value",
            "Loot Value Multiplier",
            0.1f,
            "How much does the loot value skill increase per level?"
        );
    }

    private void InitJumpHeightSettings(ConfigFile config)
    {
        config.Bind(
            "Jump Height",
            "Jump Height Enabled",
            true,
            "Enable the Jump Height skill?"
        );

        config.Bind(
            "Jump Height",
            "Jump Height Max Level",
            99999,
            "Maximum level for Jump Height."
        );

        config.Bind(
            "Jump Height",
            "Jump Height Multiplier",
            3f,
            "How much does the Jump Height skill increase per level?"
        );
    }

    private void InitSprintSpeedSettings(ConfigFile config)
    {
        config.Bind(
            "Sprint Speed",
            "Sprint Speed Enabled",
            true,
            "Enable the Sprint Speed skill?"
        );

        config.Bind(
            "Sprint Speed",
            "Sprint Speed Max Level",
            99999,
            "Maximum level for Sprint Speed."
        );

        config.Bind(
            "Sprint Speed",
            "Sprint Speed Multiplier",
            0.75f,
            "How much does the Sprint Speed skill increase per level?"
        );
    }

    private void InitStrengthSettings(ConfigFile config)
    {
        config.Bind(
            "Strength",
            "Strength Enabled",
            true,
            "Enable the strength skill?"
        );

        config.Bind(
            "Strength",
            "Strength Max Level",
            75,
            "Maximum level for Strength."
        );

        config.Bind(
            "Strength",
            "Strength Multiplier",
            1f,
            "How much does the Strength skill increase per level?"
        );
    }
}