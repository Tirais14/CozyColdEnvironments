using LinqAF;
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedImmutableArray<T> : Serialized<T[], ImmutableArray<T>>
    {
        protected override T[] ConvertToInput(ImmutableArray<T> output) => Value.ToArray();

        protected override ImmutableArray<T> ConvertToOutput(T[] input) => input.ToImmutableArray();
    }
}