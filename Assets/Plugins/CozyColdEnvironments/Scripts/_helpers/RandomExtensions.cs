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
    }
}
