using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class SprintSpeed : Skill
{
    public override string ShortName => "SPD";

    public override string Name => "Sprint Speed";

    public override string Attribute => "Sprint Speed";

    public override string Description => "The company empowers you with pure steroids, run, spaceman.";

    public override UpgradeType UpgradeType => UpgradeType.SprintSpeed;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            ISprintSpeedConfig config = LessShitConfigSystem.GetActive<ISprintSpeedConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            ISprintSpeedConfig config = LessShitConfigSystem.GetActive<ISprintSpeedConfig>();

            return config.multiplier;
        }
    }
    
    public override bool IsTeamShared => false;

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
