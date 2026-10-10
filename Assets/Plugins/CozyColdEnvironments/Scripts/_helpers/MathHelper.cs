#nullable enable
using System;

namespace CCEnvs
{
    public static class MathHelper
    {
        public static int FloorDiv(int value, int divider)
        {
            int quotient = value / divider;
            int remainder = value % divider;

            if (remainder != 0 && value < 0)
                quotient--;

            return quotient;
        }

        public static int GetRandomCount(int minCount, int maxCount, float everyNewOneDropChance)
        {
            minCount = Math.Max(minCount, 0);

            if (maxCount <= minCount)
                throw new ArgumentException($"{nameof(maxCount)} cannot be less or equals {minCount}");

            if (maxCount == 0)
                throw new ArgumentException(nameof(maxCount));
            if (everyNewOneDropChance >= 1f)
                return maxCount;
            if (everyNewOneDropChance <= 0f)
                return minCount;

            int remaining = maxCount;
            int count = 0;
            var r = new Random();
            float failureChance = 1.0f - everyNewOneDropChance;

            while (remaining > 0)
            {
                if (r.NextFloat(0f, 1f) > failureChance)
                    count++;

                remaining--;
            }

            return count;
        }
    }
}
