using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class JumpHeight : Skill
{
    public override string ShortName => "JMP";

    public override string Name => "Jump Height";

    public override string Attribute => "Jump Height";

    public override string Description => "The company installs you with jumping boots! (The company is not responsible for any broken knees.)";

    public override UpgradeType UpgradeType => UpgradeType.JumpHeight;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IJumpHeightConfig config = LessShitConfigSystem.GetActive<IJumpHeightConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IJumpHeightConfig config = LessShitConfigSystem.GetActive<IJumpHeightConfig>();

            return config.multiplier;
        }
    }

    public override bool IsTeamShared => false;

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
