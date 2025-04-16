using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("General")]
interface IGeneralConfig
{
    [ConfigName("Person Multiplier")]
    [ConfigDescription("How much does XP cost to level up go up per person?")]
    [ConfigDefault(35)]
    int personMultiplier { get; }

    [ConfigName("Quota Multiplier")]
    [ConfigDescription("How much more XP does it cost to level up go up per quota? (Percent)")]
    [ConfigDefault(30)]
    int quotaMultiplier { get; }

    [ConfigName("Starting Skill Points")]
    [ConfigDescription("The starting skill points upon new lobby, or after ejection.")]
    [ConfigDefault(5)]
    int startSkillPoints { get; }

    [ConfigName("XP Minimum")]
    [ConfigDescription("Minimum XP to level up.")]
    [ConfigDefault(40)]
    int minXP { get; }

    [ConfigName("XP Maximum")]
    [ConfigDescription("Maximum XP to level up.")]
    [ConfigDefault(750)]
    int maxXP { get; }

    [ConfigName("Unspec in Ship Only")]
    [ConfigDescription("Disallows unspecing stats if you're not currently on the ship.")]
    [ConfigDefault(false)]
    bool enableUnspecInShip { get; }

    [ConfigName("Unspec in Orbit Only")]
    [ConfigDescription("Disallows unspecing stats if you're not currently in orbit.")]
    [ConfigDefault(true)]
    bool enableUnspecInOrbit { get; }

    [ConfigName("Disable Unspec")]
    [ConfigDescription("Disallows unspecing altogether.")]
    [ConfigDefault(false)]
    bool disableUnspec { get; }

    [ConfigName("Keep progress")]
    [ConfigDescription("Keep your progress after being fired")]
    [ConfigDefault(false)]
    bool keepProgress { get; }
}