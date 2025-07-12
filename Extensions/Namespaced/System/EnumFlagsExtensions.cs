using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Utils;
using static UTIRLib.EnumFlags.EnumFlagsOptions;

#nullable enable
namespace UTIRLib.EnumFlags
{
    public static class EnumFlagsExtensions
    {
        public static bool IsFlags(this Enum value)
        {
            return value.GetType().IsDefined(typeof(FlagsAttribute));
        }
        public static bool IsFlags<T>(this T value)
            where T : Enum
        {
            return value.GetType().IsDefined(typeof(FlagsAttribute));
        }

        public static Enum[] ToArrayByFlags(this Enum value, string? exceptByName = "None")
        {
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return Array.Empty<Enum>();
            }

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
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return Array.Empty<T>();
            }

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

        /// <exception cref="EnumNotFlagsException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsFlagSetted<T>(this T value, T flag)
            where T : unmanaged, Enum
        {
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return false;
            }

            switch (UnsafeUtility.SizeOf<T>())
            {
                case 1:
                    byte valueByte = value.ToByte();
                    byte flagByte = flag.ToByte();

                    return (valueByte & flagByte) == flagByte;
                case 2:
                    ushort valueShort = value.ToUshort();
                    ushort flagShort = flag.ToUshort();

                    return (valueShort & flagShort) == flagShort;
                case 4:
                    uint valueInt = value.ToUint();
                    uint flagInt = flag.ToUint();

                    return (valueInt & flagInt) == flagInt;
                case 8:
                    ulong valueLong = value.ToUlong();
                    ulong flagLong = flag.ToUlong();

                    return (valueLong & flagLong) == flagLong;
                default:
                    throw new InvalidOperationException("Unsupported enum size.");
            }
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
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return false;
            }

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
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return false;
            }

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
            return SetFlagInternal(value, flag, isToSet: true);
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
            return SetFlagInternal(value, flag, isToSet: false);
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
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return Array.Empty<string>();
            }

            return value.ToArrayByFlags().ToStringArray();
        }
        public static string[] ToStringArrayByFlags<T>(this T value)
            where T : unmanaged, Enum
        {
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return Array.Empty<string>();
            }

            return value.ToArrayByFlags().ToStringArray();
        }

        #region Collections
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
        #endregion Collections

        /// <exception cref="EnumNotFlagsException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static T SetFlagInternal<T>(T value, T flag, bool isToSet)
            where T : unmanaged, Enum
        {
            if (!value.IsFlags())
            {
                if (ThrowNotFlagsException)
                    throw new EnumNotFlagsException(value.GetType());
                else
                    return value;
            }

            return UnsafeUtility.SizeOf<T>() switch {
                1 => SetFlagByteInternal(value, flag, isToSet),
                2 => SetFlagInt16Internal(value, flag, isToSet),
                4 => SetFlagInt32Internal(value, flag, isToSet),
                8 => SetFlagInt64Internal(value, flag, isToSet),
                _ => throw new InvalidOperationException("Unsupported enum size."),
            };
        }
        private static T SetFlagByteInternal<T>(T value, T flag, bool isToSet)
        {
            byte valueByte = UnsafeUtility.As<T, byte>(ref value);
            byte flagByte = UnsafeUtility.As<T, byte>(ref flag);

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= (byte)~flagByte;

            return UnsafeUtility.As<byte, T>(ref valueByte);
        }
        private static T SetFlagInt16Internal<T>(T value, T flag, bool isToSet)
        {
            ushort valueByte = UnsafeUtility.As<T, ushort>(ref value);
            ushort flagByte = UnsafeUtility.As<T, ushort>(ref flag);

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= (ushort)~flagByte;

            return UnsafeUtility.As<ushort, T>(ref valueByte);
        }
        private static T SetFlagInt32Internal<T>(T value, T flag, bool isToSet)
        {
            uint valueByte = UnsafeUtility.As<T, uint>(ref value);
            uint flagByte = UnsafeUtility.As<T, uint>(ref flag);

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

            return UnsafeUtility.As<uint, T>(ref valueByte);
        }
        private static T SetFlagInt64Internal<T>(T value, T flag, bool isToSet)
        {
            ulong valueByte = UnsafeUtility.As<T, ulong>(ref value);
            ulong flagByte = UnsafeUtility.As<T, ulong>(ref flag);

            if (isToSet)
                valueByte |= flagByte;
            else
                valueByte &= ~flagByte;

            return UnsafeUtility.As<ulong, T>(ref valueByte);
        }
    }
}
