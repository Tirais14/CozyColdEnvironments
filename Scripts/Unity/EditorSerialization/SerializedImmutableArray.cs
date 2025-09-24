using LinqAF;
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedImmutableArray<T> : Serialized<T[], ImmutableArray<T>>
    {
        protected override T[] GetInput() => Output.ToArray();

        protected override ImmutableArray<T> GetOutput() => input.ToImmutableArray();
    }
}