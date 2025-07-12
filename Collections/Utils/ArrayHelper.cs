#nullable enable
using System;
using UTIRLib.Collections;

namespace UTIRLib
{
    public static class ArrayHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static T[] Clone<T>(T[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (array.IsEmpty())
                return Array.Empty<T>();

            var cloned = new T[array.Length];

            array.CopyTo(cloned, array.Length);

            return cloned;
        }
    }
}
