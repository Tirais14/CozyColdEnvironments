using System.Collections.Generic;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Collections
{
    public static class IEnumeratorExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> value)
        {
            if (value.IsNull())
                throw new System.ArgumentNullException(nameof(value));

            return new EnumeratorEnumerable<T>(value);
        }
    }
}
