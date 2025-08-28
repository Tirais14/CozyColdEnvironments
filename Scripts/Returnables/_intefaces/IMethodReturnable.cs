using System;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib.Returnables
{
    public interface IMethodReturnable
    {
        bool IsValidResults { get; }
        bool IsVoidResults { get; }
        IReadOnlyList<TypeValuePair> ExplicitResults { get; }
        IReadOnlyList<object?> Results { get; }
        IReadOnlyList<Type> ResultTypes { get; }

        object? GetResult(int index);
        object? GetResult(Type type);

        T? GetResult<T>(int index);
        T? GetResult<T>();
    }
}
