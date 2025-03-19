using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Health")]
interface IHealthRegenConfig
{
    [ConfigName("Health Regen Enabled")]
    [ConfigDescription("Enable the Health Regen skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Health Regen Max Level")]
    [ConfigDescription("Maximum level for the health regen.")]
    [ConfigDefault(20)]
    int maxLevel { get; }

    [ConfigName("Health Regen Multiplier")]
    [ConfigDescription("How much does the health regen skill increase per level?")]
    [ConfigDefault(0.5f)]
    float multiplier { get; }
}