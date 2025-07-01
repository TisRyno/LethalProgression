using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("Ship Hangar Door")]
interface IShipHangarDoorConfig
{
    [ConfigName("Ship Door Battery Enabled")]
    [ConfigDescription("Enable the Ship Door Battery skill?")]
    [ConfigDefault(true)]
    bool isEnabled { get; }

    [ConfigName("Ship Door Battery Life Max Level")]
    [ConfigDescription("Maximum level for the Ship Door Battery life.")]
    [ConfigDefault(4)]
    int maxLevel { get; }

    [ConfigName("Ship Door Battery Life Multiplier")]
    [ConfigDescription("How much does the Ship Door Battery life skill increase per level?")]
    [ConfigDefault(25f)]
    float multiplier { get; }
}