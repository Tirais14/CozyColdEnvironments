using System;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib.Returnables
{
    public interface IMethodResult
    {
        bool IsValidResults { get; }
        bool IsVoidResults { get; }
        int ResultCount { get; }
        IReadOnlyList<TypeValuePair> ExplicitResults { get; }
        IReadOnlyList<object?> Results { get; }
        IReadOnlyList<Type> ResultTypes { get; }

        object? GetResult(int index);
        object? GetResult(Type type);

        T GetResult<T>(int index);
        T GetResult<T>();

        bool HasResult(int index);
        bool HasResult(Type type);

        bool HasResult<T>(int index);
        bool HasResult<T>();
    }
    public interface IMethodResult<T0> : IMethodResult
    {
        T0 Result0 { get; }
    }
    public interface IMethodResult<T0, T1> : IMethodResult<T0>
    {
        T1 Result1 { get; }
    }
    public interface IMethodResult<T0, T1, T2> : IMethodResult<T0, T1>
    {
        T2 Result2 { get; }
    }
}
