using System;
using System.Collections.Generic;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Reflection.Injections
{
    public static class MemberInjector
    {
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        public static bool InjectField(TypeValuePair target,
                                       string fieldName,
                                       object? value,
                                       InjectionOptions options = InjectionOptions.Default)
        {
            if (fieldName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(fieldName), fieldName);

            DeconstructOptions(options, out bool throwIfFailed, out bool cacheField);
            BindingFlags bindingFlags = ResolveBindingFlags(target);
            FieldInfo? field = FindField(target.type, fieldName, bindingFlags);

            if (field is null)
            {
                if (throwIfFailed)
                    throw new MemberNotFoundException(
                        target.type,
                        MemberType.Field,
                        fieldName);

                return false;
            }

            if (!throwIfFailed)
            {
                try
                {
                    SetField(target.value, value, field, cacheField);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            SetField(target.value, value, field, cacheField);
            return true;
        }
        public static bool InjectField(object target,
                                       string fieldName,
                                       object? value,
                                       InjectionOptions options = InjectionOptions.Default)
        {
            return InjectField(new TypeValuePair(target), fieldName, value, options);
        }

        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static bool InjectProperty(TypeValuePair target,
                                          string propName,
                                          object? value,
                                          InjectionOptions options = InjectionOptions.Default)
        {
            if (propName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(propName), propName);

            DeconstructOptions(options, out bool throwIfFailed, out bool cacheProp);
            BindingFlags bindingFlags = ResolveBindingFlags(target);
            PropertyInfo? prop = FindProperty(target.type, propName, bindingFlags);

            if (prop is null)
            {
                if (throwIfFailed)
                    throw new MemberNotFoundException(
                        target.type,
                        MemberType.Field,
                        propName);

                return false;
            }

            ValidateProperty(prop, throwIfFailed);

            if (!throwIfFailed)
            {
                try
                {
                    SetProperty(target.value, value, prop, cacheProp);
                    return true;    
                }
                catch (Exception)
                {
                    return false;
                }
            }

            SetProperty(target.value, value, prop, cacheProp);
            return true;
        }
        public static bool InjectProperty(object target,
                                          string propName,
                                          object? value,
                                          InjectionOptions options = InjectionOptions.Default)
        {
            return InjectProperty(new TypeValuePair(target), propName, value, options);
        }

        private static void SetField(object? target,
                                     object? value,
                                     FieldInfo field,
                                     bool cacheField)
        {
            field.SetValue(target, value);

            if (cacheField)
                field.TryCacheMember();
        }

        private static void ValidateProperty(PropertyInfo prop, bool throwIfFailed)
        {
            if (prop.SetMethod is null && throwIfFailed)
                throw new NullReferenceException($"Cannot find {nameof(prop.SetMethod)}.");
        }

        private static void SetProperty(object? target,
                                        object? value,
                                        PropertyInfo prop,
                                        bool cacheProp)
        {
            prop.SetValue(target, value);

            if (cacheProp)
                prop.TryCacheMember();
        }

        private static FieldInfo? FindField(Type type,
                                            string fieldName,
                                            BindingFlags bindingFlags)
        {
            Queue<Type> types = TypeHelper.CollectBaseTypes(type);
            var loopPredicate = new LoopPredicate(() => types.Count > 0);
            FieldInfo? field = null;
            while (loopPredicate)
            {
                field = types.Dequeue().GetField(fieldName, bindingFlags);

                if (field is not null)
                    break;
            }

            return field;
        }

        private static void DeconstructOptions(InjectionOptions options,
                                               out bool throwIfFailed,
                                               out bool cacheMember)
        {
            throwIfFailed = options.IsFlagSetted(InjectionOptions.ThrowIfFailed);
            cacheMember = options.IsFlagSetted(InjectionOptions.CacheMember);
        }

        private static BindingFlags ResolveBindingFlags(TypeValuePair target)
        {
            BindingFlags bindingFlags = target.value.IsNull()
                ?
                BindingFlagsDefault.StaticAll
                :
                BindingFlagsDefault.InstanceAll;

            bindingFlags |= BindingFlags.DeclaredOnly;

            return bindingFlags;
        }

        private static PropertyInfo? FindProperty(Type type,
                                                 string propName,
                                                 BindingFlags bindingFlags)
        {
            Queue<Type> types = TypeHelper.CollectBaseTypes(type);
            var loopPredicate = new LoopPredicate(() => types.Count > 0);

            PropertyInfo? lastFoundProp = null!;
            PropertyInfo? prop = null;
            while (loopPredicate)
            {
                prop = types.Dequeue().GetProperty(propName, bindingFlags);

                if (prop is not null)
                {
                    lastFoundProp = prop;

                    if (prop.SetMethod is not null)
                        break;
                }
            }

            prop ??= lastFoundProp;

            return prop;
        }
    }
}
