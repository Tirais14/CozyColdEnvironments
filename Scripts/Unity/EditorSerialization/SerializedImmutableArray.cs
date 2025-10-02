using System.Linq;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedImmutableArray<T> : Serialized<T[], ImmutableArray<T>>
    {
        public SerializedImmutableArray(IEnumerable<T> collection)
            :
            base(collection.ToImmutableArray())
        {

        }

        protected override T[] ConvertToInput(ImmutableArray<T> output) => Value.ToArray();

        protected override ImmutableArray<T> ConvertToOutput(T[] input) => input.ToImmutableArray();
    }
}