using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public static class ObjectExtensions
    {
        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj) => new NullValidator<T>(obj).AnyNull;

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.AnyNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj) => !new NullValidator<T>(obj);

        /// <summary>
        /// Also checks for unity null
        /// </summary>
        public static bool IsDefault<T>([NotNullWhen(false)] this T obj) =>
            EqualityComparer<T>.Default.Equals(obj, default!) || obj.IsNull();

        /// <summary>Inverted</summary>
        public static bool IsNotDefault<T>([NotNullWhen(true)] this T obj) => !obj.IsDefault();
    }
}