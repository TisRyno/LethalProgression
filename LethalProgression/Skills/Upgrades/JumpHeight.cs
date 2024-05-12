using HarmonyLib;
using GameNetcodeStuff;
using LethalProgression.Network;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalProgression.Skills.Upgrades;

[HarmonyPatch]
internal class JumpHeight
{
    public static float GetJumpForce(float defaultJumpForce)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.JumpHeight))
            return defaultJumpForce;

        float jumpForceMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.JumpHeight].GetTrueValue() / 100f);

        LethalPlugin.Log.LogInfo($"Default jump force {defaultJumpForce} with multiplier {jumpForceMultiplier} equals {defaultJumpForce * jumpForceMultiplier}");

        return defaultJumpForce * jumpForceMultiplier;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> PlayerJumpTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        FieldInfo jumpForce = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.jumpForce));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == jumpForce)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(JumpHeight).GetMethod("GetJumpForce")));

        return codes;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        FieldInfo jumpForce = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.jumpForce));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == jumpForce)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(JumpHeight).GetMethod("GetJumpForce")));

        return codes;
    }
}
