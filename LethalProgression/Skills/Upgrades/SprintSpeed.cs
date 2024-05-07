using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

[HarmonyPatch]
public static class SprintSpeed
{
    public static float sprintSpeed = 2.25f;
    public static void SprintSpeedUpdate(int updatedValue)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.SprintSpeed))
            return;

        Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.SprintSpeed];

        float addedSpeed = updatedValue * skill.GetMultiplier() / 100f * 2.25f;

        sprintSpeed += addedSpeed;
        
        LethalPlugin.Log.LogInfo($"{updatedValue} change, Adding {addedSpeed} resulting in {sprintSpeed} run speed");
    }

    public static float GetSprintSpeed()
    {
        return sprintSpeed;
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // Don't run if mike's tweaks is installed
        if (LethalPlugin.MikesTweaks)
            return instructions;

        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++)
            if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand.Equals(2.25f))
                codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(SprintSpeed).GetMethod("GetSprintSpeed")));

        return codes.AsEnumerable();
    }
}
