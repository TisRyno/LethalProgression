using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Loot Value")]
interface ILootValueConfig
{
    [ConfigName("Loot Value Enabled")]
    [ConfigDescription("Enable the Loot Value skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Loot Value Max Level")]
    [ConfigDescription("Maximum level for the loot value.")]
    [ConfigDefault(250)]
    int maxLevel { get; }

    [ConfigName("Loot Value Multiplier")]
    [ConfigDescription("How much does the loot value skill increase per level?")]
    [ConfigDefault(0.1f)]
    float multiplier { get; }
}