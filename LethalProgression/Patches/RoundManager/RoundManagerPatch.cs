using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class RoundManagerPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    static IEnumerable<CodeInstruction> SpawnScrapInLevelTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return LootValue.ScrapValueMultiplierOpCode(codes);
    }
}