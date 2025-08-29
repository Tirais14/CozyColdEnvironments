#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using CozyColdEnvironments.Utils;

namespace CozyColdEnvironments
{
    public unsafe static class EnumExtensions
    {
        public static byte ToByte(this Enum value)
        {
            return Convert.ToByte(value);
        }
        public unsafe static byte ToByte<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, byte>(ref value);
        }

        public unsafe static sbyte ToSbyte<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, sbyte>(ref value);
        }

        public static short ToShort(this Enum value)
        {
            return Convert.ToInt16(value);
        }
        public unsafe static short ToShort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, short>(ref value);
        }

        public static ushort ToUshort(this Enum value)
        {
            return Convert.ToUInt16(value);
        }
        public unsafe static ushort ToUshort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ushort>(ref value);
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
        public unsafe static int ToInt<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        public static uint ToUint(this Enum value)
        {
            return Convert.ToUInt32(value);
        }
        public unsafe static uint ToUint<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, uint>(ref value);
        }

        public static long ToLong(this Enum value)
        {
            return Convert.ToInt64(value);
        }
        public unsafe static long ToLong<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, long>(ref value);
        }

        public static ulong ToUlong(this Enum value)
        {
            return Convert.ToUInt64(value);
        }
        public unsafe static ulong ToUlong<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ulong>(ref value);
        }

        public unsafe static T As<TValue, T>(this TValue value)
            where TValue : unmanaged, Enum
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<TValue, T>(ref value);
        }
    }
}

namespace CozyColdEnvironments.Reflection
{
    public static class EnumExtensions
    {
        public static FieldInfo GetFieldInfo(this Enum value)
        {
            return EnumHelper.GetFieldInfo(value);
        }
        public static FieldInfo GetFieldInfo<T>(this T enumValue)
            where T : Enum
        {
            return EnumHelper.GetFieldInfo(enumValue);
        }
    }
}

namespace CozyColdEnvironments.Attributes.Metadata
{
    using CozyColdEnvironments.Diagnostics;
    using CozyColdEnvironments.Options;
    using CozyColdEnvironments.Reflection;

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
