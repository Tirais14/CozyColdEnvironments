using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UTIRLib.EnumFlags;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.Attributes.Metadata
{
    public static class EnumExtensions
    {
        public static string GetMetaString<T>(this T value)
            where T : Enum
        {
            return value.GetFieldInfo()
                        .GetMetadata()
                        .Single<MetaStringAttribute>().Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns><see cref="MetaStringAttribute"/> or <see cref="Enum.ToString()"/> value</returns>
        public static string TryGetMetaString<T>(this T value, out bool success)
            where T : Enum
        {
            string? data = value.GetFieldInfo()
                                .GetMetadata(throwIfNotFound: false)
                                .Single<MetaStringAttribute>().Value;

            if (data is null)
            {
                success = false;
                return value.ToString();
            }

            success = true;
            return data;
        }
        public static string TryGetMetaString<T>(this T value)
            where T : Enum
        {
            return value.TryGetMetaString(out _);
        }

        public static Type GetMetaType<T>(this T value)
            where T : Enum
        {
            return value.GetFieldInfo()
                        .GetMetadata()
                        .Single<MetaTypeAttribute>().Value;
        }
        public static bool TryGetMetaType<T>(this T value, [NotNullWhen(true)] out Type? data)
            where T : Enum
        {
            data = value.GetFieldInfo()
                        .GetMetadata(throwIfNotFound: false)
                        .Single<MetaTypeAttribute>().Value;

            return data != null;
        }

        public static string[] GetMetaStringByFlags(this Enum value,
                                                    bool useDefaultStringsIfNotFound = false)
        {
            if (!value.IsFlags())
            {
                if (EnumFlagsOptions.ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return Array.Empty<string>();
            }

            Enum[] enumValues = value.ToArrayByFlags();
            List<string> results = new(enumValues.Length);
            string enumString;
            for (int i = 0; i < enumValues.Length; i++)
            {
                enumString = value.TryGetMetaString(out bool success);

                if (success || !success && useDefaultStringsIfNotFound)
                    results.Add(enumString);
            }

            return results.ToArray();
        }
    }
}
