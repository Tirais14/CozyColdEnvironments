using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public interface IConditional
    {
        bool IsSome { get; }
        bool IsNone { get; }
        object? Target { get; }

        object? Access();
        object Access(object defaultValue);
        object Access(Func<object> defaultValueFactory);

        object AccessUnsafe();

        bool TryAccess([NotNullWhen(true)] out object? result);

        bool ItIs(object? value);
        bool ItIs(Predicate<object> predicate);

        bool ItIsUnsafe(Predicate<object?> predicate);
    }
    public interface IConditional<T> : IConditional, IEnumerable<T>
    {
        new T? Raw { get; }

        object? IConditional.Target => Raw;

        new T? GetValue();
        T GetValue(T defaultValue);
        T GetValue(Func<T> defaultValueFactory);

        new T GetValueUnsafe();

        bool TryGetValue([NotNullWhen(true)] out T? result);

        bool Contains(T? value);
        bool Contains(Predicate<T> predicate);

        bool ContainsUnsafe(Predicate<T?> predicate);

        Either<T, R> Cast<R>();

        Either<T, R> Select<R>(Func<T, R> selector);

        object? IConditional.Access() => GetValue();
        object IConditional.Access(object defaultValue)
        {
            return GetValue(defaultValue.As<T>())!;
        }
        object IConditional.Access(Func<object> defaultValueFactory)
        {
            return Access(() => defaultValueFactory())!;
        }

        object IConditional.AccessUnsafe() => GetValueUnsafe()!;

        bool IConditional.TryAccess([NotNullWhen(true)] out object? result)
        {
            var t = TryGetValue(out T? tR);

            result = tR;
            return t;
        }

        bool IConditional.ItIs(object? value)
        {
            return Contains(value.AsOrDefault<T>().GetValue());
        }
        bool IConditional.ItIs(Predicate<object> predicate)
        {
            return Contains(x => predicate(x!));
        }

        bool IConditional.ItIsUnsafe(Predicate<object?> predicate)
        {
            return ContainsUnsafe(x => predicate(x!));
        }
    }

    public interface IConditional<T, out TThis> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis Apply(T? value);

        TThis Where(Predicate<T> predicate);
    }
}
