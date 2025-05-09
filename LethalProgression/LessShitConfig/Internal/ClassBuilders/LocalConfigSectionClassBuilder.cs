﻿using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Sources;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalProgression.LessShitConfig.Internal.ClassBuilders
{
    public class LocalConfigSectionClassBuilder : ConfigSectionClassBuilder
    {
        private readonly ConfigFile configFile;

        protected override Type ConfigBaseType => typeof(LocalConfigBase);
        protected override ConstructorInfo BaseConstructor => ConfigBaseType.GetConstructor(new Type[] { typeof(string), typeof(ConfigFile) });

        public LocalConfigSectionClassBuilder(ConfigSectionData section, ConfigFile config) : base(section)
        {
            configFile = config;
        }

        protected override void AppendBaseConstructorOpcodes()
        {
            // The base constructor is LocalConfigBase(string sectionName, ConfigFile config)
            #region EMIT: base(sectionName, config)
            // Push `this` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            // Push `sectionName` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            // Push `config` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            // Call the base constructor (thus popping all three values back off the stack)
            constructorGenerator.Emit(OpCodes.Call, BaseConstructor);
            #endregion
        }

        protected override AbstractConfigBase ConstructGeneratedType(Type type)
        {
            return (AbstractConfigBase) Activator.CreateInstance(type, new object[] { sectionData.Name, configFile });
        }

        public new LocalConfigBase Build() => (LocalConfigBase) base.Build();
    }
}