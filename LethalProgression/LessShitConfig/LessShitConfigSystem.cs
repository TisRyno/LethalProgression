using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Internal;
using LethalProgression.LessShitConfig.Internal.ClassBuilders;
using LethalProgression.LessShitConfig.Sources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LethalProgression.LessShitConfig
{
    /// <summary>
    /// An interface/attribute based wrapper about the BepInE config system
    /// </summary>
    public static class LessShitConfigSystem
    {
        /// <summary>
        /// All registered entries by definition.
        /// </summary>
        private static readonly IDictionary<ConfigDefinition, ConfigEntryData> entries = new Dictionary<ConfigDefinition, ConfigEntryData>();

        private static readonly IDictionary<Type, ConfigSectionData> sections = new Dictionary<Type, ConfigSectionData>();

        private static readonly IDictionary<Type, LocalConfigBase> localSections = new Dictionary<Type, LocalConfigBase>();
        private static readonly IDictionary<Type, DictionaryOverlayConfigBase> activeSections = new Dictionary<Type, DictionaryOverlayConfigBase>();

        private static readonly IDictionary<ConfigDefinition, object> hostConfigOverlay = new Dictionary<ConfigDefinition, object>();

        public static object ActiveConfig { get; internal set; }

        /// <summary>
        /// Registers a new config section interface.
        /// </summary>
        /// <typeparam name="T">The config section interface to register.</typeparam>
        public static void RegisterSection<T>()
        {
            var type = typeof(T);
            
            LethalPlugin.Log.LogDebug($"RegisterSection: {type.FullName}");
            
            if (sections.ContainsKey(type))
                throw new ConfigRegistrationException($"The config section defintion has already been registered: {type.FullName}");

            var section = new ConfigSectionData(type);
            sections[type] = section;

            foreach (var entry in section.Entries)
            {
                entries[entry.Definition] = entry;
            }
        }

        /// <summary>
        /// Completes the setup of the config configuration system.
        /// </summary>
        /// <param name="configFile">The BepInEx config file instance of the plugin.</param>
        public static void Bake(ConfigFile configFile)
        {
            if (configFile == null)
                throw new ArgumentNullException("config");

            if (localSections.Count > 0)
                throw new InvalidOperationException("Calling Bind multiple times is not allowed.");

            foreach (ConfigEntryData entry in entries.Values)
            {
                entry.Bind(configFile);
            }

            foreach (KeyValuePair<Type, ConfigSectionData> section in sections)
            {
                LocalConfigSectionClassBuilder sectionBuilder = new LocalConfigSectionClassBuilder(section.Value, configFile);

                foreach (ConfigEntryData entry in section.Value.Entries) {
                    sectionBuilder.AddEntry(entry);
                }

                LocalConfigBase localSection = sectionBuilder.Build();
                localSections[section.Key] = localSection;

                DictionaryOverlayConfigSectionClassBuilder overlayBuilder = new DictionaryOverlayConfigSectionClassBuilder(section.Value, localSection, hostConfigOverlay);

                foreach (ConfigEntryData entry in section.Value.Entries) {
                    overlayBuilder.AddEntry(entry);
                }

                activeSections[section.Key] = overlayBuilder.Build();
            }
        }


        internal static T GetActive<T>() => (T)(object)activeSections[typeof(T)];
        
        internal static string SerializeLocalConfigs()
        {
            IDictionary<string, IDictionary<string, string>> entryDict = new Dictionary<string, IDictionary<string, string>>();
            foreach (var entry in entries.Values)
            {
                var section = entry.Definition.Section;
                var key = entry.Definition.Key;
                if (!entryDict.ContainsKey(section))
                {
                    entryDict[section] = new Dictionary<string, string>();
                }

                object value = localSections[entry.Section.InterfaceType].GetValue(entry.Definition);
                entryDict[section][key] = TomlTypeConverter.ConvertToString(value, entry.Property.PropertyType);
            }
            return JsonConvert.SerializeObject(entryDict);
        }
        internal static void ApplyHostConfigs(string configString)
        {
            IDictionary<string, IDictionary<string, string>> entryDict = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(configString);
            foreach (var section in entryDict)
            {
                foreach (var entry in section.Value)
                {
                    var definition = new ConfigDefinition(section.Key, entry.Key);
                    var type = entries[definition].Property.PropertyType;
                    hostConfigOverlay[definition] = TomlTypeConverter.ConvertToValue(entry.Value, type);
                    LethalPlugin.Log.LogInfo($"Applied host config override: {definition} = {entry.Value}");
                }
            }
        }
        internal static void ClearHostConfigs()
        {
            hostConfigOverlay.Clear();
            LethalPlugin.Log.LogInfo($"Cleared host config overrides");
        }
    }
}
