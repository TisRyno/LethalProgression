using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalProgression.Skills.Upgrades;

[HarmonyPatch]
internal class HPRegen
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    private static void HPRegenUpdate(PlayerControllerB __instance)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return;

        if (__instance.healthRegenerateTimer > 0f)
        {
            __instance.healthRegenerateTimer -= Time.deltaTime;
            return;
        }
        
        Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.HPRegen];
        // Then turn that into seconds. So, if hps is 0.5, then it will take 2 seconds to regen 1 health.
        __instance.healthRegenerateTimer = 1f / skill.GetTrueValue(); // 0.05 * 5 = 0.25
        __instance.health++;

        if (__instance.health >= 20)
            __instance.MakeCriticallyInjured(false);

        if (!__instance.IsOwner || (__instance.IsServer && !__instance.isHostPlayerObject))
            return;

        if (!__instance.isPlayerControlled || __instance.health >= 100 || __instance.isPlayerDead)
            return;

        HUDManager.Instance.UpdateHealthUI(__instance.health, false);
    }

    public static List<CodeInstruction> DisableBaseGameHPRegen(List<CodeInstruction> codes)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return codes;

        FieldInfo health = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.health));
        
        for (int index = 0; index < codes.Count; index++)
        {
            if (codes[index].opcode != OpCodes.Ldfld || (FieldInfo)codes[index].operand != health)
                continue;
            
            if (codes[index + 1].opcode != OpCodes.Ldc_I4 || (int) codes[index + 1].operand != 20)
                continue;
            
            // Branch off if Greater than or equal to (skip if less than only)
            if (codes[index + 2].opcode != OpCodes.Bge_S)
                continue;

            // Remove 20 arg from top of the stack
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Pop));
            // Duplicate the value of health (health < health) == false
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Dup));
        }

        return codes;
    }
}
