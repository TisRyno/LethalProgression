namespace LethalProgression.Network;

internal struct PlayerJumpHeightData
{
    public ulong clientId;

    public int jumpSkillValue;

    public PlayerJumpHeightData(ulong clientId, int jumpSkillValue)
    {
        this.clientId = clientId;
        this.jumpSkillValue = jumpSkillValue;
    }
}