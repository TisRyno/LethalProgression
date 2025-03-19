using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LethalProgression.Skills.Upgrades;

internal class BatteryLife
{
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
