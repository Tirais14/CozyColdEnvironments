#nullable enable
using System;

namespace CCEnvs
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
