using System;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
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

            bool throwIfFailed = options.IsFlagSetted(InjectionOptions.ThrowIfFailed);
            bool cacheField = options.IsFlagSetted(InjectionOptions.CacheMember);

            FieldInfo? field = target.type.GetField(fieldName, BindingFlagsDefault.All);
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
                    field.SetValue(target, value);

                    if (cacheField)
                        field.TryCacheMember();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            field.SetValue(target, value);

            if (cacheField)
                field.TryCacheMember();

            return true;
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

            bool throwIfFailed = options.IsFlagSetted(InjectionOptions.ThrowIfFailed);
            bool cacheField = options.IsFlagSetted(InjectionOptions.CacheMember);

            PropertyInfo? prop = target.type.GetProperty(propName, BindingFlagsDefault.All);
            if (prop is null)
            {
                if (throwIfFailed)
                    throw new MemberNotFoundException(
                        target.type,
                        MemberType.Field,
                        propName);

                return false;
            }
            if (prop.SetMethod is null)
            {
                if (throwIfFailed)
                    throw new NullReferenceException($"Cannot find {nameof(prop.SetMethod)}.");

                return false;
            }

            if (!throwIfFailed)
            {
                try
                {
                    prop.SetValue(target, value);

                    if (cacheField)
                        prop.TryCacheMember();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            prop.SetValue(target, value);

            if (cacheField)
                prop.TryCacheMember();

            return true;
        }
    }
}
