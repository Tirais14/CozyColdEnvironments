#nullable enable
using System;

namespace CCEnvs
{
    /// <summary>
    /// Converts to specified type
    /// </summary>
    public interface IMutableType
    {
        Type MutateResultType { get; }

        object MutateType();
    }
    public interface IMutableType<out T> : IMutableType
    {
        Type IMutableType.MutateResultType => typeof(T);

        new T MutateType();

        object IMutableType.MutateType() => MutateType()!;
    }
}
