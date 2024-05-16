using System;

namespace LethalProgression.LessShitConfig.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class ConfigNameAttribute : Attribute
    {
        public readonly string name;

        public ConfigNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
