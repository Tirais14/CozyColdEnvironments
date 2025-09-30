using ZLinq;
using System.Collections.Generic;
using CCEnvs.Linq;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization.Linq
{
    public static class IEnumerableQueries
    {
        public static IEnumerable<SerializedTuple<T1, T2>> AsSerializedTuples<T1, T2>(
            this IEnumerable<(T1, T2)> source)
        {
            return source.AsValueEnumerable().Select(x => x.ToSerializedTuple()).AsEnumerable();
        }
    }
}
