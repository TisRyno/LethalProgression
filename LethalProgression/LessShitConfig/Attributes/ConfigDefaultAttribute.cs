using System;

namespace LethalProgression.LessShitConfig.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigDefaultAttribute : Attribute
    {
        public object Value;
        public ConfigDefaultAttribute(object value)
        {
            Value = value;
        }
    }
}
