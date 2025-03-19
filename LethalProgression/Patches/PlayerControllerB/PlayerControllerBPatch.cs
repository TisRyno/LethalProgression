﻿using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using System.Collections.Generic;
using LethalProgression.Skills.Upgrades;
using LethalProgression.Skills;
using UnityEngine;

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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    private static void HPRegenUpdate(PlayerControllerB __instance)
    {
        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return;

        if (__instance.healthRegenerateTimer > 0f)
        {
            __instance.healthRegenerateTimer -= Time.deltaTime;
            return;
        }
        
        Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.HPRegen];
        // Then turn that into seconds. So, if hps is 0.5, then it will take 2 seconds to regen 1 health.
        __instance.healthRegenerateTimer = 1f / skill.GetTrueValue(); // 0.05 * 5 = 0.25
        __instance.health++;

        if (__instance.health >= 20)
            __instance.MakeCriticallyInjured(false);

        if (!__instance.IsOwner || (__instance.IsServer && !__instance.isHostPlayerObject))
            return;

        if (!__instance.isPlayerControlled || __instance.health >= 100 || __instance.isPlayerDead)
            return;

        HUDManager.Instance.UpdateHealthUI(__instance.health, false);
    }
}
