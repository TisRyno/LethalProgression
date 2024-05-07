using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

internal class Stamina
{
    public static float GetJumpStaminaUsage(float defaultJumpStaminaUsage)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Stamina))
            return defaultJumpStaminaUsage;

        float staminaMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Stamina].GetTrueValue() / 100f);
        float usageMultiplier = 1 / staminaMultiplier;

        return defaultJumpStaminaUsage * usageMultiplier;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    private static IEnumerable<CodeInstruction> Jump_performedTransplier(IEnumerable<CodeInstruction> instructions) 
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldc_R4 && codes[index].operand.Equals(0.08f))
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(Stamina).GetMethod("GetStamina")));

        return codes;
    }

    
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    private static IEnumerable<CodeInstruction> LateUpdateTransplier(IEnumerable<CodeInstruction> instructions) 
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        FieldInfo sprintTime = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.sprintTime));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == sprintTime)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(Stamina).GetMethod("GetSprintTime")));

        return codes;
    } 

    public static void StaminaUpdate(int updatedValue)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Stamina))
            return;

        Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Stamina];
        PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
        // 1 level adds 2% more to 5. So that is equals to 5 * 1.02.

        float addedStamina = updatedValue * skill.GetMultiplier() / 100f * 11f;
        
        localPlayer.sprintTime += addedStamina;
        LethalPlugin.Log.LogInfo($"{updatedValue} change, Adding {addedStamina} resulting in {localPlayer.sprintTime} stamina");
    }
}
