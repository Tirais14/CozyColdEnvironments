#nullable enable
using System;

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
    }
}

