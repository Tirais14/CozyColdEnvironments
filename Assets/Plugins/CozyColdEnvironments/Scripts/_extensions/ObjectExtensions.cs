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

            return source.To<TOutput>();
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

            return source.To<TOutput>();
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
            CC.Guard.IsNotNull(action, nameof(action));

            if (source.IsNotDefault())
                action(source);

            return source;
        }

        public static T? IfNotDefault<T>(
            this T? source,
            Func<T, T> action)
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (source.IsNotDefault())
                return action(source);

            return source;
        }

        public static TOutput IfNotDefault<TInput, TOutput>(
            this TInput source,
            Func<TInput, TOutput> action,
            Func<TInput, TOutput> ifDefaultAction)
        {
            CC.Guard.IsNotNull(action, nameof(action));
            CC.Guard.IsNotNull(ifDefaultAction, nameof(ifDefaultAction));

            if (source.IsNotDefault())
                return action(source);

            return ifDefaultAction(source);
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

            try
            {
                return (T)obj;
            }
            catch (InvalidCastException)
            {
                throw CC.ThrowHelper.InvalidCastException(obj.GetType(), typeof(T));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> As<T>(this object? obj)
        {
            return obj.Is<T>(out var typedObj) ? typedObj : default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> As<TObj, TValue>(this TObj? obj)
        {
            return obj.Is<TValue>(out var typedObj) ? typedObj : default!;
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

namespace CCEnvs.Conversations
{
    public static class ObjectExtensions
    {

    }
}
