using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using System.Collections.Generic;
using LethalProgression.Skills.Upgrades;

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

    /// <summary>
    /// Jump skill to modify the force amount of the jump
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> PlayerJumpTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return JumpHeight.PlayerJumpOpCode(codes);
    }

    /// <summary>
    /// Stamina skill modification to reduce the stamina usage of a jump 
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    private static IEnumerable<CodeInstruction> Jump_performedTransplier(IEnumerable<CodeInstruction> instructions) 
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        return Stamina.PlayerJumpStaminaOpCode(codes);
    }

    /// <summary>
    /// Jump and Stamina skill modications
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        codes = JumpHeight.PlayerJumpOpCode(codes);
        codes = SprintSpeed.PlayerSprintSpeedOpCode(codes);

        return codes;
    }

    /// <summary>
    /// Skip over the base game HP regen by changing the comparison of
    /// `if (health < 20)`
    /// to
    /// `if (health < health)`
    /// which is ALWAYS false
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        codes = HPRegen.DisableBaseGameHPRegen(codes);
        codes = Stamina.PlayerSprintTimeOpCode(codes);

        return codes;
    }
}
