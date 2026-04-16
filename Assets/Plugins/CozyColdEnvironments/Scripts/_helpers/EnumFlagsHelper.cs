using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;
using CCEnvs.Reflection;
using CCEnvs.Utils;
using Unity.Collections.LowLevel.Unsafe;

#nullable enable
namespace CCEnvs
{
    public unsafe static class EnumFlagsHelper
    {
        /// <exception cref="InvalidOperationException"></exception>
        public unsafe static T SetFlag<T>(T value,
                                          T flag,
                                          bool isToSet)
            where T : unmanaged, Enum
        {

#if UNITY_2017_1_OR_NEWER
            return UnsafeUtility.SizeOf<T>() switch
#else
            return Unsafe.SizeOf<T>() switch
#endif
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

#if UNITY_2017_1_OR_NEWER
            return UnsafeUtility.As<byte, T>(ref valueByte);
#else
            return Unsafe.As<byte, T>(ref valueByte);
#endif
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
            ushort valueByte = Unsafe.As<T, ushort>(ref value);
            ushort flagByte = Unsafe.As<T, ushort>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= (ushort)~flagByte;

#if UNITY_2017_1_OR_NEWER
            return UnsafeUtility.As<ushort, T>(ref valueByte);
#else
            return Unsafe.As<ushort, T>(ref valueByte);
#endif
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
            uint valueByte = Unsafe.As<T, uint>(ref value);
            uint flagByte = Unsafe.As<T, uint>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

#if UNITY_2017_1_OR_NEWER
            return UnsafeUtility.As<uint, T>(ref valueByte);
#else
            return Unsafe.As<uint, T>(ref valueByte);
#endif
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
            ulong valueByte = Unsafe.As<T, ulong>(ref value);
            ulong flagByte = Unsafe.As<T, ulong>(ref flag);
#endif

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

#if UNITY_2017_1_OR_NEWER
            return UnsafeUtility.As<ulong, T>(ref valueByte);
#else
            return Unsafe.As<ulong, T>(ref valueByte);
#endif
        }

        /// <exception cref="InvalidOperationException"></exception>
        public unsafe static bool HasFlagT<T>(this T value, T flag)
            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            int size = global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<T>();
#else
            int size = Unsafe.SizeOf<T>();
#endif

            switch (size)
            {
                case 1:
                    byte valueByte = value.ToByteUnsafe();
                    byte flagByte = flag.ToByteUnsafe();

                    return (valueByte & flagByte) == flagByte;
                case 2:
                    ushort valueShort = value.ToUshortUnsafe();
                    ushort flagShort = flag.ToUshortUnsafe();

                    return (valueShort & flagShort) == flagShort;
                case 4:
                    uint valueInt = value.ToUintUnsafe();
                    uint flagInt = flag.ToUintUnsafe();

                    return (valueInt & flagInt) == flagInt;
                case 8:
                    ulong valueLong = value.ToUlongUnsafe();
                    ulong flagLong = flag.ToUlongUnsafe();

                    return (valueLong & flagLong) == flagLong;
                default:
                    throw new InvalidOperationException("Unsupported enum size.");
            }
        }

        public static IList<Enum> ToArrayByFlags(this Enum source, string? excludeName = "None")
        {
            CC.Guard.IsNotNullSource(source);

            bool hasExcludeName = excludeName.IsNotNullOrWhiteSpace();
            Enum[] values = EnumCache.GetFieldValues(source.GetType());
            var results = new List<Enum>(values.Length);

            Enum current;
            for (int i = 0; i < values.Length; i++)
            {
                current = values[i];

                if (!source.HasFlag(current))
                    continue;

                if (hasExcludeName && current.ToString().EqualsOrdinal(excludeName!))
                    continue;

                results.Add(current);
            }

            return results;
        }

        public static IList<T> ToArrayByFlags<T>(this T value, string? excludeName = "None")
            where T : unmanaged, Enum
        {
            bool hasExcludeName = excludeName.IsNotNullOrWhiteSpace();
            T[] values = EnumCache<T>.Values;
            var results = new List<T>(values.Length);

            T current;
            for (int i = 0; i < values.Length; i++)
            {
                current = values[i];

                if (!value.HasFlagT(current))
                    continue;

                if (hasExcludeName && current.ToString().EqualsOrdinal(excludeName!))
                    continue;

                results.Add(current);
            }

            return results;
        }

        public static bool HasFlags<T>(this T value, IEnumerable<T> flags)
            where T : unmanaged, Enum
        {
            if (flags.IsNull())
                throw new ArgumentNullException(nameof(flags));
            if (flags.IsEmpty())
                return false;

            foreach (var flag in flags)
            {
                if (!value.HasFlagT(flag))
                    return false;
            }

            return true;
        }
        public static bool HasFlags<T>(this T value, params T[] flags)
            where T : unmanaged, Enum
        {
            if (flags is null)
                throw new ArgumentNullException(nameof(flags));
            if (flags.IsEmpty())
                return false;

            int flagsCount = flags.Length;
            for (int i = 0; i < flagsCount; i++)
            {
                if (!value.HasFlagT(flags[i]))
                    return false;
            }

            return true;
        }
        public static bool HasFlagsT<T>(this T value, T flags)
            where T : unmanaged, Enum
        {
            return value.HasFlags(flags.ToArrayByFlags());
        }

        public static T SetFlag<T>(this T value, T flag)
            where T : unmanaged, Enum
        {
            return SetFlag(value, flag, isToSet: true);
        }
        public static T SetFlags<T>(this T value, IEnumerable<T> flags)
            where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value.SetFlag(flag);

            return value;
        }
        public static T SetFlags<T>(this T value, params T[] flags)
            where T : unmanaged, Enum
        {
            for (int i = 0; i < flags.Length; i++)
                value.SetFlag(flags[i]);

            return value;
        }
        public static T SetFlags<T>(this T value, T flags)
            where T : unmanaged, Enum
        {
            return value.SetFlags(flags.ToArrayByFlags());
        }

        public static T ResetFlag<T>(this T value, T flag)
            where T : unmanaged, Enum
        {
            return SetFlag(value, flag, isToSet: false);
        }
        public static T ResetFlags<T>(this T value, IEnumerable<T> flags)
            where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value.ResetFlag(flag);

            return value;
        }
        public static T ResetFlags<T>(this T value, params T[] flags)
            where T : unmanaged, Enum
        {
            for (int i = 0; i < flags.Length; i++)
                value.SetFlag(flags[i]);

            return value;
        }
        public static T ResetFlags<T>(this T value, T flags)
            where T : unmanaged, Enum
        {
            return value.ResetFlags(flags.ToArrayByFlags());
        }

        public static T UniteFlags<T>(this T[] values)
            where T : unmanaged, Enum
        {
            T result = default;
            for (int i = 0; i < values.Length; i++)
                result.SetFlag(values[i]);

            return result;
        }
        public static T UniteFlags<T>(this IEnumerable<T> values)
            where T : unmanaged, Enum
        {
            T result = default;
            foreach (var value in values)
                result.SetFlag(value);

            return result;
        }

        /// <exception cref="EnumNotFlagsException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasFlags(this Enum value, params Enum[] flags)
        {
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
    }
}
