using BepInEx.Configuration;
using System.Collections.Generic;

namespace LethalProgression.LessShitConfig.Sources
{
    public abstract class DictionaryOverlayConfigBase : AbstractConfigBase
    {
        private readonly AbstractConfigBase baseConfig;
        private readonly IDictionary<ConfigDefinition, object> overlay;

        public DictionaryOverlayConfigBase(string sectionName, AbstractConfigBase baseConfig, IDictionary<ConfigDefinition, object> overlay) : base(sectionName)
        {
            this.baseConfig = baseConfig;
            this.overlay = overlay;
        }

        public override object GetValue(ConfigDefinition definition)
        {
            if (overlay.TryGetValue(definition, out object value))
                return value;
            return baseConfig.GetValue(definition);
        }

        public override T GetValue<T>(ConfigDefinition definition)
        {
            if (overlay.TryGetValue(definition, out object value))
                return (T) value;
            return baseConfig.GetValue<T>(definition);
        }
    }
}
