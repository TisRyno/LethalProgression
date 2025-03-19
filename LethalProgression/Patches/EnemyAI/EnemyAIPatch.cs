using System.Collections.Generic;
using HarmonyLib;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class EnemyAIPatch
{
    private static Dictionary<string, int> _enemyReward = new Dictionary<string, int>()
    {
        { "HoarderBug (EnemyType)" , 30 },
        { "BaboonBird (EnemyType)", 15},
        { "MouthDog (EnemyType)", 200},
        { "Centipede (EnemyType)", 30 },
        { "Flowerman (EnemyType)", 200 },
        { "SandSpider (EnemyType)", 50 },
        { "Crawler (EnemyType)", 50 },
        { "Puffer (EnemyType)", 15 },
    };

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), "KillEnemy")]
    private static void CalculateXPForEnemyDeath(EnemyAI __instance)
    {
        // Only trigger on host
        if (!GameNetworkManager.Instance.isHostingGame)
            return;

        string enemyType = __instance.enemyType.ToString();
        LethalPlugin.Log.LogInfo("Enemy type: " + enemyType);
        
        // Give XP for the amount of money this scrap costs.
        int enemyReward = 30;
        if (_enemyReward.ContainsKey(enemyType))
        {
            enemyReward = _enemyReward[enemyType];
        }

        LP_NetworkManager.xpInstance.updateTeamXPClientMessage.SendServer(enemyReward);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), "HitEnemyServerRpc")]
    private static void HitEnemyTrigger(EnemyAI __instance, int force, int playerWhoHit)
    {
        LethalPlugin.Log.LogInfo($"Player {playerWhoHit} hit enemy {__instance.GetType()} with force {force}");
        // __instance.GetInstanceID();

        // rpc the instance id + player id

        // on server add or get the tracker
        // foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies)
        // {
        //     if (enemy.GetInstanceID() == __instance.GetInstanceID()) {
        //         enemy.gameObject.AddComponent(typeof(EnemyDamageTrackerComponent));
        //     }
        // }

        // todo: Set a component on the Server instance of Enemy which informs us of
        /////////
        /// TODO
        /// - Send Server RPC with enemy identifier and player identifier
        /// - Set a component on the EnemyAI that says who hit the enemy
        /// - On Death check for component on the Server
    }
}