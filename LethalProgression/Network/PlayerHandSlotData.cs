namespace LethalProgression.Network;

internal struct PlayerHandSlotData
{
    public ulong clientId;
    public int additionalSlots;

    public PlayerHandSlotData(ulong clientId, int additionalSlots)
    {
        this.clientId = clientId;
        this.additionalSlots = additionalSlots;
    }
}