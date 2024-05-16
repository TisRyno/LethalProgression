using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Jump Height")]
interface IJumpHeightConfig
{
    [ConfigName("Jump Height Enabled")]
    [ConfigDescription("Enable the Jump Height skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Jump Height Max Level")]
    [ConfigDescription("Maximum level for the jump height.")]
    [ConfigDefault(99999)]
    int maxLevel { get; }

    [ConfigName("Jump Height Multiplier")]
    [ConfigDescription("How much does the jump height skill increase per level?")]
    [ConfigDefault(3f)]
    float multiplier { get; }
}