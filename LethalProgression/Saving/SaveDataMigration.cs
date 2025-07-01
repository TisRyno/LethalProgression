using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace LethalProgression.Saving;

internal static class SaveDataMigration
{
    public static void MigrateOldSaves()
    {
        LethalPlugin.Log.LogInfo("Checking for legacy save files to migrate.");

        if (!Directory.Exists(Application.persistentDataPath + "/lethalprogression"))
        {
            LethalPlugin.Log.LogInfo("No legacy save files found to migrate. ");
            return;
        }

        List<string> directories = new List<string>(Directory.EnumerateDirectories(Application.persistentDataPath + "/lethalprogression", "save*"));

        if (directories.Count == 0)
        {
            LethalPlugin.Log.LogInfo("No legacy save files found to migrate. ");
            return;
        }

        LethalPlugin.Log.LogInfo($"Found {directories.Count} saves to migrate.");

        foreach (string dir in directories)
        {
            string dirName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            
            LethalPlugin.Log.LogInfo($"Found {dirName} in {dir} to migrate.");

            if (dirName.StartsWith("save"))
            {
                LethalPlugin.Log.LogWarning($"Found save: {dirName} - now migrating to ES3");
                MigrateSave(dir);
            }
        }
    }

    public static void MigrateSave(string dir)
    {
        string currentSave = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

        currentSave = currentSave.Replace("save", "LCSaveFile");

        List<string> files = new List<string>(Directory.EnumerateFiles(dir));

        foreach (string file in files)
        {
            string fileName =  file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            fileName = fileName.Replace(".json", "");

            if (fileName == "shared") {
                LethalPlugin.Log.LogInfo($"Found shared data at {file}");

                string json = File.ReadAllText(file);

                SaveSharedData currentData = JsonConvert.DeserializeObject<SaveSharedData>(json);

                LethalPlugin.Log.LogInfo($"Old data: {currentData}");

                ES3SaveManager.SaveShared(currentData, currentSave);
            } else {
                LethalPlugin.Log.LogInfo($"Found player data at {file}");

                string playerJson = File.ReadAllText(file);

                ulong.TryParse(fileName, out ulong steamId);

                LethalPlugin.Log.LogInfo($"Parsed Steam ID: {steamId}");

                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(playerJson);

                LethalPlugin.Log.LogInfo($"Old data: {saveData}");

                ES3SaveManager.Save(steamId, saveData, currentSave);
            }

            File.Delete(file);
        }

        Directory.Delete(dir);
    }
}