using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Strength")]
interface IStrengthConfig
{
    [ConfigName("Strength Enabled")]
    [ConfigDescription("Enable the Strength skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Strength Max Level")]
    [ConfigDescription("Maximum level for the strength.")]
    [ConfigDefault(75)]
    int maxLevel { get; }

    [ConfigName("Strength Multiplier")]
    [ConfigDescription("How much does the strength skill increase per level?")]
    [ConfigDefault(1f)]
    float multiplier { get; }
}