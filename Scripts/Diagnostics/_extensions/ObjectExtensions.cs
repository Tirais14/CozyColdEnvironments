#nullable enable
using CCEnvs.Reflection.Cached;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        {
            return new NullValidator<T>(obj).IsNull;
        }

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.IsNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        {
            return !new NullValidator<T>(obj).IsNull;
        }

        /// <summary>
        /// Also checks for unity null
        /// </summary>
        public static bool IsDefault([NotNullWhen(false)] this object? value,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            if (value.IsNull())
                return true;

            Type type = value.GetType();
            if (type.IsClass)
                return false;

            if (!TypeCache.TryGetDefaultValue(type, out object? defaultValue))
            {
                defaultValue = Activator.CreateInstance(type, nonPublic: true);
                TypeCache.TryCacheDefaultValue(type, defaultValue);
            }

            if (value.Equals(defaultValue))
                return true;

            if (value is string str)
            {
                return option switch
                {
                    EqualsDefaultOption.IncludeNullOrEmptyString => str.IsNullOrEmpty(),
                    EqualsDefaultOption.IncludeWhitespaceOrEmptyString => str.IsNullOrWhiteSpace(),
                    _ => throw new InvalidOperationException(),
                };
            }

            return false;
        }
        public static bool IsDefault([NotNullWhen(false)] this object? obj,
            object[] customDefaultValues,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            if (obj.IsDefault(option))
                return true;
            else if (customDefaultValues.Contains(obj))
                return true;

            return false;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            return !obj.IsDefault(option);
        }
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            object[] customDefaultValues,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            return !obj.IsDefault(customDefaultValues, option);
        }
    }
}
