using System;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs
{
    public static class FloatHelper
    {
        public const float FIRST_EPSILON = 1e-5f;
        public const float SECOND_EPSILON = 1e-6f;

        public static bool NearlyEquals(this float value,
                                             float other,
                                             float? customEpsilon = null)
        {
            float epsilon;

            if (customEpsilon.HasValue)
                epsilon = customEpsilon.Value;
            else
            {
                if (MathF.Abs(value) > 1)
                    epsilon = SECOND_EPSILON;
                else
                    epsilon = FIRST_EPSILON;
            }

            return MathF.Abs(value - other) < epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotNearlyEquals(this float value,
                                                float other,
                                                float? customEpsilon = null)
        {
            return !value.NearlyEquals(other, customEpsilon);
        }
    }
}