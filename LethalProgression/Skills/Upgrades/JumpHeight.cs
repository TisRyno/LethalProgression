using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

internal class JumpHeight
{
    public static float GetJumpForce(float defaultJumpForce)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.JumpHeight))
            return defaultJumpForce;

        float jumpForceMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.JumpHeight].GetTrueValue() / 100f);

        return defaultJumpForce * jumpForceMultiplier;
    }

    public static List<CodeInstruction> PlayerJumpOpCode(List<CodeInstruction> codes)
    {
        FieldInfo jumpForce = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.jumpForce));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == jumpForce)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(JumpHeight).GetMethod("GetJumpForce")));

        return codes;
    }
}
