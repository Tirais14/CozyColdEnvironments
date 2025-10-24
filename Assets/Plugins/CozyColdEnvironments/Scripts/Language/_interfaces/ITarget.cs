#nullable enable
using System;

namespace CCEnvs.Language
{
    public interface ITarget<TThis, T> : IConditional<T>
        where TThis : struct, IConditional<T>
    {
        TThis IfSome(Action<T> action);

        TThis IfNone(Action action);

        TThis Match(Action<T> some, Action none);
    }
}
