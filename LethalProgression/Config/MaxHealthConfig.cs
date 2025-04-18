using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("MaxHealth")]
interface IMaxHealthConfig
{
    [ConfigName("Max Health Enabled")]
    [ConfigDescription("Enable the Max Health skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Max Health Max Level")]
    [ConfigDescription("Maximum level for the maximum health skill.")]
    [ConfigDefault(40)]
    int maxLevel { get; }

    [ConfigName("Max Health Multiplier")]
    [ConfigDescription("How much does max health skill increase per level?")]
    [ConfigDefault(0.05f)]
    float multiplier { get; }
}