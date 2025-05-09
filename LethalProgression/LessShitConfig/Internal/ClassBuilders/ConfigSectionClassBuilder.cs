﻿using BepInEx.Configuration;
using LethalProgression.LessShitConfig.Sources;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalProgression.LessShitConfig.Internal.ClassBuilders
{
    public abstract class ConfigSectionClassBuilder
    {
        protected static readonly MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;
        protected static readonly MethodInfo AbstractConfigBase_GetDefinitionMethod = typeof(AbstractConfigBase).GetMethod(
            "GetDefinition",
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(string) },
            null
        );

        protected readonly TypeBuilder typeBuilder;
        protected readonly ILGenerator constructorGenerator;

        protected readonly ConfigSectionData sectionData;

        protected abstract Type ConfigBaseType { get; }

        protected abstract ConstructorInfo BaseConstructor { get; }

        public ConfigSectionClassBuilder(ConfigSectionData section)
        {
            sectionData = section;
            var name = $"DynamicConfigClass_{ConfigBaseType.Name}_{section.Name}"; // Used for assembly, module and type.

            // Create an assembly and module to hold our class)
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(name);

            // Now we can start with defining the type for our class
            typeBuilder = module.DefineType(
                $"{ConfigBaseType.Name}_{section.Name}",
                TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit,
                ConfigBaseType,
                new Type[] { section.InterfaceType }
            );

            Type[] constructorParameters = BaseConstructor.GetParameters().Select(param => param.ParameterType).ToArray();

            // We will need a constructor. Let's START creating this.
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                constructorParameters
            );
            constructorGenerator = constructor.GetILGenerator();

            // We need to call the base constructor. We do this in a seperate method to make subclasses able to have different constructor arguments.
            AppendBaseConstructorOpcodes();

            // We leave the constructor opcodes unfinished at this point. We'll add the ending Ret (return) opcode when we build the class.
            // This is because we need to append to the constructor for each added.

        }

        protected abstract void AppendBaseConstructorOpcodes();

        public void AddEntry(ConfigEntryData entry)
        {
            var entryProp = entry.Property;
            var entryType = entryProp.PropertyType;

            // We first create a field to hold the definition for this entry.
            var definitionFieldName = $"Definition_{entryProp.Name}";
            FieldBuilder definitionField = typeBuilder.DefineField(definitionFieldName, typeof(ConfigDefinition), FieldAttributes.Private);

            MethodInfo ConfigBaseType_GetValueTMethod = ConfigBaseType.GetMethods().First(m => m.Name == "GetValue" && m.IsGenericMethod);

            // The field has no value set yet. Let's append that to our constructor.
            // The `AbstractConfigBase` class has a helpful method for constructing `ConfigDefinition` instances to makes our life easier.
            // Let's just call that. The method signature is: `ConfigDefinition GetDefinition(string)`
            #region EMIT: this.<definitionField> = this.GetDefinition(<entry.Definition.Key>);
            // > Push `this` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            
            // > Push `this` on to the stack.
            constructorGenerator.Emit(OpCodes.Ldarg_0);

            // > Push the value of `entry.Definition.Key` on to the stack.
            //   (This is the value of the current scope, not the scope of the generated constructor when it is executed)
            constructorGenerator.Emit(OpCodes.Ldstr, entry.Definition.Key);

            // > Call the `GetDefinition(string)` method.
            //   This pops the two values that we just pushed back off of the stack.
            //   The `ConfigDefinition` instance that is returned is pushed on to the stack.
            constructorGenerator.Emit(OpCodes.Callvirt, AbstractConfigBase_GetDefinitionMethod);

            // > Pop the `ConfigDefinition` and `this` off the stack and store `ConfigDefinition` in the field of `this`.
            constructorGenerator.Emit(OpCodes.Stfld, definitionField);
            #endregion

            // The getter of a property is actually just a method internally. Let's create that method
            var propertyGetName = $"get_{entryProp.Name}";
            var propertyGetMethod = typeBuilder.DefineMethod(propertyGetName, propertyMethodAttributes, entryType, Type.EmptyTypes);
            var propertyGetGenerator = propertyGetMethod.GetILGenerator();

            #region EMIT: return this.GetValue<T>(this.<definitionField>)
            //                   AAAA          B  CCCC

            // > Push `this` on to the stack (A)
            propertyGetGenerator.Emit(OpCodes.Ldarg_0);
            // > Push `this` on to the stack (C)
            propertyGetGenerator.Emit(OpCodes.Ldarg_0);

            // > Get the value of our field.
            //   This pops `this` (C) off the stack and pushes the field value (D) on to the stack.
            propertyGetGenerator.Emit(OpCodes.Ldfld, definitionField);

            // > Call the `GetValue<T>(ConfigDefinition)` method.
            //   This pops `this` (A) and our field value (D) off the stack.
            //   The resulting value that is returned is pushed on to the stack (E).
            propertyGetGenerator.Emit(OpCodes.Callvirt, ConfigBaseType_GetValueTMethod.MakeGenericMethod(entryType));

            // Finally we return. The value returned by our getting is the value we have left on the top of the stack (E)
            propertyGetGenerator.Emit(OpCodes.Ret);
            #endregion

            // Now we actually define our new property and configure the getter to use our method.
            var property = typeBuilder.DefineProperty(entryProp.Name, PropertyAttributes.None, entryType, Type.EmptyTypes);
            property.SetGetMethod(propertyGetMethod);
        }

        public AbstractConfigBase Build()
        {
            // First we must finish generating the constructor by adding a Ret (return) opcode
            constructorGenerator.Emit(OpCodes.Ret);

            // Now we can build the class! Woo!
            Type type = typeBuilder.CreateType();
            
            // Now let's construct an instance of it and return it! We do this in a seperate method to make subclasses able to have different constructor arguments.
            return ConstructGeneratedType(type);
        }

        protected abstract AbstractConfigBase ConstructGeneratedType(Type type);
    }
}
