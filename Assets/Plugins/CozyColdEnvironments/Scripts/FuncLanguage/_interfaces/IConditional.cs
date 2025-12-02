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
        object? Raw { get; }

        object? GetValue();
        object GetValue(object defaultValue);
        object GetValue(Func<object> defaultValueFactory);

        object GetValueUnsafe();
        object GetValueUnsafe(Exception exception);
        object GetValueUnsafe(Func<Exception> exceptionFactory);

        bool TryGetValue([NotNullWhen(true)] out object? result);

        bool ItIs(object? value);
        bool ItIs(Predicate<object> predicate);

        bool ItIsUnsafe(Predicate<object?> predicate);
    }
    public interface IConditional<T> : IConditional, IEnumerable<T>
    {
        new T? Raw { get; }

        object? IConditional.Raw => Raw;

        new T? GetValue();
        T GetValue(T defaultValue);
        T GetValue(Func<T> defaultValueFactory);

        new T GetValueUnsafe();
        new T GetValueUnsafe(Exception exception);
        new T GetValueUnsafe(Func<Exception> exceptionFactory);

        bool TryGetValue([NotNullWhen(true)] out T? result);

        bool Has(T? value);
        bool Has(Predicate<T> predicate);

        bool HasUnsafe(Predicate<T?> predicate);

        Either<T, R> Cast<R>();

        Either<T, R> Select<R>(Func<T, R> selector);

        object? IConditional.GetValue() => GetValue();
        object IConditional.GetValue(object defaultValue)
        {
            return GetValue(defaultValue.To<T>())!;
        }
        object IConditional.GetValue(Func<object> defaultValueFactory)
        {
            return GetValue(() => defaultValueFactory())!;
        }

        object IConditional.GetValueUnsafe() => GetValueUnsafe()!;
        object IConditional.GetValueUnsafe(Exception exception) => GetValueUnsafe(exception)!;
        object IConditional.GetValueUnsafe(Func<Exception> exceptionFactory)
        {
            return GetValueUnsafe(exceptionFactory)!;
        }

        bool IConditional.TryGetValue([NotNullWhen(true)] out object? result)
        {
            var t = TryGetValue(out T? tR);

            result = tR;
            return t;
        }

        bool IConditional.ItIs(object? value)
        {
            return Has(value.As<T>().GetValue());
        }
        bool IConditional.ItIs(Predicate<object> predicate)
        {
            return Has(x => predicate(x!));
        }

        bool IConditional.ItIsUnsafe(Predicate<object?> predicate)
        {
            return HasUnsafe(x => predicate(x!));
        }
    }

    public interface IConditional<T, out TThis> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis Apply(T? value);

        TThis Where(Predicate<T> predicate);
    }
}
