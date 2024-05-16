using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Patches;

internal class GrabbableObjectPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GrabbableObject), "UseItemBatteries")]
    static IEnumerable<CodeInstruction> UseItemBatteriesTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return BatteryLife.UseItemBatteriesOpCode(codes);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GrabbableObject), "Update")]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return BatteryLife.BatteryDegradeUpdateOpCode(codes);
    }
}