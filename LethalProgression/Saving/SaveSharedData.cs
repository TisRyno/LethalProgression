namespace LethalProgression.Saving;

internal struct SaveSharedData
{
    public int xp;
    public int level;
    public int quota;

    public SaveSharedData(int xp, int level, int quota)
    {
        this.xp = xp;
        this.level = level;
        this.quota = quota;
    }
}
