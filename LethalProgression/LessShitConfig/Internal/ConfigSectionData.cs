using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LethalProgression.LessShitConfig.Internal
{
    public class ConfigSectionData
    {
        public readonly Type InterfaceType;
        public readonly ICollection<ConfigEntryData> Entries;

        public readonly string Name;
        public ConfigDescription Description { get; private set; }

        public ConfigSectionData(Type type)
        {
            if (!type.IsInterface)
                throw new ConfigRegistrationException($"Config section definitions must be interfaces: {type.FullName}");

            var sectionAttr = type.GetCustomAttribute<ConfigSectionAttribute>();
            if (sectionAttr == null)
                throw new ConfigRegistrationException($"Cannot register a config section without a ConfigSection attribute: {type.FullName}");

            Name = sectionAttr.section;

            InterfaceType = type;
            Entries = new List<ConfigEntryData>();

            foreach (var member in type.GetMembers())
            {
                if (member.MemberType != MemberTypes.Property) // TODO: This might conflict with the internal methods for property getters/setters
                {
                    if (member.MemberType == MemberTypes.Method && ((MethodInfo) member).Name.StartsWith("get_"))
                        continue;
                    throw new ConfigRegistrationException($"Non-property members are not allowed in config section definitions: ({member.Name} is {member.MemberType})");
                }

                LethalPlugin.Log.LogDebug($"Register: {type.Name} = {member.Name}");

                var entry = new ConfigEntryData(this, (PropertyInfo)member);
                Entries.Add(entry);
            }
        }
    }
}