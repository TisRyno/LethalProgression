using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.LessShitConfig;
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
        IGeneralConfig generalConfig = LessShitConfigSystem.GetActive<IGeneralConfig>();

        if (generalConfig.keepProgress)
            return;
        
        var xpInstance = LP_NetworkManager.xpInstance;
        int saveFileNum = GameNetworkManager.Instance.saveFileNum + 1;
        SaveManager.DeleteSave(saveFileNum);

        xpInstance.teamXPRequired.Value = xpInstance.CalculateXPRequirement();
        foreach (Skill skill in xpInstance.skillList.skills.Values)
        {
            skill.SetLevel(0);
        }

        xpInstance.SetSkillPoints(xpInstance.GetDefaultStartingSkillPoints());

        xpInstance.teamXP.Value = 0;
        xpInstance.teamTotalValue.Value = 0;
        xpInstance.teamLevel.Value = 0;

        xpInstance.teamLootLevel.Value = 0;
    }
}
