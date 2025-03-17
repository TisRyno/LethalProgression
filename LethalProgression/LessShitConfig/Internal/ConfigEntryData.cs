using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LethalProgression.LessShitConfig.Internal
{
    public class ConfigEntryData
    {
        public readonly ConfigSectionData Section;
        public readonly PropertyInfo Property;

        public readonly ConfigDefinition Definition;
        public readonly ConfigDescription Description;
        public readonly object DefaultValue;

        public ConfigEntryData(ConfigSectionData section, PropertyInfo property)
        {
            if (!TomlTypeConverter.CanConvert(property.PropertyType))
                throw new ConfigRegistrationException($"There is no TOML converter registered for {property.PropertyType.FullName} entries: {section.InterfaceType.FullName}");

            if (property.CanWrite)
                throw new ConfigRegistrationException($"Config section entries must not have property setter: ({property.Name} in {section.InterfaceType.FullName})");
            if (!property.CanRead)
                throw new ConfigRegistrationException($"Config section entries must have property getter: ({property.Name} in {section.InterfaceType.FullName})");

            var name = property.GetCustomAttribute<ConfigNameAttribute>();
            if (name == null)
                throw new ConfigRegistrationException($"Config section must have a named defined with Configname ({property.Name} in {section.InterfaceType.FullName})");

            var descriptionAttr = property.GetCustomAttribute<ConfigDescriptionAttribute>();
            var defaultValueAttr = property.GetCustomAttribute<ConfigDefaultAttribute>(true);

            LethalPlugin.Log.LogInfo($"Register Property: {section.Name} {name.name}");

            Section = section;
            Property = property;
            Definition = new ConfigDefinition(section.Name, name.name);
            Description = descriptionAttr?.Description;
            DefaultValue = ResolveDefaultValue(defaultValueAttr);
        }

        private object ResolveDefaultValue(ConfigDefaultAttribute defaultValue)
        {
            if (defaultValue != null)
                return defaultValue.Value;
            return Property.PropertyType.IsValueType ? Activator.CreateInstance(Property.PropertyType) : null;
        }

        public void Bind(ConfigFile file)
        {
            var bindMethod = typeof(ConfigFile).GetMethod(
                "Bind",
                new Type[] {
                    typeof(ConfigDefinition),
                    Type.MakeGenericMethodParameter(0),
                    typeof(ConfigDescription)
                }
            ).MakeGenericMethod(Property.PropertyType);
            bindMethod.Invoke(file, new List<object> { Definition, DefaultValue, Description }.ToArray());
        }
    }

/*    public class ConfigEntryData<T> : ConfigEntryData
    {
        public new readonly T DefaultValue;

        public ConfigEntryData(ConfigSectionData section, PropertyInfo property) : base(section, property)
        {
            DefaultValue = (T) base.DefaultValue;
        }

        public override void Bind(ConfigFile file) => file.Bind(Definition, DefaultValue, Description);
    }*/
}
