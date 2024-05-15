using System;
using System.Collections.Generic;
using System.Globalization;
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
        IDictionary<string, string> hostConfig = LethalPlugin.ModConfig.hostConfig;

        if (bool.Parse(hostConfig["Health Regen Enabled"]))
        {
            CreateSkill(UpgradeType.HPRegen,
                "Health Regen",
                "The company installs a basic healer into your suit, letting you regenerate health slowly. Only regenerate up to 100 HP.",
                "HPR",
                "Health Regeneration",
                1,
                int.Parse(hostConfig["Health Regen Max Level"]),
                float.Parse(hostConfig["Health Regen Multiplier"], CultureInfo.InvariantCulture)
            );
        }

        if (bool.Parse(hostConfig["Stamina Enabled"]))
        {
            CreateSkill(UpgradeType.Stamina,
                "Stamina",
                "Hours on that company gym finally coming into play. Allows you to run for longer, but has to regenerate it slower.",
                "STM",
                "Stamina",
                1,
                int.Parse(hostConfig["Stamina Max Level"]),
                float.Parse(hostConfig["Stamina Multiplier"], CultureInfo.InvariantCulture)
            );
        }

        if (bool.Parse(hostConfig["Battery Life Enabled"]))
        {
            CreateSkill(UpgradeType.Battery,
                "Battery Life",
                "The company provides you with better batteries. Replace your batteries AT THE SHIP'S CHARGER to see an effect.",
                "BAT",
                "Battery Life",
                1,
                int.Parse(hostConfig["Battery Life Max Level"]),
                float.Parse(hostConfig["Battery Life Multiplier"], CultureInfo.InvariantCulture)
            );
        }

        if (bool.Parse(hostConfig["Hand Slots Enabled"]) && !LethalPlugin.ReservedSlots)
        {
                CreateSkill(UpgradeType.HandSlot,
                "Hand Slot",
                "The company finally gives you a better belt! Fit more stuff! (One slot every 100%.)",
                "HND",
                "Hand Slots",
                1,
                int.Parse(hostConfig["Hand Slots Max Level"]),
                float.Parse(hostConfig["Hand Slots Multiplier"], CultureInfo.InvariantCulture),
                HandSlots.HandSlotsUpdate
            );
        }

        if (bool.Parse(hostConfig["Loot Value Enabled"]))
        {
            CreateSkill(UpgradeType.Value,
                "Loot Value",
                "The company gives you a better pair of eyes, allowing you to see the value in things.",
                "VAL",
                "Loot Value",
                1,
                int.Parse(hostConfig["Loot Value Max Level"]),
                float.Parse(hostConfig["Loot Value Multiplier"], CultureInfo.InvariantCulture),
                LootValue.LootValueUpdate,
                true
            );
        }
        
        if (bool.Parse(hostConfig["Strength Enabled"]))
        {
            CreateSkill(UpgradeType.Strength,
                "Strength",
                "More work at the Company's gym gives you pure muscles! You can carry better. (Reduces weight by a percentage.)",
                "STR",
                "Weight Reduction",
                1,
                int.Parse(hostConfig["Strength Max Level"]),
                float.Parse(hostConfig["Strength Multiplier"], CultureInfo.InvariantCulture),
                Strength.StrengthUpdate
            );
        }

        if (bool.Parse(hostConfig["Jump Height Enabled"]))
        {
            CreateSkill(UpgradeType.JumpHeight,
                "Jump Height",
                "The company installs you with jumping boots! (The company is not responsible for any broken knees.)",
                "JMP",
                "Jump Height",
                1,
                int.Parse(hostConfig["Jump Height Max Level"]),
                float.Parse(hostConfig["Jump Height Multiplier"], CultureInfo.InvariantCulture)
            );
        }

        if (bool.Parse(hostConfig["Sprint Speed Enabled"]))
        {
            CreateSkill(UpgradeType.SprintSpeed,
                "Sprint Speed",
                "The company empowers you with pure steroids, run, spaceman.",
                "SPD",
                "Sprint Speed",
                1,
                int.Parse(hostConfig["Sprint Speed Max Level"]),
                float.Parse(hostConfig["Sprint Speed Multiplier"], CultureInfo.InvariantCulture)
            );
        }
    }
}
