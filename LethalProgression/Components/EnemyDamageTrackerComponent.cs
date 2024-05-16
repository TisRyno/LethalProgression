using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalProgression.Components
{
    internal class EnemyDamageTrackerComponent : MonoBehaviour
    {
        List<PlayerControllerB> playersWhoHarmedEnemy = new List<PlayerControllerB>();
    }
}