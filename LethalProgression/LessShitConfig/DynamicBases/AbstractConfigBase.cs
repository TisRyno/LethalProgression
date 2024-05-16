using BepInEx.Configuration;

namespace LethalProgression.LessShitConfig.Sources
{
    public abstract class AbstractConfigBase
    {
        private string sectionName;

        protected AbstractConfigBase(string sectionName)
        {
            this.sectionName = sectionName;
        }
        protected ConfigDefinition GetDefinition(string key) => new ConfigDefinition(this.sectionName, key);
        public abstract object GetValue(ConfigDefinition definition);
        public abstract T GetValue<T>(ConfigDefinition definition);
    }
}
