using HarmonyLib;
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

    /// <summary>
    /// Change the clamping for damage
    /// `health = Mathf.Clamp(health - damageNumber, 0, 100)`
    /// to
    /// `health = Mathf.Clamp(health - damageNumber, 0, <getNewMaxHP>)`
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    static IEnumerable<CodeInstruction> DamagePlayerTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        codes = MaxHP.UncapMaxHealth(codes);

        return codes;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    private static void HPRegenUpdate(PlayerControllerB __instance)
    {
        if (!__instance.IsOwner)
            return;

        if (__instance.IsServer && !__instance.isHostPlayerObject) 
            return;

        // Ignore if they are dead or not player controlled
        if (!__instance.isPlayerControlled || __instance.isPlayerDead)
            return;

        // Get max health based off of MaxHP Skill
        int maxHealth = MaxHP.GetNewMaxHealth(100);

        // If their current health exceeds their max health reset it
        if (__instance.health > maxHealth)
        {
            __instance.health = maxHealth;

            HUDManager.Instance.UpdateHealthUI(__instance.health, false);
        }

        if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HPRegen))
            return;


        if (__instance.health < 20 && __instance.healthRegenerateTimer > 1f)
            __instance.healthRegenerateTimer = 1f;

        if (__instance.healthRegenerateTimer > 0f)
            {
                __instance.healthRegenerateTimer -= Time.deltaTime;
                return;
            }

        if (__instance.health >= 20)
            __instance.MakeCriticallyInjured(false);

        if (__instance.health >= maxHealth)
            return;


        Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.HPRegen];
        // Then turn that into seconds. So, if hps is 0.5, then it will take 2 seconds to regen 1 health.
        float newRegenerateTimer = 1f / skill.GetTrueValue(); // 1 / (0.05 * 5 = 0.25) = 4

        if (__instance.health < 20 && newRegenerateTimer > 1f)
            newRegenerateTimer = 1f;

        __instance.healthRegenerateTimer = newRegenerateTimer;
        __instance.health++;

        HUDManager.Instance.UpdateHealthUI(__instance.health, false);
    }
}
