using System;

namespace LethalProgression.Skills.Upgrades;

internal class LootValue
{
    public static int GetNewScrapValueMultiplier(int defaultScrapValue)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return defaultScrapValue;

        float mult = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetMultiplier();
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
