using GameNetcodeStuff;
using HarmonyLib;
using LethalProgression.Config;
using LethalProgression.GUI.Skills;
using LethalProgression.LessShitConfig;
using LethalProgression.Saving;
using LethalProgression.Skills;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Patches;
    
[HarmonyPatch]
internal class StartOfRoundPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "FirePlayersAfterDeadlineClientRpc")]
    private static void ResetXPValues()
    {
        IGeneralConfig generalConfig = LessShitConfigSystem.GetActive<IGeneralConfig>();

        if (generalConfig.keepProgress)
            return;
        
        var xpInstance = LP_NetworkManager.xpInstance;

        ES3SaveManager.DeleteSave();

        xpInstance.teamXPRequired.Value = xpInstance.CalculateXPRequirement();
        foreach (Skill skill in xpInstance.skillList.skills.Values)
        {
            skill.SetLevel(0, false);
        }

        LethalPlugin.SkillsGUI.UpdateAllStats();

        ES3SaveManager.TriggerHostProfileSave();

        xpInstance.SetSkillPoints(xpInstance.GetDefaultStartingSkillPoints());

        xpInstance.teamXP.Value = 0;
        xpInstance.teamTotalValue.Value = 0;
        xpInstance.teamLevel.Value = 0;

        xpInstance.teamLootLevel.Value = 0;
        xpInstance.teamShipDoorBatteryLevel.Value = 0;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
    static void ReviveDeadPlayersPostfix()
    {
        PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;

        int startingHP =  MaxHP.GetNewMaxHealth(100);

        playerControllerB.health = startingHP;
        HUDManager.Instance.UpdateHealthUI(startingHP, false);
    }
}
