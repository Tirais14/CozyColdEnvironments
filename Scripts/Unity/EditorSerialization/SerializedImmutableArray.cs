using LinqAF;
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedImmutableArray<T> : Serialized<T[], ImmutableArray<T>>
    {
        public SerializedImmutableArray()
            : 
            base(input => input.ToImmutableArray(), output => output.ToArray())
        {
        }
    }
}
