using HarmonyLib;
using LethalProgression.Saving;
using LethalProgression.Skills;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Patches;
    
[HarmonyPatch]
internal class StartOfRoundPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartOfRound), "ResetMiscValues")]
    private static void ResetMiscValues_PrePatch()
    {
        JumpHeight.JumpHeightUpdate(0);
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
    private static void OnShipLandedMiscEvents_PrePatch()
    {
        JumpHeight.JumpHeightUpdate(0);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "FirePlayersAfterDeadlineClientRpc")]
    private static void ResetXPValues(StartOfRound __instance)
    {
        var xpInstance = LP_NetworkManager.xpInstance;
        int saveFileNum = GameNetworkManager.Instance.saveFileNum + 1;
        SaveManager.DeleteSave(saveFileNum);

        xpInstance.teamXPRequired.Value = xpInstance.CalculateXPRequirement();
        foreach (Skill skill in xpInstance.skillList.skills.Values)
        {
            skill.SetLevel(0);
        }

        xpInstance.SetSkillPoints(5);

        xpInstance.teamXP.Value = 0;
        xpInstance.teamTotalValue.Value = 0;
        xpInstance.teamLevel.Value = 0;

        xpInstance.teamLootLevel.Value = 0;
    }
}
