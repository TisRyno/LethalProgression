using HarmonyLib;
using LethalProgression.Saving;
using LethalProgression.Skills;
using LethalProgression.Skills.Upgrades;
using Unity.Netcode;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class GameNetworkManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    private static void DisconnectXPHandler()
    {
        if (LP_NetworkManager.xpInstance.skillList.GetSkill(UpgradeType.Value).GetLevel() != 0)
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
