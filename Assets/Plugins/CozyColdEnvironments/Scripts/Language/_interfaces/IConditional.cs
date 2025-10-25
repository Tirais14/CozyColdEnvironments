#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Language
{
    public interface IConditional
    {
        bool IsSome { get; }
        bool IsNone { get; }
    }
    public interface IConditional<T> : IConditional
    {
        bool Check(T? value);
        bool Check(Predicate<T> predicate);

        bool CheckUnsafe(Predicate<T?> predicate);

        T? Access();
        bool Access([NotNullWhen(true)] out T? result);
        T? Access(T? defaultValue);
        T? Access(Func<T?> defaultValueFactory);

        T AccessUnsafe();
    }
    public interface IConditional<TThis, T> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis IfSome(Action<T> action);

        TThis IfNone(Action action);

        TThis Match(Action<T> some, Action none);
        Conditional<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none);

        TThis Apply(T? value);

        Conditional<TOut> MapUnsafe<TOut>(Func<T?, TOut> selector);

        Conditional<TOut> Select<TOut>(Func<T, TOut> selector);

        TThis Where(Predicate<T> predicate);
    }
}
