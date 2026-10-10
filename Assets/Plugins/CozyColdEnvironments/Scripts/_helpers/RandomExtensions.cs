using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random source, double min, double max)
        {
            CC.Guard.IsNotNullSource(source);
            return source.NextDouble() * (max - min) + min;
        }

        public static float NextFloat(this Random source)
        {
            CC.Guard.IsNotNullSource(source);
            return (float)source.NextDouble();
        }

        public static float NextFloat(this Random source, float min, float max)
        {
            CC.Guard.IsNotNullSource(source);
            return (float)(source.NextDouble() * (max - min) + min);
        }

        public static long NextLong(this Random source)
        {
            Guard.IsNotNull(source);
            Span<byte> buffer = stackalloc byte[8];
            source.NextBytes(buffer);
            return BitConverter.ToInt64(buffer) & long.MaxValue;
        }
        public static long NextLong(this Random source, long min, long max)
        {
            Guard.IsNotNull(source);
            uint low = (uint)source.Next();
            uint high = (uint)source.Next();
            ulong combined = ((ulong)high << 32) | low;
            return (long)(combined % (ulong)(max - min)) + min;
        }
    }
}
