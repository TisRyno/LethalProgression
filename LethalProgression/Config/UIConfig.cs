using LethalProgression.LessShitConfig.Attributes;

namespace LethalProgression.Config;

[ConfigSection("UI")]
interface IUIConfig
{
    [ConfigName("Disable Level Up Notification")]
    [ConfigDescription("If you prefer to not have a notification when levelling up, set this is true")]
    [ConfigDefault(false)]
    bool levelUpEnabled { get; }

    [ConfigName("Level Up Notification Position")]
    [ConfigDescription("Options are: 1 = Top, 0 = Bottom")]
    [ConfigDefault(0)]
    int levelUpPosition { get; }

    [ConfigName("Level Up Notification Scale")]
    [ConfigDescription("How large the notification should display, 1 = 100%, 0 = 0% (hidden)")]
    [ConfigDefault(0.5f)]
    float levelUpScale { get; }
}