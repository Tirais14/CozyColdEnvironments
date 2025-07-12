#nullable enable
using System;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class EnumFieldHelper
    {
        /// <exception cref="ArgumentException"></exception>
        public static bool SetFlagsValues(EnumFieldEntry[] flags,
                                          EnumType inheritType = EnumType.Int)
        {
            if (flags.HasNullElement())
                throw new ArgumentException("Array cannot not contain null element.");

            if (IsOverflowed(flags.Length, inheritType))
                return false;

            ulong flagValue = 1;
            EnumFieldEntry flag;
            for (int i = 0; i < flags.Length; i++)
            {
                flag = flags[i];
                if (flag.FieldName != null
                    &&
                    flag.FieldName.Equals("none", StringComparison.InvariantCultureIgnoreCase)
                    )
                    continue;

                flagValue *= 2uL;
            }

            return true;
        }

        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsOverflowed(int flagsCount, EnumType inheritType)
        {
            return inheritType switch {
                EnumType.Byte => flagsCount > 1,
                EnumType.Sbyte => flagsCount > 1,
                EnumType.Short => flagsCount > 16,
                EnumType.Ushort => flagsCount > 16,
                EnumType.Int => flagsCount > 32,
                EnumType.Uint => flagsCount > 32,
                EnumType.Long => flagsCount > 64,
                EnumType.Ulong => flagsCount > 64,
                _ => throw new InvalidOperationException(inheritType.ToString())
            };
        }
    }
}
