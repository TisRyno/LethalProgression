using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace LethalProgression.Skills.Upgrades;

internal class MaxHP
{
    public static int GetNewMaxHealth(int defaultValue) {
        if (LP_NetworkManager.xpInstance == null)
            return defaultValue;
            
        Skill maxHpSkill = LP_NetworkManager.xpInstance.skillList.GetSkill(UpgradeType.MaxHealth);

        return defaultValue * (int) Math.Round(1 + maxHpSkill.GetTrueValue());
    }

    public static List<CodeInstruction> UncapMaxHealth(List<CodeInstruction> codes)
    {
        if (!LP_NetworkManager.xpInstance || !LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return codes;

        MethodInfo mathFClamp = typeof(Mathf).GetMethod("Clamp", new Type[] { typeof(int), typeof(int), typeof(int) });
        
        for (int index = 0; index < codes.Count; index++)
        {
            if (codes[index].Calls(mathFClamp))
            {
                if (codes[index - 2].opcode == OpCodes.Ldc_I4_0 && codes[index - 1].opcode == OpCodes.Ldc_I4_S)
                {
                    codes.Insert(index, new CodeInstruction(OpCodes.Call, typeof(MaxHP).GetMethod("GetNewMaxHealth")));
                    break;
                }
            }
        }

        return codes;
    }
}
