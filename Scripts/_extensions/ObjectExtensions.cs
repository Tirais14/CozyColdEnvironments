using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs
{
    public static class ObjectExtensions
    {
        public static bool IsEmptyObject<T>(this T? value)
        {
            if (value is null)
                return false;

            return value.Equals(CC.EmptyObject) 
                   ||
                   value.GetType() == typeof(object);
        }

        public static bool IsNotEmptyObject<T>(this T? value)
        {
            return !value.IsEmptyObject();
        }

        public static bool IsCacheable<T>(this T value)
        {
            Validate.ArgumentNull(value, nameof(value));

            if (value is ICacheable)
                return true;

            return value.GetType().IsDefined<CacheableAttribute>(inherit: false);
        }
    }
}

namespace CCEnvs.TypeMatching
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

        public static T? IsQ<T>(this object? obj)
        {
            return obj is T typedObj ? typedObj : default;
        }

        public static TValue? IsQ<TObj, TValue>(this TObj? obj)
        {
            return obj is TValue typedObj ? typedObj : default;
        }
    }
}

namespace CCEnvs.Converting
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
