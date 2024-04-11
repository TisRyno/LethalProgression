using GameNetcodeStuff;
using LethalProgression.Network;
using Newtonsoft.Json;

namespace LethalProgression.Skills
{
    internal class JumpHeight
    {
        public static void JumpHeightUpdate(int updatedValue)
        {
            if (!LP_NetworkManager.xpInstance.skillList.IsSkillListValid())
                return;

            if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.JumpHeight))
                return;

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.JumpHeight];

            PlayerJumpHeightData networkData = new PlayerJumpHeightData()
            {
                clientId = localPlayer.playerClientId,
                jumpSkillValue = skill.GetLevel()
            };

            LethalPlugin.Log.LogInfo($"Jump skill now {updatedValue}, sending to Server");
            
            LP_NetworkManager.xpInstance.updatePlayerJumpForceClientMessage.SendServer(networkData);
        }
    }
}
