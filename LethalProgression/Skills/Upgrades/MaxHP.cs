using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;
using UnityEngine;

namespace LethalProgression.Skills.Upgrades;

internal class MaxHP : Skill
{
    public override string ShortName => "MHP";

    public override string Name => "Max Health";

    public override string Attribute => "Maximum Health";

    public override string Description => "The company is trialling some new inhalents from exotic moons, giving you a health boost.";

    public override UpgradeType UpgradeType => UpgradeType.MaxHealth;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IMaxHealthConfig config = LessShitConfigSystem.GetActive<IMaxHealthConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IMaxHealthConfig config = LessShitConfigSystem.GetActive<IMaxHealthConfig>();

            return config.multiplier;
        }
    }

    public override bool IsTeamShared => false;

    public static int GetNewMaxHealth(int defaultValue) {
        if (LP_NetworkManager.xpInstance == null)
            return defaultValue;
            
        Skill maxHpSkill = LP_NetworkManager.xpInstance.skillList.GetSkill(UpgradeType.MaxHealth);

        if (maxHpSkill == null)
            return defaultValue;

        return (int) Math.Floor(defaultValue * (1 + (maxHpSkill.GetTrueValue() / 100f)));
    }

    public static List<CodeInstruction> UncapMaxHealth(List<CodeInstruction> codes)
    {
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
