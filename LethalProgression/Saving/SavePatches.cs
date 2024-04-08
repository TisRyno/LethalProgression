using HarmonyLib;
using Unity.Netcode;
using Steamworks;

namespace LethalProgression.Saving
{
    [HarmonyPatch]
    internal class SavePatches : NetworkBehaviour
    {
        // Whenever game saves, do save!
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameNetworkManager), "SaveGame")]
        [HarmonyPriority(Priority.First)]
        private static void SaveGamePrefix()
        {
            LethalPlugin.Log.LogDebug("Invoked DoSave via SaveGame");
            SaveManager.TriggerHostProfileSave();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DeleteFileButton), "DeleteFile")]
        private static void DeleteSaveFile(DeleteFileButton __instance)
        {
            SaveManager.DeleteSave(__instance.fileToDelete);
        }
    }
}
