using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace CCEnvs.Common
{
    public static class EnumExtensions
    {
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

        public unsafe static short ToShort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, short>(ref value);
        }

        public unsafe static ushort ToUshort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ushort>(ref value);
        }

        public unsafe static int ToInt<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        public unsafe static uint ToUint<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, uint>(ref value);
        }

        public unsafe static long ToLong<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, long>(ref value);
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
