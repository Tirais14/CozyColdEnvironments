#nullable enable
using C5;
using CCEnvs.Collections;
using System;

namespace CCEnvs
{
    public static class Range
    {
        public static T[] From<T>(params T[] values)
        {
            CC.Guard.NullArgument(values, nameof(values));

            return values;
        }
    }
}
