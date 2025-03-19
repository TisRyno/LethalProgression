using System.Collections.Generic;
using BepInEx.Configuration;

namespace LethalProgression.Config;

internal class ModConfig
{
    public IDictionary<string, string> hostConfig = new Dictionary<string, string>();

    public ModConfig(ConfigFile config)
    {
    }
}