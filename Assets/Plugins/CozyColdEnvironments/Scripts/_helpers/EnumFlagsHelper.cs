using CCEnvs.Collections;
using CCEnvs.Reflection;
using CCEnvs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public unsafe static bool IsFlagSetted<T>(this T value, T flag)
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

        public static bool IsFlags(this Enum value)
        {
            return value.GetType().IsDefined<FlagsAttribute>();
        }
        public static bool IsFlags<T>(this T value)
            where T : Enum
        {
            return value.GetType().IsDefined<FlagsAttribute>();
        }

        public static Enum[] ToArrayByFlags(this Enum value, string? exceptByName = "None")
        {
            bool toExceptByName = exceptByName.IsNotNullOrEmpty();
            Enum[] typeValues = Enum.GetValues(value.GetType()).Cast<Enum>().ToArray();
            List<Enum> result = new(typeValues.Length);
            for (int i = 0; i < typeValues.Length; i++)
            {
                if (value.HasFlag(typeValues[i])
                    &&
                    (!toExceptByName
                        ||
                        toExceptByName
                        &&
                        typeValues[i].ToString()
                        !=
                        exceptByName
                        )
                    )
                    result.Add(typeValues[i]);
            }

            return result.ToArray();
        }
        /// <exception cref="EnumNotFlagsException"></exception>
        public static T[] ToArrayByFlags<T>(this T value, string? exceptByName = "None")
            where T : unmanaged, Enum
        {
            bool toExceptByName = exceptByName.IsNotNullOrEmpty();
            List<T> result = new();
            T[] typeValues = EnumCache<T>.Values;
            for (int i = 0; i < typeValues.Length; i++)
            {
                if (value.IsFlagSetted(typeValues[i])
                    &&
                    (!toExceptByName
                        ||
                        toExceptByName
                        &&
                        typeValues[i].ToString()
                        !=
                        exceptByName
                        )
                    )
                    result.Add(typeValues[i]);
            }

            return result.ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EnumNotFlagsException"></exception>
        public static bool IsFlagsSetted<T>(this T value, IEnumerable<T> flags)
            where T : unmanaged, Enum
        {
            if (flags.IsNull())
                throw new ArgumentNullException(nameof(flags));
            if (flags.IsEmpty())
                return false;

            foreach (var flag in flags)
            {
                if (!value.IsFlagSetted(flag))
                    return false;
            }

            return true;
        }
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EnumNotFlagsException"></exception>
        public static bool IsFlagsSetted<T>(this T value, params T[] flags)
            where T : unmanaged, Enum
        {
            if (flags is null)
                throw new ArgumentNullException(nameof(flags));
            if (flags.IsEmpty())
                return false;

            int flagsCount = flags.Length;
            for (int i = 0; i < flagsCount; i++)
            {
                if (!value.IsFlagSetted(flags[i]))
                    return false;
            }

            return true;
        }
        public static bool IsFlagsSetted<T>(this T value, T flags)
            where T : unmanaged, Enum
        {
            return value.IsFlagsSetted(flags.ToArrayByFlags());
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

        public static string[] ToStringArrayByFlags(this Enum value)
        {
            return value.ToArrayByFlags().ToStringArray();
        }
        public static string[] ToStringArrayByFlags<T>(this T value)
            where T : unmanaged, Enum
        {
            return value.ToArrayByFlags().ToStringArray();
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

        public static string[] ToStringArray(this Enum[] values)
        {
            return values.Select(x => x.ToString()).ToArray();
        }
        public static string[] ToStringArray<T>(this T[] values)
            where T : unmanaged, Enum
        {
            return values.Select(x => x.ToString()).ToArray();
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
