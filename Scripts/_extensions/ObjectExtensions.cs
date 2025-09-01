using System;
using System.Diagnostics.CodeAnalysis;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Cached;

#nullable enable
namespace CCEnvs
{
    public static class ObjectExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetTypeName<T>(this T? obj,
            TypeNameConvertingAttributes attributes = TypeNameConvertingAttributes.Default)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetName(attributes);
        }
    }
}

namespace CCEnvs.TypeMatching
{
    public static class ObjectExtensions
    {
        public static bool IsType(this object? value, Type type)
        {
            return type.IsInstanceOfType(value);
        }
        public static bool IsType(this object? value,
            Type type,
            [NotNullWhen(true)] out object? result)
        {
            if (!value.IsType(type))
            {
                result = null;
                return false;
            }

            result = value;
            return result.IsNotNull();
        }

        public static bool IsNotType(this object? value, Type type)
        {
            return value.IsType(type);
        }

        public static bool IsNotType(this object? value,
            Type type,
            out object? result)
        {
            if (value.IsNotType(type))
            {
                result = null;
                return true;
            }

            result = null;
            return false;
        }
    }
}

namespace CCEnvs.Unity.TypeMatching
{
    public static class ObjectExtensions
    {
        public static bool Is<T>(this object? obj)
        {
            if (obj is T && obj.IsNotNull())
                return true;

            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj)
        {
            if (obj is T && obj.IsNotNull())
                return true;

            return false;
        }
        public static bool Is<T>(this object? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj is T typedObj && obj.IsNotNull())
            {
                result = typedObj;
                return true;
            }

            result = default;
            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj is T typedObj && obj.IsNotNull())
            {
                result = typedObj;
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsNot<T>(this object? obj)
        {
            return !obj.Is<T>();
        }

        public static bool IsNot<TThis, T>(this TThis? obj)
        {
            return !obj.Is<TThis, T>();
        }

        public static bool IsNot<T>(this object? obj, [NotNullWhen(false)] out T? result)
        {
            return !obj.Is(out result);
        }

        public static bool IsNot<TThis, T>(this TThis? obj, [NotNullWhen(false)] out T? result)
        {
            return !obj.Is(out result);
        }
    }
}

namespace CCEnvs.TypeConverting
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/>
        /// </summary>
        public static object ChangeType(this object obj, Type conversionType)
        {
            return System.Convert.ChangeType(obj, conversionType);
        }

        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/>
        /// </summary>
        public static T ChangeType<T>(this object obj)
        {
            return (T)System.Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/> but in <see langword="try"/>-<see langword="catch"/>
        /// </summary>
        public static bool TryChangeType(this object obj,
                                         Type conversionType,
                                         [NotNullWhen(true)] out object? result)
        {
            if (obj.GetType() == conversionType)
            {
                result = obj;
                return true;
            }

            try
            {
                result = Convert.ChangeType(obj, conversionType);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}

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
