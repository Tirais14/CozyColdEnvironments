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
        bool Check(Predicate<T> predicate);

        bool CheckUnsafe(Predicate<T?> predicate);

        T? Value();
        bool Value([NotNullWhen(true)] out T? result);
        T? Value(T? defaultValue);
        T? Value(Func<T?> defaultValueFactory);

        T ValueUnsafe();
    }
}
