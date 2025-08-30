using System;
using Unity.Collections.LowLevel.Unsafe;

#nullable enable
namespace CCEnvs.Common
{
    public static class EnumFlagsExtensions
    {
        /// <exception cref="InvalidOperationException"></exception>
        public unsafe static bool IsFlagSetted<T>(this T value, T flag)
            where T : unmanaged, Enum
        {
#if UNITY_2017_1_OR_NEWER
            int size = UnsafeUtility.SizeOf<T>();
#else
            int size = Unsafe.SizeOf<T>();
#endif

            switch (size)
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
    }
}
