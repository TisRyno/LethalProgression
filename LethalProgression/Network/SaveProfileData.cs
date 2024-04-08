using LethalProgression.Saving;

namespace LethalProgression.Network
{
    internal class SaveProfileData
    {
        public ulong steamId { get; set; }
        public SaveData saveData { get; set; }

        public SaveProfileData(ulong steamId, SaveData saveData)
        {
            this.steamId = steamId;
            this.saveData = saveData;
        }
    }
}