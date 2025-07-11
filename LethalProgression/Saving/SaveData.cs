﻿using LethalProgression.Skills;
using System.Collections.Generic;

namespace LethalProgression.Saving;

internal struct SaveData
{
    public ulong steamId;
    public int skillPoints;
    public Dictionary<UpgradeType, int> skillAllocation = new Dictionary<UpgradeType, int>();

    public SaveData(ulong steamId, int skillPoints)
    {
        this.steamId = steamId;
        this.skillPoints = skillPoints;
    }

    public override readonly string ToString()
    {
        string collatedSkills = "";

        foreach (UpgradeType type in skillAllocation.Keys)
        {
            collatedSkills += $" | {type}: {skillAllocation[type]}";
        }

        return $"skillPoints: {skillPoints} | Stats {collatedSkills}";
    }
}
