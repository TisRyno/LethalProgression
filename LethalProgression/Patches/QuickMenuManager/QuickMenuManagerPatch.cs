﻿using HarmonyLib;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class QuickMenuManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
    private static void OpenQuickMenu_Postfix(QuickMenuManager __instance)
    {
        // Check if menucontainer is active
        if (!__instance.isMenuOpen)
            return;

        LethalPlugin.XPBarGUI.Show();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(QuickMenuManager), "Update")]
    private static void QuickMenuManager_Postfix(QuickMenuManager __instance)
    {
        // If the settings menu or exit game menu is open, we don't want to show the XP bar.
        LethalPlugin.XPBarGUI.Update(__instance.mainButtonsPanel.activeSelf);

        // If the menu is open, close menu.
        if (!LethalPlugin.SkillsGUI.isMenuOpen)
        {
            LethalPlugin.SkillsGUI.CloseSkillMenu();
            return;
        }

        if (bool.Parse(LethalPlugin.ModConfig.hostConfig["Unspec in Ship Only"]) && !bool.Parse(LethalPlugin.ModConfig.hostConfig["Disable Unspec"]))
        {
            // Check if you are in the ship right now
            if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
            {
                LethalPlugin.SkillsGUI.SetUnspec(true);
            }
            else
            {
                LethalPlugin.SkillsGUI.SetUnspec(false);
            }
        }

        if (bool.Parse(LethalPlugin.ModConfig.hostConfig["Unspec in Orbit Only"]))
        {
            // Check if you are in orbit right now
            if (StartOfRound.Instance.inShipPhase)
            {
                LethalPlugin.SkillsGUI.SetUnspec(true);
            }
            else
            {
                LethalPlugin.SkillsGUI.SetUnspec(false);
            }
        }

        if (bool.Parse(LethalPlugin.ModConfig.hostConfig["Disable Unspec"]))
        {
            LethalPlugin.SkillsGUI.SetUnspec(false);
        }

        LethalPlugin.SkillsGUI.OpenSkillMenu();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
    private static void SkillMenuClose()
    {
        LethalPlugin.SkillsGUI.CloseSkillMenu();
    }
}