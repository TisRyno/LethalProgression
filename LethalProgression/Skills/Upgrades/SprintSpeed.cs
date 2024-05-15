using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

public static class SprintSpeed
{   
    public static float GetSprintSpeed(float defaultSprintSpeed)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.SprintSpeed))
            return defaultSprintSpeed;

        float speedMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.SprintSpeed].GetTrueValue() / 100f);

        return defaultSprintSpeed * speedMultiplier;
    }

    public static List<CodeInstruction> PlayerSprintSpeedOpCode(List<CodeInstruction> codes)
    {
         for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldc_R4 && codes[index].operand.Equals(2.25f))
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(SprintSpeed).GetMethod("GetSprintSpeed")));
        
        return codes;
    }
}
