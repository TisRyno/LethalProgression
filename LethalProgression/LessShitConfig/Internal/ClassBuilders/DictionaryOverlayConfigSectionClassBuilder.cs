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

        protected override ConstructorInfo BaseConstructor => ConfigBaseType.GetConstructor(new Type[] { typeof(string), typeof(AbstractConfigBase), typeof(Dictionary<ConfigDefinition, object>) });

        public DictionaryOverlayConfigSectionClassBuilder(ConfigSectionData section, AbstractConfigBase baseConfig, IDictionary<ConfigDefinition, object> overlay) : base(section)
        {
            this.baseConfig = baseConfig;
            this.overlay = overlay;
        }

        protected override void AppendBaseConstructorOpcodes()
        {
            // The base constructor is DictionaryOverlayConfigBase(string sectionName, AbstractConfigBase baseConfig, DictionaryOverlayConfigSectionClassBuilder overlay)
            #region EMIT: base(sectionName, baseConfig, overlay)
            // Push `this` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            // Push `sectionName` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            // Push `baseConfig` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            // Push `overlay` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_3);
            // Call the base constructor (thus popping all four values back off the stack)
            constructorGenerator.Emit(OpCodes.Call, BaseConstructor);
            #endregion
        }

        protected override AbstractConfigBase ConstructGeneratedType(Type type)
        {
            return (AbstractConfigBase) Activator.CreateInstance(type, new object[] { sectionData.Name, baseConfig, overlay });
        }
        
        public new DictionaryOverlayConfigBase Build() => (DictionaryOverlayConfigBase) base.Build();
    }
}