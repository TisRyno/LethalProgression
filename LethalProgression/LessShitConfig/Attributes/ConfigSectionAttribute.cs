using System;

namespace LethalProgression.LessShitConfig.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    class ConfigSectionAttribute : Attribute
    {
        public readonly string section;

        public ConfigSectionAttribute(string section)
        {
            this.section = section;
        }
    }
}
