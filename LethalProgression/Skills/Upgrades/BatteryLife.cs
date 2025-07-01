using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class BatteryLife : Skill
{
    public override string ShortName => "BAT";

    public override string Name => "Battery Life";

    public override string Attribute => "Battery Life";

    public override string Description => "The company provides you with better batteries found from exotic moons. Don't forget to recharge them AT THE SHIP'S CHARGER.";

    public override UpgradeType UpgradeType => UpgradeType.Battery;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IBatteryLifeConfig config = LessShitConfigSystem.GetActive<IBatteryLifeConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IBatteryLifeConfig config = LessShitConfigSystem.GetActive<IBatteryLifeConfig>();

            return config.multiplier;
        }
    }

    public override bool IsTeamShared => false;

    public static float GetUseItemBatteryUsage(float defaultBatteryUsage)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Battery))
            return defaultBatteryUsage;

        float batteryMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Battery].GetTrueValue() / 100f);
        float usageMultiplier = 1 / batteryMultiplier;

        return defaultBatteryUsage * usageMultiplier;
    }

    public static List<CodeInstruction> UseItemBatteriesOpCode(List<CodeInstruction> codes)
    {
        FieldInfo batteryUsage = typeof(Item).GetField(nameof(Item.batteryUsage));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == batteryUsage)
                // Higher is faster
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(BatteryLife).GetMethod("GetUseItemBatteryUsage")));

        return codes;
    }

    public static float GetUpdateBatteryUsage(float defaultBatteryUsage)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Battery))
            return defaultBatteryUsage;

        float batteryMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Battery].GetTrueValue() / 100f);

        return defaultBatteryUsage * batteryMultiplier;
    }

    public static List<CodeInstruction> BatteryDegradeUpdateOpCode(List<CodeInstruction> codes)
    {
        FieldInfo batteryUsage = typeof(Item).GetField(nameof(Item.batteryUsage));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == batteryUsage)
                // Higher is slower
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(BatteryLife).GetMethod("GetUpdateBatteryUsage")));

        return codes;
    }
}
