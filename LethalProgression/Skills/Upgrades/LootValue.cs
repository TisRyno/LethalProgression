using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

[HarmonyPatch]
internal class LootValue
{
    public static float GetScrapValueMultiplier(float defaultScrapValueMultiplier)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return defaultScrapValueMultiplier;

        float mult = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetMultiplier();
        float value = LP_NetworkManager.xpInstance.teamLootLevel.Value * mult;
        float valueMultiplier = 1 + (value / 100f);

        return defaultScrapValueMultiplier * valueMultiplier;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    static IEnumerable<CodeInstruction> SpawnScrapInLevelTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        FieldInfo scrapValueMultiplier = typeof(RoundManager).GetField(nameof(RoundManager.scrapValueMultiplier));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == scrapValueMultiplier)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(LootValue).GetMethod("GetScrapValueMultiplier")));

        return codes;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    private static void AddLootValue()
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return;

        float mult = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetMultiplier();
        float value = LP_NetworkManager.xpInstance.teamLootLevel.Value * mult;

        float scrapValueAdded = value / 100;

        try
        {
            RoundManager.Instance.scrapValueMultiplier += scrapValueAdded;
        }
        catch { }

        LethalPlugin.Log.LogDebug($"Added {scrapValueAdded} to scrap value multiplier, resulting in {RoundManager.Instance.scrapValueMultiplier}");
    }

    public static void LootValueUpdate(int change)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return;

        LP_NetworkManager.xpInstance.updateTeamLootLevelClientMessage.SendServer(change);
    }
}
