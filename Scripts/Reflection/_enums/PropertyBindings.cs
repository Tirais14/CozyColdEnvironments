#nullable enable
using System;

namespace CCEnvs.Reflection
{
    [Flags]
    public enum PropertyBindings
    {
        None,
        HasGetter,
        HasSetter,
    }
}
