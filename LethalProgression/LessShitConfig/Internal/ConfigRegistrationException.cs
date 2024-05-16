using System;

namespace LethalProgression.LessShitConfig.Internal
{
    public class ConfigRegistrationException : Exception
    {
        public ConfigRegistrationException(string message) : base(message) { }
    }
}
