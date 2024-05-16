using BepInEx.Configuration;
using System;

namespace LethalProgression.LessShitConfig.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class ConfigDescriptionAttribute : Attribute
    {
        public readonly ConfigDescription Description;

        public ConfigDescriptionAttribute(string description)
        {
            Description = new ConfigDescription(description);
        }
    }
}
