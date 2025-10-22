using System;

namespace CCEnvs.Language
{
    public interface IGhost
    {
        bool IsNone { get; }
        bool IsSome { get; }
    }
    public interface IGhost<T> : IGhost
    {
        Ghost<T> IfNone(Action action);
        T IfNone(Func<T> defaultValueFactory);
        T IfNone(T defaultValue);
        Ghost<T> IfSome(Action<T> action);
        Ghost<TOther> Map<TOther>(Func<T, TOther> selector);
        Ghost<T> Match(Action<T> some, Action none);
        Ghost<TOther> Match<TOther>(Func<T, TOther> some, Func<TOther> none);
        T Value();
        T ValueUnsafe();
    }
}