using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using Unity.Netcode;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class PlayerControllerBPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    private static void ConnectClientToPlayerObjectHandler()
    {   
        ulong steamID = SteamClient.SteamId;

        LethalPlugin.Log.LogInfo($"Player {steamID} has joined the game.");

        LP_NetworkManager.xpInstance.requestProfileDataClientMessage.SendServer(steamID);
    }
}
