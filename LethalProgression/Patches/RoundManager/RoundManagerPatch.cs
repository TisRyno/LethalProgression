using System.Collections.Generic;
using HarmonyLib;
using LethalProgression.Skills.Upgrades;
using Unity.Netcode;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class RoundManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
    [HarmonyPriority(Priority.Last)]
    public static void OnWaitForScrapToSpawnToSync(ref NetworkObjectReference[] spawnedScrap, ref int[] scrapValues)
    {
        List<int> newScrapValues = new List<int>();

        for(int i = 0; i < scrapValues.Length; i++)
        {
            newScrapValues.Add(LootValue.GetNewScrapValueMultiplier(scrapValues[i]));
        }

        scrapValues = newScrapValues.ToArray();
    }
}