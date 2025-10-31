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
        object? Wrapped { get; }

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
        new T? Wrapped { get; }

        object? IConditional.Wrapped => Wrapped;

        new T? Access();
        T Access(T defaultValue);
        T Access(Func<T> defaultValueFactory);

        new T AccessUnsafe();

        bool TryAccess([NotNullWhen(true)] out T? result);

        bool ItIs(T? value);
        bool ItIs(Predicate<T> predicate);

        bool ItIsUnsafe(Predicate<T?> predicate);

        Either<T, R> Cast<R>();

        Either<T, R> Select<R>(Func<T, R> selector);

        object? IConditional.Access() => Access();
        object IConditional.Access(object defaultValue)
        {
            return Access(defaultValue.As<T>())!;
        }
        object IConditional.Access(Func<object> defaultValueFactory)
        {
            return Access(() => defaultValueFactory())!;
        }

        object IConditional.AccessUnsafe() => AccessUnsafe()!;

        bool IConditional.TryAccess([NotNullWhen(true)] out object? result)
        {
            var t = TryAccess(out T? tR);

            result = tR;
            return t;
        }

        bool IConditional.ItIs(object? value)
        {
            return ItIs(value.AsOrDefault<T>().Access());
        }
        bool IConditional.ItIs(Predicate<object> predicate)
        {
            return ItIs(x => predicate(x!));
        }

        bool IConditional.ItIsUnsafe(Predicate<object?> predicate)
        {
            return ItIsUnsafe(x => predicate(x!));
        }
    }

    public interface IConditional<T, out TThis> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis Apply(T? value);

        TThis Where(Predicate<T> predicate);
    }
}
