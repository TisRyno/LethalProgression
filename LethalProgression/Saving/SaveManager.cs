using UnityEngine;
using Newtonsoft.Json;
using Steamworks;
using System.IO;
using System.Collections.Generic;
using LethalProgression.Skills;
using LethalProgression.Network;

namespace LethalProgression.Saving
{
    internal static class SaveManager
    {
        public static int saveFileSlot = 0;

        public static void TriggerHostProfileSave()
        {
            ulong _steamId = SteamClient.SteamId;

            SaveData saveData = new SaveData
            {
                steamId = _steamId,
                skillPoints = LP_NetworkManager.xpInstance.skillPoints
            };

            foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
            {
                LethalPlugin.Log.LogInfo($"Skill is {skill.Key} and value is {skill.Value.GetLevel()}");
                saveData.skillAllocation.Add(skill.Key, skill.Value.GetLevel());
            }
            
            LethalPlugin.Log.LogInfo($"Invoke saveProfileDataClientMessage({_steamId}, {JsonConvert.SerializeObject(saveData)})");

            LP_NetworkManager.xpInstance.saveProfileDataClientMessage.SendServer(JsonConvert.SerializeObject(new SaveProfileData(_steamId, saveData)));
        }

        public static void Save(ulong steamid, SaveData data)
        {
            saveFileSlot = GameNetworkManager.Instance.saveFileNum;

            LethalPlugin.Log.LogInfo($"Saving to slot {saveFileSlot + 1} in {GetSavePath()}");

            // If file doesn't exist, create it
            if (!Directory.Exists(GetSavePath()))
            {
                Directory.CreateDirectory(GetSavePath());
            }

            File.WriteAllText(GetSavePath() + steamid + ".json", JsonConvert.SerializeObject(data));
        }

        public static void SaveShared(int xp, int level, int quota)
        {
            saveFileSlot = GameNetworkManager.Instance.saveFileNum;

            LethalPlugin.Log.LogInfo("Saving to slot " + saveFileSlot + 1);

            // If file doesn't exist, create it
            if (!Directory.Exists(GetSavePath()))
            {
                Directory.CreateDirectory(GetSavePath());
            }

            File.WriteAllText(GetSavePath() + "shared.json", JsonConvert.SerializeObject(new SaveSharedData(xp, level, quota)));
        }

        public static void DeleteSave(int _saveFileSlot)
        {
            saveFileSlot = _saveFileSlot;
            // Delete entire folder
            if (Directory.Exists(GetSavePath()))
            {
                Directory.Delete(Application.persistentDataPath + "/lethalprogression/save" + (saveFileSlot + 1), true);
            }
        }

        public static string GetSavePath()
        {
            return Application.persistentDataPath + "/lethalprogression/save" + (saveFileSlot + 1) + "/";
        }

        public static string LoadPlayerFile(ulong steamId)
        {
            saveFileSlot = GameNetworkManager.Instance.saveFileNum;

            if (!File.Exists(GetSavePath() + steamId + ".json"))
            {
                LethalPlugin.Log.LogInfo($"Player file for {steamId} doesn't exist");
                return null;
            }

            LethalPlugin.Log.LogInfo($"Player file for {steamId} found");

            return File.ReadAllText(GetSavePath() + steamId + ".json");
        }

        public static SaveSharedData LoadSharedFile()
        {
            saveFileSlot = GameNetworkManager.Instance.saveFileNum;

            if (!File.Exists(GetSavePath() + "shared.json"))
            {
                LethalPlugin.Log.LogInfo("Shared file doesn't exist");
                return null;
            }

            LethalPlugin.Log.LogInfo("Shared file exists");

            string json = File.ReadAllText(GetSavePath() + "shared.json");

            return JsonConvert.DeserializeObject<SaveSharedData>(json);
        }
    }
}
