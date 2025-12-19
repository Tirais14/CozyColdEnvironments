using System;

#nullable enable
namespace CCEnvs
{
    public static class DoubleHelper
    {
        public static bool NearlyEquals(this double value,
                                        double other,
                                        double toleranceFactor = 1e-15f)
        {
            double epsilon = Math.Max(Math.Abs(value), Math.Abs(other)) * toleranceFactor;
            return Math.Abs(value - other) <= epsilon;
        }
    }
}
