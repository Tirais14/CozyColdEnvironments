using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public static class ObjectExtensions
    {
        public static T IfDefault<T>(this T? source, T value)
        {
            if (source.IsDefault())
                return value;

            return source;
        }
        public static T IfDefault<T>(this T? source, Func<T> factory)
        {
            CC.Guard.NullArgument(factory, nameof(factory));

            if (source.IsDefault())
                return factory();

            return source;
        }
        public static TOutput IfDefault<TInput, TOutput>(this TInput? source, TOutput output)
        {
            if (source.IsDefault())
                return output;

            try
            {
                return source.MutateType<TOutput>();
            }
            catch (Exception ex)
            {
                typeof(ObjectExtensions).PrintExceptionAsLog(ex, DebugArguments.IsAdditive);
            }

            return source.As<TOutput>();
        }
        public static TOutput IfDefault<TInput, TOutput>(this TInput? source,
            Func<TOutput> factory)
        {
            if (source.IsDefault())
                return factory();

            try
            {
                return source.MutateType<TOutput>();
            }
            catch (Exception ex)
            {
                typeof(ObjectExtensions).PrintExceptionAsLog(ex, DebugArguments.IsAdditive);
            }

            return source.As<TOutput>();
        }

        public static TOutput? IfNotDefault<TInput, TOutput>(
            this TInput? source,
            TOutput output,
            TOutput? ifDefault = default)
        {
            if (source.IsNotDefault())
                return output;

            return ifDefault;
        }

        public static T? IfNotDefault<T>(
            this T? source,
            Action<T> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            if (source.IsNotDefault())
                action(source);

            return source;
        }

        public static T? IfNotDefault<T>(
            this T? source,
            Func<T, T> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            if (source.IsNotDefault())
                return action(source);

            return source;
        }

        public static TOutput IfNotDefault<TInput, TOutput>(
            this TInput source,
            Func<TInput, TOutput> action,
            Func<TInput, TOutput> ifDefaultAction)
        {
            CC.Guard.NullArgument(action, nameof(action));
            CC.Guard.NullArgument(ifDefaultAction, nameof(ifDefaultAction));

            if (source.IsNotDefault())
                return action(source);

            return ifDefaultAction(source);
        }

        public static bool TrySwitch<T>(this T? source,
            params (Predicate<T?> predicate, Action<T> action)[] conditions)
        {
            CC.Guard.NullArgument(conditions, nameof(conditions));
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
            out TResult result,
            params (Predicate<T?> predicate, Func<T, TResult> func)[] conditions)
        {
            CC.Guard.NullArgument(conditions, nameof(conditions));
            if (conditions.IsEmpty())
            {
                result = default!;
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

        public static Ghost<T> AsOrDefault<T>(this object? obj)
        {
            return obj is T typedObj ? typedObj : default;
        }

        public static Ghost<TValue> AsOrDefault<TObj, TValue>(this TObj? obj)
        {
            return obj is TValue typedObj ? typedObj : default;
        }

        /// <summary>
        /// Checks for <see cref="CC.EmptyObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <inheritdoc cref="TypeMutator.MutateType(object, Type)"/>
        public static object MutateType(this object obj, Type conversionType)
        {
            return TypeMutator.MutateType(obj, conversionType);
        }

        /// <inheritdoc cref="TypeMutator.MutateType(object, Type)"/>
        public static T MutateType<T>(this object obj)
        {
            return obj.MutateType<T>();
        }
    }
}
