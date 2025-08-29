#nullable enable
using System;

namespace CCEnvs.Collections
{
    public static class ArrayFactory
    {
        public static T[] Create<T>(params T[] values) => values;
    }
}
