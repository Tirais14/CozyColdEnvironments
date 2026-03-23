using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CCEnvs.Collections;
using CCEnvs.Conversations;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs
{
    public static class ObjectExtensions
    {
        [Obsolete("Usless and heavy")]
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

        [Obsolete("Usless and heavy")]
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
        public static bool IsDefault([NotNullWhen(false)] this object? value)
        {
            if (value.IsNull())
                return true;

            Type type = value.GetType();

            if (!type.IsValueType)
                return false;

            var defaultValue = TypeHelper.GetDefaultValue(type);

            return value.Equals(defaultValue);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault<T>(this T source)
            where T : struct, IEquatable<T>
        {
            return source.Equals(default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotDefault([NotNullWhen(true)] this object? obj)
        {
            return !obj.IsDefault();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool IsNotDefault<T>(this T source)
            where T : struct, IEquatable<T>
        {
            return !source.IsDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CastTo<T>(this object? obj)
        {
            if (obj.IsNull())
                return default!;

            if (obj is not T)
                throw CC.ThrowHelper.InvalidCastException(obj.GetType(), typeof(T));

            return (T)obj;
        }

        [Obsolete("User simple As instead")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> AsObsolete<T>(this object? obj)
        {
            return obj.Is<T>(out var typedObj) ? typedObj : default!;
        }

        [Obsolete("User simple As instead")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> AsObsolete<TObj, TValue>(this TObj? obj)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Let<T>(this T source, out T local)
        {
            local = source;

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOut Let<T, TOut>(
            this T source,
            Func<T, TOut> converter,
            out TOut local)
        {
            Guard.IsNotNull(converter);

            if (source.IsNull())
            {
                local = default!;
                return local;
            }

            local = converter(source);
            return local;
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

        public static T? IfNotNull<T>(this T? source, Action<T> action)
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (source.IsNull())
                return source;

            action(source);

            return source;
        }

        public static T? IfNotNull<T, TState>(this T? source, TState state, Action<T, TState> action)
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (source.IsNull())
                return source;

            action(source, state);

            return source;
        }

        public static T? IfNotNullNullable<T>(this T? source, Action<T> action)
            where T : struct
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (!source.HasValue)
                return source;

            action(source.Value);

            return source;
        }

        public static T? IfNotNullNullable<T, TState>(this T? source, TState state, Action<T, TState> action)
            where T : struct
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (!source.HasValue)
                return source;

            action(source.Value, state);

            return source;
        }

        public static T? IfNotDefault<T>(this T? source, Action<T> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsDefault())
                return default;

            action(source);

            return source;
        }

        public static T? IfNotDefault<T, TState>(this T? source, TState state, Action<T, TState> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsDefault())
                return default;

            action(source, state);

            return source;
        }

        public static T IfNull<T>(this T? source, T other)
        {
            if (!source.IsNull())
                return source;

            return other;
        }

        public static T IfNull<T>(this T? source, Func<T> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (!source.IsNull())
                return source;

            return factory();
        }

        public static T IfNull<T, TState>(this T? source, TState state, Func<TState, T> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (!source.IsNull())
                return source;

            return factory(state);
        }
    }
}
