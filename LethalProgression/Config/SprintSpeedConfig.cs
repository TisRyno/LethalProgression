using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Sprint Speed")]
interface ISprintSpeedConfig
{
    [ConfigName("Sprint Speed Enabled")]
    [ConfigDescription("Enable the Sprint Speed skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Sprint Speed Max Level")]
    [ConfigDescription("Maximum level for the sprint speed.")]
    [ConfigDefault(99999)]
    int maxLevel { get; }

    [ConfigName("Sprint Speed Multiplier")]
    [ConfigDescription("How much does the sprint speed skill increase per level?")]
    [ConfigDefault(0.75f)]
    float multiplier { get; }
}