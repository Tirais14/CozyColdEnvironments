#nullable enable
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace CCEnvs
{
    public unsafe static class EnumExtensions
    {
        public static byte ToByte(this Enum value)
        {
            return Convert.ToByte(value);
        }

        public static sbyte ToSByte(this Enum value)
        {
            return Convert.ToSByte(value);
        }

        public static short ToShort(this Enum value)
        {
            return Convert.ToInt16(value);
        }

        public static ushort ToUshort(this Enum value)
        {
            return Convert.ToUInt16(value);
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static uint ToUint(this Enum value)
        {
            return Convert.ToUInt32(value);
        }

        public static long ToLong(this Enum value)
        {
            return Convert.ToInt64(value);
        }

        public static ulong ToUlong(this Enum value)
        {
            return Convert.ToUInt64(value);
        }

        public unsafe static byte ToByteUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, byte>(ref value);
        }

        public unsafe static sbyte ToSbyteUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, sbyte>(ref value);
        }

        public unsafe static short ToShortUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, short>(ref value);
        }

        public unsafe static ushort ToUshortUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ushort>(ref value);
        }

        public unsafe static int ToIntUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        public unsafe static uint ToUintUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, uint>(ref value);
        }

        public unsafe static long ToLongUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, long>(ref value);
        }

        public unsafe static ulong ToUlongUnsafe<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ulong>(ref value);
        }

        public unsafe static T AsUnsafe<TValue, T>(this TValue value)
            where TValue : unmanaged, Enum
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<TValue, T>(ref value);
        }
    }
}

