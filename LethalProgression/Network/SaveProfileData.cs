using LethalProgression.Saving;

namespace LethalProgression.Network;

internal struct SaveProfileData
{
    public ulong steamId;
    public SaveData saveData;

    public SaveProfileData(ulong steamId, SaveData saveData)
    {
        this.steamId = steamId;
        this.saveData = saveData;
    }
}
