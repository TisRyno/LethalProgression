using HarmonyLib;
using LethalProgression.Skills.Upgrades;
using System.Collections.Generic;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class HangarShipDoorPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(HangarShipDoor), "Update")]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return ShipDoorBattery.BatteryDegradeUpdateOpCode(codes);
    }
}