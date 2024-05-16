using HarmonyLib;
using LethalProgression.Saving;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class SavePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DeleteFileButton), "DeleteFile")]
    private static void DeleteSaveFile(DeleteFileButton __instance)
    {
        SaveManager.DeleteSave(__instance.fileToDelete);
    }
}
