using HarmonyLib;
using LethalProgression.Saving;
using LethalProgression.Skills;
using LethalProgression.Skills.Upgrades;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class GameNetworkManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPriority(Priority.First)]
    private static void DisconnectXPHandler()
    {
        if (LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
        {
            int localLootLevel = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetLevel();

            LP_NetworkManager.xpInstance.updateTeamLootLevelClientMessage.SendServer(-localLootLevel);
        }

        HandSlots.currentSlotCount = 4;

        LethalPlugin.SkillsGUI.CleanupGUI();
        LethalPlugin.SkillsGUI.CloseSkillMenu();
    }

    // Whenever game saves, do save!
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), "SaveGame")]
    [HarmonyPriority(Priority.First)]
    private static void SaveGamePrefix()
    {
        LethalPlugin.Log.LogDebug("Invoked DoSave via SaveGame");
        SaveManager.TriggerHostProfileSave();
    }
}
