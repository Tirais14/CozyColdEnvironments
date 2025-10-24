#nullable enable
using System;

namespace CCEnvs.Language
{
    public interface IConditional
    {
        bool IsSome { get; }
        bool IsNone { get; }
    }
    public interface IConditional<T> : IConditional
    {
        bool Check(Predicate<T> predicate);

        bool CheckUnsafe(Predicate<T?> predicate);

        T? Value();
        T? Value(T? defaultValue);
        T? Value(Func<T?> defaultValueFactory);

        T ValueUnsafe();
    }
}
