using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Reflection.Cached;

#nullable enable
namespace UTIRLib
{
    public static class ObjectExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetTypeName<T>(this T? obj,
            TypeNameAttributes attributes = TypeNameAttributes.Default)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetName(attributes);
        }
    }
}

namespace UTIRLib.Unity.TypeMatching
{
    public static class ObjectExtensions
    {
        public static bool Is<T>(this object? obj)
        {
            if (obj.IsNotNull() && obj is T)
                return true;

            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj)
        {
            if (obj.IsNotNull() && obj is T)
                return true;

            return false;
        }
        public static bool Is<T>(this object? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj.IsNotNull() && obj is T typedObj)
            {
                result = typedObj;
                return true;
            }

            result = default;
            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj.IsNotNull() && obj is T typedObj)
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

namespace UTIRLib.Extensions
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
        public static bool TryChangeType(this object? obj,
                                         Type conversionType,
                                         [NotNullWhen(true)] out object? result)
        {
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

namespace UTIRLib.Diagnostics
{
    public static class ObjectExtensions
    {
        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        {
            return new NullValidator<T>(obj).AnyNull;
        }

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.AnyNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        {
            return !new NullValidator<T>(obj).AnyNull;
        }

        /// <summary>
        /// Also checks for unity null
        /// </summary>
        public static bool IsDefault([NotNullWhen(false)] this object? obj,
            IsDefaultOption option = IsDefaultOption.None)
        {
            if (obj.IsNull())
                return true;

            object? defaultValue = TypeCache.GetDefaultValue(obj.GetType());

            if (obj.Equals(defaultValue))
                return true;

            if (obj is string str)
            {
                return option switch
                {
                    IsDefaultOption.IncludeNullOrEmptyString => str.IsNullOrEmpty(),
                    IsDefaultOption.IncludeWhitespaceOrEmptyString => str.IsNullOrWhiteSpace(),
                    _ => throw new InvalidOperationException(),
                };
            }

            return false;
        }
        public static bool IsDefault([NotNullWhen(false)] this object? obj,
            object[] customDefaultValues,
            IsDefaultOption option = IsDefaultOption.None)
        {
            if (obj.IsDefault(option))
                return true;
            else if (customDefaultValues.Contains(obj))
                return true;

            return false;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            IsDefaultOption option = IsDefaultOption.None)
        {
            return !obj.IsDefault(option);
        }
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            object[] customDefaultValues,
            IsDefaultOption option = IsDefaultOption.None)
        {
            return !obj.IsDefault(customDefaultValues, option);
        }
    }
}
