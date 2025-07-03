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
    [ConfigDescription("**Warning** - Making this multiplier too high will make you almost never lose HP. Multiplier x Max Level should equal 1 at most. (1hp every 1 second)")]
    [ConfigDefault(0.05f)]
    float multiplier { get; }
}