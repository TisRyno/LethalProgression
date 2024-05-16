using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Hand Slots")]
interface IHandSlotsConfig
{
    [ConfigName("Hand Slots Enabled")]
    [ConfigDescription("Enable the Hand Slots skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Hand Slots Max Level")]
    [ConfigDescription("Maximum level for the hand slots.")]
    [ConfigDefault(30)]
    int maxLevel { get; }

    [ConfigName("Hand Slots Multiplier")]
    [ConfigDescription("How much does the hand slots skill increase per level?")]
    [ConfigDefault(10f)]
    float multiplier { get; }
}