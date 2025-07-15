using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/>
        /// </summary>
        public static object Convert(this object obj, Type conversionType)
        {
            return System.Convert.ChangeType(obj, conversionType);
        }

        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/>
        /// </summary>
        public static T Convert<T>(this object obj)
        {
            return (T)System.Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// Same as the <see cref="Convert.ChangeType(object, Type)"/> but in <see langword="try"/>-<see langword="catch"/>
        /// </summary>
        public static bool TryConvert(this object? obj, Type conversionType, [NotNullWhen(true)] out object? result)
        {
            try
            {
                result = System.Convert.ChangeType(obj, conversionType);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}