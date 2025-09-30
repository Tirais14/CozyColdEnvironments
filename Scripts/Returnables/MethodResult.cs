using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Returnables
{
    public class MethodResult : IMethodResult
    {
        public readonly static MethodResult Void = new(isValidResults: true);
        public readonly static MethodResult Failed = new(isValidResults: false);

        public bool IsValid { get; }
        public bool IsVoid { get; }
        public IReadOnlyList<TypeValuePair> ExplicitResults { get; }
        public IReadOnlyList<object?> Results { get; }
        public IReadOnlyList<Type> ResultTypes { get; }
        public int ResultCount => Results.Count;

        protected MethodResult(bool isValidResults)
        {
            IsValid = isValidResults;
            IsVoid = true;

            ExplicitResults = Array.Empty<TypeValuePair>();
            Results = Array.Empty<object?>();
            ResultTypes = Array.Empty<Type>();
        }
        public MethodResult(bool isValidResults, params TypeValuePair[] explicitResults)
        {
            IsValid = isValidResults;

            ExplicitResults = new ReadOnlyCollection<TypeValuePair>(explicitResults);

            Results = new ReadOnlyCollection<object?>(
                explicitResults.Select(x => x.Value).ToArray());

            ResultTypes = new ReadOnlyCollection<Type>(
                explicitResults.Select(x => x.Type).ToArray());

            IsVoid = ResultTypes.All(x => x.IsAnyType(typeof(void)));
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
            return result.IsValid;
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

        public T GetResult<T>(int index)
        {
            return (T)GetResult(index)!;
        }
        public T GetResult<T>()
        {
            return (T)GetResult(typeof(T))!;
        }

        public bool HasResult(int index)
        {
            return index < ResultCount;
        }
        public bool HasResult(Type type)
        {
            return ResultTypes.Contains(type);
        }

        public bool HasResult<T>(int index)
        {
            return HasResult(index) && HasResult(typeof(T));
        }
        public bool HasResult<T>()
        {
            return HasResult(typeof(T));
        }
    }
    public class MethodResult<T0>
        :
        MethodResult,
        IMethodResult<T0>
    {
        new public readonly static MethodResult<T0> Void = new(isValidResults: true);
        new public readonly static MethodResult<T0> Failed = new(isValidResults: false);

        public T0 Result0 { get; } = default!;

        public MethodResult(bool isValidResults, T0 result0)
            :
            base(isValidResults,
                 Range.From(new TypeValuePair(typeof(T0), result0)))
        {
            Result0 = result0;
        }
        protected MethodResult(bool isValidResults,
                               params TypeValuePair[] explicitResults)
            :
            base(isValidResults, explicitResults)
        {
        }
        protected MethodResult(bool isValidResults)
            :
            base(isValidResults)
        {
        }
    }
    public class MethodResult<T0, T1>
        :
        MethodResult<T0>,
        IMethodResult<T0, T1>
    {
        new public readonly static MethodResult<T0, T1> Void = new(isValidResults: true);
        new public readonly static MethodResult<T0, T1> Failed = new(isValidResults: false);

        public T1 Result1 { get; } = default!;

        public MethodResult(bool isValidResults, T0 result0, T1 result1)
            :
            base(isValidResults,
                 Range.From(new TypeValuePair(typeof(T0), result0),
                            new TypeValuePair(typeof(T1), result1)))
        {
            Result1 = result1;
        }
        protected MethodResult(bool isValidResults,
                               params TypeValuePair[] explicitResults)
            :
            base(isValidResults, explicitResults)
        {
        }
        protected MethodResult(bool isValidResults)
            :
            base(isValidResults)
        {
        }
    }
    public class MethodResult<T0, T1, T2>
        :
        MethodResult<T0, T1>,
        IMethodResult<T0, T1, T2>
    {
        new public readonly static MethodResult<T0, T1, T2> Void = new(isValidResults: true);
        new public readonly static MethodResult<T0, T1, T2> Failed = new(isValidResults: false);

        public T2 Result2 { get; } = default!;

        protected MethodResult(bool isValidResults,
                               params TypeValuePair[] explicitResults)
            :
            base(isValidResults, explicitResults)
        {
        }
        public MethodResult(bool isValidResults,
                            T0 result0,
                            T1 result1,
                            T2 result2)
            :
            base(isValidResults,
                 Range.From(new TypeValuePair(typeof(T0), result0),
                            new TypeValuePair(typeof(T1), result1),
                            new TypeValuePair(typeof(T2), result2)))
        {
            Result2 = result2;
        }
    }
}
