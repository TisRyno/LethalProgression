using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Battery Life")]
interface IBatteryLifeConfig
{
    [ConfigName("Battery Life Enabled")]
    [ConfigDescription("Enable the Battery Life skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Battery Life Max Level")]
    [ConfigDescription("Maximum level for the battery life.")]
    [ConfigDefault(99999)]
    int maxLevel { get; }

    [ConfigName("Battery Life Multiplier")]
    [ConfigDescription("How much does the battery life skill increase per level?")]
    [ConfigDefault(5f)]
    float multiplier { get; }
}