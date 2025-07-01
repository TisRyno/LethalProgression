using System;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class LootValue : Skill
{
    public override string ShortName => "VAL";

    public override string Name => "Loot Value";

    public override string Attribute => "Loot Value";

    public override string Description => "The company gives you a better pair of eyes, allowing you to see the value in things.";

    public override UpgradeType UpgradeType => UpgradeType.Value;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            ILootValueConfig config = LessShitConfigSystem.GetActive<ILootValueConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            ILootValueConfig config = LessShitConfigSystem.GetActive<ILootValueConfig>();

            return config.multiplier;
        }
    }

    public override bool IsTeamShared => true;

    public LootValue(): base(LootValueUpdate) {}

    public static int GetNewScrapValueMultiplier(int defaultScrapValue)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return defaultScrapValue;

        float mult = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].Multiplier;
        float value = LP_NetworkManager.xpInstance.teamLootLevel.Value * mult;
        float valueMultiplier = 1 + (value / 100f);
        int newValue = (int) Math.Round(defaultScrapValue * valueMultiplier);

        LethalPlugin.Log.LogDebug($"Current scrap value {defaultScrapValue} multiplied by {valueMultiplier} to {newValue}");

        return newValue;
    }

    public static void LootValueUpdate(int change)
    {
        // Do not use `IsSkillValid()` as it also checks the skill has > 0 points put into it.
        if (!LP_NetworkManager.xpInstance.skillList.skills.ContainsKey(UpgradeType.Value))
            return;

        LP_NetworkManager.xpInstance.updateTeamLootLevelClientMessage.SendServer(change);
    }
}
