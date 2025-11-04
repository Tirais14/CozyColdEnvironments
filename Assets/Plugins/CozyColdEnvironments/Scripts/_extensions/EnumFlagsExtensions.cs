using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public static class EnumFlagsExtensions
    {
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
            return EnumFlagsHelper.SetFlagInternal(value, flag, isToSet: true);
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
            return EnumFlagsHelper.SetFlagInternal(value, flag, isToSet: false);
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
    }
}
