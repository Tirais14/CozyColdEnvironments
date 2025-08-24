#nullable enable
using System;

namespace UTIRLib
{
    [Flags]
    public enum TypeNameConvertingAttributes
    {
        None,
        ShortName,
        IncludeGenericArguments = 2,
        Default = ShortName | IncludeGenericArguments,
    }
}
