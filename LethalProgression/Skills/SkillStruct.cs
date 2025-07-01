using System;

namespace LethalProgression.Skills;

interface ISkill
{
    public string ShortName { get; }
    public string Name { get; }
    public string Attribute { get; }
    public string Description { get; }
    public UpgradeType UpgradeType { get; }
    public int Cost { get; }
    public int MaxLevel { get; }
    public float Multiplier { get; }
    public bool IsTeamShared { get; }
    public int CurrentLevel { get; set; }
}