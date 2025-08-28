using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Returnables
{
    public class MethodResult : IMethodReturnable
    {
        public readonly static MethodResult Void = new();

        public bool IsValidResults { get; }
        public bool IsVoidResults { get; }
        public IReadOnlyList<TypeValuePair> ExplicitResults { get; }
        public IReadOnlyList<object?> Results { get; }
        public IReadOnlyList<Type> ResultTypes { get; }

        private MethodResult()
        {
            IsValidResults = true;
            IsVoidResults = true;

            ExplicitResults = Array.Empty<TypeValuePair>();
            Results = Array.Empty<object?>();
            ResultTypes = Array.Empty<Type>();
        }
        public MethodResult(bool isValidResults, params TypeValuePair[] explicitResults)
        {
            IsValidResults = isValidResults;

            ExplicitResults = new ReadOnlyCollection<TypeValuePair>(explicitResults);

            Results = new ReadOnlyCollection<object?>(
                explicitResults.Select(x => x.value).ToArray());

            ResultTypes = new ReadOnlyCollection<Type>(
                explicitResults.Select(x => x.type).ToArray());

            IsVoidResults = ResultTypes.All(x => x.IsAnyType(typeof(void)));
        }
        public MethodResult(bool isValidResults,
                            params object[] results)
            :
            this(isValidResults,
                 results.Select(x => new TypeValuePair(x.GetType(), x)).ToArray())
        {
        }

        public static implicit operator bool(MethodResult result)
        {
            return result.IsValidResults;
        }

        public object? GetResult(int index)
        {
            return Results[index];
        }
        public object? GetResult(Type type)
        {
            int index = ((ReadOnlyCollection<Type>)ResultTypes).IndexOf(type);

            return GetResult(index);
        }

        public T? GetResult<T>(int index)
        {
            return (T?)GetResult(index);
        }
        public T? GetResult<T>()
        {
            return (T?)GetResult(typeof(T));
        }
    }
}
