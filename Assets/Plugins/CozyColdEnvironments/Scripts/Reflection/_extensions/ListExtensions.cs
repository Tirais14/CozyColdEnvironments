using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ListExtensions
    {
        public static T[] GetInternalArray<T>(this IList<T> list)
        {
            return ListCache<T>.GetInternalArray(list);
        }
    }
}
