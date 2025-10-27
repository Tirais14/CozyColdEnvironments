#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;

namespace CCEnvs.Language
{
    public interface IConditional
    {
        bool IsSome { get; }
        bool IsNone { get; }

        bool Check(object? value);
        bool Check(Predicate<object> predicate);

        bool CheckUnsafe(Predicate<object?> predicate);

        object? Access();
        bool Access([NotNullWhen(true)] out object? result);
        object? Access(object? defaultValue);
        object? Access(Func<object?> defaultValueFactory);

        object AccessUnsafe();

        Maybe<TOut> Cast<TOut>();

        IConditional IfSome(Action<object> action);

        IConditional IfNone(Action action);
        IConditional IfNone(Func<object> selector);

        IConditional Match(Action<object> some, Action none);
        IConditional Match(Func<object, object?> some, Func<object?> none);

        IConditional Map(Func<object, object?> selector);

        IConditional MapUnsafe(Func<object?, object?> selector);
    }
    public interface IConditional<T> : IConditional, IEnumerable<T>
    {
        IConditional IfNone<TOut>(Func<TOut> selector);

        bool ItIs(T? value);
        bool ItIs(Predicate<T> predicate);

        bool CheckUnsafe(Predicate<T?> predicate);

        new T? Access();
        bool Access([NotNullWhen(true)] out T? result);
        T? Access(T? defaultValue);
        T? Access(Func<T?> defaultValueFactory);

        new T AccessUnsafe();

        Maybe<TOut> Map<TOut>(Func<T, TOut?> selector);

        Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none);

        Maybe<TOut> MapUnsafe<TOut>(Func<T?, TOut?> selector);

        Maybe<TOut> Select<TOut>(Func<T, TOut?> selector);

        IConditional IConditional.IfNone(Func<object> selector) => IfNone(() => selector());

        bool IConditional.Check(object? value) => ItIs(value.AsOrDefault<T>().Access());
        bool IConditional.Check(Predicate<object> predicate) => ItIs(x => predicate(x!));

        bool IConditional.CheckUnsafe(Predicate<object?> predicate) => CheckUnsafe(x => predicate(x!));

        object? IConditional.Access() => Access();
        bool IConditional.Access([NotNullWhen(true)] out object? result)
        {
            var t = Access(out T? tR);

            result = tR;
            return t;
        }
        object? IConditional.Access(object? defaultValue) => Access(defaultValue.AsOrDefault<T>().Access());
        object? IConditional.Access(Func<object?> defaultValueFactory) => Access(() => defaultValueFactory());

        object IConditional.AccessUnsafe() => AccessUnsafe()!;

        IConditional IConditional.Map(Func<object, object?> selector) => Map(x => selector(x!));

        IConditional IConditional.Match(Func<object, object?> some, Func<object?> none) => Match(x => some(x!), () => none());

        IConditional IConditional.MapUnsafe(Func<object?, object?> selector) => MapUnsafe((x) => selector(x));
    }
    public interface IConditional<TThis, T> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis IfSome(Action<T> action);

        new TThis IfNone(Action action);

        TThis Match(Action<T> some, Action none);

        TThis Apply(T? value);

        TThis Where(Predicate<T> predicate);

        IConditional IConditional.IfSome(Action<object> action) => IfSome(x => action(x!));

        IConditional IConditional.IfNone(Action action) => IfNone(() => action());

        IConditional IConditional.Match(Action<object> some, Action none) => Match(x => some(x!), () => none());
    }
}
