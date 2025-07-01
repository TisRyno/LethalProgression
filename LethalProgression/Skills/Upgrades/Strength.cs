using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;

namespace LethalProgression.Skills.Upgrades;

[HarmonyPatch]
internal class Strength : Skill
{
    public override string ShortName => "STR";

    public override string Name => "Strength";

    public override string Attribute => "Strength";

    public override string Description => "More work at the Company's gym gives you pure muscles! You can carry better. (Reduces weight by a percentage.)";

    public override UpgradeType UpgradeType => UpgradeType.Strength;

    public override int Cost => 1;

    public override int MaxLevel {
        get {
            IStrengthConfig config = LessShitConfigSystem.GetActive<IStrengthConfig>();

            return config.maxLevel;
        }
    }

    public override float Multiplier {
        get {
            IStrengthConfig config = LessShitConfigSystem.GetActive<IStrengthConfig>();

            return config.multiplier;
        }
    }
    
    public override bool IsTeamShared => false;

    public Strength(): base(StrengthUpdate) {}

    public static void StrengthUpdate(int _change = 0)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Strength))
            return;

        PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

        // Get every object in our hands.
        List<GrabbableObject> objects = player.ItemSlots.ToList<GrabbableObject>();

        LethalPlugin.Log.LogDebug($"Carry weight was {player.carryWeight}");

        // Apply multiplier
        float level = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Strength].GetTrueValue();
        float multiplier = level / 100;

        float newCarryWeight = 0f;
        foreach (GrabbableObject obj in objects)
        {
            if (obj == null) continue;

            float oldItemWeight = obj.itemProperties.weight - 1f;
            oldItemWeight *= 1 - multiplier;

            newCarryWeight += oldItemWeight;
            LethalPlugin.Log.LogDebug($"Item weight was {obj.itemProperties.weight - 1f} and is now {oldItemWeight}");
            LethalPlugin.Log.LogDebug($"Adding carryweight.. now up to {newCarryWeight}");
        }

        // Reduce by percent
        player.carryWeight = Math.Clamp(1 + newCarryWeight, 0, 10f);

        LethalPlugin.Log.LogDebug($"Player carry weight is now {player.carryWeight}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "GrabObjectClientRpc")]
    private static void UpdateObjects()
    {
        // have to recalculate, grabobject runs too many times.. somewhat inefficient
        StrengthUpdate();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GrabbableObject), "DiscardItem")]
    private static void UpdateByDiscard(GrabbableObject __instance)
    {
        if (__instance.IsOwner)
            StrengthUpdate();
    }
}
