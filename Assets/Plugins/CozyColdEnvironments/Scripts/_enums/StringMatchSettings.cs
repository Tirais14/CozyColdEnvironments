#nullable enable
using System;

namespace CCEnvs
{
    [Flags]
    public enum StringMatchSettings
    {
        None,
        Partial = 1,
        IgnoreCase = 2,
        Invariant = 4,
        Ordinal = 8,
        Culture = 16,
        PartialFromStart = 32,
        Default = Partial | Ordinal
    }
}
