using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Skills;
using Newtonsoft.Json;

namespace LethalProgression.Saving
{
    [HarmonyPatch]
    internal class SavePatches
    {
        // Whenever game saves, do save!
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameNetworkManager), "SaveGame")]
        private static void SaveGame(GameNetworkManager __instance)
        {
            LethalPlugin.Log.LogDebug("Invoked DoSave via SaveGame");
            DoSave(__instance);
        }

        // Whenever disconnect
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        private static void Disconnect(GameNetworkManager __instance)
        {
            if (__instance.currentLobby == null)
                return;

            LethalPlugin.Log.LogDebug("Invoked DoSave via Disconnect");

            DoSave(__instance);
        }

        public static void DoSave(GameNetworkManager __instance)
        {
            SaveData saveData = new SaveData
            {
                steamId = __instance.localPlayerController.playerSteamId,
                skillPoints = LP_NetworkManager.xpInstance.skillPoints
            };

            foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
            {
                LethalPlugin.Log.LogInfo($"Skill is {skill.Key} and value is {skill.Value.GetLevel()}");
                saveData.skillAllocation.Add(skill.Key, skill.Value.GetLevel());
            }

            string data = JsonConvert.SerializeObject(saveData);
            LP_NetworkManager.xpInstance.SaveData_ServerRpc(__instance.localPlayerController.playerSteamId, data);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DeleteFileButton), "DeleteFile")]
        private static void DeleteSaveFile(DeleteFileButton __instance)
        {
            SaveManager.DeleteSave(__instance.fileToDelete);
        }
    }
}
