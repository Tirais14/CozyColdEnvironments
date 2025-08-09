#nullable enable
using System;

namespace UTIRLib
{
    [Flags]
    public enum TypeNameAttributes
    {
        None,
        ShortName,
        IncludeGenericArguments = 2,
        Default = ShortName | IncludeGenericArguments,
    }
}
