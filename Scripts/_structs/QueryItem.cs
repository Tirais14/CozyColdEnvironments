using System;

#nullable enable
namespace CozyColdEnvironments
{
    public readonly struct QueryItem<TIn, TOut>
    {
        public readonly TIn inArg;
        public readonly TOut outArg;

        public QueryItem(TIn inArg, TOut outArg)
        {
            this.inArg = inArg;
            this.outArg = outArg;
        }

        //public QueryItem Do<T0, T1>(Func<T0, T1> action)
        //{
        //    return new QueryItem<T0, T1>()
        //}

        public static implicit operator TIn(QueryItem<TIn, TOut> value)
        {
            return value.inArg;
        }

        public static implicit operator TOut(QueryItem<TIn, TOut> value)
        {
            return value.outArg;
        }
    }
}
