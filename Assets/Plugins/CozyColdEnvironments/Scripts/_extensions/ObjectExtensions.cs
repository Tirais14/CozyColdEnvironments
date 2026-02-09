using CCEnvs.Collections;
using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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
            CC.Guard.IsNotNull(factory, nameof(factory));

            if (source.IsDefault())
                return factory();

            return source;
        }

        public static bool TrySwitch<T>(this T? source,
            params (Predicate<T?> predicate, Action<T> action)[] conditions)
        {
            CC.Guard.IsNotNull(conditions, nameof(conditions));
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
            CC.Guard.IsNotNull(conditions, nameof(conditions));
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault([NotNullWhen(false)] this object? obj,
            object[] customDefaultValues,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            if (obj.IsDefault(option) || customDefaultValues.Contains(obj))
                return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            return !obj.IsDefault(option);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj,
            object[] customDefaultValues,
            EqualsDefaultOption option = EqualsDefaultOption.None)
        {
            return !obj.IsDefault(customDefaultValues, option);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this object? obj)
        {
            if (obj.IsNull())
                return default!;

            if (obj is not T)
                throw CC.ThrowHelper.InvalidCastException(obj.GetType(), typeof(T));

            return (T)obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> AsMaybe<T>(this object? obj)
        {
            return obj.Is<T>(out var typedObj) ? typedObj : default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> AsMaybe<TObj, TValue>(this TObj? obj)
        {
            return obj.Is<TValue>(out var typedObj) ? typedObj : default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? As<T>(this object? obj)
        {
            if (!obj.Is<T>(out var casted))
                return default;

            return casted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? As<TObj, TValue>(this TObj? obj)
        {
            if (!obj.Is<TValue>(out var casted))
                return default;

            return casted;
        }

        public static bool Let<T>(this T? source, [NotNullWhen(true)] out T? local)
        {
            return source.Is<T>(out local);
        }
        public static bool Let<T, TOut>(
            this T? source,
            Func<T, TOut?> converter,
            [NotNullWhen(true)] out TOut? local)
        {
            Guard.IsNotNull(converter);

            if (source.IsNull())
            {
                local = default!;
                return false;
            }

            return converter(source).Is<TOut>(out local);
        }

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
