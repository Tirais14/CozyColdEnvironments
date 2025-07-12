using System;

namespace UTIRLib.Initializer
{
    [Flags]
    public enum InitializeParameters
    {
        None,
        ArgumentsNotNull,
        ArgumentsMayBeNull = 2
    }
}