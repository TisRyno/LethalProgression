using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Saving;
using LethalProgression.Skills;

namespace LethalProgression.Patches;
    
[HarmonyPatch]
internal class StartOfRoundPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "FirePlayersAfterDeadlineClientRpc")]
    private static void ResetXPValues(StartOfRound __instance)
    {
        IDictionary<string, string> hostConfig = LethalPlugin.ModConfig.hostConfig;

        if (bool.Parse(hostConfig["Keep progress"]))
            return;
        
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
