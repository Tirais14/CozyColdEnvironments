using System;

namespace CCEnvs
{
    [Flags]
    public enum CompareTypes
    {
        None,
        Equals,
        Smaller = 2,
        Bigger = 4,
    }
}
