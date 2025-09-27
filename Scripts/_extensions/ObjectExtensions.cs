using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public static class ObjectExtensions
    {
        public static bool TrySwitch<T>(this T? source,
            params (Predicate<T?> predicate, Action<T> action)[] conditions)
        {
            CC.Validate.ArgumentNull(conditions, nameof(conditions));
            if (conditions.IsEmpty())
                return false;

            foreach (var (predicate, action) in conditions)
            {
                if (predicate(source))
                {
                    action(source!);
                    return true;
                }
            }

            return false;
        }

        public static bool TrySwitch<T, TResult>(this T? source,
            [NotNullIfNotNull(nameof(result))] out TResult? result,
            params (Predicate<T?> predicate, Func<T, TResult> func)[] conditions)
        {
            CC.Validate.ArgumentNull(conditions, nameof(conditions));
            if (conditions.IsEmpty())
            {
                result = default;
                return false;
            }

            foreach ((Predicate<T?> predicate, Func<T, TResult> func) in conditions)
            {
                if (predicate(source))
                {
                    result = func(source!);
                    return true;
                }
            }

            result = default;
            return false;
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

            if (!TypeCache.DefaultValues.TryGetValue(type, out object? defaultValue))
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
            if (obj.IsDefault(option) || customDefaultValues.Contains(obj))
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

        public static T As<T>(this object? obj)
        {
            if (obj.IsNull())
                return default!;

            try
            {
                return (T)obj;
            }
            catch (InvalidCastException)
            {
                return CC.Throw.InvalidCast(obj.GetType(), typeof(T)).As<T>();
            }
        }

        public static T? AsOrDefault<T>(this object? obj)
        {
            return obj is T typedObj ? typedObj : default;
        }

        public static TValue? AsOrDefault<TObj, TValue>(this TObj? obj)
        {
            return obj is TValue typedObj ? typedObj : default;
        }

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
    }
}

namespace CCEnvs.Conversations
{
    public static class ObjectExtensions
    {
        /// <inheritdoc cref="TypeTransformer.DoTransform(object, Type)"/>
        public static object TransformType(this object obj, Type conversionType)
        {
            return TypeTransformer.DoTransform(obj, conversionType);
        }

        /// <inheritdoc cref="TypeTransformer.DoTransform(object, Type)"/>
        public static T TransformType<T>(this object obj)
        {
            return obj.TransformType<T>();
        }
    }
}
