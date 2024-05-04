using HarmonyLib;
using LethalProgression.Skills;
using LethalProgression.GUI;
using LethalProgression.Config;
using GameNetcodeStuff;
using LethalProgression.Saving;
using Steamworks;
using Unity.Netcode;

namespace LethalProgression.Patches
{
    [HarmonyPatch]
    internal class XPPatches : NetworkBehaviour
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "FirePlayersAfterDeadlineClientRpc")]
        private static void ResetXPValues(StartOfRound __instance)
        {
            if (!bool.Parse(SkillConfig.hostConfig["Keep progress"]))
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        private static void DisconnectXPHandler()
        {
            if (LP_NetworkManager.xpInstance.skillList.GetSkill(UpgradeType.Value).GetLevel() != 0)
            {
                int localLootLevel = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetLevel();

                LP_NetworkManager.xpInstance.updateTeamLootLevelClientMessage.SendServer(-localLootLevel);
            }

            SprintSpeed.sprintSpeed = 2.25f;
            HandSlots.currentSlotCount = 4;

            GUIUpdate.isMenuOpen = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        private static void ConnectClientToPlayerObjectHandler()
        {   
            ulong steamID = SteamClient.SteamId;

            LethalPlugin.Log.LogInfo($"Player {steamID} has joined the game.");

            LP_NetworkManager.xpInstance.requestProfileDataClientMessage.SendServer(steamID);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        private static void ProfitQuotaUpdate(TimeOfDay __instance)
        {
            if (!GameNetworkManager.Instance.isHostingGame)
            {
                return;
            }

            LP_NetworkManager.xpInstance.teamXPRequired.Value = LP_NetworkManager.xpInstance.CalculateXPRequirement();
        }
    }
}
