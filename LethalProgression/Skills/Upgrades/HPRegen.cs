﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class HPRegen : Skill
{
    public override string ShortName => "HPR";

    public override string Name => "Health Regen";

    public override string Attribute => "Health Regeneration";

    public override string Description => "The company installs a basic healer into your suit, letting you regenerate health slowly. Only regenerate up to your max HP.";

    public override UpgradeType UpgradeType => UpgradeType.HPRegen;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IHealthRegenConfig config = LessShitConfigSystem.GetActive<IHealthRegenConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IHealthRegenConfig config = LessShitConfigSystem.GetActive<IHealthRegenConfig>();

            return config.multiplier;
        }
    }

    public override bool IsTeamShared => false;

    public static List<CodeInstruction> DisableBaseGameHPRegen(List<CodeInstruction> codes)
    {
        if (!LP_NetworkManager.xpInstance || !LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return codes;

        FieldInfo healthField = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.health));
        
        for (int index = 0; index < codes.Count; index++)
        {
            if (codes[index].opcode != OpCodes.Ldfld || (FieldInfo)codes[index].operand != healthField)
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
