#if IMMUTABLE_COLLECTIONS
using LinqAF;
using System

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
            Queue
        }
    }
}
#endif //IMMUTABLE_COLLECTIONS