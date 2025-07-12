using System;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

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

            return type.GetProccessedName();
        }

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
    }
}
