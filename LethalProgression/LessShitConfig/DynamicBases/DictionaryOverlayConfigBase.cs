using BepInEx.Configuration;
using System.Collections.Generic;

namespace LethalProgression.LessShitConfig.Sources
{
    public class DictionaryOverlayConfigBase
    {
        private readonly AbstractConfigBase baseConfig;
        private readonly IDictionary<ConfigDefinition, object> overlay;

        public DictionaryOverlayConfigBase(AbstractConfigBase baseConfig, IDictionary<ConfigDefinition, object> overlay)
        {
            this.baseConfig = baseConfig;
            this.overlay = overlay;
        }

        public object GetValue(ConfigDefinition definition)
        {
            if (overlay.TryGetValue(definition, out object value))
                return value;
            return baseConfig.GetValue(definition);
        }

        public T GetValue<T>(ConfigDefinition definition)
        {
            if (overlay.TryGetValue(definition, out object value))
                return (T) value;
            return baseConfig.GetValue<T>(definition);
        }
    }
}
