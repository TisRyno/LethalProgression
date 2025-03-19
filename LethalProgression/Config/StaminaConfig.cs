using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Stamina")]
interface IStaminaConfig
{
    [ConfigName("Stamina Enabled")]
    [ConfigDescription("Enable the Stamina skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Stamina Max Level")]
    [ConfigDescription("Maximum level for the stamina.")]
    [ConfigDefault(99999)]
    int maxLevel { get; }

    [ConfigName("Stamina Multiplier")]
    [ConfigDescription("How much does the stamina skill increase per level?")]
    [ConfigDefault(2f)]
    float multiplier { get; }
}