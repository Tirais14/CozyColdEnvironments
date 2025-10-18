#nullable enable
using System;

namespace CCEnvs
{
    /// <summary>
    /// Converts to specified type
    /// </summary>
    public interface IMutable
    {
        Type MutateResultType { get; }

        object MutateType();
    }
    public interface IMutable<out T> : IMutable
    {
        Type IMutable.MutateResultType => typeof(T);

        new T MutateType();

        object IMutable.MutateType() => MutateType()!;
    }
}
