using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Sources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalProgression.LessShitConfig.Internal.ClassBuilders
{
    public class DictionaryOverlayConfigSectionClassBuilder : ConfigSectionClassBuilder
    {
        private readonly AbstractConfigBase baseConfig;
        private readonly IDictionary<ConfigDefinition, object> overlay;

        protected override Type ConfigBaseType => typeof(DictionaryOverlayConfigBase);

        private ConstructorInfo BaseConstructor => ConfigBaseType.GetConstructor(new List<Type> { typeof(AbstractConfigBase), typeof(Dictionary<ConfigDefinition, object>) }.ToArray());

        public DictionaryOverlayConfigSectionClassBuilder(ConfigSectionData section, AbstractConfigBase baseConfig, IDictionary<ConfigDefinition, object> overlay) : base(section)
        {
            this.baseConfig = baseConfig;
            this.overlay = overlay;
        }

        protected override void AppendBaseConstructorOpcodes()
        {
            // The base constructor is LocalConfigBase(AbstractConfigBase baseConfig, DictionaryOverlayConfigSectionClassBuilder overlay)
            #region EMIT: base(sectionName)
            // Push `this` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            // Push `baseConfig` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            // Push `overlay` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            // Call the base constructor (thus popping all three values back off the stack)
            constructorGenerator.Emit(OpCodes.Call, BaseConstructor);
            #endregion
        }

        protected override AbstractConfigBase ConstructGeneratedType(Type type)
        {
            return (AbstractConfigBase) Activator.CreateInstance(type, new List<object> { baseConfig, overlay }.ToArray());
        }
    }
}