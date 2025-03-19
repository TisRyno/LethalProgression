using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalProgression;

[HarmonyPatch]
internal class LP_NetworkManager
{
    public static LC_XP xpInstance;
    public static GameObject xpNetworkObject;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), "Start")]
    public static void Init()
    {
        if (xpNetworkObject != null)
            return;

        xpNetworkObject = (GameObject)LethalPlugin.skillBundle.LoadAsset("LP_XPHandler");
        xpNetworkObject.AddComponent<LC_XP>();

        NetworkManager.Singleton.AddNetworkPrefab(xpNetworkObject);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    static void SpawnNetworkHandler()
    {
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
            return;

        GameObject networkHandlerHost = Object.Instantiate(xpNetworkObject, Vector3.zero, Quaternion.identity);

        networkHandlerHost.GetComponent<NetworkObject>().Spawn();

        xpInstance = networkHandlerHost.GetComponent<LC_XP>();

        LethalPlugin.Log.LogInfo("XPHandler Initialized.");
    }
}
