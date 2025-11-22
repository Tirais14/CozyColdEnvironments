#nullable enable
using System;

namespace CCEnvs.FuncLanguage
{
    public interface IMaybe : IConditional
    {
        IMaybe IfSome(Action<object> action);

        IMaybe IfNone(Action action);
        Either<object, object> IfNone(Func<object> factory);

        IMaybe Do(Action<object> some, Action none);

        IMaybe Map(Func<object, object?> selector);

        IMaybe BiMap(Func<object, object?> some, Func<object?> none);

        IMaybe MapUnsafe(Func<object?, object?> selector);
    }
    public interface IMaybe<T>
        : IConditional<T>,
        IMaybe
    {
        Either<T, R> IfNone<R>(Func<R> factory);

        Maybe<TOut> Map<TOut>(Func<T, TOut?> selector);

        Maybe<TOut> BiMap<TOut>(Func<T, TOut?> some, Func<TOut?> none);

        Maybe<TOut> MapUnsafe<TOut>(Func<T?, TOut?> selector);

        T Match(Action<T> some, Func<T> none);
        TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none);

        Either<object, object> IMaybe.IfNone(Func<object> factory)
        {
            return IfNone(factory.As<Func<T>>()).Cast<object, object>();
        }

        IMaybe IMaybe.Map(Func<object, object?> selector) => Map(x => selector(x!));

        IMaybe IMaybe.BiMap(Func<object, object?> some, Func<object?> none) => BiMap(x => some(x!), () => none());

        IMaybe IMaybe.MapUnsafe(Func<object?, object?> selector) => MapUnsafe((x) => selector(x));
    }
    public interface IMaybe<T, TThis> 
        : IMaybe<T>,
        IConditional<T, TThis>

        where TThis : struct, IMaybe<T>
    {
        TThis IfSome(Action<T> action);

        new TThis IfNone(Action action);

        TThis Do(Action<T> some, Action none);

        IMaybe IMaybe.IfSome(Action<object> action) => IfSome(x => action(x!));

        IMaybe IMaybe.IfNone(Action action) => IfNone(() => action());

        IMaybe IMaybe.Do(Action<object> some, Action none) => Do(x => some(x!), () => none());
    }
}
