using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

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

    public static List<CodeInstruction> ScrapValueMultiplierOpCode(List<CodeInstruction> codes)
    {
        FieldInfo scrapValueMultiplier = typeof(RoundManager).GetField(nameof(RoundManager.scrapValueMultiplier));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == scrapValueMultiplier)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(LootValue).GetMethod("GetScrapValueMultiplier")));

        return codes;
    }

    public static void LootValueUpdate(int change)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
            return;

        LP_NetworkManager.xpInstance.updateTeamLootLevelClientMessage.SendServer(change);
    }
}
