#nullable enable
using System;

namespace CozyColdEnvironments
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
