using System;
using LethalProgression.Saving;

namespace LethalProgression.Skills;

internal abstract class Skill
{
    protected readonly Action<int> _callback;
    public int CurrentLevel { get; protected set; }
    public abstract string ShortName { get; }
    public abstract string Name { get; }
    public abstract string Attribute { get; }
    public abstract string Description { get; }
    public abstract UpgradeType UpgradeType { get; }
    public abstract int Cost { get; }
    public abstract int MaxLevel { get; }
    public abstract float Multiplier { get; }
    public abstract bool IsTeamShared { get; }

    public Skill(Action<int> callback = null)
    {
        CurrentLevel = 0;
        _callback = callback;
    }

    public float GetTrueValue()
    {
        return Multiplier * CurrentLevel;
    }

    public void SetLevel(int newLevel, bool triggerHostProfileSave = true)
    {
        int oldLevel = CurrentLevel;

        CurrentLevel = newLevel;
        // level is number of changes
        _callback?.Invoke(newLevel - oldLevel);

        if (triggerHostProfileSave)
            ES3SaveManager.TriggerHostProfileSave();
    }

    public void AddLevel(int change)
    {
        CurrentLevel += change;

        _callback?.Invoke(change);

        ES3SaveManager.TriggerHostProfileSave();
    }
}
