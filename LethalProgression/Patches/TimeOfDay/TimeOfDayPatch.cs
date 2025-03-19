using HarmonyLib;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class TimeOfDayPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
    private static void ProfitQuotaUpdate(TimeOfDay __instance)
    {
        if (!GameNetworkManager.Instance.isHostingGame)
            return;

        LP_NetworkManager.xpInstance.teamXPRequired.Value = LP_NetworkManager.xpInstance.CalculateXPRequirement();
    }
}
