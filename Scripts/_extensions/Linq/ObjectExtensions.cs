using System;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.Linq
{
    public static class ObjectExtensions
    {
        public static T ThrowIf<T>(this T obj, T? value, Exception? exception = null)
        {
#nullable disable
            if (EqualityComparer<T>.Default.Equals(obj, value))
            {
                throw exception ?? new Exception($"Value cannot be {value}");
            }

            return obj;
#nullable enable
        }

        public static T ThrowIfNull<T>(this T? obj, Exception? exception = null)
            where T : class
        {
            if (obj == null)
            {
                throw exception ?? new NullReferenceException("Throwed by query.");
            }

            return obj;
        }

        /// <summary>
        /// Same as the <see langword="is"/> keyword
        /// </summary>
        public static T? IsQ<T>(this object? obj)
        {
            return obj is T typedObj ? typedObj : default;
        }

        /// <summary>
        /// Same as the <see langword="is"/> keyword
        /// </summary>
        public static TValue? IsQ<TObj, TValue>(this TObj? obj)
        {
            return obj is TValue typedObj ? typedObj : default;
        }
    }
}