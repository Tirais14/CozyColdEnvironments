using System;

namespace UTIRLib
{
    [Flags]
    public enum UpdateMethod
    {
        None,
        Normal,
        Fixed = 2,
        Late = 4
    }
}