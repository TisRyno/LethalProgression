using System;
using System.Collections.Generic;
using System.Globalization;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Skills;

internal class SkillList
{
    public Dictionary<UpgradeType, Skill> skills = new();

    public void CreateSkill(UpgradeType upgrade, string name, string description, string shortname, string attribute, int cost, int maxLevel, float multiplier, Action<int> callback = null, bool teamShared = false)
    {
        Skill newSkill = new Skill(name, description, shortname, attribute, upgrade, cost, maxLevel, multiplier, callback, teamShared);
        skills.Add(upgrade, newSkill);
    }

    public bool IsSkillValid(UpgradeType upgrade) => skills.ContainsKey(upgrade) && skills[upgrade].GetLevel() > 0;

    public Skill GetSkill(UpgradeType upgrade)
    {
        if (!IsSkillValid(upgrade))
        {
            return null;
        }

        return skills[upgrade];
    }

    public Dictionary<UpgradeType, Skill> GetSkills() => skills;

    public void InitializeSkills()
    {
        IHealthRegenConfig hpConfig = LessShitConfigSystem.GetActive<IHealthRegenConfig>();
        IStaminaConfig staminaConfig = LessShitConfigSystem.GetActive<IStaminaConfig>();
        IBatteryLifeConfig batteryConfig = LessShitConfigSystem.GetActive<IBatteryLifeConfig>();
        IHandSlotsConfig handSlotConfig = LessShitConfigSystem.GetActive<IHandSlotsConfig>();
        ILootValueConfig lootValueConfig = LessShitConfigSystem.GetActive<ILootValueConfig>();
        IStrengthConfig strengthConfig = LessShitConfigSystem.GetActive<IStrengthConfig>();
        IJumpHeightConfig jumpHeightConfig = LessShitConfigSystem.GetActive<IJumpHeightConfig>();
        ISprintSpeedConfig sprintSpeedConfig = LessShitConfigSystem.GetActive<ISprintSpeedConfig>();

        if (hpConfig.isEnabled)
        {
            CreateSkill(UpgradeType.HPRegen,
                "Health Regen",
                "The company installs a basic healer into your suit, letting you regenerate health slowly. Only regenerate up to 100 HP.",
                "HPR",
                "Health Regeneration",
                1,
                hpConfig.maxLevel,
                hpConfig.multiplier
            );
        }

        if (staminaConfig.isEnabled)
        {
            CreateSkill(UpgradeType.Stamina,
                "Stamina",
                "Hours on that company gym finally coming into play. Allows you to run for longer, but has to regenerate it slower.",
                "STM",
                "Stamina",
                1,
                staminaConfig.maxLevel,
                staminaConfig.multiplier
            );
        }

        if (batteryConfig.isEnabled)
        {
            CreateSkill(UpgradeType.Battery,
                "Battery Life",
                "The company provides you with better batteries. Replace your batteries AT THE SHIP'S CHARGER to see an effect.",
                "BAT",
                "Battery Life",
                1,
                batteryConfig.maxLevel,
                batteryConfig.multiplier
            );
        }

        if (handSlotConfig.isEnabled && !LethalPlugin.ReservedSlots)
        {
                CreateSkill(UpgradeType.HandSlot,
                "Hand Slot",
                "The company finally gives you a better belt! Fit more stuff! (One slot every 100%.)",
                "HND",
                "Hand Slots",
                1,
                handSlotConfig.maxLevel,
                handSlotConfig.multiplier,
                HandSlots.HandSlotsUpdate
            );
        }

        if (lootValueConfig.isEnabled)
        {
            CreateSkill(UpgradeType.Value,
                "Loot Value",
                "The company gives you a better pair of eyes, allowing you to see the value in things.",
                "VAL",
                "Loot Value",
                1,
                lootValueConfig.maxLevel,
                lootValueConfig.multiplier,
                LootValue.LootValueUpdate,
                true
            );
        }
        
        if (strengthConfig.isEnabled)
        {
            CreateSkill(UpgradeType.Strength,
                "Strength",
                "More work at the Company's gym gives you pure muscles! You can carry better. (Reduces weight by a percentage.)",
                "STR",
                "Weight Reduction",
                1,
                strengthConfig.maxLevel,
                strengthConfig.multiplier,
                Strength.StrengthUpdate
            );
        }

        if (jumpHeightConfig.isEnabled)
        {
            CreateSkill(UpgradeType.JumpHeight,
                "Jump Height",
                "The company installs you with jumping boots! (The company is not responsible for any broken knees.)",
                "JMP",
                "Jump Height",
                1,
                jumpHeightConfig.maxLevel,
                jumpHeightConfig.multiplier
            );
        }

        if (sprintSpeedConfig.isEnabled)
        {
            CreateSkill(UpgradeType.SprintSpeed,
                "Sprint Speed",
                "The company empowers you with pure steroids, run, spaceman.",
                "SPD",
                "Sprint Speed",
                1,
                sprintSpeedConfig.maxLevel,
                sprintSpeedConfig.multiplier
            );
        }
    }
}
