using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace LethalProgression.LessShitConfig.Sources
{

    public abstract class LocalConfigBase : AbstractConfigBase
    {
        private readonly ConfigFile configFile;
        public LocalConfigBase(string sectionName, ConfigFile config) : base(sectionName)
        {
            configFile = config;
        }

        public override object GetValue(ConfigDefinition definition)
        {
            IDictionary<ConfigDefinition, ConfigEntryBase> asDict = configFile;
            if (asDict.TryGetValue(definition, out ConfigEntryBase entry))
                return entry.BoxedValue;
            throw new ArgumentException($"The config entry {definition} is not registered.", "definition");
        }

        public override T GetValue<T>(ConfigDefinition definition)
        {
            if (configFile.TryGetEntry(definition, out ConfigEntry<T> entry))
                return entry.Value;
            throw new ArgumentException($"The config entry {definition} is not registered.", "definition");
        }
    }
}
