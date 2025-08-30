using System;
using Unity.Collections.LowLevel.Unsafe;

#nullable enable
namespace CCEnvs.Common
{
    public static class EnumFlagsHelper
    {
        /// <exception cref="InvalidOperationException"></exception>
        public unsafe static T SetFlagInternal<T>(T value,
                                                  T flag,
                                                  bool isToSet)
            where T : unmanaged, Enum
        {
            return UnsafeUtility.SizeOf<T>() switch
            {
                1 => SetFlagByteInternal(value, flag, isToSet),
                2 => SetFlagInt16Internal(value, flag, isToSet),
                4 => SetFlagInt32Internal(value, flag, isToSet),
                8 => SetFlagInt64Internal(value, flag, isToSet),
                _ => throw new NotSupportedException("Unsupported enum size."),
            };
        }

        private unsafe static T SetFlagByteInternal<T>(T value,
                                                       T flag,
                                                       bool isToSet)

            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            byte valueByte = UnsafeUtility.As<T, byte>(ref value);
            byte flagByte = UnsafeUtility.As<T, byte>(ref flag);
#else
            byte valueByte = Unsafe.As<T, byte>(ref value);
            byte flagByte = Unsafe.As<T, byte>(ref flag);
#endif
            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= (byte)~flagByte;

            return UnsafeUtility.As<byte, T>(ref valueByte);
        }
        private unsafe static T SetFlagInt16Internal<T>(T value,
                                                        T flag,
                                                        bool isToSet)
            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            ushort valueByte = UnsafeUtility.As<T, ushort>(ref value);
            ushort flagByte = UnsafeUtility.As<T, ushort>(ref flag);
#else
            ushort valueByte = Unsafe.As<T, byte>(ref value);
            ushort flagByte = Unsafe.As<T, byte>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= (ushort)~flagByte;

            return UnsafeUtility.As<ushort, T>(ref valueByte);
        }
        private unsafe static T SetFlagInt32Internal<T>(T value,
                                                        T flag,
                                                        bool isToSet)
            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            uint valueByte = UnsafeUtility.As<T, uint>(ref value);
            uint flagByte = UnsafeUtility.As<T, uint>(ref flag);
#else
            uint valueByte = Unsafe.As<T, byte>(ref value);
            uint flagByte = Unsafe.As<T, byte>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

            return UnsafeUtility.As<uint, T>(ref valueByte);
        }
        private unsafe static T SetFlagInt64Internal<T>(T value,
                                                        T flag,
                                                        bool isToSet)
            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            ulong valueByte = UnsafeUtility.As<T, ulong>(ref value);
            ulong flagByte = UnsafeUtility.As<T, ulong>(ref flag);
#else
            ulong valueByte = Unsafe.As<T, byte>(ref value);
            ulong flagByte = Unsafe.As<T, byte>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

            return UnsafeUtility.As<ulong, T>(ref valueByte);
        }
    }
}
