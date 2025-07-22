using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib
{
    public static class ObjectExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetTypeName<T>(this T obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType().Name;
        }

        public static string GetProccessedTypeName<T>(this T? obj)
        {
            Type? type = obj?.GetType();

            return type.GetName();
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

        public static bool IsNot<T>(this object obj)
        {
            return !obj.Is<T>();
        }

        public static bool IsNot<TThis, T>(this TThis obj)
        {
            return !obj.Is<TThis, T>();
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
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj) => new NullValidator<T>(obj).AnyNull;

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.AnyNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj) => !new NullValidator<T>(obj);

        /// <summary>
        /// Also checks for unity null
        /// </summary>
        public static bool IsDefault<T>([NotNullWhen(false)] this T obj) =>
            EqualityComparer<T>.Default.Equals(obj, default!) || obj.IsNull();

        /// <summary>Inverted</summary>
        public static bool IsNotDefault<T>([NotNullWhen(true)] this T obj) => !obj.IsDefault();
    }
}
