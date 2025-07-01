using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class Stamina : Skill
{
    public override string ShortName => "STM";

    public override string Name => "Stamina";

    public override string Attribute => "Stamina";

    public override string Description => "Hours on that company gym finally coming into play. Allows you to run for longer, but has to regenerate it slower.";

    public override UpgradeType UpgradeType => UpgradeType.Stamina;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IStaminaConfig config = LessShitConfigSystem.GetActive<IStaminaConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IStaminaConfig config = LessShitConfigSystem.GetActive<IStaminaConfig>();

            return config.multiplier;
        }
    }
    
    public override bool IsTeamShared => false;

    public static float GetJumpStaminaUsage(float defaultJumpStaminaUsage)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Stamina))
            return defaultJumpStaminaUsage;

        float staminaMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Stamina].GetTrueValue() / 100f);
        float usageMultiplier = 1 / staminaMultiplier;

        return defaultJumpStaminaUsage * usageMultiplier;
    }

    public static List<CodeInstruction> PlayerJumpStaminaOpCode(List<CodeInstruction> codes)
    {
        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldc_R4 && codes[index].operand.Equals(0.08f))
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(Stamina).GetMethod("GetJumpStaminaUsage")));

        return codes;
    }

    public static float GetSprintTime(float defaultStaminaTime)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Stamina))
            return defaultStaminaTime;

        float staminaMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Stamina].GetTrueValue() / 100f);

        return defaultStaminaTime * staminaMultiplier;
    }

    public static List<CodeInstruction> PlayerSprintTimeOpCode(List<CodeInstruction> codes) 
    {
        FieldInfo sprintTime = typeof(PlayerControllerB).GetField(nameof(PlayerControllerB.sprintTime));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == sprintTime)
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(Stamina).GetMethod("GetSprintTime")));

        return codes;
    }
}
