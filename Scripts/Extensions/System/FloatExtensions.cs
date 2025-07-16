using System;
using System.Runtime.CompilerServices;

#nullable enable

namespace UTIRLib
{
    public static class FloatExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsX(this float a, float b, float epsilon = 0.0001f)
        {
            return MathF.Abs(a - b) < epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqualsWithEpsilon(this float a, float b, float epsilon = 0.0001f)
        {
            return !a.EqualsX(b, epsilon);
        }
    }
}