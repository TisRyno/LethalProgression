using HarmonyLib;
using LethalProgression.Skills;

namespace LethalProgression.Patches
{
    
    [HarmonyPatch]
    internal class StartOfRoundPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ResetMiscValues")]
        private static void ResetMiscValues_PrePatch()
        {
            JumpHeight.JumpHeightUpdate(0);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        private static void OnShipLandedMiscEvents_PrePatch()
        {
            JumpHeight.JumpHeightUpdate(0);
        }
    }
}