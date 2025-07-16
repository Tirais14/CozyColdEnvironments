using System;
using UTIRLib.Collections;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.Linq
{
    public static class ArrayQueries
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static T[] ConcatToBegin<T>(this T[] source, T toAdd)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            T[] result = new T[source.Length + 1];
            result[0] = toAdd;

            source.CopyTo(result, 1);

            return result;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static T[] ConcatToBegin<T>(this T[] source, T[] toAdd)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (toAdd is null)
                throw new ArgumentNullException(nameof(toAdd));
            if (toAdd.IsEmpty())
                return source;

            int sourceLength = source.Length;
            int toAddLength = toAdd.Length;
            T[] result = new T[sourceLength + toAddLength];

            for (int i = 0; i < toAddLength; i++)
                result[i] = toAdd[i];

            for (int i = toAddLength; i < sourceLength; i++)
                result[i] = source[i];

            return result;
        }
    }
}
