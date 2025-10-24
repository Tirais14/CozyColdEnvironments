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

        T? Access();
        bool Access([NotNullWhen(true)] out T? result);
        T? Access(T? defaultValue);
        T? Access(Func<T?> defaultValueFactory);

        T AccessUnsafe();
    }
}
