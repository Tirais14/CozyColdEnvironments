#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UTIRLib.EnumFlags;
using UTIRLib.Utils;

namespace UTIRLib.Extensions
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

        public static byte ToByte(this Enum value)
        {
            return Convert.ToByte(value);
        }
        public static byte ToByte<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, byte>(ref value);
        }

        public static sbyte ToSbyte<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, sbyte>(ref value);
        }

        public static short ToShort(this Enum value)
        {
            return Convert.ToInt16(value);
        }
        public static short ToShort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, short>(ref value);
        }

        public static ushort ToUshort(this Enum value)
        {
            return Convert.ToUInt16(value);
        }
        public static ushort ToUshort<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ushort>(ref value);
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
        public static int ToInt<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        public static uint ToUint(this Enum value)
        {
            return Convert.ToUInt32(value);
        }
        public static uint ToUint<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, uint>(ref value);
        }

        public static long ToLong(this Enum value)
        {
            return Convert.ToInt64(value);
        }
        public static long ToLong<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, long>(ref value);
        }

        public static ulong ToUlong(this Enum value)
        {
            return Convert.ToUInt64(value);
        }
        public static ulong ToUlong<T>(this T value)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, ulong>(ref value);
        }

        #region Flags
        /// <exception cref="EnumNotFlagsException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasFlags(this Enum value, params Enum[] flags)
        {
            if (!value.IsFlags())
                throw new EnumNotFlagsException(value.GetType());
            if (flags is null)
                throw new ArgumentNullException(nameof(flags));
            if (flags.IsEmpty())
                return false;

            for (int i = 0; i < flags.Length; i++)
            {
                if (!value.HasFlag(flags[i]))
                    return false;
            }

            return true;
        }
        public static bool HasFlags(this Enum value, IEnumerable<Enum> flags)
        {
            return value.HasFlags(flags.ToArray());
        }
        public static bool HasFlags(this Enum value, Enum flags)
        {
            return value.HasFlags(flags.ToArrayByFlags());
        }

        #endregion Flags
    }
}
