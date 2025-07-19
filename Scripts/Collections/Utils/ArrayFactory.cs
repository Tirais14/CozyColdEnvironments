#nullable enable
using System;

namespace UTIRLib.Collections
{
    [Obsolete]
    public static class ArrayFactory
    {
        public static T[] Create<T>(params T[] values) => values;
    }
}
