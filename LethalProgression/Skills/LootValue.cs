using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Unity.Netcode;

namespace LethalProgression.Skills
{
    [HarmonyPatch]
    internal class LootValue
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
        private static void AddLootValue()
        {
            if (!LP_NetworkManager.xpInstance.skillList.IsSkillListValid())
                return;

            if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
                return;

            float scrapValueAdded = LP_NetworkManager.xpInstance.teamLootValue.Value / 100;

            try
            {
                RoundManager.Instance.scrapValueMultiplier += scrapValueAdded;
            }
            catch { }

            LethalPlugin.Log.LogDebug($"Added {scrapValueAdded} to scrap value multiplier, resulting in {RoundManager.Instance.scrapValueMultiplier}");
        }

        public static void LootValueUpdate(int change)
        {
            if (!LP_NetworkManager.xpInstance.skillList.IsSkillListValid())
                return;

            if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.Value))
                return;

            LP_NetworkManager.xpInstance.TeamLootValueUpdate(change);
        }
    }
}
