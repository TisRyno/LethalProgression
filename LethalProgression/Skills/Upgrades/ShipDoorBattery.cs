using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

internal class ShipDoorBattery : Skill
{
    public override string ShortName => "DRB";

    public override string Name => "Ship Door Battery";

    public override string Attribute => "Ship Door Battery";

    public override string Description => "The company gives you a better pair of eyes, allowing you to see the value in things.";

    public override UpgradeType UpgradeType => UpgradeType.ShipDoorBattery;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IShipHangarDoorConfig config = LessShitConfigSystem.GetActive<IShipHangarDoorConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IShipHangarDoorConfig config = LessShitConfigSystem.GetActive<IShipHangarDoorConfig>();

            return config.multiplier;
        }
    }
    
    public ShipDoorBattery(): base(ShipDoorBattery.ShipDoorBatteryUpdate) {}

    public override bool IsTeamShared => false;

    public static float GetUpdateBatteryUsage(float defaultBatteryDuration)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.ShipDoorBattery))
            return defaultBatteryDuration;

        float batteryMultiplier = 1 + (LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.ShipDoorBattery].GetTrueValue() / 100f);

        return defaultBatteryDuration * batteryMultiplier;
    }

    public static List<CodeInstruction> BatteryDegradeUpdateOpCode(List<CodeInstruction> codes)
    {
        FieldInfo doorBattery = typeof(HangarShipDoor).GetField(nameof(HangarShipDoor.doorPowerDuration));

        for (int index = 0; index < codes.Count; index++)
            if (codes[index].opcode == OpCodes.Ldfld && (FieldInfo) codes[index].operand == doorBattery)
                // Higher is slower
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, typeof(ShipDoorBattery).GetMethod("GetUpdateBatteryUsage")));

        return codes;
    }

    public static void ShipDoorBatteryUpdate(int change)
    {
        // Do not use `IsSkillValid()` as it also checks the skill has > 0 points put into it.
        if (!LP_NetworkManager.xpInstance.skillList.skills.ContainsKey(UpgradeType.ShipDoorBattery))
            return;

        LP_NetworkManager.xpInstance.updateTeamShipDoorBatteryLevelClientMessage.SendServer(change);
    }
}
