using Newtonsoft.Json;
using Steamworks;
using System.Collections.Generic;
using LethalProgression.Skills;
using LethalProgression.Network;

namespace LethalProgression.Saving;

internal static class ES3SaveManager
{

    public static void TriggerHostProfileSave()
    {
        ulong _steamId = SteamClient.SteamId;

        SaveData saveData = new()
        {
            steamId = _steamId,
            skillPoints = LP_NetworkManager.xpInstance.skillPoints,
            skillAllocation = new Dictionary<UpgradeType, int>()
        };

        foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
        {
            LethalPlugin.Log.LogDebug($"Skill is {skill.Key} and value is {skill.Value.CurrentLevel}");
            saveData.skillAllocation.Add(skill.Key, skill.Value.CurrentLevel);
        }
        
        LethalPlugin.Log.LogDebug($"Invoke saveProfileDataClientMessage({_steamId}, {JsonConvert.SerializeObject(saveData)})");

        LP_NetworkManager.xpInstance.saveProfileDataClientMessage.SendServer(JsonConvert.SerializeObject(new SaveProfileData(_steamId, saveData)));
    }

    public static void DeleteSave()
    {
        string saveFileSlot = GameNetworkManager.Instance.currentSaveFileName;

        string[] keys = ES3.GetKeys(saveFileSlot);

        foreach (string key in keys)
        {
            if (key.StartsWith("LethalProgression_"))
            {
                LethalPlugin.Log.LogDebug($"Found key {key} to delete in {saveFileSlot}");
                ES3.DeleteKey(key, saveFileSlot);
            }
        }
    }

    public static void Save(ulong steamId, SaveData data)
    {
        Save(steamId, data, null);
    }

    public static void Save(ulong steamId, SaveData data, string overrideSaveFile)
    {
        string saveFileSlot = overrideSaveFile;

        if (overrideSaveFile == null) {
            saveFileSlot = GameNetworkManager.Instance.currentSaveFileName;
        }

        LethalPlugin.Log.LogDebug($"Saving player {steamId} data to {saveFileSlot}");

        ES3.Save($"LethalProgression_{steamId}_Data", JsonConvert.SerializeObject(data), saveFileSlot);
    }

    public static void SaveShared(SaveSharedData data, string overrideSaveFile)
    {
        SaveShared(data.xp, data.level, data.quota, overrideSaveFile);
    }

    public static void SaveShared(int xp, int level, int quota)
    {
        SaveShared(xp, level, quota, null);
    }

    public static void SaveShared(int xp, int level, int quota, string overrideSaveFile)
    {
        string saveFileSlot = overrideSaveFile;

        if (overrideSaveFile == null) {
            saveFileSlot = GameNetworkManager.Instance.currentSaveFileName;
        }

        LethalPlugin.Log.LogDebug("Saving to save file " + saveFileSlot);

        ES3.Save($"LethalProgression_shared_Data", JsonConvert.SerializeObject(new SaveSharedData(xp, level, quota)), saveFileSlot);
    }

    public static string LoadPlayerFile(ulong steamId)
    {
        string saveFileSlot = GameNetworkManager.Instance.currentSaveFileName;

        if (!ES3.KeyExists($"LethalProgression_{steamId}_Data", saveFileSlot))
        {
            LethalPlugin.Log.LogDebug($"Player file for {steamId} doesn't exist");
            return null;
        }

        LethalPlugin.Log.LogDebug($"Player file for {steamId} found");

        return (string) ES3.Load($"LethalProgression_{steamId}_Data", saveFileSlot);
    }

    public static SaveSharedData? LoadSharedFile()
    {
        string saveFileSlot = GameNetworkManager.Instance.currentSaveFileName;

        if (!ES3.KeyExists("LethalProgression_shared_Data", saveFileSlot))
        {
            LethalPlugin.Log.LogDebug("Shared file doesn't exist");
            
            return null;
        }

        LethalPlugin.Log.LogDebug("Shared file exists");

        string json = (string) ES3.Load("LethalProgression_shared_Data", saveFileSlot);

        return JsonConvert.DeserializeObject<SaveSharedData>(json);
    }
}
