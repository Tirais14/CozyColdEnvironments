using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public static class EnumFlagsCollectionExtensions
    {
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
