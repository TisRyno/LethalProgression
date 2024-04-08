using LethalProgression.Saving;

namespace LethalProgression.Network
{
    internal class PlayerHandSlotData
    {
        public ulong clientId { get; set; }
        public int additionalSlots { get; set; }

        public PlayerHandSlotData(ulong clientId, int additionalSlots)
        {
            this.clientId = clientId;
            this.additionalSlots = additionalSlots;
        }
    }
}